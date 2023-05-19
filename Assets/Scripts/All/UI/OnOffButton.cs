using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrono.UI
{
    public class OnOffButton : CustomButton
    {
        [SerializeField] Image _BG;
        [SerializeField] TMP_Text _state;

        [SerializeField] Color32 _onColor = Color.green, _offColor = Color.white;

        public void SetOn()
        {
            _state.text = "ON";

            _BG.color = _onColor;
            _state.color = _offColor;
        }

        public void SetOff()
        {
            _state.text = "OFF";

            _BG.color = _offColor;
            _state.color = _onColor;
        }
    }
}


