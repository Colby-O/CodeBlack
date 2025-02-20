using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBlack
{
	public interface IInteractable 
	{
        public void AddOutline();

        public void RemoveOutline();

        public bool IsInteractable();

        public bool IsPickupable();

        public void OnPickup(Interactor interactor);

        /// <summary>
        /// Interaction with a Interactor
        /// </summary>
        public bool Interact(Interactor interactor);

        /// <summary>
        /// Method to be called once Interaction is complete
        /// </summary>
        public void EndInteraction();

        public string GetHint();
    }
}
