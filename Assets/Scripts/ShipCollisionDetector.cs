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
            //todo: damage the ship based on velocity
            //todo: set up layer collisions correctly in project settings (ship on its own layer and obstacles on their own layer)
            //todo: push the ship in the direction opposite to impact, with force based on velocity
            //todo: calculate velocity vector in ship controller
        }
    }
}
