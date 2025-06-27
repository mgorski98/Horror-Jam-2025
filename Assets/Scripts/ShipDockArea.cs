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

        public Transform Ship;

        public GameObject DockAreaIndicatorGizmo;
        public float ShowDockGizmoDistance;

        public ShipController SController;

        private bool ShipInArea;

        private void Awake() {
            if (SController == null)
                SController = FindObjectOfType<ShipController>(true);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Ship")) {
                SController.OnDockingAreaEntered(this);
                Debug.Log("Docking entered");
                ShipInArea = true;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Ship")) {
                SController.OnDockingAreaExited(this);
                Debug.Log("Docking exited");
                ShipInArea = false;
            }
        }

        private void Update() {
            if (Ship == null)
                return;

            if (!SController.IsDocked && !ShipInArea && Vector3.Distance(Ship.position.ToFlatXZ(), DockAreaIndicatorGizmo.transform.position.ToFlatXZ()) <= ShowDockGizmoDistance) {
                this.DockAreaIndicatorGizmo.SetActive(true);
            } else this.DockAreaIndicatorGizmo.SetActive(false);
        }
    }
}
