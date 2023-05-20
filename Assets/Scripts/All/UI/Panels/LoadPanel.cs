using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPanel : Panel
{
    [SerializeField] SliderBar _slider;
    public override void Init()
    {
        base.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void UpdateLoadBar(float amount)
    {
        _slider.SmoothSetValue(amount);
    }
}
