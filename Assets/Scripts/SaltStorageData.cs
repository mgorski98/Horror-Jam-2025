using Assets.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    //note: nie wiem czy coś tutaj jeszcze dodać, zobaczymy
    public class SaltStorageData : MonoBehaviour {
        [Tooltip("How much salt do we currently have stored, in grams")]
        public ObservableValue<int> CurrentStoredSalt = new(0);
        [Tooltip("How much can we store")]
        public int MaxSaltStorage;

        public bool IsFull => CurrentStoredSalt >= MaxSaltStorage;

        [Min(0), Tooltip("Max value of how much to slow down the player ship due to stored salt")]
        public float MaxSaltEncumbranceSlowDown;
        public float CurrentSlowDownValue => Mathf.Lerp(0, MaxSaltEncumbranceSlowDown, CurrentStoredSalt / (float)MaxSaltStorage);

        //todo: wskaźnik zapelnienia niech będzie w świecie gry - doczepiony do statku, trochę jak miernik ciśnienia z paskiem zapełnienia obok
        //depozyt soli w stacjach może wyglądać tak, że stacja ma określone miejsce gdzie trzeba wpłynąć i jak tam wpłyniemy to odblokowywuje się przycisk do zdeponowania soli
        //todo: ewentualnie zamiast używania img można użyć cube'a, któremu się zmienia skalę od 0 do 1 jak to nie bedzie wyglądać ok
        //public Image SaltStorageTakenProgressBar;
        public Transform BarometerArmRotationTransform;
        public Vector3 ZeroBarometerRotation;
        public Vector3 MaxBarometerRotation;
        public float BarometerPointerOscillationStrength = 0.1f;

        //todo: backgorund tego można będzie zrobić w tym shaderze: https://www.youtube.com/watch?v=V5h2ClMUguQ , używając sprite renderera

        private void Awake() {
            CurrentStoredSalt.OnValueChanged.AddListener((oldval, newval) => {
                var ratio = Mathf.Clamp01(CurrentStoredSalt.Value / (float)MaxSaltStorage);

                //SaltStorageTakenProgressBar.fillAmount = ratio;
                BarometerArmRotationTransform.localRotation = Quaternion.Euler(Vector3.Lerp(ZeroBarometerRotation, MaxBarometerRotation, ratio));
            });
        }

        public void AddSalt(int salt) {
            var totalSalt = CurrentStoredSalt + salt;
            if (totalSalt > MaxSaltStorage)
                totalSalt = MaxSaltStorage;

            CurrentStoredSalt.Value = totalSalt;
        }
    }
}
