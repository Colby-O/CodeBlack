using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack
{
    public class Thermostat : MonoBehaviour, IInteractable
    {
        private Patient _patient;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField, ColorUsage(true, true)] private Color _outlineColor;
        [SerializeField] float _outlineScale = 1.03f;

        private void Awake()
        {
            _patient = transform.parent.GetComponent<Patient>();
        }
        
        public void AddOutline()
        {
            foreach (Material mat in _meshRenderer.materials) {
                mat.SetColor("_OutlineColor", _outlineColor);
                mat.SetFloat("_Scale", _outlineScale);
            }
        }
        
        public void RemoveOutline()
        {
            foreach (Material mat in _meshRenderer.materials)
            {
                mat.SetFloat("_Scale", 0);
            }
        }

        public bool IsInteractable() => true;

        public bool IsPickupable() => false;

        public void OnPickup(Interactor interactor)
        {
        }

        public bool Interact(Interactor interactor)
        {
            _patient.ApplyCure(Cure.Type.Heat);
            return true;
        }

        public void EndInteraction()
        {
        }

        public string GetHint()
        {
            return "Fix Thermostat";
        }
        
        protected virtual void Update()
        {
           RemoveOutline();
        }
    }
}
