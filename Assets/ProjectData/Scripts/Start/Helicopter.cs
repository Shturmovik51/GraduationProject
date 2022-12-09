using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private Transform _holder;
    [SerializeField] private Transform _forwardPoint;
    [SerializeField] private Transform _backPoint;
    [SerializeField] private Transform _bladeOne;
    [SerializeField] private Transform _bladeTwo;

    private void Start()
    {
        MoveForward();
    }

    private void Update()
    {
        _bladeOne.Rotate(0, 4, 0);
        _bladeTwo.Rotate(0, 4, 0);
    }

    public void MoveForward()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_holder.DOMove(_forwardPoint.position, 60));
        sequence.OnComplete(MoveBackward);
    }

    public void MoveBackward()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_holder.DOMove(_backPoint.position, 60));
        sequence.OnComplete(MoveForward);
    }
}
