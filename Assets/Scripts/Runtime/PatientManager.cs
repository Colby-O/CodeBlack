using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack {
    public class PatientManager : MonoBehaviour
    {
        private Patient[] _patients;

        private void Awake()
        {
            _patients = GetComponentsInChildren<Patient>();
            Debug.Log(_patients.Length);
        }

        private List<Action<Patient>> _patientDiedCallbacks;

        public void SubscribePatientDied(Action<Patient> callback)
        {
            _patientDiedCallbacks.Add(callback);
        }
        
        public void EmitPatientDied(Patient patient)
        {
            _patientDiedCallbacks.ForEach(c => c(patient));
        }
    }
}
