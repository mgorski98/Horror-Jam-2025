using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets {
    public class ShipCollisionDetector : MonoBehaviour {
        public ShipController ShipControl;

        //zeby przechowywac kolizje i nie odpalaly sie dzieki temu kilka razy w ciagu jednej klatki
        private Dictionary<int, GameObject> Collisions = new();

        private void Awake() {
            if (ShipControl == null) {
                ShipControl = FindObjectOfType<ShipController>(true);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            Debug.Log("COLLISION! " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("Obstacle")) {
                //jak prędkość jest za mała to olewamy
                if (ShipControl.VelocityVector.magnitude < ShipControl.VelocityDmgThreshold)
                    return;
                //jak juz w ciagu klatki weszlismy z tym w kolizje to olewamy
                if (Collisions.TryGetValue(collision.gameObject.GetInstanceID(), out var _))
                    return;
                ShipControl.DecrementLife();
                Collisions[collision.gameObject.GetInstanceID()] = collision.gameObject;
                StartCoroutine(ClearCollisionEntry(collision.gameObject.GetInstanceID()));

                //todo: play hit sound
                //todo: shake camera, fade in a red overlay, etc.
            }
        }

        private IEnumerator ClearCollisionEntry(int instanceId) {
            yield return new WaitForSeconds(0.5f);
            Collisions.Remove(instanceId);
        }
    }
}
