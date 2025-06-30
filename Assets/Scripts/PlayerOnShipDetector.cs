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
        public UIDockingIndicator DockIndicator;
        public Transform CheckOriginTransform;
        public float CheckRange;
        public LayerMask CheckMask;

        private void Awake() {
            if (Player == null)
                Player = GameObject.FindWithTag("Player").transform;
        }

        private void Update() {
            if (Physics.Raycast(CheckOriginTransform.position, Vector3.down, CheckRange, CheckMask)) {
                Player.transform.SetParent(ShipRoot);
            }
            else {
                Player.transform.SetParent(null);
            }
        }
    }
}
