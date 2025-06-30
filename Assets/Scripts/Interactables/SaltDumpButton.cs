using System;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Interactables {
    //todo: w miejscu, w którym ustawimy flagę że gracz jest w tej strefie depozytu soli trzeba też odpalić animację zamknięcia/otwarcia pojemnika z guziczkiem czy cośtakiego
    //ten pojemnik to może być skrzynka z zasuwanymi drzwiczkami
    public class SaltDumpButton : InteractableObject {
        public ShipController ShipControl;
        public SaltStorageData SaltStorageRef;
        public GameObject ButtonGuard;

        public float PressDuration = 0.4f;

        private Vector3 DefaultLocalPos;
        [SerializeField]
        private Vector3 PressOffset;
        private bool IsBeingPressed = false;

        private void Awake() {
            if (ShipControl == null)
                ShipControl = FindAnyObjectByType<ShipController>(UnityEngine.FindObjectsInactive.Include);

            if (SaltStorageRef == null)
                SaltStorageRef = FindAnyObjectByType<SaltStorageData>(UnityEngine.FindObjectsInactive.Include);

            DefaultLocalPos = this.transform.localPosition;
        }

        public override void DoInteract() {
            if (ShipControl.IsNearSaltDepositStation == false)
                return;

            AnimateButtonPress();
            if (this.SaltStorageRef.CurrentStoredSalt.Value >= ShipControl.CurrentDepositStation.NeededSaltInKg) {
                this.SaltStorageRef.CurrentStoredSalt.Value -= ShipControl.CurrentDepositStation.NeededSaltInKg;
                ShipControl.CurrentDepositStation.NeededSaltInKg = 0;
            }
            else {
                ShipControl.CurrentDepositStation.NeededSaltInKg -= this.SaltStorageRef.CurrentStoredSalt.Value;
                this.SaltStorageRef.CurrentStoredSalt.Value = 0;
            }
            ShipControl.CurrentDepositStation.UpdateProgress();
        }

        public override string GetInteractionName() {
            return ShipControl.IsNearSaltDepositStation == false ? "" : InteractionName;
        }

        public override bool ShouldShowBindingKey() {
            return ShipControl.IsNearSaltDepositStation;
        }

        public void OpenButtonGuard() {
            ButtonGuard.SetActive(false);
        }

        public void CloseButtonGuard() {
            ButtonGuard.SetActive(true);
        }

        public void AnimateButtonPress() {
            IsBeingPressed = true;

            this.transform.DOLocalMove(DefaultLocalPos - PressOffset, PressDuration).onComplete += () => {
                this.transform.DOLocalMove(DefaultLocalPos, PressDuration).onComplete += () => {
                    IsBeingPressed = false;
                };
            };
        }

        public override void StopInteract() {
        }
    }
}
