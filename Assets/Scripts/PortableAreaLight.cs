using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts {
    public class PortableAreaLight : MonoBehaviour {
        [SerializeField]
        private Animator MoveAnimator;
        [SerializeField]
        private Light AreaLight;

        public bool IsLightActive = false;

        public void ToggleLight_Action(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            IsLightActive = !IsLightActive;
            AreaLight.gameObject.SetActive(IsLightActive);
        }
    }
}
