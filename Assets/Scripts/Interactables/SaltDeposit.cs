using Assets.Scripts;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaltDeposit : InteractableObject
{
    [Tooltip("How much salt there is inside, in grams")]
    public int SaltValue = 0; //how much salt there is, in grams
    public Transform[] SaltCrystalsToShrink;
    public Vector3[] StartingCrystalScaleValues;
    public Transform[] MiningSpots;

    private MiningMinigameController MiningController;

    private void Awake() {
        if (MiningController == null)
            MiningController = FindObjectOfType<MiningMinigameController>(true);

        StartingCrystalScaleValues = new Vector3[SaltCrystalsToShrink.Length];
        for (int i = 0; i < StartingCrystalScaleValues.Length; i++) {
            StartingCrystalScaleValues[i] = SaltCrystalsToShrink[i].localScale;
        }
    }

    public override void DoInteract() {
        if (MiningController != null && !MiningController.enabled) {
            MiningController.InitMining(this);
        }
    }

    public Transform GetClosestMiningSpot() {
        if (MiningSpots == null || MiningSpots.Length == 0)
            return null;

        return MiningSpots.OrderBy(ms => Vector3.Distance(ms.position, MiningController.transform.position)).First();
    }

    public override string GetInteractionName() {
        if (MiningController.StorageRef.IsFull)
            return "Storage full!";
        return InteractionName;
    }

    public override bool ShouldShowBindingKey() {
        return MiningController.StorageRef.IsFull == false;
    }
}
