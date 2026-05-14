using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    internal static class SnekEditorAlertManager
    {
        private static List<SnekEditorAlert> _activeEditorAlerts;

        [InitializeOnLoadMethod]
        private static void InitializePopupList()
        {
            _activeEditorAlerts = new List<SnekEditorAlert>();
        }

        internal static void ShowAlert(SnekEditorAlert alert)
        {
            if (!alert.IsOpenedCorrectly)
            {
                Debug.LogError(
                    $"Invalid method used for showing editor alert\n" +
                    $"Please use: {nameof(SnekGUIUtility)}.{nameof(SnekGUIUtility.ShowEditorAlert)}");

                return;
            }

            alert.ShowUtility();

            _activeEditorAlerts.Add(alert);

            OnActiveEditorAlertsListChange();
        }

        internal static void CloseAlert(SnekEditorAlert alert)
        {
            _activeEditorAlerts.Remove(alert);

            OnActiveEditorAlertsListChange();

            alert.OnCloseCallback?.Invoke();
            alert.Close();
        }

        internal static int GetActiveAlertCount()
        {
            if (_activeEditorAlerts == null)
                return 0;

            return _activeEditorAlerts.Count;
        }

        internal static SnekEditorAlert GetOldestAlert()
        {
            if(_activeEditorAlerts == null || _activeEditorAlerts.Count < 1)
            {
                Debug.LogError("Trying to get oldest alert while none are cached.");

                return null;
            }

            return _activeEditorAlerts[0];
        }

        private static void OnActiveEditorAlertsListChange()
        {
            if (_activeEditorAlerts != null && _activeEditorAlerts.Count > 0)
                _activeEditorAlerts[0].Focus();
        }
    }
}
