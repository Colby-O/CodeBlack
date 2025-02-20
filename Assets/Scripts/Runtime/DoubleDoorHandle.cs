using UnityEngine;

namespace CodeBlack
{
    public class DoubleDoorHandle : MonoBehaviour, IInteractable
    {
        public bool Interact(Interactor interactor)
        {
            transform.parent.parent.GetComponent<DoubleDoor>().Interact(interactor);
            return true;
        }

        public void Open(Transform from)
        {
            transform.parent.parent.GetComponent<DoubleDoor>().Open(from);
        }

        public string GetHint()
        {
            return string.Empty;
        }

        public bool IsPickupable()
        {
            return false;
        }

        public void OnPickup(Interactor interactor)
        {
            
        }

        public void EndInteraction()
        {
        }

        public void AddOutline()
        {

        }

        public void RemoveOutline()
        {

        }

        public bool IsInteractable()
        {
            return false;
        }
    }
}
