using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    //todo: moze jeszcze dodac callback odpalany przy udanej interakcji? pozwoli to odpali� np. animacj� albo zupdate'owa� story
    public string InteractionName;
    public abstract void DoInteract();
    public virtual string GetInteractionName() => InteractionName;
    public virtual bool ShouldShowBindingKey() => true;
}
