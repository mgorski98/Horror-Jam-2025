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
        //todo: prze³¹cz sterowanie na statek, ustaw boola
        //todo: jak gracz skrêca statkiem to ko³o statku te¿ siê krêci na podstawie input vectora (w X)
        ShipController.TakeControlOfShip();
    }
}
