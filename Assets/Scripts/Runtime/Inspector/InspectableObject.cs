using CodeBlack.Player;
using CodeBlack.UI;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack.Inspectables
{
    public enum ExamineType
    {
        Goto,
        ComeTo
    }

    public class InspectableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] protected ExamineType _type = ExamineType.Goto;
        [SerializeField] protected GameObject offsetPoint = null;

        [SerializeField] protected AudioSource _auidoSource;
        [SerializeField] protected AudioClip _auidoclip;

        [SerializeField] protected MeshRenderer _meshRenderer;
        [SerializeField, ColorUsage(true, true)] protected Color _outlineColor;
        [SerializeField] protected float _outlineScale = 1.03f;

        [ColorUsage(true, true)] private Color _backupColor;

        public bool allowInteract = true;

        public virtual bool Interact(Interactor interactor)
        {
            if (!allowInteract) return false;

            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            if (_auidoclip != null && _auidoSource != null) _auidoSource.PlayOneShot(_auidoclip);
            PlayerController pc = interactor.GetComponent<PlayerController>();
            pc.ZeroInput();
            inspector.StartExamine(transform, _type, offsetPoint);
            return true;
        }

        public virtual string GetHint()
        {
            return "Click 'E' To Interact";
        }

        public void SetOutlineScale(float scale)
        {
            _outlineScale = scale;
        }

        public void SetOutlineColor(Color col)
        {
            _outlineColor = col;
        }

        public void RestoreOutlineColor()
        {
            _outlineColor = _backupColor;
        }

        public void EndInteraction()
        {

        }

        public virtual bool IsPickupable()
        {
            return false;
        }

        public virtual void OnPickup(Interactor interactor)
        {

        }

        public void AddOutline()
        {
            foreach (Material mat in _meshRenderer.materials) {
                mat.SetColor("_OutlineColor", _outlineColor);
                mat.SetFloat("_Scale", _outlineScale);
            }
        }

        protected virtual void Awake()
        {
            _backupColor = _outlineColor;
        }

        protected virtual void Update()
        {
           RemoveOutline();
        }

        public void RemoveOutline()
        {
            foreach (Material mat in _meshRenderer.materials)
            {
                mat.SetFloat("_Scale", 0);
            }
        }

        public bool IsInteractable()
        {
            return allowInteract;
        }
    }
}
