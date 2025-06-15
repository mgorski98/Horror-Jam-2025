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
        public float MinContactDamage = 0;
        public float MaxContactDamage = 7.5f;

        private void Awake() {
            if (ShipControl == null) {
                ShipControl = FindObjectOfType<ShipController>(true);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            //todo: damage the ship based on velocity
            if (collision.gameObject.CompareTag("Obstacle")) {
                var contact = collision.GetContact(0);
                var dirToObstacle = (collision.gameObject.transform.position - transform.position).normalized;
                var newVelocity = Vector3.Reflect(dirToObstacle, contact.normal);
                //todo: nadpisać wektor movementu w ShipControllerze
                //todo: odjąć hp statku
            }
        }
    }
}
