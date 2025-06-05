using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Assets.Scripts {
    public class InputActionSupplier : MonoBehaviour {
        public InputActionAsset Actions;
        public string ActionName;

        private InputAction m_Action;

        public InputAction Action => m_Action;

        private void Awake() {
            this.m_Action = Actions.FindAction(ActionName);
        }
    }
}
