using PlazmaGames.Core;
using PlazmaGames.Animation;
using UnityEngine;

namespace CodeBlack
{
    public class Door : MonoBehaviour, IInteractable
    {
        private Transform _pivot;
        private Transform _center;
        private bool _isOpen = false;
        private bool _inProgress = false;
        [SerializeField] float _openSpeed = 1.5f;

        private void Awake()
        {
            _pivot = transform.parent;
            _center = transform.parent.parent;
        }

        private float CurrentAngle()
        {
            float angle = _pivot.localRotation.eulerAngles.y;
            angle %= 360;
            angle= angle > 180 ? angle - 360 : angle;
            return angle;
        }
        
        public bool Interact(Interactor interactor)
        {
            if (_isOpen) Close();
            else Open(interactor.transform);
            return true;
        }

        public void Open(Transform from)
        {
            if (_isOpen) return;
            if (_inProgress) return;
            _isOpen = true;
            _inProgress = true;
            float start = CurrentAngle();
            float target = 90;
            if (Vector3.Dot(_center.forward, (_center.position - from.position).normalized) > 0)
            {
                target = -90;
            }
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _openSpeed,
                (float progress) =>
                {
                    _pivot.localRotation = Quaternion.Euler(0, start + (target - start) * progress, 0);
                },
                () =>
                {
                    _inProgress = false;
                }
            );
        }

        public void Close()
        {
            if (!_isOpen) return;
            if (_inProgress) return;
            _inProgress = true;
            _isOpen = false;
            float start = CurrentAngle();
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _openSpeed,
                (float progress) =>
                {
                    _pivot.localRotation = Quaternion.Euler(0, start - progress * start, 0);
                },
                () =>
                {
                    _inProgress = false;
                }
            );
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
