using PlazmaGames.Attribute;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using CodeBlack.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeBlack.Trigger
{
    public abstract class Trigger : MonoBehaviour
    {
        [SerializeField] protected bool _canTriggerMoreThanOnce = false;

        [SerializeField, ReadOnly] protected bool _isTriggered = false;
        [SerializeField, ReadOnly] protected bool _isTriggeredOut = false;

        protected abstract void OnEnter();

        protected abstract void OnExit();

        protected virtual bool Condition()
        {
            return true;
        }
        protected virtual bool ConditionOut()
        {
            return true;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("player"))
            {
                if (_isTriggered || !Condition()) return;
                _isTriggered = true;
                _isTriggeredOut = false;
                OnEnter();
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("player"))
            {
                if (_isTriggeredOut || !ConditionOut()) return;
                if (_canTriggerMoreThanOnce) _isTriggered = false;
                _isTriggeredOut = true;
                OnExit();
            }
        }
    }

    public class Trigger1 : Trigger
    {
        private PlayerController _player;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>();
        }

        protected override bool Condition() => _player.IsPushingCart();
        protected override bool ConditionOut() => _player.IsPushingCart();

        protected override void OnEnter()
        {
            CodeBlackGameManager.firstStockCart = true;
        }

        protected override void OnExit()
        {
            CodeBlackGameManager.firstStockCart = false;
        }
    }
}