using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets {
    public class ShipCollisionDetector : MonoBehaviour {
        public ShipController ShipControl;

        private void Awake() {
            if (ShipControl == null) {
                ShipControl = FindObjectOfType<ShipController>(true);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            Debug.Log("COLLISION! " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("Obstacle")) {
                //może wywołać to tylko jak prędkość jest większa od X (np. 0.5?)
                ShipControl.DecrementLife();
            }
        }
    }
}
