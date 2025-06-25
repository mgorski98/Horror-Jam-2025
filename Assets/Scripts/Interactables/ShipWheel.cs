using UnityEngine;
using DG.Tweening;
using Assets.Scripts;

public class ShipWheel : InteractableObject {
    public FPSPlayerController FPSController;
    public ShipController ShipController;

    private void Awake() {
        if (FPSController == null)
            FPSController = FindObjectOfType<FPSPlayerController>(true);

        if (ShipController == null)
            ShipController = FindObjectOfType<ShipController>(true);
    }

    public override void DoInteract() {
        ShipController.TakeControlOfShip();
    }

    public override void StopInteract() {
        ShipController.StopControllingShip();
    }
}
