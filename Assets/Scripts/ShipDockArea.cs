using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts {
    public class ShipDockArea : MonoBehaviour {
        public Transform DockPosition;
        public float DockSpeed;
        public Ease DockingEaseFunc;

        public ShipController SController;

        private void Awake() {
            if (SController == null)
                SController = FindObjectOfType<ShipController>(true);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Ship")) {
                SController.OnDockingAreaEntered(this);
                Debug.Log("Docking entered");
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Ship")) {
                SController.OnDockingAreaExited(this);
                Debug.Log("Docking exited");
            }
        }
    }
}
