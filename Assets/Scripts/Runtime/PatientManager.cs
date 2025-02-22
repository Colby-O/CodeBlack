using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Core.Utils;
using UnityEngine;

namespace CodeBlack {
    public class PatientManager : MonoBehaviour
    {
        [SerializeField] private PatientSettings _settings;
        private Patient[] _patients;

        private void Awake()
        {
            _settings.tickRate = _settings.stage1TickRate;
            _patients = GetComponentsInChildren<Patient>();
            SubscribePatientsAllDead(() => Debug.Log("All Dead!"));
            SubscribePatientDead(p => Debug.Log($"Patient {p.name} died!"));
            SubscribePatientUnhealthy(p => Debug.Log($"Patient {p.name} is very sick!"));
            SubscribeNightEnd(() => Debug.Log($"Night End!"));
        }

        public bool AnySickPatients() => _patients.Any(p => p.IsSick());
        public bool AnyPatientsFlatLine() => _patients.Any(p => p.IsDead() && !p.IsDeadForReal());
        public bool AnyPatientsDeadForReal() => _patients.Any(p => p.IsDeadForReal());
        public bool AllPatientsDeadForReal() => _patients.All(p => p.IsDeadForReal());
        public bool AllPatientsDead() => _patients.All(p => p.IsDead());

        private List<Action<Patient>> _patientDeadCallbacks = new();
        public void SubscribePatientDead(Action<Patient> callback) => _patientDeadCallbacks.Add(callback);
        public void EmitPatientDead(Patient patient)
        {
            _patientDeadCallbacks.ForEach(c => c(patient));
        }
        
        private List<Action> _patientsAllDeadCallbacks = new();
        public void SubscribePatientsAllDead(Action callback) => _patientsAllDeadCallbacks.Add(callback);
        public void EmitPatientsAllDead() => _patientsAllDeadCallbacks.ForEach(c => c());
        
        private List<Action<Patient>> _patientUnhealthyCallbacks = new();
        public void SubscribePatientUnhealthy(Action<Patient> callback) => _patientUnhealthyCallbacks.Add(callback);
        public void EmitPatientUnhealthy(Patient patient) => _patientUnhealthyCallbacks.ForEach(c => c(patient));

        private List<Action<Patient>> _patientDeadForRealCallbacks = new();
        public void SubscribePatientDeadForReal(Action<Patient> callback) => _patientDeadForRealCallbacks.Add(callback);
        public void EmitPatientDeadForReal(Patient patient)
        {
            _patientDeadForRealCallbacks.ForEach(c => c(patient));
            if (_patients.All(p => p.IsDeadForReal())) EmitPatientsAllDead();
	}

        private List<Action> _nightEndCallbacks = new();
        public void SubscribeNightEnd(Action callback) => _nightEndCallbacks.Add(callback);
        public void EmitNightEnd() => _nightEndCallbacks.ForEach(c => c());
        private bool _nightEnded = false;

        private void FixedUpdate()
        {
            //if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 7)
            //{
            //    if (!_nightEnded) EmitNightEnd();
            //    _nightEnded = true;
            //    if (!AllPatientsDead())
            //    {
            //        _patients.ForEach(p => p.Kill());
            //    }
            //}
            if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 6) _settings.tickRate = _settings.stage8TickRate;
            else if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 5) _settings.tickRate = _settings.stage7TickRate;
            else if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 4) _settings.tickRate = _settings.stage6TickRate;
            else if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 3) _settings.tickRate = _settings.stage5TickRate;
            else if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 2) _settings.tickRate = _settings.stage4TickRate;
            else if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 1) _settings.tickRate = _settings.stage3TickRate;
            else if ((int)CodeBlackGameManager.RunningTime() / (60 * 60) > 0) _settings.tickRate = _settings.stage2TickRate;
            else _settings.tickRate = _settings.stage8TickRate / ((int)CodeBlackGameManager.RunningTime() / (60 * 60));

        }
    }
}
