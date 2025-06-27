using UnityEngine;

namespace Assets.Scripts {
    public class SaltDepositStationDumpingArea : MonoBehaviour {
        public SaltDepositShipStation DepositStation;
        public ShipController ShipControl;
        public Transform Ship;

        public GameObject AreaIndicatorGizmoObject;
        public float AreaIndicatorShowDistance;

        private bool PlayerEnteredArea;

        private void Awake() {
            if (ShipControl == null)
                ShipControl = FindObjectOfType<ShipController>(true);
        }

        public void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                DepositStation.UpdateShipDumpStatus(true);
                ShipControl.CurrentDepositStation = this.DepositStation;
            }
            if (other.gameObject.CompareTag("Ship")) {
                PlayerEnteredArea = true;
                AreaIndicatorGizmoObject.SetActive(false);
            }
        }

        public void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                DepositStation.UpdateShipDumpStatus(false);
                if (DepositStation == ShipControl.CurrentDepositStation)
                    ShipControl.CurrentDepositStation = null;
            }

            if (other.gameObject.CompareTag("Ship")) {
                PlayerEnteredArea = false;
            }
        }

        private void Update() {
            if (Ship == null)
                return;

            if (Vector3.Distance(Ship.position, transform.position) <= AreaIndicatorShowDistance && !PlayerEnteredArea) {
                AreaIndicatorGizmoObject.SetActive(true);
            }
            else {
                AreaIndicatorGizmoObject.SetActive(false);
            }
        }
    }
}
