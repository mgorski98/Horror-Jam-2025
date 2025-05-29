using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Assets.Scripts {
    public class ShipController : MonoBehaviour {
        private Vector3 OldPosition;
        private Quaternion OldRotation;

        public float SnapDuration = 0.25f;
        public Transform ShipControlTransformPositionTemplate;
        public FPSPlayerController FPSController;
        public InteractionDetector InteractionDetector;
        public PlayerInput Input;

        public InputActionSupplier MoveSupplier;

        public Transform ShipWheelTransform;
        public float ShipWheelRotateSpeed;

        public Camera PlayerCamera;

        public Transform ShipRoot;
        public CharacterController ShipRBody;

        public float ShipSpeedModifier = 10;
        public float ShipRotationSpeedModifier = 10;
        public float MaxSpeed;
        public float Acceleration;
        public float IdleShipDrag;
        public float MaxRotationSpeed;
        public float RotationIdleDrag;
        public float CurrentSpeed;
        public float CurrentTurnSpeed;

        public void StopControllingShip_Action(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            if (this.enabled == false)
                return;

            StopControllingShip();
        }

        public void StopControllingShip() {
            //todo przypisac nowego parenta (brak), o ile w ogole. moze przypisac na starcie?
            this.enabled = false;
            FPSController.enabled = true;
            this.InteractionDetector.enabled = true;
            Input.SwitchCurrentActionMap("Player");
            transform.SetParent(null);
            PlayerCamera.transform.SetParent(transform);

            PlayerCamera.transform.DOMove(OldPosition, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(OldRotation, SnapDuration);
        }

        public void TakeControlOfShip() {
            //todo jak to nie zadziala to albo rpzesunac sama kamere albo wylaczyc collidery i grawitacje
            //todo przypisac nowego parenta (statek)
            //todo wyłączyć interaction detector
            this.enabled = true;
            FPSController.enabled = false;
            this.InteractionDetector.enabled = false;
            OldPosition = PlayerCamera.transform.position;
            OldRotation = PlayerCamera.transform.rotation;
            Input.SwitchCurrentActionMap("Ship");
            transform.SetParent(ShipRoot.transform);
            PlayerCamera.transform.SetParent(ShipRoot.transform);


            PlayerCamera.transform.DOMove(ShipControlTransformPositionTemplate.position, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(ShipControlTransformPositionTemplate.rotation, SnapDuration);

        }

        private void FixedUpdate() {
            var input = MoveSupplier.Action.ReadValue<Vector2>();
            var shipRotation = input.x;
            var shipAcceleration = input.y;

            CurrentSpeed += shipAcceleration * Time.fixedDeltaTime * (ShipSpeedModifier);
            if (Mathf.Approximately(0f, shipAcceleration)) {
                if (CurrentSpeed > 0) {
                    CurrentSpeed -= IdleShipDrag * Time.fixedDeltaTime;
                } else if (CurrentSpeed < 0) {
                    CurrentSpeed += IdleShipDrag * Time.fixedDeltaTime;
                }
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, -MaxSpeed, MaxSpeed);

            //only rotate ship when there is the vertical input (forward/backward)
            if (Mathf.Abs(shipAcceleration) > 0) {
                CurrentTurnSpeed += shipRotation * Time.fixedDeltaTime * ShipRotationSpeedModifier;
            }
            if (Mathf.Approximately(0f, shipRotation)) {
                if (CurrentTurnSpeed > 0) {
                    CurrentTurnSpeed -= RotationIdleDrag * Time.fixedDeltaTime;
                } else if (CurrentTurnSpeed < 0) {
                    CurrentTurnSpeed += RotationIdleDrag * Time.fixedDeltaTime;
                }
            }

            CurrentTurnSpeed = Mathf.Clamp(CurrentTurnSpeed, -MaxRotationSpeed, MaxRotationSpeed);

            var boatDirection = PlayerCamera.transform.forward;
            ShipRBody.Move(CurrentSpeed * Time.fixedDeltaTime * boatDirection);
            ShipRBody.transform.rotation = (ShipRBody.transform.rotation * Quaternion.Euler(0, CurrentTurnSpeed, 0));

            ShipWheelTransform.Rotate(shipRotation * ShipWheelRotateSpeed * Time.fixedDeltaTime * Vector3.up);
        }
    }
}
