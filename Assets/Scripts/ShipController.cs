using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Assets.Scripts.Interactables;

namespace Assets.Scripts {
    public class ShipController : MonoBehaviour {
        private Vector3 OldPosition;
        private Quaternion OldRotation;

        public float SnapDuration = 0.25f;
        public Transform ShipControlTransformPositionTemplate;
        public FPSPlayerController FPSController;
        public InteractionDetector InteractionDetector;
        public PlayerInput Input;
        public SaltDumpButton DumpButton;

        public InputActionSupplier MoveSupplier;

        public Transform ShipWheelTransform;
        public float ShipWheelRotateSpeed;

        public Camera PlayerCamera;
        public float CameraMaxYRotRange;
        public float CameraMaxXRotRange;
        public float CameraSensitivity;
        public float CameraTargetRotationSpeed;

        public Transform ShipRoot;
        public CharacterController ShipRBody;
        public Transform CameraParentWhenPiloting;

        public float ShipSpeedModifier = 10;
        public float ShipRotationSpeedModifier = 10;
        public float MaxSpeed;
        public float Acceleration;
        public float IdleShipDrag;
        public float MaxRotationSpeed;
        public float RotationIdleDrag;
        public float CurrentSpeed;
        public float CurrentTurnSpeed;

        public AudioSource HonkAudioSource;
        public AudioClip[] Honks;

        public Light[] ShipLights;

        public float ShipHealth;
        public float MaxShipHealth;

        public bool IsNearSaltDepositStation = false;

        //na potrzeby wyliczania obrażeń od kolizji
        public Vector3 VelocityVector => ShipRBody.transform.forward * CurrentSpeed;

        public void StopControllingShip_Action(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            if (this.enabled == false)
                return;

            StopControllingShip();
        }

        public void StopControllingShip() {
            FPSController.enabled = true;
            this.InteractionDetector.enabled = true;
            Input.SwitchCurrentActionMap("Player");
            transform.SetParent(null);
            PlayerCamera.transform.SetParent(transform);

            PlayerCamera.transform.DOLocalMove(OldPosition, SnapDuration);
            PlayerCamera.transform.DOLocalRotateQuaternion(OldRotation, SnapDuration).onComplete += () => this.enabled = false;
        }

        public void TakeControlOfShip() {
            FPSController.enabled = false;
            this.InteractionDetector.enabled = false;
            //todo: zapisac lokalna pozycje kamery w obiekcie gracza i tam ja przywrocic wtedy, bo obecnie zle sie ta pozycja przywraca
            OldPosition = PlayerCamera.transform.localPosition;
            OldRotation = PlayerCamera.transform.localRotation;
            Input.SwitchCurrentActionMap("Ship");
            transform.SetParent(ShipRoot.transform);
            PlayerCamera.transform.SetParent(CameraParentWhenPiloting);

            PlayerCamera.transform.DOMove(ShipControlTransformPositionTemplate.position, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(ShipControlTransformPositionTemplate.rotation, SnapDuration).onComplete += () => this.enabled = true;
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

            var additionalMultiplier = 1f;
            if ((CurrentTurnSpeed < 0 && shipRotation > 0) || (CurrentTurnSpeed > 0 && shipRotation < 0)) {
                additionalMultiplier = 2f;
            }
            CurrentTurnSpeed += shipRotation * additionalMultiplier * Time.fixedDeltaTime * ShipRotationSpeedModifier;
            if (Mathf.Approximately(0f, shipRotation)) {
                if (CurrentTurnSpeed > 0) {
                    CurrentTurnSpeed -= RotationIdleDrag * Time.fixedDeltaTime;
                } else if (CurrentTurnSpeed < 0) {
                    CurrentTurnSpeed += RotationIdleDrag * Time.fixedDeltaTime;
                }
            }

            CurrentTurnSpeed = Mathf.Clamp(CurrentTurnSpeed, -MaxRotationSpeed, MaxRotationSpeed);

            var boatDirection = CameraParentWhenPiloting.transform.forward;
            ShipRBody.Move(CurrentSpeed * Time.fixedDeltaTime * boatDirection);
            ShipRBody.transform.rotation = (ShipRBody.transform.rotation * Quaternion.Euler(0, CurrentTurnSpeed, 0));

            ShipWheelTransform.Rotate(shipRotation * ShipWheelRotateSpeed * Time.fixedDeltaTime * Vector3.up);
        }

        public void HONK(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            if ((Honks?.Length ?? 0) <= 0)
                return;

            HonkAudioSource.clip = Honks[Random.Range(0, Honks.Length)];
            HonkAudioSource.Play();
        }


        private float xRotAcc = 0;
        private float yRotAcc = 0;

        private Vector2 CameraInput;
        public void ToggleShipLights(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            if (ShipLights == null || ShipLights.Length <= 0)
                return;

            //todo: dodać check czy gra jest spauzowana - jak tak to wyjść

            foreach (var light in ShipLights) {
                light.gameObject.SetActive(!light.gameObject.activeSelf);
            }
        }

        public void UpdateCameraRotation(InputAction.CallbackContext ctx) {
            if (ctx.canceled)
                return;

            CameraInput = CameraSensitivity * Time.deltaTime * ctx.ReadValue<Vector2>();
            xRotAcc += -CameraInput.y;
            yRotAcc += CameraInput.x;

            xRotAcc = Mathf.Clamp(xRotAcc, -CameraMaxXRotRange, CameraMaxXRotRange);
            yRotAcc = Mathf.Clamp(yRotAcc, - CameraMaxYRotRange, CameraMaxYRotRange);
        }

        private void LateUpdate() {
            PlayerCamera.transform.localRotation = Quaternion.RotateTowards(PlayerCamera.transform.localRotation, Quaternion.Euler(xRotAcc, yRotAcc, 0), Time.deltaTime * CameraTargetRotationSpeed);
        }
    }
}
