using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekEditorAlert : SnekWindow
    {
        private const float WindowWidth = 400f;
        private const float WindowHeight = 250f;

        private const float ButtonWidth = 100f;
        private const float ButtonHeight = 50f;

        public string Message;
        public Action OnCloseCallback;

        public bool IsOpenedCorrectly = false;

        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;

        protected override void OnCreateWindowInstance()
        {
            minSize = new Vector2(WindowWidth, WindowHeight);
            maxSize = new Vector2(WindowWidth, WindowHeight);

            position = new Rect(position)
            {
                center = EditorGUIUtility.GetMainWindowPosition().center
            };
        }

        private bool IsUserInputDetected()
        {
            var currentEvent = Event.current;

            if (currentEvent == null)
                return false;

            return EditorApplication.isFocused
                || currentEvent.type == EventType.MouseDown
                || currentEvent.type == EventType.MouseUp
                || currentEvent.type == EventType.MouseDrag
                || currentEvent.type == EventType.KeyDown
                || currentEvent.type == EventType.KeyUp
                || currentEvent.type == EventType.ScrollWheel;
        }

        private bool IsOldestAlert()
        {
            return SnekEditorAlertManager.GetOldestAlert() == this;
        }

        private bool IsLeftOverAfterRecompile()
        {
            return SnekEditorAlertManager.GetActiveAlertCount() < 1;
        }

        private void InitializeStyles()
        {
            if (_labelStyle == null)
                _labelStyle = SnekGUIStyles.BoldLabel(16, TextAnchor.MiddleCenter, true, true, 20, true);

            if (_buttonStyle == null)
                _buttonStyle = SnekGUIStyles.BoldTextButton(20);
        }



        protected override void DrawContent()
        {
            InitializeStyles();

            if (IsLeftOverAfterRecompile())
                Close();

            if (IsOldestAlert() && IsUserInputDetected() && focusedWindow != this)
                SnekEditorAlertManager.CloseAlert(this);

            DrawMessage();

            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                DrawCloseButton();

            GUILayout.Space(20f);
        }

        private void DrawMessage()
        {
            using (new SnekGUIColoredScope(_layoutSettings.GetContentColor()))
                GUILayout.Box(Message, _labelStyle);
        }

        private void DrawCloseButton()
        {
            bool buttonClicked = GUILayout.Button(
                "Close",
                _buttonStyle,
                GUILayout.Width(ButtonWidth),
                GUILayout.Height(ButtonHeight));

            if (buttonClicked)
                SnekEditorAlertManager.CloseAlert(this);
        }
    }
}