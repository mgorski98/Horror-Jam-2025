using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class SaltDepositShipStation : MonoBehaviour {
        public int NeededSaltInKg = 0;
        public bool IsDone => NeededSaltInKg <= 0;

        private int NeededSalt_Snapshot;

        public ShipController PlayerShip;

        public SaltStorageData SaltStorage;

        public TMP_Text NeededSaltText;

        public Light[] StatusLights;

        public TMP_Text ProgressText;
        public Image ProgressBar;

        private void Awake() {
            NeededSalt_Snapshot = NeededSaltInKg;
        }

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

        public void UpdateProgress() {
            var ratio = (float)NeededSaltInKg / NeededSalt_Snapshot;
            ProgressBar.fillAmount = 1f - ratio;
            ProgressText.text = $"{Mathf.RoundToInt(100 * (1f - ratio))}%";

            NeededSaltText.text = NeededSaltInKg <= 0 ? "All done!" : $"Salt needed:\n{NeededSaltInKg} kg";
        }
    }
}
