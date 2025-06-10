using UnityEngine;

namespace Assets.Scripts {
    public class SaltDepositStationDumpingArea : MonoBehaviour {
        public SaltDepositShipStation DepositStation;

        public void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player"))
                DepositStation.UpdateShipDumpStatus(true);
        }

        public void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Player"))
                DepositStation.UpdateShipDumpStatus(false);
        }
    }
}
