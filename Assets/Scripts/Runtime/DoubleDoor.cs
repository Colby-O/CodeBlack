using System;
using UnityEngine;
using PlazmaGames.Core;
using PlazmaGames.Animation;

namespace CodeBlack
{
    public class DoubleDoor : MonoBehaviour
    {
        private Transform _pivot1;
        private Transform _pivot2;
        private bool _isOpen = false;
        [SerializeField] private bool _isLocked = false;
        private bool _inProgress = false;
        [SerializeField] float _openSpeed = 1.5f;

        private void Awake()
        {
            _pivot1 = transform.GetChild(0);
            _pivot2 = transform.GetChild(1);
        }

        public void SetLockedState(bool state)
        {
            _isLocked = state;
        }

        public void Interact(Interactor interactor)
        {
            if (_isLocked) return;

            if (_isOpen) Close();
            else Open(interactor.transform);
        }
        
        private float CurrentAngleOf(Transform pivot)
        {
            float angle = pivot.localRotation.eulerAngles.y;
            angle %= 360;
            angle= angle > 180 ? angle - 360 : angle;
            return angle;
        }
        
        public void Open(Transform from)
        {
            if (_isOpen || _isLocked) return;
            if (_inProgress) return;
            _isOpen = true;
            _inProgress = true;
            {
                float start = CurrentAngleOf(_pivot1);
                float target = -90;
                if (Vector3.Dot(transform.right, (transform.position - from.position).normalized) > 0)
                {
                    target = 90;
                }

                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    _pivot1.GetChild(0).GetComponent<DoubleDoorHandle>(),
                    _openSpeed,
                    (float progress) => { _pivot1.localRotation = Quaternion.Euler(0, start + (target - start) * progress, 0); },
                    () => { _inProgress = false; }
                );
            }
            {
                float start = CurrentAngleOf(_pivot2);
                float target = 90;
                if (Vector3.Dot(transform.right, (transform.position - from.position).normalized) > 0)
                {
                    target = -90;
                }

                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    _pivot2.GetChild(0).GetComponent<DoubleDoorHandle>(),
                    _openSpeed,
                    (float progress) => { _pivot2.localRotation = Quaternion.Euler(0, start + (target - start) * progress, 0); },
                    () => { _inProgress = false; }
                );
            }
        }

        public void Close()
        {
            if (!_isOpen || _isLocked) return;
            if (_inProgress) return;
            _inProgress = true;
            _isOpen = false;
            {
                float start = CurrentAngleOf(_pivot1);
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    _pivot1.GetChild(0).GetComponent<DoubleDoorHandle>(),
                    _openSpeed,
                    (float progress) => { _pivot1.localRotation = Quaternion.Euler(0, start - progress * start, 0); },
                    () => { _inProgress = false; }
                );
            }
            {
                float start = CurrentAngleOf(_pivot2);
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    _pivot2.GetChild(0).GetComponent<DoubleDoorHandle>(),
                    _openSpeed,
                    (float progress) => { _pivot2.localRotation = Quaternion.Euler(0, start - progress * start, 0); },
                    () => { _inProgress = false; }
                );
            }
        }
    }
}
