using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerOnShipDetector : MonoBehaviour {
        public Transform ShipRoot;
        public Transform Player;

        private void Awake() {
            if (Player == null)
                Player = GameObject.FindWithTag("Player").transform;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                Player.transform.SetParent(ShipRoot);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                Player.transform.SetParent(null);
            }
        }
    }
}
