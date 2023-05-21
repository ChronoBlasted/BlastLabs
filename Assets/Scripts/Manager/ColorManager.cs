using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoSingleton<ColorManager>
{
    public Color _baseColor1, _baseColor2, _baseColor3;
    public Color _complimentary1, _complimentary2;
    public Color _white, _black;
}
