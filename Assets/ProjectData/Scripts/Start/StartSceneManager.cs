using DG.Tweening;
using Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private Transform _leftPosition;
    [SerializeField] private Transform _rightPosition;
    [SerializeField] private Transform _topPosition;
    [SerializeField] private Transform _bottomPosition;
    [SerializeField] private Button _startButton;
    [SerializeField] private MutePanelView _mutePanelView;

    private Mouse _mouse;
    private Camera _camera;
    private SceneLoader _sceneLoader;
    private float _lastMousePositionX;
    private float _lastMousePositionY;
    private SoundManager _soundManager;
    private InputSystemController _inputSystemController;

    private bool _isFirstUpdate;

    private void Awake()
    {
        _soundManager = FindObjectOfType<SoundManager>();
        _sceneLoader = FindObjectOfType<SceneLoader>();   
    }

    private void Start()
    {
        _mouse = Mouse.current;
        _camera = Camera.main;
        _isFirstUpdate = true;
        _inputSystemController = new InputSystemController();
        _mutePanelView.Init();
        _mutePanelView.SetMuteState(_soundManager.IsMuted);

        _startButton.onClick.AddListener(() => _sceneLoader.LoadScene(SceneType.Scene1));       
        _inputSystemController.GetInputSystem().Player.AnyKey.performed += (context) => _sceneLoader.LoadScene(SceneType.Scene1);
        _mutePanelView.SubscribeButton(_soundManager.MuteOrUnmuteSound);
        _soundManager.SubscribeStartScreenButtons();
        _soundManager.PlayMenuSound();
    }

    void Update()
    {    
        float x = _mouse.position.ReadValue().x;
        float y = _mouse.position.ReadValue().y;

        if (_isFirstUpdate)
        {
            _lastMousePositionX = x;
            _lastMousePositionY = y;
            _isFirstUpdate = false;
            return;
        }

        if (x < _lastMousePositionX)
        {
            var pos = Vector3.Lerp(_camera.gameObject.transform.position, _leftPosition.position, 4f * Time.deltaTime);
            _camera.gameObject.transform.position = pos;
        }
        if (x > _lastMousePositionX)
        {
            var pos = Vector3.Lerp(_camera.gameObject.transform.position, _rightPosition.position, 4f * Time.deltaTime);
            _camera.gameObject.transform.position = pos;
        }
        if (y < _lastMousePositionY)
        {
            var pos = Vector3.Lerp(_camera.gameObject.transform.position, _bottomPosition.position, 4f * Time.deltaTime);
            _camera.gameObject.transform.position = pos;
        }
        if (y > _lastMousePositionY)
        {
            var pos = Vector3.Lerp(_camera.gameObject.transform.position, _topPosition.position, 4f * Time.deltaTime);
            _camera.gameObject.transform.position = pos;
        }

        _lastMousePositionX = x;
        _lastMousePositionY = y;
    }
       

    private void OnDisable()
    {
        _startButton.onClick.RemoveAllListeners();
        _inputSystemController.CleanUp();
        _mutePanelView.UnsubscribeButton();
    }
}
