using PlazmaGames.Attribute;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeBlack.Trigger
{
    public abstract class Trigger : MonoBehaviour
    {
        [SerializeField] protected bool _canTriggerMoreThanOnce = false;

        [SerializeField, ReadOnly] protected bool _isTriggered = false;

        protected abstract void OnEnter();

        protected abstract void OnExit();

        protected virtual bool Condition()
        {
            return true;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || !Condition()) return;

            if (other.gameObject.CompareTag("player"))
            {
                _isTriggered = true;
                OnEnter();
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("player"))
            {
                if (_isTriggered && _canTriggerMoreThanOnce) _isTriggered = false;
                else return;
                OnExit();
            }
        }
    }

    public class Trigger1 : Trigger
    {
        private bool _wasTrueBefore = false;

        protected override void OnEnter()
        {
            if (CodeBlackGameManager.isPaused)
            {
                _wasTrueBefore = true;
            }
            else
            {
                _wasTrueBefore = false;
            }
            CodeBlackGameManager.isPaused = true;
        }

        protected override void OnExit()
        {
            if (!_wasTrueBefore) CodeBlackGameManager.isPaused = false;
        }
    }
}