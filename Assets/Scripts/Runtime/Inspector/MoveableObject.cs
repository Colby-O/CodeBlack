using CodeBlack.Player;
using CodeBlack.UI;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack.Inspectables
{
    public class MoveableObject : InspectableObject
    {
        public override bool Interact(Interactor interactor)
        {
            if (!allowInteract || CodeBlackGameManager.player.IsPushingCart()) return false;
            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            if (_auidoSource != null && _auidoclipPickup != null) _auidoSource.PlayOneShot(_auidoclipPickup);
            inspector.StartExamine(transform, _type, offsetPoint, string.Empty, true);
            return true;
        }

        public override string GetHint()
        {
            if (TryGetComponent(out Cure cure))
            {
                return $"Click 'E' To Pickup {cure.cure}";
            }
            else return base.GetHint();
        }
    }
}
