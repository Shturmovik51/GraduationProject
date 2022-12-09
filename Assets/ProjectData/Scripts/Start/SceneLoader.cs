using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image _blackScreenImage;

    public void LoadScene(SceneType scene)
    {
        var sequence = DOTween.Sequence();
        var color = _blackScreenImage.color;
        color.a = 1;
        sequence.Append(_blackScreenImage.DOColor(color, 0.1f));
        sequence.OnComplete(() =>
        {
            switch (scene)
            {
                case SceneType.None:
                    break;
                case SceneType.Scene0:
                    SceneManager.LoadScene(0);
                    break;
                case SceneType.Scene1:
                    SceneManager.LoadScene(1);
                    break;
                case SceneType.Scene2:
                    SceneManager.LoadScene(2);
                    break;
                default:
                    break;
            }
        });
    }

    public void CompleteLoadScene()
    {
        var sequence = DOTween.Sequence();
        var color = _blackScreenImage.color;
        color.a = 0;
        sequence.AppendInterval(1f);
        sequence.Append(_blackScreenImage.DOColor(color, 0.2f));
    }
}
