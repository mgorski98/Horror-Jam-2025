using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    //todo: moze jeszcze dodac callback odpalany przy udanej interakcji? pozwoli to odpaliæ np. animacjê albo zupdate'owaæ story
    public string InteractionName;
    public bool LocksInInteraction;
    public abstract void DoInteract();
    public abstract void StopInteract();
    public virtual string GetInteractionName() => InteractionName;
    public virtual bool ShouldShowBindingKey() => true;
}
