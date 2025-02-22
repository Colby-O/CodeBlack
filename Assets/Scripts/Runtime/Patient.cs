using UnityEngine;
using CodeBlack.ECG;
using TMPro;
using PlazmaGames.Attribute;
using CodeBlack.Helpers;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using PlazmaGames.Audio;
using PlazmaGames.Runtime.DataStructures;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace CodeBlack
{
    public class Patient : MonoBehaviour
    {
        [SerializeField] private PatientSettings _settings;
        [SerializeField] private AudioSource _audioSource;

        private PatientManager _manager;
        
        private Heart _heart;
        private EKG _ekg;

        [SerializeField] SerializableDictionary<Cure.Type, AudioClip> _sfx;

        [SerializeField] private bool _generateRandomInfo = true;

        [SerializeField, ReadOnly] private bool _isPaused = false;

        [SerializeField] private MeshRenderer _icon;
        [SerializeField] private bool _hasIcon = true;

        [SerializeField] private bool _god = false;

        [SerializeField] private AudioClip _dyingAudio;
        [SerializeField] private AudioClip _deathAudio;
        [SerializeField] private string _name;
        [SerializeField] private string _sex;
        [SerializeField] private string _age;
        [SerializeField] private string _unit;
        [SerializeField] private string _room;

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
        [SerializeField] private float _hungerDepletionRate;
        
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

        private float _nextCough = 0;

        private bool _wasDead = false;
        private bool _wasHealthy = true;
        private bool _wasDeadForReal = false;

        private bool _runningEvent => _achRaising || _achLowering || _crpRaising || _crpLowering || _bnpRaising || _tempLowering;

        private TMP_Text _patientText;
        private TMP_Text _bpmText;
        private TMP_Text _ibpText;
        private TMP_Text _sugarsText;
        private TMP_Text _oxygenText;

        private float _lastTick;

        private float _startTime;
        
        [Header("View")]
        [SerializeField] private int _tick = 0;

        private Transform _body;

        private void TryCough()
        {
            if ((int)Time.time == (int)_nextCough)
            {
                if (!IsDead() && !IsSick()) _audioSource.PlayOneShot(_settings.coughSounds[Random.Range(0, _settings.coughSounds.Count - 1)]);
                _nextCough = Time.time + Random.Range(_settings.coughTimeLow, _settings.coughTimeHigh);
            }
            else if (_nextCough == 0)
            {
                _nextCough = Time.time + Random.Range(_settings.coughTimeLow, _settings.coughTimeHigh);
            }
        }

        public AudioClip GetDyingAudiio()
        {
            return _dyingAudio;
        }

        public AudioClip GetDeathAudiio()
        {
            return _deathAudio;
        }

        private void Awake()
        {
            _startTime = Time.time;

            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();

            if (_generateRandomInfo) _name = NameGenerator.GenerateName();
            if (_generateRandomInfo) _age = Random.Range(18, 70).ToString();
            if (_generateRandomInfo) _sex = Random.value < 0.5 ? "M" : "F";
            _diabetes = Random.value < 0.4;

            _patientText = transform.Find("MonitorDisplay/PatientText").GetComponent<TMP_Text>();
            _bpmText = transform.Find("MonitorDisplay/BpmText").GetComponent<TMP_Text>();
            _ibpText = transform.Find("MonitorDisplay/IbpText").GetComponent<TMP_Text>();
            _sugarsText = transform.Find("MonitorDisplay/SugarsText").GetComponent<TMP_Text>();
            _oxygenText = transform.Find("MonitorDisplay/OxygenText").GetComponent<TMP_Text>();
            transform.Find("MonitorDisplay/UnitTitle").GetComponent<TMP_Text>().text = _unit;
            transform.Find("MonitorDisplay/RoomTitle").GetComponent<TMP_Text>().text = _room;

            _manager = transform.parent.GetComponent<PatientManager>();
            _body = transform.Find("Body");
            _heart = GetComponentInChildren<Heart>();
            _ekg = GetComponentInChildren<EKG>();
            if (_hasIcon && transform.Find("Icon").TryGetComponent<MeshRenderer>(out MeshRenderer icon)) _icon = icon;
            else _icon = null;

            _temp = _settings.normalTemp;

            _restingHeartRate = Random.Range(_settings.restingHeartRateLow, _settings.restingHeartRateHigh);
            _meanHeartRate = _restingHeartRate;
            _hungerDepletionRate = ApplyVariance(_settings.hungerDepletionRate, 0.2f);

            _lastTick = Time.time;

            SetPatientText();

            _ekg.SetHeart(_heart);
            
            TryCough();
        }

        public void SetAchRasing()
        {
            _achRaising = true;
        }

        private void SetPatientText()
        {
            _patientText.text = $"{_name.Replace('\r', ' ')} - {_sex.Replace('\r', ' ')} - {_age.Replace('\r', ' ')}";
        }

        public bool IsDeadForReal() => _heart.IsDead() && _heart.DeadTime() > _settings.reviveTime;

        private int Sbp() => IsDeadForReal() ? 0 : (int)_heart.Sbp();
        private int Dbp() => IsDeadForReal() ? 0 : (int)_heart.Dbp();

        private void FixedUpdate()
        {
            TryCough();
            _isPaused = CodeBlackGameManager.isPaused || !CodeBlackGameManager.hasStarted;

            if (_icon != null)
            {
                if (IsDeadForReal()) _icon.material.color = new Color(0, 0, 0, 0);
                else if (IsDead()) _icon.material.color = new Color(0.37f, 0.37f, 0.37f);
                else if (_heart.IsHealthty()) _icon.material.color = Color.green;
                else _icon.material.color = Color.red;
            }
            
            if (Time.time > _lastTick + _settings.tickRate) Tick();

            _bpmText.text = ((int)_heart.Bpm()).ToString();
            _ibpText.text = $"{Sbp()}/{Dbp()}";
            _sugarsText.text = $"{(int)_bloodSugar}";
            _oxygenText.text = $"{(int)_oxygen}";

            if (_god) return;

            if (IsDeadForReal())
            {
                if (!_wasDeadForReal)
                {
                    _heart.SetSound(false);
                    _wasDeadForReal = true;
                    _manager.EmitPatientDeadForReal(this);
                }
            }

            if (_heart.IsDead())
            {
                if (!_wasDead)
                {
                    _wasDead = true;
                    _manager.EmitPatientDead(this);
                }
            }
            else if (_wasDead)
            {
                _wasDead = false;
            }
            
            if (!_heart.IsHealthty())
            {
                if (Time.time - _startTime > 10 && _wasHealthy)
                {
                    _wasHealthy = false;
                    _manager.EmitPatientUnhealthy(this);
                }
            }
            else if (!_wasHealthy)
            {
                _wasHealthy = true;
            }
        }

        public bool IsDead() => _heart.IsDead();
        public bool IsSick() => !_heart.IsHealthty();

        public void ApplyCure(Cure.Type cure)
        {
            Debug.Log($"curing with {cure}");

            if (_sfx.ContainsKey( cure ) )
            {
                _audioSource.PlayOneShot(_sfx[ cure ] );
            }

            switch (cure)
            {
                case Cure.Type.Adrenaline:
                    if (_achRaising)
                    {
                        if (CanCardiacArrest()) _heart.CauseCardiacArrest(true);
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    else if (_achLowering)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _achLowering = false;
                        _ach = 2;
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _achRaising = true;
                    }
                    break;
                case Cure.Type.BetaBlockers:
                    if (_achLowering)
                    {
                        if (CanCardiacArrest()) _heart.CauseCardiacArrest(true);
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    else if (_achRaising)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _achRaising = false;
                        _ach = 2;
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _achLowering = true;
                    }
                    break;
                case Cure.Type.CalciumBlockers:
                    if (_heart.HasAtrialFibrillation())
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _heart.SetAtrialFibrillation(false);
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _heart.SetAtrialFibrillation(true);
                    }
                    break;
                case Cure.Type.Atropine:
                    if (_heart.HasBlock())
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _heart.SetBlock(0);
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _heart.SetBlock(Random.Range(1, 2));
                    }
                    break;
                case Cure.Type.Digoxin:
                    if (_crpLowering)
                    {
                        if (CanCardiacArrest()) _heart.CauseCardiacArrest(true);
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    else if (_crpRaising)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _crpRaising = false;
                        _crp = 2;
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _crpLowering = true;
                    }
                    break;
                case Cure.Type.Ibuprofen:
                    if (_crpRaising)
                    {
                        if (CanCardiacArrest()) _heart.CauseCardiacArrest(true);
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    else if (_crpLowering)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _crpLowering = false;
                        _crp = 2;
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _crpRaising = true;
                    }
                    break;
                case Cure.Type.Furosemide:
                    if (_bnpRaising) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    else
                    {
                        if (CanCardiacArrest()) _heart.CauseCardiacArrest(true);
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }

                    _bnpRaising = false;
                    _bnp = 200;
                    break;
                case Cure.Type.Defibrillator:
                    DefibAnimation();
                    if (_heart.HasVentricularFibrillation())
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _heart.SetVentricularFibrillation(false);
                    }
                    else if (_heart.IsDead())
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        Revive();
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);

                        _heart.CauseCardiacArrest(true);
                    }
                    break;
                case Cure.Type.Insulin:
                    if (_diabetesAttacking)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                        _diabetesAttacking = false;
                        _hunger = 1;
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("incorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                        _hunger = _settings.tooMuchInsulinHungerLevel;
                    }
                    break;
                case Cure.Type.Food:
                    _hunger = 1f;
                    break;
                case Cure.Type.Heat:
                    if (_tempLowering)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                        _temp = Random.Range(34, 37);
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    _tempLowering = false;
                    break;
                case Cure.Type.Oxygen:
                    if (_oxygenAttacking)
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("CorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    else
                    {
                        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("IncorrectTreatment", PlazmaGames.Audio.AudioType.Sfx, false, true);
                    }
                    _oxygenAttacking = false;
                    break;
            }
        }

        private void DefibAnimation()
        {
            float rotX = _body.localRotation.eulerAngles.x;
            float rotY = _body.localRotation.eulerAngles.y;
            float rotZ = _body.localRotation.eulerAngles.z;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _settings.defibAniDuration,
                (progress) => _body.localRotation = Quaternion.Euler(rotX, rotY, rotZ + Mathf.Sin(progress * _settings.defibAniSpeed) * _settings.defibAniAmplitude),
                () => _body.localRotation = Quaternion.Euler(rotX, rotY, rotZ));
        }

        private void Revive()
        {
            if (!IsDead()) return;
            if (_heart.DeadTime() > _settings.reviveTime) return;
            _heart.Revive();
        }

        private void InjectInsulin()
        {
            _diabetesAttacking = false;
        }

        private void Tick()
        {
            if (_isPaused) return;
            _lastTick = Time.time;

            bool godSave = _god;
            if (_settings.tickRate > 1) _god = true;

            if (IsDeadForReal())
            {
                _oxygen = 0;
                _temp = 0;
                _bloodSugar = 0;
                return;
            }

            if (!_god && !_oxygenAttacking && Random.value < (1f / _settings.oxygenAttackChance))
                _oxygenAttacking = true;

            if (_oxygenAttacking) _oxygen = Mathf.Max(_settings.oxygenMinValue, _oxygen - _settings.oxygenDepletionRate);
            else _oxygen = Mathf.Min(99f, _oxygen + _settings.oxygenRestorationRate);

            if (!_god && _diabetes && !_diabetesAttacking && Random.value < (1f / _settings.diabetesAttackChance) && _hunger < 1)
            {
                _diabetesAttacking = true;
                NextBloodSugarNormalValue();
            }

            if (!_god) _hunger = Mathf.Max(0, _hunger - _settings.hungerDepletionRate);
            if (_diabetesAttacking) _hunger = 1f;
            
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

            if (_oxygen < _settings.oxygenDangerousLevel && !_achRaising)
                _meanHeartRate = MapRange(
                    _oxygen,
                    _settings.oxygenDeathValue, _settings.oxygenDangerousLevel,
                    10, _restingHeartRate);
            else if (_bloodSugar < _settings.bloodSugarDangerousLow && !_achRaising)
                _meanHeartRate = MapRange(
                    _bloodSugar,
                    20, _settings.bloodSugarDangerousLow,
                    10, _restingHeartRate);
            else if (_bloodSugar > _settings.bloodSugarDangerousHigh && !_achLowering)
                _meanHeartRate = MapRange(
                    _bloodSugar,
                    _settings.bloodSugarDangerousHigh, _settings.diabetesAttackBloodSugarValue,
                    _restingHeartRate, 280);

            if (!_god && _tick % 5 == 0)
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
            if (_crpLowering) _crp -= _settings.crpRate;
            else if (_crpRaising) _crp += _settings.crpRate;
            if (_bnpRaising) _bnp += _settings.bnpRate;
            
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
                if (CanCardiacArrest() && Random.value < _settings.caProb) _heart.CauseCardiacArrest(true);
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
                if (CanCardiacArrest() && Random.value < _settings.caProb) _heart.CauseCardiacArrest(true);
            }
            else
            {
                _heart.SetJWave(0);
            }

            _god = godSave;
        
            _tick += 1;
        }

        private bool CanCardiacArrest() => !IsDead() && _heart.DeadTime() > _settings.heartGracePeriod;

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
