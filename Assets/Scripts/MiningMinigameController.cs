using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using Assets.Utils;

namespace Assets.Scripts {
    public class MiningMinigameController : MonoBehaviour {
        private SaltDeposit Deposit;

        public AudioSource MiningSoundsAudioSource;
        public AudioClip[] MiningClips;
        public AudioClip[] FailedHitClips;

        [SerializeField, Tooltip("how much percentage of a rock to chip with each hit")]
        private float MiningStrength;
        [SerializeField]
        private PlayerInput Input;
        [SerializeField]
        private float SnapDuration = 0.2f;
        [SerializeField]
        private float FailedHitStopDuration = 0.5f;
        [SerializeField]
        private float IndicatorMoveSpeed = 1f;
        [SerializeField]
        private float MinigameFadeInDuration = 0.2f;
        [SerializeField]
        private float MiningProgressDecaySpeed = 1f;

        [SerializeField]
        private Camera PlayerCamera;
        [SerializeField]
        private FPSPlayerController FPSController;
        [SerializeField]
        private InteractionDetector InteractDetector;
        [SerializeField]
        private SaltStorageData SaltStorage;

        public SaltStorageData StorageRef => SaltStorage;

        [SerializeField]
        private RectTransform MainBarTransform;
        [SerializeField]
        private RectTransform IndicatorSpawnPosition;
        [SerializeField]
        private RectTransform HitDetectionTransform;
        [SerializeField]
        private Vector2 IndicatorMoveDirection = Vector2.left;
        [SerializeField]
        private GameObject SaltCrystalIndicatorPrefab;
        [SerializeField]
        private Image MiningProgressBar;
        [SerializeField]
        private CanvasGroup MinigameCanvasGroup;

        public float[] SpawnIntervals;

        //todo: ustawić te rzeczy (może też dodać parametry dla kamery i gracza osobno?)
        private Vector3 OldPosition;
        private Quaternion OldRotation;

        private List<GameObject> Indicators = new();

        public ObservableValue<float> MiningProgress = new(0);

        private float UnsuccessfulHitTimer = 0;
        private float SpawnSaltIndicatorTimer = 0;

        public Animator PickaxeAnimator;
        public GameObject Pickaxe;
        [SerializeField]
        private Vector3 PickaxeStartLocalPos;
        [SerializeField]
        private Vector3 PickaxeHiddenOffset;
        private Vector3 PickaxeHiddenPos => PickaxeHiddenOffset + PickaxeStartLocalPos;
        private readonly static int HIT_ANIM = Animator.StringToHash("HIT_SUCCESS");
        private readonly static int HIT_FAIL_ANIM = Animator.StringToHash("HIT_FAIL");

        private void Awake() {
            Pickaxe.transform.localPosition = PickaxeHiddenPos;
            MiningProgress.OnValueChanged.AddListener((old, new_) => {
                if (this.Deposit == null)
                    return;

                var ratio = Mathf.Clamp01(new_ / 100f);
                MiningProgressBar.fillAmount = ratio;
                for (int i = 0; i < Deposit.SaltCrystalsToShrink.Length; ++i) {
                    Deposit.SaltCrystalsToShrink[i].localScale = Deposit.StartingCrystalScaleValues[i] * (1 - ratio);
                }

                if (ratio >= 1f) {
                    FinishMining();
                }
            });
        }

        public void InitMining(SaltDeposit depositToMine) {
            this.Deposit = depositToMine;

            var targetSpot = depositToMine.GetClosestMiningSpot(); //pozycja i rotacja z której wydobywamy
            FPSController.enabled = false;
            InteractDetector.enabled = false;
            Input.SwitchCurrentActionMap("Mining");
            //musimy kliknąć lewy myszki kiedy kryształ jest w tym wewnętrznym pasku żeby wydobyć sól, nietrafienie oznacza brak postępu i chwilową niemożność uderzenia ponownie (jakieś 0.5 sekundy np.)
            var directionToDeposit = Quaternion.LookRotation((depositToMine.transform.position - targetSpot.position).normalized, Vector3.up);
            OldPosition = PlayerCamera.transform.position;
            OldRotation = PlayerCamera.transform.rotation;
            MiningProgress.Value = 0f;
            PlayerCamera.transform.DOMove(targetSpot.position, SnapDuration);
            //TODO: ustawić rotację w kierunku od wykrytego spotu do depozytu - pominąć y w wektorze kierunku (ustawić na 0)
            PlayerCamera.transform.DORotateQuaternion(directionToDeposit, SnapDuration).onComplete += () => {
                this.enabled = true;
                MiningProgressBar.fillAmount = 0;
                Indicators.ForEach(i => Destroy(i));
                Indicators.Clear();

                MinigameCanvasGroup.DOFade(1f, MinigameFadeInDuration).onComplete += () => {
                    SpawnSaltIndicatorTimer = SpawnIntervals[UnityEngine.Random.Range(0, SpawnIntervals.Length)];
                };
                Pickaxe.gameObject.SetActive(true);
                PickaxeAnimator.gameObject.SetActive(true);
                PickaxeAnimator.StopPlayback();
                Pickaxe.transform.DOLocalMove(PickaxeStartLocalPos, MinigameFadeInDuration).onComplete += () => PickaxeAnimator.enabled = true;
            };
        }

        public void Update() {
            UpdateMinigame();
            UpdateIndicatorPositions();
        }

        private void UpdateMinigame() {
            MiningProgress.Value -= Time.deltaTime * MiningProgressDecaySpeed;
            if (MiningProgress.Value < 0)
                MiningProgress.Value = 0;

            //jak nie trafiliśmy to tutaj ma pierwszeństwo
            if (UnsuccessfulHitTimer > 0) {
                UnsuccessfulHitTimer -= Time.deltaTime;
                return;
            }

            if (SpawnSaltIndicatorTimer > 0) {
                SpawnSaltIndicatorTimer -= Time.deltaTime;
            } else {
                var indicator = Instantiate(SaltCrystalIndicatorPrefab, IndicatorSpawnPosition.position, Quaternion.identity, MainBarTransform);
                Indicators.Add(indicator);
                SpawnSaltIndicatorTimer = SpawnIntervals[UnityEngine.Random.Range(0, SpawnIntervals.Length)];
            }
        }

        private void UpdateIndicatorPositions() {
            Indicators.ForEach(indicator => {
                if (indicator == null)
                    return;

                (indicator.transform as RectTransform).anchoredPosition += IndicatorMoveSpeed * Time.deltaTime * IndicatorMoveDirection;
            });
        }

        public void QuitMining_Action(InputAction.CallbackContext ctx) {
            if (!ctx.performed)
                return;

            if (!this.enabled)
                return;

            QuitMining();
        }

        public void QuitMining() {
            this.enabled = false;
            PickaxeAnimator.enabled = false;
            PickaxeAnimator.gameObject.SetActive(false);
            Pickaxe.transform.DOLocalMove(PickaxeHiddenPos, SnapDuration);
            for (int i = 0; i < Deposit.StartingCrystalScaleValues.Length; i++) {
                Deposit.SaltCrystalsToShrink[i].localScale = Deposit.StartingCrystalScaleValues[i];
            }
            PlayerCamera.transform.DOMove(OldPosition, SnapDuration);
            PlayerCamera.transform.DORotateQuaternion(OldRotation, SnapDuration).onComplete += () => {
                FPSController.enabled = true;
                InteractDetector.enabled = true;
                Input.SwitchCurrentActionMap("Player");
            };
            MinigameCanvasGroup.DOFade(0, SnapDuration);
        }

        private void FinishMining() {
            //wygaszamy ui od minigry i jak się wygasi, to przywracamy starą pozycję gracza sprzed miningu
            this.enabled = false;
            this.SpawnSaltIndicatorTimer = -1f;
            PickaxeAnimator.enabled = false;
            Pickaxe.transform.DOLocalMove(PickaxeHiddenPos, SnapDuration).onComplete += () => Pickaxe.SetActive(false);
            AddSalt();
            MinigameCanvasGroup.DOFade(0f, SnapDuration).onComplete += () => {
                PlayerCamera.transform.DOMove(OldPosition, SnapDuration);
                PlayerCamera.transform.DORotateQuaternion(OldRotation, SnapDuration).onComplete += () => {
                    FPSController.enabled = true;
                    InteractDetector.enabled = true;
                    Input.SwitchCurrentActionMap("Player");
                };
            };
        }

        private void AddSalt() {
            SaltStorage.AddSalt(Deposit.SaltValue);
            Destroy(Deposit);
        }

        public void TryHitSaltDeposit_Action(InputAction.CallbackContext ctx) {
            if (ctx.performed == false)
                return;

            if (this.enabled == false)
                return;

            if (this.UnsuccessfulHitTimer > 0)
                return;

            this.PickaxeAnimator.StopPlayback();
            var indicator = GetFirstOverlappingHitIndicator();
            if (indicator == null) {
                //todo: nietrafiony hit, wyświetl o tym informację i ustaw timer, odtwórz GŁOŚNY DŹWIĘK WABIĄCY REKINA, obniż progress o wartość mining strength lub 2*miningstrength
                UnsuccessfulHitTimer = FailedHitStopDuration;
                this.PickaxeAnimator.Play("miss", -1, normalizedTime: 0);
                //todo: tutaj będzie potrzrebny jakiś audio system + scriptable object dźwięku wraz z zasięgiem na jakim słychać dźwięk
                //todo: wtedy rekin może zasubskrybować do tego systemu żeby słuchać konkretnych dźwięków i jak jest w zasięgu to go przyciągamy
                //tutaj można użyć tego systemu jak nie trafimy w element na pasku
                Debug.Log("BOOHOO, MISSED");
            } else {
                MiningProgress.Value += MiningStrength;
                if (MiningSoundsAudioSource != null && MiningClips.Length > 0)
                    MiningSoundsAudioSource.PlayOneShot(MiningClips[UnityEngine.Random.Range(0, MiningClips.Length)]);
                Debug.Log("YEEHAW, HIT");
                Indicators.Remove(indicator.gameObject);
                Destroy(indicator.gameObject);
                this.PickaxeAnimator.Play("hit", -1, normalizedTime: 0);
            }
        }

        private RectTransform GetFirstOverlappingHitIndicator() {
            var hitDetectWorldRect = GetWorldRect(HitDetectionTransform);
            var hitDetectX = hitDetectWorldRect.xMin;
            var width = hitDetectWorldRect.width;
            var indicator = Indicators.FirstOrDefault(i => {
                var objWorldRect = GetWorldRect(i.transform as RectTransform);
                var objWidth = objWorldRect.width;
                var objX = objWorldRect.xMin;
                return objX >= hitDetectX && (objX + objWidth/2) <= (hitDetectX + width); //dzielimy przez 2 - tolerancja dla błędu
            });
            return indicator == null ? null : indicator.transform as RectTransform;
        }

        private Rect GetWorldRect(RectTransform rectTrans) {
            return new Rect {
                min = rectTrans.TransformPoint(rectTrans.rect.min),
                max = rectTrans.TransformPoint(rectTrans.rect.max)
            };
        }
    }
}
