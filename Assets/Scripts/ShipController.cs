using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Assets.Scripts.Interactables;
using UnityEngine.InputSystem.Utilities;
using Assets.Utils;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NUnit.Framework;
using System.Collections.Generic;

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
        public CharacterController PlayerController;

        public InputActionSupplier MoveSupplier;

        public Transform ShipWheelTransform;
        public float ShipWheelRotateSpeed;

        public Camera PlayerCamera;
        public float CameraMaxYRotRange;
        public float CameraMaxXRotRange;
        public float CameraSensitivity;
        public float CameraTargetRotationSpeed;

        public Transform ShipRoot;
        public Transform ShipBase;
        public Rigidbody ShipMovementController;
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

        public ObservableValue<int> ShipHealth = new();
        public int MaxShipHealth = 5;

        public bool IsNearSaltDepositStation = false;

        private bool IsDed = false;

        private ShipDockArea CurrentDockArea;

        public UIDockingIndicator DockIndicator;

        //na potrzeby wyliczania obrażeń od kolizji
        public float VelocityDmgThreshold = 1f;
        //public Vector3 VelocityVector => CameraParentWhenPiloting.transform.forward * CurrentSpeed;
        public Vector3 VelocityVector => ShipMovementController.linearVelocity;

        public SaltDepositShipStation CurrentDepositStation;

        public bool UseWavesHeight = true;
        public WavesManager WavesMgr;

        private void OnEnable() {
            ShipMovementController.isKinematic = false;
            PlayerController.enabled = false;
        }
        private void OnDisable() {
            ShipMovementController.isKinematic = true;
            PlayerController.enabled = true;
        }
        private void Awake() {
            this.ShipHealth.OnValueChanged.AddListener((o, n) => {
                if (IsDed)
                    return;

                if (n <= 0) {
                    IsDed = true;
                    HandleGameOver();
                }
            });
        }

        private void HandleGameOver() {
            GameManager.Instance.DoGameOver();
        }

        public void OnDockingAreaEntered(ShipDockArea area) {
            this.CurrentDockArea = area;
            DockIndicator.Show();
        }

        public void OnDockingAreaExited(ShipDockArea area) {
            if (this.CurrentDockArea == area) {
                IsDocking = false;
                this.CurrentDockArea = null;
                DockingTweens.ForEach(t => t.Kill(false));
                DockingTweens.Clear();
                DockIndicator.Hide();
            }
        }

        private bool IsDocking = false;
        private bool IsDocked = false;
        private List<Tweener> DockingTweens = new();
        public void TryDocking_Action(InputAction.CallbackContext ctx) {
            if (this.CurrentDockArea == null)
                return;

            if (ctx.canceled)
                DockIndicator.UpdateFill(0);

            if (IsDocked && ctx.performed) {
                IsDocked = false;
                IsDocking = false;
                DockIndicator.SetDocked(false);
                DockIndicator.UpdateFill(0);
                return;
            }

            if (ctx.performed && !IsDocked) {
                //załączyć tweena, wyłączyć input i symulację ruchu
                IsDocking = true;
                DockingTweens.Add(ShipRoot.DOLocalMove(CurrentDockArea.DockPosition.position, CurrentDockArea.DockSpeed));
                DockingTweens.Add(ShipRoot.DOLocalRotateQuaternion(CurrentDockArea.DockPosition.rotation, CurrentDockArea.DockSpeed));
                DockingTweens[1].onComplete += () => {
                    //otworzyć drzwiczki czy tam aktywować coś co pozwoli wyjść na molo/dok
                    IsDocked = true;
                    DockIndicator.SetDocked(true);
                };

                DockingTweens[1].onUpdate += () => {
                    var progress = DockingTweens[1].position / CurrentDockArea.DockSpeed;
                    DockIndicator.UpdateFill(progress);
                };

                CurrentSpeed = 0;
                CurrentTurnSpeed = 0;
            }

            if (ctx.canceled && !IsDocked) {
                IsDocking = false;
                DockingTweens.ForEach(t => t.Kill(false));
                DockingTweens.Clear();
            }
        }

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
            transform.SetParent(ShipBase);
            PlayerCamera.transform.SetParent(transform);

            PlayerCamera.transform.DOLocalMove(OldPosition, SnapDuration);
            PlayerCamera.transform.DOLocalRotateQuaternion(OldRotation, SnapDuration).onComplete += () => this.enabled = false;
        }

        public void TakeControlOfShip() {
            FPSController.enabled = false;
            this.InteractionDetector.enabled = false;
            OldPosition = PlayerCamera.transform.localPosition;
            OldRotation = PlayerCamera.transform.localRotation;
            Input.SwitchCurrentActionMap("Ship");
            transform.SetParent(ShipRoot.transform);
            PlayerCamera.transform.SetParent(CameraParentWhenPiloting);

            PlayerCamera.transform.DOMove(ShipControlTransformPositionTemplate.position, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(ShipControlTransformPositionTemplate.rotation, SnapDuration).onComplete += () => this.enabled = true;
        }

        private void FixedUpdate() {
            //todo: trzeba będzie ogarnąć jak tą symulację robić bez wyłączania skryptu, żeby np. kolizje się obliczały jak jesteśmy na statku
            //może wystarczy po prostu flaga? i niewyłączanie shipcontrollera?
            var input = (IsDocking || IsDocked) ? Vector2.zero : MoveSupplier.Action.ReadValue<Vector2>();
            var shipRotation = input.x;
            var shipAcceleration = input.y;

            var moveDir = CameraParentWhenPiloting.transform.forward;
            ShipMovementController.AddForce(shipAcceleration * ShipSpeedModifier * moveDir, ForceMode.Force);
            ShipMovementController.AddTorque(shipRotation * ShipRotationSpeedModifier * Vector3.up, ForceMode.Force);
            if (UseWavesHeight)
                ShipMovementController.transform.position = ShipMovementController.transform.position.Copy(
                    overrideY: WavesMgr.GetVerticalPositionFromPoint(ShipMovementController.transform.position)
                );

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

        public void DecrementLife() {
            ShipHealth.Value--;
        }
    }
}
