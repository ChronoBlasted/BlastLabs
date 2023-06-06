using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabBarButton : MonoBehaviour
{
    [SerializeField] LayoutElement _layoutElement, _layoutElementIco;
    [SerializeField] RectTransform _RTIcoText;
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _title;

    [SerializeField] GameObject _content;

    Sequence _scaleTween;

    public void Init()
    {
        Close();
    }

    public void Open()
    {
        if (_scaleTween.IsActive()) _scaleTween.Kill();

        _layoutElement.preferredWidth = 300;
        _layoutElement.preferredHeight = 250;

        _layoutElementIco.preferredWidth = 100;
        _layoutElementIco.preferredHeight = 100;

        _RTIcoText.sizeDelta = Vector2.zero;

        _scaleTween = DOTween.Sequence()
            .Join(_layoutElement.DOPreferredSize(new Vector2(400, 300), .2f).SetEase(Ease.OutQuart))
            .Join(_layoutElementIco.DOPreferredSize(new Vector2(150, 150), .2f).SetEase(Ease.OutQuart))
            .Join(_RTIcoText.DOSizeDelta(new Vector2(0, 90), .2f).SetEase(Ease.OutQuart))
            .Join(_title.DOFade(1, .2f).SetEase(Ease.OutQuart));


        _content.SetActive(true);
    }

    public void Close()
    {
        if (_scaleTween.IsActive()) _scaleTween.Kill();

        _layoutElement.preferredWidth = 400;
        _layoutElement.preferredHeight = 300;

        _layoutElementIco.preferredWidth = 150;
        _layoutElementIco.preferredHeight = 150;

        _RTIcoText.sizeDelta = new Vector2(0, 90);

        _scaleTween = DOTween.Sequence()
            .Join(_layoutElement.DOPreferredSize(new Vector2(300, 250), .2f).SetEase(Ease.OutQuart))
            .Join(_layoutElementIco.DOPreferredSize(new Vector2(100, 100), .2f).SetEase(Ease.OutQuart))
            .Join(_RTIcoText.DOSizeDelta(new Vector2(0, 0), .2f).SetEase(Ease.OutQuart))
            .Join(_title.DOFade(0, .2f).SetEase(Ease.OutQuart));


        _content.SetActive(false);
    }
}
