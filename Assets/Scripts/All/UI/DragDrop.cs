using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool DesactiveScript;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform rectTransform;

    float _mainScaleFactor;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _mainScaleFactor = UIManager.Instance.MainCanvas.scaleFactor;
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / _mainScaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.localPosition = Vector3.zero;

        if (DesactiveScript) enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

}
