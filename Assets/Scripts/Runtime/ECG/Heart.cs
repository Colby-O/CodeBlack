using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Attribute;

namespace CodeBlack.ECG
{
	public class Heart : MonoBehaviour
	{

		[SerializeField] private bool _triggerSANode = false; 
		private bool _triggerAVNode = false;
		[SerializeField] private bool _playSound = true;
		[SerializeField] private bool _triggerCardiacArrest = false;

		[SerializeField, ReadOnly] private float _vSum;  
		[SerializeField, ReadOnly] private float _vSumLast;    
		private float _vP;       
		private float _vQRS;      
		private float _vT;       
		private float _vU;      
		private float _vOsborn;
		
		private float _prevRWave;

		private float _ptt;
		private float _sbp;
		private float _dbp;
		private float _k1;
		private float _k2;
		private float _k3;
		private float _k4;

		private List<float> _prevBeats = new List<float>();
		private int _bmpBufferSize = 8;

		[SerializeField] private float _cardiacCycle;
		[SerializeField, ReadOnly] private float _cardiacCycleRequest;
		private float _cardiacCycleRaw = 1.0f;

		private float[] intervalArrayP;

		[SerializeField] private float _heartRate = 60.0f;
		[SerializeField] private float _invertUWave = 2.0f;
		[SerializeField] private float _addJWave = 0.0f;
		[SerializeField] private float _patientMovement = 0.0f;
		[SerializeField] private int _heartBlock = 0;
		[SerializeField] private bool _blockRandom = false;
		[SerializeField] private bool _atrialFibrillation = false;
		[SerializeField, ReadOnly] private bool _ventricularFibrillation = false;
		[SerializeField, ReadOnly] private int _blocks = 0;
		private float _sbpScale = 1;
		private float _dbpScale = 1;
		private float _deadTime;

		public float Bpm() => 60.0f / (_prevBeats.Sum() / _prevBeats.Count);
		public float Sbp() => (_k1 - _k2 * _ptt) * _sbpScale;
		public float Dbp() => (_k3 - _k4 * _ptt) * _dbpScale;

		public float DeadTime() => Time.time - _deadTime;

		public void SetJWave(float jWave)
		{
			_addJWave = jWave;
		}

		public bool HasBlock() => _heartBlock > 0;
		public void SetBlock(int n)
		{
			_heartBlock = n;
		}

		public bool HasAtrialFibrillation() => _atrialFibrillation;
		public void SetAtrialFibrillation(bool state)
		{
			_atrialFibrillation = state;
		}

		public bool HasVentricularFibrillation() => _ventricularFibrillation;
		public void SetVentricularFibrillation(bool state, bool autoRevive = true, bool force = false)
		{
			if ((state == _ventricularFibrillation || _triggerCardiacArrest) && !force) return;
			if (state)
			{
				CauseCardiacArrest(true);
				_patientMovement = 10;
			}
			else
			{
				CauseCardiacArrest(false);
				_patientMovement = 0;
				if (autoRevive) _triggerSANode = true;
			}
			
			_ventricularFibrillation = state;
		}

		public bool GetAtrialFibrillationState()
		{
			return _atrialFibrillation;
		}

		public bool GetBlockState()
		{
			return _heartBlock != 0;
		}

		public void SetSound(bool sound)
		{
			_playSound = sound;
			GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().enabled = sound;
        }

		public bool HasCardiacArrest() => _triggerCardiacArrest;

		public bool IsDead() => _triggerCardiacArrest && !_ventricularFibrillation;
		public bool IsAlive() => !IsDead();

		public void Revive()
		{
			CauseCardiacArrest(false);
			_triggerSANode = true;
		}
		
		public void CauseCardiacArrest(bool state)
		{
			if (_playSound) gameObject.GetComponent<AudioSource>().loop = state;

			if (state)
			{
				_deadTime = Time.time;
				if (_ventricularFibrillation) _ventricularFibrillation = false;
				if (_playSound)
				{
					gameObject.GetComponent<AudioSource>().pitch = 4;
					gameObject.GetComponent<AudioSource>().Play();
				}
				_patientMovement = 0;
				_prevBeats.Clear();
				_prevBeats.Add(Mathf.Infinity);  
			}
			else
			{
				if (_playSound) gameObject.GetComponent<AudioSource>().pitch = 1;
			}
			_triggerCardiacArrest = state;
		}

		public void StartHeart(bool state)
		{
			_triggerSANode = true;
		}

		public void SetHeartRate(float rate)
		{
			_cardiacCycleRequest = rate;
			//StopAllCoroutines();
			//_triggerSANode = true;
		}

		public float GetHeartRate() => _cardiacCycle;

		public bool IsHealthty()
		{
			return Bpm() > 22 && Bpm() < 190 && !_atrialFibrillation && !_ventricularFibrillation && !_triggerCardiacArrest && _addJWave == 0 && !_blockRandom && _heartBlock == 0;
		}

		public Vector2 GetVoltage()
		{
			return new Vector2(_vSum, _vSumLast);
		}

		private IEnumerator PInterval()
		{
			_heartRate = 0.0f;
			for (int i = 0; i < 4; i++)
			{
				intervalArrayP[i] = intervalArrayP[i + 1];
				_heartRate += intervalArrayP[i];
			}
			intervalArrayP[4] = _cardiacCycleRaw;
			_heartRate += intervalArrayP[4];
			_heartRate *= 0.2f;
			yield return new WaitForSeconds(_cardiacCycleRaw);

			_triggerSANode = true;
		}


		private IEnumerator PWave()
		{
			if (_blockRandom)
			{
				if (_blocks < 0) _blocks = Random.Range(0, 4);
			}
			else
			{
				if (_blocks < 0) _blocks = Mathf.Abs(_heartBlock);
			}

			float i = 0.0f;

			float rate = 12.5f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vP = Mathf.SmoothStep(0.0f, 1.5f, i);
				yield return new WaitForFixedUpdate();
			}

			i = 0.0f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vP = Mathf.SmoothStep(1.5f, 0.0f, i);
				yield return new WaitForFixedUpdate();
			}
			yield return new WaitForSeconds(0.02f);
			_triggerAVNode = _blocks == 0;
			_blocks--;
		}

		private IEnumerator QRSWave()   
		{
			float i = 0.0f;

			float rate; 

			float complexTimer;

			complexTimer = 0.08f;    
			rate = 25;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vQRS = Mathf.SmoothStep(0.0f, -1.0f, i);
				yield return new WaitForFixedUpdate();
			}

			i = 0.0f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vQRS = Mathf.Lerp(-1.0f, 8.0f, i);
				yield return new WaitForFixedUpdate();
			}

			if (_playSound)
			{
				gameObject.GetComponent<AudioSource>().Play(); 
			}
			

			float now = Time.time;
			_ptt = Time.time - _prevRWave;
			_prevRWave = now;
			
			_prevBeats.Add(_ptt);
			if (_prevBeats.Count > _bmpBufferSize) _prevBeats.RemoveAt(0);

			i = 0.0f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vQRS = Mathf.Lerp(8.0f, -2.0f, i);
				yield return new WaitForFixedUpdate();
			}

			i = 0.0f;

			rate = 100.0f;

			StartCoroutine(JWave());

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vQRS = Mathf.Lerp(-2.0f, 0.0f, i);
				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds(complexTimer * 0.5f); 
			StartCoroutine(TWave());
		}


		private IEnumerator JWave()
		{
			float i = 0.0f;

			float rate = 25.0f; 

			float tempCoreSqrd;

			tempCoreSqrd = _addJWave * _addJWave;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vOsborn = Mathf.SmoothStep(0.0f, 6.0f * tempCoreSqrd, i);
				yield return new WaitForFixedUpdate();
			}

			i = 0.0f;

			rate = 25.0f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vOsborn = Mathf.SmoothStep(6.0f * tempCoreSqrd, 0.0f, i);
				yield return new WaitForFixedUpdate();
			}
		}


		private IEnumerator TWave()   
		{
			float i = 0.0f;

			float rate = 6.25f; 

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vT = Mathf.SmoothStep(0.0f, 2.0f, i);
				yield return new WaitForFixedUpdate();
			}

			i = 0.0f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vT = Mathf.SmoothStep(2.0f, 0.0f, i);
				yield return new WaitForFixedUpdate();
			}
			StartCoroutine(UWave());
		}

		private IEnumerator UWave()
		{
			float i = 0.0f;

			float rate = 25.0f; 

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vU = Mathf.SmoothStep(0.0f, 2.0f - _invertUWave, i);
				yield return new WaitForFixedUpdate();
			}

			i = 0.0f;

			while (i < 1.0f)
			{
				i += Time.deltaTime * rate;
				_vU = Mathf.SmoothStep(2.0f - _invertUWave, 0.0f, i);
				yield return new WaitForFixedUpdate();
			}
		}

        private void OnEnable()
        {
			_prevBeats.Clear();
            for (int i = 0; i < _bmpBufferSize; i++) _prevBeats.Add(0.6f);
        }

        private void Awake()
		{
			for (int i = 0; i < _bmpBufferSize; i++) _prevBeats.Add(0.6f);
			intervalArrayP = new float[5];
			_triggerSANode = false;
			_triggerAVNode = false;
			_k1 = Random.Range(100f, 140f);
			_k2 = Random.Range(0.5f, 1.5f);
			_k3 = Random.Range(70f, 100f);
			_k4 = Random.Range(0.2f, 0.8f);
		}

		private void Start()
		{
			_triggerSANode = true; 
		}

		private void Update()
		{
			if (_triggerCardiacArrest) 
			{
				_triggerSANode = false;
				_triggerAVNode = false;
				_vP = 0.0f;
				_vQRS = 0.0f;
				_vT = 0.0f;
				_vU = 0.0f;
				_vOsborn = 0.0f;
			}
			else   
			{
				if (_triggerSANode)  
				{
					if (_atrialFibrillation) _cardiacCycleRaw = Mathf.Clamp(Random.value, 0.2f, 1.0f) * _cardiacCycle;
					else if (_cardiacCycleRequest != 0)
					{
						_cardiacCycle = _cardiacCycleRequest;
						_cardiacCycleRaw = _cardiacCycle;
						_cardiacCycleRequest = 0;
					}
					else _cardiacCycleRaw = _cardiacCycle;
					StartCoroutine(PWave());   
					StartCoroutine(PInterval());   
					_triggerSANode = false;
				}
				if (_triggerAVNode)
				{
					StartCoroutine(QRSWave());
					_triggerAVNode = false;
				}
			}
		}


		private void FixedUpdate()
		{
			_vSumLast = _vSum;
			if (!_atrialFibrillation) _vSum = _vP + _vQRS + _vT + _vU + _vOsborn + (_patientMovement * (-0.5f + Random.value));
			else _vSum = _vP + _vQRS + _vT + _vU + _vOsborn + ((_patientMovement + 1.0f) * (-0.5f + Random.value));
			
			
		}
	}
}
