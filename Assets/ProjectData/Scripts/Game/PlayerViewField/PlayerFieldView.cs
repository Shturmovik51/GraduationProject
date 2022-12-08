using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldView : MonoBehaviour
{
    [SerializeField] private Transform _leftPosition;
    [SerializeField] private Transform _rightPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _menuPosition;

    private Camera _camera;
    private Sequence _moveCameraSequence;

    private CameraPositionType _lastPosition;
    private CameraPositionType _currentPosition;

    private bool _isMenuPosition;

    private void Awake()
    {
        _camera = Camera.main;
        _lastPosition = CameraPositionType.Left;
        _currentPosition = CameraPositionType.Menu;
        _isMenuPosition = true;
    }

    public void SetCameraLeftPosition()
    {
        if(_currentPosition == CameraPositionType.Left || _isMenuPosition)
        {
            return;
        }

        if(_currentPosition != CameraPositionType.Menu)
        {
            _lastPosition = _currentPosition;
        }

        _currentPosition = CameraPositionType.Left;
        MoveCameraToPosition(_leftPosition);       
    }

    public void SetCameraRightPosition()
    {
        if (_currentPosition == CameraPositionType.Right || _isMenuPosition)
        {
            return;
        }

        if (_currentPosition != CameraPositionType.Menu)
        {
            _lastPosition = _currentPosition;
        }

        _currentPosition = CameraPositionType.Right;
        MoveCameraToPosition(_rightPosition);        
    }

    public void SetCameraCenterPosition()
    {
        if (_currentPosition == CameraPositionType.Center || _isMenuPosition)
        {
            return;
        }

        if (_currentPosition != CameraPositionType.Menu)
        {
            _lastPosition = _currentPosition;
        }
        
        _currentPosition= CameraPositionType.Center;
        MoveCameraToPosition(_centerPosition);        
    }

    public void SetCameraMenuPosition()
    {
        _lastPosition = _currentPosition;
        _currentPosition = CameraPositionType.Menu;
        _isMenuPosition = true;
        MoveCameraToPosition(_menuPosition);
    }

    private void MoveCameraToPosition(Transform point)
    {
        DOTween.Kill($"Move");
        _moveCameraSequence = DOTween.Sequence();
        _moveCameraSequence.SetId($"Move");
        _moveCameraSequence.Append(_camera.transform.DOMove(point.position, 1));
        _moveCameraSequence.Join(_camera.transform.DORotate(point.rotation.eulerAngles, 1));
    }

    public void SetLastPosition()
    {
        if (_isMenuPosition)
        {
            return;
        }

        switch (_lastPosition)
        {
            case CameraPositionType.None:
                break;
            case CameraPositionType.Left:
                SetCameraLeftPosition();
                break;
            case CameraPositionType.Right:
                SetCameraRightPosition();
                break;
            case CameraPositionType.Center:
                SetCameraCenterPosition();
                break;
            case CameraPositionType.Menu:
                break;
            default:
                break;
        }
    }

    public void SetLastPositionAfterMenu()
    {
        _isMenuPosition = false;
        SetLastPosition();
    }
}
