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
using PlazmaGames.Core.Utils;
using UnityEngine;

namespace CodeBlack.Inspectables {
    public class ReplenishableObject : InspectableObject
    {
        [SerializeField] private float _replenishTime = 2.0f;
        private Vector3 _position;
        private Quaternion _rotation;
        private Transform _parent;

        private bool _hasReplenishable = false;

        private Rigidbody _rb;
        public override bool Interact(Interactor interactor)
        {
            _rb.isKinematic = false;
            if (!allowInteract || CodeBlackGameManager.player.IsPushingCart()) return false;
            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            if (_auidoSource != null && _auidoclipPickup != null) _auidoSource.PlayOneShot(_auidoclipPickup);
            inspector.StartExamine(transform, _type, offsetPoint, string.Empty, true);
            if (!_hasReplenishable) StartCoroutine(Replenish());
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            _position = transform.position;
            _rotation = transform.rotation;
            _parent = transform.parent;

            _hasReplenishable = false;

            if (_rb == null) _rb = GetComponent<Rigidbody>();
            GetComponents<Collider>().ForEach(c => c.enabled = true);
            _rb.isKinematic = true;
        }

        public override string GetHint()
        {
            if (TryGetComponent(out Cure cure))
            {
                return $"Click 'E' To Pickup {cure.cure}";
            }
            else return base.GetHint();
        }

        private IEnumerator Replenish()
        {
            yield return new WaitForSeconds(_replenishTime);
            Instantiate(gameObject, _position, _rotation, _parent);
            _hasReplenishable = true;
        }
    }
}
