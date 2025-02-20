using System;
using UnityEngine;

namespace CodeBlack
{
    public class Body : MonoBehaviour
    {
        private Patient _patient;

        private void Awake()
        {
            _patient = transform.parent.GetComponent<Patient>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Cure>(out Cure cure))
            {
                _patient.ApplyCure(cure.cure);
                Destroy(other.gameObject);
            }
        }
    }
}
