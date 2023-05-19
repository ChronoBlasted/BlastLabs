using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;

    [SerializeField] TMP_Text sliderValue;

    public void Setup(float value, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SmoothSetValue(float newValue, float duration = 0.2f, Ease ease = Ease.OutSine)
    {
        DOTween.Kill(slider);

        slider.DOValue(newValue, duration).SetEase(ease);
    }


    #region TextUpdateOnValueChange
    public void UpdateTextWithSlash() => sliderValue.text = Mathf.RoundToInt(slider.value) + "/" + slider.maxValue;
    public void UpdateTextValue() => sliderValue.text = Mathf.RoundToInt(slider.value).ToString();
    public void UpdateTextValueWithSuffixe(string suffixe) => sliderValue.text = Mathf.RoundToInt(slider.value) + suffixe;
    public void UpdateTextValueWithPrefix(string prefix) => sliderValue.text = prefix + Mathf.RoundToInt(slider.value);
    public void UpdateText(string prefix = "", string suffixe = "", bool slash = false) => sliderValue.text = prefix + Mathf.RoundToInt(slider.value) + (slash ? "/" + slider.maxValue : "") + suffixe;

    #endregion
}
