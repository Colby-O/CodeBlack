using System.Collections;
using CodeBlack.Player;
using CodeBlack.UI;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack.Inspectables {
    public class ReplenishableObject : InspectableObject
    {
        [SerializeField] private float _replenishTime = 2.0f;
        private Vector3 _position;
        private Quaternion _rotation;
        private Transform _parent;
        public override bool Interact(Interactor interactor)
        {
            if (!allowInteract || CodeBlackGameManager.player.IsPushingCart()) return false;
            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            if (_auidoSource != null && _auidoclip != null) _auidoSource.PlayOneShot(_auidoclip);
            inspector.StartExamine(transform, _type, offsetPoint, string.Empty, true);
            StartCoroutine(Replenish());
            return true;
        }

        private void Awake()
        {
            base.Awake();
            _position = transform.position;
            _rotation = transform.rotation;
            _parent = transform.parent;
        }

        private IEnumerator Replenish()
        {
            yield return new WaitForSeconds(_replenishTime);
            Instantiate(gameObject, _position, _rotation, _parent);
        }
    }
}
