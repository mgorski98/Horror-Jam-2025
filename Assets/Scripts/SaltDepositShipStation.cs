using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    public class SaltDepositShipStation : MonoBehaviour {
        public int NeededSaltInKg = 0;
        public bool IsDone => NeededSaltInKg <= 0;

        public ShipController PlayerShip;

        public SaltStorageData SaltStorage;

        public TMP_Text NeededSaltText;

        public Light[] StatusLights;

        //todo: albo ustawić potrzebną sól obok guzika, albo gdzieś obok stacji. może niech wsysa sól z ładowni duża rura nad nami po kliknięciu?
        public void UpdateShipDumpStatus(bool entered) {
            if (entered) {
                PlayerShip.DumpButton.OpenButtonGuard();
                NeededSaltText.text = $"Salt needed:\n{NeededSaltInKg} kg";
            } else {
                PlayerShip.DumpButton.CloseButtonGuard();
                NeededSaltText.text = string.Empty;
            }
            PlayerShip.IsNearSaltDepositStation = entered;
        }

        public void Update() {
            if (IsDone)
                foreach (var item in StatusLights)
                {
                    item.color = Color.green;
                }
        }
    }
}
