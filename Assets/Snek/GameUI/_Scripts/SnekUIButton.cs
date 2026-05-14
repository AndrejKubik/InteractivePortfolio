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

        private Action _externalCallback;

        protected override void Initialize()
        {
            _button = GetComponent<Button>();
        }

        protected override void Validate()
        {
            if (_button)
                _button.onClick.AddListener(OnButtonClickInternal);
            else
                FailValidation("Cannot find Button component.");
        }

        protected virtual void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClickInternal);
        }

        public void SetExternalCallback(Action callback)
        {
            _externalCallback = callback;
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
