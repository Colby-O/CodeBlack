using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBlack {
    public class PatientManager : MonoBehaviour
    {
        private Patient[] _patients;

        private void Awake()
        {
            _patients = GetComponentsInChildren<Patient>();
            SubscribePatientsAllDead(() => Debug.Log("All Dead!"));
            SubscribePatientDead(p => Debug.Log($"Patient {p.name} died!"));
            SubscribePatientUnhealthy(p => Debug.Log($"Patient {p.name} is very sick!"));
        }

        private List<Action<Patient>> _patientDeadCallbacks = new();
        public void SubscribePatientDead(Action<Patient> callback) => _patientDeadCallbacks.Add(callback);
        public void EmitPatientDead(Patient patient)
        {
            _patientDeadCallbacks.ForEach(c => c(patient));
            if (_patients.All(p => p.IsDeadForReal())) EmitPatientsAllDead();
        }
        
        private List<Action> _patientsAllDeadCallbacks = new();
        public void SubscribePatientsAllDead(Action callback) => _patientsAllDeadCallbacks.Add(callback);
        public void EmitPatientsAllDead() => _patientsAllDeadCallbacks.ForEach(c => c());
        
        private List<Action<Patient>> _patientUnhealthyCallbacks = new();
        public void SubscribePatientUnhealthy(Action<Patient> callback) => _patientUnhealthyCallbacks.Add(callback);
        public void EmitPatientUnhealthy(Patient patient) => _patientUnhealthyCallbacks.ForEach(c => c(patient));
    }
}
