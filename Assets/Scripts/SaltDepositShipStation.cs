using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    public class SaltDepositShipStation : MonoBehaviour {
        public int NeededSaltInGrams = 0;
        public bool IsDone => NeededSaltInGrams <= 0;

        public ShipController PlayerShip;

        public SaltStorageData SaltStorage;

        public TMP_Text NeededSaltText;

        //todo: albo ustawić potrzebną sól obok guzika, albo gdzieś obok stacji. może niech wsysa sól z ładowni duża rura nad nami po kliknięciu?
        public void UpdateShipDumpStatus(bool entered) {
            if (entered) {
                PlayerShip.DumpButton.OpenButtonGuard();
                NeededSaltText.text = $"Salt needed:\n{NeededSaltInGrams / 1000} kg";
            } else {
                PlayerShip.DumpButton.CloseButtonGuard();
                NeededSaltText.text = string.Empty;
            }
            PlayerShip.IsNearSaltDepositStation = entered;
        }
    }
}
