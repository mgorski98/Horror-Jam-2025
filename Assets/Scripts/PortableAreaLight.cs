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
        [SerializeField]
        private Transform LampRoot;

        public bool IsLightActive = false;

        public void ToggleLight_Action(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            //todo: dodać sprawdzanie czy gra jest spauzowana, jak tak to wyjść

            //todo: zasymulować kaganek - użyć jointa, żeby ładnie dyndało może?
            IsLightActive = !IsLightActive;
            LampRoot.gameObject.SetActive(IsLightActive);
        }
    }
}
