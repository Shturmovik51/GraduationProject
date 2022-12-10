using DG.Tweening;
using UnityEngine;

public class HidablePanel : MonoBehaviour
{
    [SerializeField] protected GameObject _hidablePanel;
    [SerializeField] Transform _hidePoint;

    protected Vector3 _normalPosition;

    private Sequence _showSequence;
    private Sequence _hideSequence;

    public void SetStartPosition()
    {
        _normalPosition = _hidablePanel.transform.position;
    }

    public void ShowUpPanel()
    {
        DOTween.Kill($"HidePanel");
        _showSequence = DOTween.Sequence();
        _showSequence.SetId($"ShowUpPanel");
        _showSequence.Append(_hidablePanel.transform.DOMove(_normalPosition, 1));
    }

    public void HidePanel()
    {
        DOTween.Kill($"ShowUpPanel");
        _hideSequence = DOTween.Sequence();
        _hideSequence.SetId($"HidePanel");
        _hideSequence.Append(_hidablePanel.transform.DOMove(_hidePoint.position, 1));
    }
}
