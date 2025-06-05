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
        //todo: prze��cz sterowanie na statek, ustaw boola
        //todo: jak gracz skr�ca statkiem to ko�o statku te� si� kr�ci na podstawie input vectora (w X)
        ShipController.TakeControlOfShip();
    }
}
