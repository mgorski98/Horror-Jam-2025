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
        public PlayerInput Input;

        public InputActionSupplier MoveSupplier;

        public float ShipSpeedModifier = 10;
        public float ShipRotationSpeedModifier = 10;
        public float MaxSpeed;

        public Transform ShipWheelTransform;
        public float ShipWheelRotateSpeed;

        public Camera PlayerCamera;

        public Transform ShipRoot;
        public Rigidbody ShipRBody;

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
            Input.SwitchCurrentActionMap("Player");
            transform.SetParent(null);
            PlayerCamera.transform.SetParent(transform);

            PlayerCamera.transform.DOMove(OldPosition, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(OldRotation, SnapDuration);

        }

        public void TakeControlOfShip() {
            //todo jak to nie zadziala to albo rpzesunac sama kamere albo wylaczyc collidery i grawitacje
            //todo przypisac nowego parenta (statek)
            this.enabled = true;
            FPSController.enabled = false;
            Input.SwitchCurrentActionMap("Ship");
            transform.SetParent(ShipRoot.transform);
            PlayerCamera.transform.SetParent(ShipRoot.transform);

            OldPosition = transform.position;
            OldRotation = transform.rotation;

            PlayerCamera.transform.DOMove(ShipControlTransformPositionTemplate.position, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(ShipControlTransformPositionTemplate.rotation, SnapDuration);

        }

        private void FixedUpdate() {
            var input = MoveSupplier.Action.ReadValue<Vector2>();
            var shipRotation = input.x;
            var shipAcceleration = input.y;

            //not ideal
            ShipRBody.AddForce(ShipSpeedModifier * shipAcceleration * PlayerCamera.transform.forward, ForceMode.Force);
            ShipRBody.AddTorque(ShipRotationSpeedModifier * shipRotation * ShipRBody.transform.up, ForceMode.Force);

            ShipWheelTransform.Rotate(shipRotation * ShipWheelRotateSpeed * Time.fixedDeltaTime * Vector3.up);
        }
    }
}
