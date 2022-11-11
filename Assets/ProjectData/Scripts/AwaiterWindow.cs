using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwaiterWindow : MonoBehaviour
{
    [SerializeField] private Canvas _avaiterCanvas;
    [SerializeField] private Image _helloImage;
    [SerializeField] private Image _awaitImage;
    [SerializeField] private Image _errorImage;

    public void SetHelloState()
    {
        _avaiterCanvas.enabled = true;
        _helloImage.enabled = true;
        _awaitImage.enabled = false;
        _errorImage.enabled = false;
    }

    public void SetAwaitState()
    {
        _helloImage.enabled = false;
        _awaitImage.enabled = true;
        _errorImage.enabled = false;
    }

    public void SetErrorState()
    {
        _helloImage.enabled = false;
        _awaitImage.enabled = false;
        _errorImage.enabled = true;
    }

    public void SetSuccessState()
    {
        _helloImage.enabled = false;
        _awaitImage.enabled = false;
        _errorImage.enabled = false;
        _avaiterCanvas.enabled = false;
    }
}
