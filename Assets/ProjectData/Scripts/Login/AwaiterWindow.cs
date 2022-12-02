using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwaiterWindow : MonoBehaviour
{
    [SerializeField] private Canvas _avaiterCanvas;
    [SerializeField] private Image _helloImage;
    [SerializeField] private Image _awaitImage;
    [SerializeField] private Image _errorImage;
    [SerializeField] private Image _succesImage;
    [SerializeField] private Image _joinOpponentImage;

    private List<Image> _allImages;

    private void Awake()
    {
        _allImages = new List<Image>()
        {
            _helloImage,
            _awaitImage,
            _errorImage,
            _succesImage,
            _joinOpponentImage,
        };
    }

    public void SetHelloState()
    {
        ResetImages();
        _helloImage.enabled = true;       
        _avaiterCanvas.enabled = true;
    }

    public void SetAwaitState()
    {
        ResetImages();
        _awaitImage.enabled = true;
    }

    public void SetErrorState()
    {
        ResetImages();
        _errorImage.enabled = true;
    }

    public void SetSuccessState()
    {
        ResetImages();
        _succesImage.enabled = true;
    }

    public void SetOpponentDetectionState()
    {
        ResetImages();
        _joinOpponentImage.enabled = true;
    }

    private void ResetImages()
    {
        foreach (Image image in _allImages)
        {
            image.enabled = false;
        }
    }
}