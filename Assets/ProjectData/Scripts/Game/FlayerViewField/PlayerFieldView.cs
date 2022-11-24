using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldView : MonoBehaviour
{
    [SerializeField] private Transform _leftPosition;
    [SerializeField] private Transform _rightPosition;
    [SerializeField] private Transform _centerPosition;

    private Camera _camera;
    private Sequence _moveCameraSequence;
    private bool _isLeftPosition;
    private bool _isCenterPosition;

    private void Awake()
    {
        _camera = Camera.main;
        _isLeftPosition = true;
    }

    public void SetCameraLeftPosition()
    {
        _isLeftPosition = true;
        _isCenterPosition = false;
               
        DOTween.Kill($"Move");
        _moveCameraSequence = DOTween.Sequence();
        _moveCameraSequence.SetId($"Move");
        _moveCameraSequence.Append(_camera.transform.DOMove(_leftPosition.position, 1));
        _moveCameraSequence.Join(_camera.transform.DORotate(_leftPosition.rotation.eulerAngles, 1));
    }

    public void SetCameraRightPosition()
    {
        _isLeftPosition = false;
        _isCenterPosition = false;

        DOTween.Kill($"Move");
        _moveCameraSequence = DOTween.Sequence();
        _moveCameraSequence.SetId($"Move");
        _moveCameraSequence.Append(_camera.transform.DOMove(_rightPosition.position, 1));
        _moveCameraSequence.Join(_camera.transform.DORotate(_rightPosition.rotation.eulerAngles, 1));
    }

    public void SetCameraCenterPosition()
    {
        _isCenterPosition = true;

        DOTween.Kill($"Move");
        _moveCameraSequence = DOTween.Sequence();
        _moveCameraSequence.SetId($"Move");
        _moveCameraSequence.Append(_camera.transform.DOMove(_centerPosition.position, 1));
        _moveCameraSequence.Join(_camera.transform.DORotate(_centerPosition.rotation.eulerAngles, 1));
    }

    public void SetCameraLeftOrRightPosition()
    {
        if(_isLeftPosition && _isCenterPosition)
        {
            SetCameraLeftPosition();
        }

        if (!_isLeftPosition && _isCenterPosition)
        {
            SetCameraRightPosition();
        }
    }
}
