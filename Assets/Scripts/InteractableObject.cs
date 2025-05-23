using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public string InteractionName;
    public abstract void DoInteract();
}
