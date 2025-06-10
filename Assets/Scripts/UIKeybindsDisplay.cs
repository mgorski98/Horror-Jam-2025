using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Assets.Scripts {
    [System.Serializable]
    public class InputMapShowActionsData {
        public string ActionMapName;
        public string[] ActionNames;
    }
    public class UIKeybindsDisplay : MonoBehaviour {
        public PlayerInput Input;
        public InputActionAsset ActionsStore;
        public List<InputMapShowActionsData> ActionsToDisplay;
        public TMP_Text BindingsText;
        private string CurrentActionMapName;

        public string GlobalActionMapName;
        public string[] GlobalActionsToDisplay;

        private void Awake() {
            if (Input == null)
                Input = FindAnyObjectByType<PlayerInput>(FindObjectsInactive.Include);
        }

        private void Start() {
            CurrentActionMapName = Input.currentActionMap.name;
            UpdateUIBindingDisplay();
        }

        private void Update() {
            if (Input.currentActionMap.name != CurrentActionMapName) {
                CurrentActionMapName = Input.currentActionMap.name;
                UpdateUIBindingDisplay();
            }
        }

        private void UpdateUIBindingDisplay() {
            var data = ActionsToDisplay.Find(a => a.ActionMapName == CurrentActionMapName);
            if (data != null) {
                var sb = new StringBuilder();

                foreach (var name in GlobalActionsToDisplay) {
                    var action = ActionsStore.FindAction($"{GlobalActionMapName}/{name}");
                    sb.AppendLine($"[{action.GetBindingDisplayString()}] {action.name}");
                }

                foreach (var actionName in data.ActionNames) {
                    var fullPath = $"{data.ActionMapName}/{actionName}";
                    var action = ActionsStore.FindAction(fullPath);

                    sb.AppendLine($"[{action.GetBindingDisplayString()}] {action.name}");
                }

                BindingsText.text = sb.ToString();
            }
        }
    }
}
