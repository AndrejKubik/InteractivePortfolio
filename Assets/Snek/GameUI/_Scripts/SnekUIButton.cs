using System;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Snek.GameUI
{
    [UseSnekInspector]
    [RequireComponent(typeof(Button))]
    public class SnekUIButton : SnekMonoBehaviour
    {
        protected Button _button { get; private set; }
        protected RectTransform _inputRectTransform { get; private set; }
        protected Image _buttonImage { get; private set; }

        private Action _externalCallback;

        protected override void Initialize()
        {
            _button = GetComponent<Button>();
        }

        protected override void Validate()
        {
            if (!_button)
                FailValidation("Cannot find Button component.");
            else if (!_button.targetGraphic)
                FailValidation("Button doesn't have a target graphic assigned.");
        }

        protected override void OnInitializationSuccess()
        {
            _button.onClick.AddListener(OnButtonClickInternal);

            _inputRectTransform = _button.targetGraphic.rectTransform;
            _buttonImage = _button.targetGraphic as Image;
        }

        protected virtual void OnDestroy()
        {
            if (_isValid)
                _button.onClick.RemoveListener(OnButtonClickInternal);
        }

        public void SetExternalCallback(Action callback)
        {
            _externalCallback = callback;
        }

        public void EnableInteraction(bool state)
        {
            _button.interactable = state;
        }

        private void OnButtonClickInternal()
        {
            OnButtonClick();

            _externalCallback?.Invoke();
        }

        protected virtual void OnButtonClick()
        {

        }
    }
}
