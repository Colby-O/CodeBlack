using System.Collections;
using System.Globalization;
using UnityEngine;
using CodeBlack.ECG;
using TMPro;
using UnityEngine.Serialization;
using PlazmaGames.Attribute;
using CodeBlack.Helpers;
using Unity.Burst.Intrinsics;

namespace CodeBlack
{
    public class Patient : MonoBehaviour
    {
        [SerializeField] private PatientSettings _settings;
        
        private Heart _heart;
        private EKG _ekg;
        
        [SerializeField] private string _name;
        [SerializeField] private string _sex;
        [SerializeField] private string _age;
        
        [Header("Medical Conditions")]
        [SerializeField] private bool _diabetes = false;

        [Header("Diabetes")]
        [SerializeField] private bool _diabetesAttacking = false;

        [Header("Temperature")]
        [SerializeField] private float _temp = 37.0f;
        
        [Header("Oxygen")]
        [SerializeField] private float _oxygen = 99.0f;
        [SerializeField] private bool _oxygenAttacking = false;
        
        [Header("Heart")]
        [SerializeField] float _restingHeartRate = 80f;
        [SerializeField] float _meanHeartRate = 80f;

        [Header("Hunger")]
        [SerializeField] private float _hunger = 1.0f;
        
        [Header("Blood Sugar")]
        [SerializeField] private float _bloodSugar = 120;
        [SerializeField] private float _bloodSugarRate = 120;
        [SerializeField] private float _bloodSugarTarget = 120;
        [SerializeField] private float _bloodSugarCurrentNormalValue = 120;

        [Header("Heart Levels")]
        [SerializeField, ReadOnly] private float _ach = 2;
        [SerializeField, ReadOnly] private float _crp = 2;
        [SerializeField, ReadOnly] private float _bnp = 200;

        [SerializeField] private bool _achRaising = false;
        [SerializeField] private bool _achLowering = false;

        [SerializeField] private bool _crpRaising = false;
        [SerializeField] private bool _crpLowering = false;

        [SerializeField] private bool _bnpRaising = false;

        [SerializeField] private bool _tempLowering = false;

        private bool _runningEvent => _achRaising || _achLowering || _crpRaising || _crpLowering || _bnpRaising || _tempLowering;

        private TMP_Text _patientText;
        private TMP_Text _bpmText;
        private TMP_Text _ibpText;
        private TMP_Text _sugarsText;
        private TMP_Text _oxygenText;

        private float _lastTick;
        
        [Header("View")]
        [SerializeField] private int _tick = 0;

        private void Awake()
        {
            _name = NameGenerator.GenerateName();

            _patientText = transform.Find("MonitorDisplay/PatientText").GetComponent<TMP_Text>();
            _bpmText = transform.Find("MonitorDisplay/BpmText").GetComponent<TMP_Text>();
            _ibpText = transform.Find("MonitorDisplay/IbpText").GetComponent<TMP_Text>();
            _sugarsText = transform.Find("MonitorDisplay/SugarsText").GetComponent<TMP_Text>();
            _oxygenText = transform.Find("MonitorDisplay/OxygenText").GetComponent<TMP_Text>();

            _heart = GetComponentInChildren<Heart>();
            _ekg = GetComponentInChildren<EKG>();

            _temp = _settings.normalTemp;

            _restingHeartRate = Random.Range(_settings.restingHeartRateLow, _settings.restingHeartRateHigh);
            _meanHeartRate = _restingHeartRate;

            _lastTick = Time.time;

            SetPatientText();

            _ekg.SetHeart(_heart);
        }

        private void SetPatientText()
        {
            _patientText.text = $"{_name} - {_sex} - {_age}";
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastTick + _settings.tickRate) Tick();

            _bpmText.text = ((int)_heart.Bpm()).ToString();
            _ibpText.text = $"{(int)_heart.Sbp()}/{(int)_heart.Dbp()}";
            _sugarsText.text = $"{(int)_bloodSugar}";
            _oxygenText.text = $"{(int)_oxygen}";
        }

        public void ApplyCure(Cure.Type cure)
        {
            Debug.Log($"curing with {cure}");
            switch (cure)
            {
                case Cure.Type.Adrenaline:
                    if (_achLowering)
                    {
                        _achLowering = false;
                        _ach = 2;
                    }
                    else
                    {
                        _achRaising = true;
                    }
                    break;
                case Cure.Type.BetaBlockers:
                    if (_achRaising)
                    {
                        _achRaising = false;
                        _ach = 2;
                    }
                    else
                    {
                        _achLowering = true;
                    }
                    break;
                case Cure.Type.CalciumBlockers:
                    if (_heart.HasAtrialFibrillation()) _heart.SetAtrialFibrillation(false);
                    else _heart.SetAtrialFibrillation(true);
                    break;
                case Cure.Type.Atropine:
                    if (_heart.HasBlock()) _heart.SetBlock(0);
                    else _heart.SetBlock(Random.Range(1, 3));
                    break;
                case Cure.Type.Digoxin:
                    if (_crpRaising)
                    {
                        _crpRaising = false;
                        _crp = 2;
                    }
                    else
                    {
                        _crpLowering = true;
                    }
                    break;
                case Cure.Type.Ibuprofen:
                    if (_crpLowering)
                    {
                        _crpLowering = false;
                        _crp = 2;
                    }
                    else
                    {
                        _crpRaising = true;
                    }
                    break;
                case Cure.Type.Furosemide:
                    _bnpRaising = false;
                    _bnp = 200;
                    break;
                case Cure.Type.Defibrillator:
                    if (_heart.HasVentricularFibrillation()) _heart.SetVentricularFibrillation(false);
                    else if (_heart.IsDead()) _heart.Revive();
                    else _heart.CauseCardiacArrest(true);
                    break;
                case Cure.Type.Insulin:
                    if (_diabetesAttacking) _diabetesAttacking = false;
                    else _hunger = _settings.tooMuchInsulinHungerLevel;
                    break;
                case Cure.Type.Food:
                    _hunger = 1f;
                    break;
                case Cure.Type.Heat:
                    _tempLowering = false;
                    break;
                case Cure.Type.Oxygen:
                    _oxygenAttacking = false;
                    break;
            }
        }

        private void InjectInsulin()
        {
            _diabetesAttacking = false;
        }

        private void Tick()
        {
            _lastTick = Time.time;

            if (!_oxygenAttacking && Random.value < (1f / _settings.oxygenAttackChance))
                _oxygenAttacking = true;

            if (_oxygenAttacking) _oxygen = Mathf.Max(_settings.oxygenMinValue, _oxygen - _settings.oxygenDepletionRate);
            else _oxygen = Mathf.Min(99f, _oxygen + 1);

            if (_diabetes && !_diabetesAttacking && Random.value < (1f / _settings.diabetesAttackChance) && _hunger < 1)
            {
                _diabetesAttacking = true;
                NextBloodSugarNormalValue();
            }

            _hunger = Mathf.Max(0, _hunger - _settings.hungerDepletionRate);
            
            if (Random.value < (1f / _settings.bloodSugarNormalValueChangeRate))
            {
                NextBloodSugarNormalValue();
            }

            float effectiveHunger = 1.0f - ((_hunger > _settings.hungerCutoff) ? 1.0f : _hunger / _settings.hungerCutoff);
            _bloodSugarTarget = Mathf.Lerp(
                _bloodSugarCurrentNormalValue, _settings.bloodSugarHungerLow,
                Mathf.Pow(effectiveHunger, 1.3f));

            if (_tick % _settings.bloodSugarRateChangeRate == 0)
            {
                _bloodSugarRate = (_bloodSugarTarget - _bloodSugar) * (1.0f / _settings.bloodSugarRestorationRate);
                if (Mathf.Abs(_bloodSugarRate) > _settings.bloodSugarMaxRate)
                    _bloodSugarRate = Mathf.Sign(_bloodSugarRate) * _settings.bloodSugarMaxRate;
                if (Mathf.Abs(_bloodSugarRate) < _settings.bloodSugarMinRate)
                    _bloodSugarRate = Mathf.Sign(_bloodSugarRate) * _settings.bloodSugarMinRate;
            }
            _bloodSugar += _bloodSugarRate;

            if (_oxygen < _settings.oxygenDangerousLevel)
                _meanHeartRate = MapRange(
                    _oxygen,
                    _settings.oxygenDeathValue, _settings.oxygenDangerousLevel,
                    10, _restingHeartRate);
            else if (_bloodSugar < _settings.bloodSugarDangerousLow)
                _meanHeartRate = MapRange(
                    _bloodSugar,
                    20, _settings.bloodSugarDangerousLow,
                    10, _restingHeartRate);
            else if (_bloodSugar > _settings.bloodSugarDangerousHigh)
                _meanHeartRate = MapRange(
                    _bloodSugar,
                    _settings.bloodSugarDangerousHigh, _settings.diabetesAttackBloodSugarValue,
                    _restingHeartRate, 240);

            if (_tick % 5 == 0)
            {
                if (!_runningEvent && Random.value < _settings.eventProb)
                {
                    int val = Random.Range(0, 6);

                    if (val == 0) _achLowering = true;
                    else if (val == 1) _crpRaising = true;
                    else if (val == 2) _achRaising = true;
                    else if (val == 3) _crpLowering = true;
                    else if (val == 4) _bnpRaising = true;
                    else if (val == 5) _tempLowering = true;
                }
            }

            _ach += Random.Range(-_settings.achRateRand, _settings.achRateRand);
            _crp += Random.Range(-_settings.crpRateRand, _settings.crpRateRand);
            _bnp += Random.Range(-_settings.bnpRateRand, _settings.bnpRateRand);
            _temp += Random.Range(-_settings.tempRateRand, _settings.tempRateRand);

            if (_achLowering) _ach -= _settings.achRate;
            else if (_achRaising) _ach += _settings.achRate * 2f;
            else if (_crpLowering) _crp -= _settings.crpRate;
            else if (_crpRaising) _crp += _settings.crpRate;
            else if (_bnpRaising) _bnp += _settings.bnpRate;
            
            if (_tempLowering) _temp = Mathf.Max(_settings.tempDeadLevel, _temp - _settings.tempRate);
            else if (_temp < _settings.normalTemp) _temp = Mathf.Min(_settings.normalTemp, _temp + _settings.tempRecoverRate);

            _ach = Mathf.Clamp(_ach, 1, 4);
            _crp = Mathf.Clamp(_crp, 1, 3);
            _bnp = Mathf.Clamp(_bnp, 200, 400);

            float newHeartRate = -1;
            if (_ach > 2f) newHeartRate = Mathf.Lerp(60f / 250f, 60f / _meanHeartRate, Mathf.Clamp(1f - (_ach - 2f) / 2f, 0, 1));
            else newHeartRate = Mathf.Lerp(60f / _meanHeartRate, 3f, Mathf.Clamp(2f - _ach, 0, 1));
            if (newHeartRate > -1) _heart.SetHeartRate(newHeartRate);

            if (_heart.Bpm() < 22 || _heart.Bpm() > 190 || _heart.GetAtrialFibrillationState() || _heart.GetBlockState())
            {
                if (Random.value < _settings.caProb) _heart.CauseCardiacArrest(true);
            }

            if (_crp <= 1)
            {
                if (Random.value < _settings.blockProb) _heart.SetBlock(Random.Range(1, 3));
            }
            if (_crp >= 3)
            {
                if (Random.value < _settings.afProb) _heart.SetAtrialFibrillation(true);
            }

            if (_bnp >= 400)
            {
                if (Random.value < _settings.vfProb) _heart.SetVentricularFibrillation(true);
            }

            if (_temp <= _settings.tempDangerousLevel)
            {
                _heart.SetJWave(Mathf.Lerp(0.2f, 1f, Mathf.Clamp((32 - _temp) / 7, 0, 1)));
            }
            else if (_temp <= _settings.tempDeadLevel)
            {
                if (Random.value < _settings.caProb) _heart.CauseCardiacArrest(true);
            }
            else
            {
                _heart.SetJWave(0);
            }

            _tick += 1;
        }

        private void NextBloodSugarNormalValue()
        {
            float value;
            if (_diabetesAttacking) value = _settings.diabetesAttackBloodSugarValue;
            else value = _settings.bloodSugarNormalValue;
            _bloodSugarCurrentNormalValue = ApplyVariance(value, _settings.bloodSugarVariance);
        }

        private static float ApplyVariance(float value, float variance)
        {
            float v = value * variance;
            return value + v * (Random.value * 2f - 1f);
        }

        private static float MapRange(float v, float a, float b, float c, float d)
        {
            return Mathf.Lerp(c, d, Mathf.InverseLerp(a, b, v));
        }
    }
}
