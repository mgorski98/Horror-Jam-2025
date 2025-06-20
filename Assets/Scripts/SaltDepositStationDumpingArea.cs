using UnityEngine;

namespace Assets.Scripts {
    public class SaltDepositStationDumpingArea : MonoBehaviour {
        public SaltDepositShipStation DepositStation;
        public ShipController ShipControl;

        private void Awake() {
            if (ShipControl == null)
                ShipControl = FindObjectOfType<ShipController>(true);
        }

        public void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                DepositStation.UpdateShipDumpStatus(true);
                ShipControl.CurrentDepositStation = this.DepositStation;
            }
        }

        public void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                DepositStation.UpdateShipDumpStatus(false);
                if (DepositStation == ShipControl.CurrentDepositStation)
                    ShipControl.CurrentDepositStation = null;
            }
        }
    }
}
