using CodeBlack.UI;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeBlack
{
	[RequireComponent(typeof(PlayerInput))]
	public class Interactor : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private PlayerInput _playerInput;
		[SerializeField] private Transform _playerhead;
		//public PickupManager pickupManager;

		[SerializeField] private Transform _interactionPoint;
		[SerializeField] private LayerMask _interactionLayer;
		[SerializeField] private float _interactionRadius = 0.1f;
		[SerializeField] private float _spehreCastRadius = 0.1f;
		
		private InputAction _interactAction;
		private InputAction _pickupAction;

        
        private string _hint;

		private void StartInteraction(IInteractable interactable)
		{
			interactable.Interact(this);
		}
		private void StartPickup(IInteractable interactable)
		{
			interactable.OnPickup(this);
		}

		private void CheckForInteractionInteract()
		{
            if (CodeBlackGameManager.player.IsPushingCart()) return;

            Debug.DrawRay(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, Color.red, 10000f);
            if 
			(
                Physics.Raycast(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer) ||
                Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out hit, _interactionRadius, _interactionLayer)
			)
            {
				IInteractable interactable = hit.collider.GetComponent<IInteractable>();
				if (interactable != null) StartInteraction(interactable);
			}
		}

		private void CheckForInteractionPickup()
		{
            if (CodeBlackGameManager.player.IsPushingCart()) return;

            Debug.DrawRay(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, Color.red, 10000f);
			if 
			(
				Physics.Raycast(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer) ||
				Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out hit, _interactionRadius, _interactionLayer)
			)
			{
				IInteractable interactable = hit.collider.GetComponent<IInteractable>();
				if (interactable != null && interactable.IsPickupable()) StartPickup(interactable);
			}
		}

        private void CheckForPossibleInteractionInteract()
        {
			if (CodeBlackGameManager.player.IsPushingCart()) return;

            GameView gv = GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentView<GameView>();
            if (gv == null) return;
            if
            (
                Physics.Raycast(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer) ||
                Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out hit, _interactionRadius, _interactionLayer)
			)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (!interactable.IsInteractable()) return;
                    GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint(interactable.GetHint(), 1);
                    interactable.AddOutline();
                }
            }
        }

        private void Interact(InputAction.CallbackContext e)
		{
			CheckForInteractionInteract();
		}

		private void Pickup(InputAction.CallbackContext e)
		{
			CheckForInteractionPickup();
		}

		private void Awake()
		{
			if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
			//if (pickupManager == null) pickupManager = GetComponent<PickupManager>();

			_interactAction = _playerInput.actions["Interact"];

			_interactAction.performed += Interact;
		}

        private void LateUpdate()
        {
            CheckForPossibleInteractionInteract();
        }
    }
}

