using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    public class FPSCounter :MonoBehaviour{
        [field: SerializeField]
        private TextMeshProUGUI FPSCounterText { get; set; }

        [field: SerializeField]
        [Tooltip("Update interval (in seconds) for fps counter display update")]
        private float UpdateInterval { get; set; }

        private void Start() {
            StartCoroutine(UpdateFPSDisplay());
        }

        private IEnumerator UpdateFPSDisplay() {
            var waitObj = new WaitForSeconds(UpdateInterval);
            while (true) {
                var fps = (int)(1f / Time.unscaledDeltaTime);
                if (FPSCounterText.gameObject.activeSelf)
                    FPSCounterText.SetText($"FPS: {fps}");
                yield return waitObj;
            }
        }
    }
}
