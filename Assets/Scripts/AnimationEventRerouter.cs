using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts {
    public class AnimationEventRerouter : MonoBehaviour {
        public UnityEvent Callbacks;

        public void Execute() {
            Callbacks.Invoke();
        }
    }
}
