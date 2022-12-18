using DG.Tweening;
using Engine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class AutoBattleController :IUpdatable, ICleanable, IController
{
    private AutoBattleView _autoBattleView;
    private TurnController _turnController;
    private MouseRaycaster _mouseRaycaster;
    private EndBattleController _endBattleController;

    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsRight;

    private bool _isAutobattle;
    private Action _onStartBattleAction;

    public AutoBattleController(GameData gameData, TurnController turnController, List<FieldCell> masterCellsRight,
            List<FieldCell> opponentCellsRight, MouseRaycaster mouseRaycaster, EndBattleController endBattleController)
    {
        _autoBattleView = gameData.AutoBattleView;
        _turnController = turnController;
        _masterCellsRight = masterCellsRight;
        _opponentCellsRight = opponentCellsRight;
        _endBattleController = endBattleController;
        _mouseRaycaster = mouseRaycaster;

        _onStartBattleAction = () => _autoBattleView.SetButtonVisibility(true);

        _autoBattleView.SubscribeButton(SetAutobattleState);
        _turnController.OnSetPlayerTurn += StartFindMove;
        _turnController.OnStartBattle += _onStartBattleAction;
        _endBattleController.OnEndBattle += DeactivateAutoBAttle;

        _autoBattleView.SetButtonVisibility(false);  //включение при старте батла после рола
    }

    public void LocalUpdate(float deltaTime)
    {
        if (_isAutobattle)
        {
            _autoBattleView.RotateButton(deltaTime);
        }
    }

    private void StartFindMove()
    {
        if (!_isAutobattle)
        {
            return;
        }

        _mouseRaycaster.SetHitCellAvaliability(false);
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(2);
        sequence.OnComplete(MakeMove);

    }

    private void MakeMove()
    {
        if (!_isAutobattle)
        {
            return;
        }

        FieldCell cell;

        if (PhotonNetwork.IsMasterClient)
        {
            FindCell(_masterCellsRight);            
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            FindCell(_opponentCellsRight);
        }
        else
        {
            throw new System.Exception("No cells to search");
        }

        cell.InitAction();
        cell.SendClickEvent();

        if (cell.IsShipTarget)
        {
            StartFindMove();
        }

        void FindCell(List<FieldCell> cells)
        {
            var currentCell = cells[Random.Range(0, _masterCellsRight.Count)];
            if (!currentCell.IsUsed)
            {
                cell = currentCell;
            }
            else
            {
                FindCell(cells);
            }
        }
    }

    private void SetAutobattleState()
    {
        _isAutobattle = !_isAutobattle;

        _autoBattleView.SetTextVisibility(_isAutobattle);

        if (_turnController.IsPlayerTurn && _isAutobattle)
        {
            StartFindMove();
        }
    }    

    public void DeactivateAutoBAttle()
    {
        _isAutobattle = false;
    }

    public void CleanUp()
    {
        _autoBattleView.UnsubscribeButton();
        _turnController.OnSetPlayerTurn -= StartFindMove;
        _turnController.OnStartBattle -= _onStartBattleAction;
        _endBattleController.OnEndBattle -= DeactivateAutoBAttle;
    }    
}
