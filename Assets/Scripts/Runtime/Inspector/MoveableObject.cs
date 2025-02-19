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
            if (_auidoSource != null && _auidoclip != null) _auidoSource.PlayOneShot(_auidoclip);
            inspector.StartExamine(transform, _type, offsetPoint, string.Empty, true);
            return true;
        }
    }
}
