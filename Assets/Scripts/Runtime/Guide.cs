using CodeBlack.ECG;
using PlazmaGames.Runtime.DataStructures;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBlack
{
    public enum HeartCondition
    {
        Normal,
        Tachycardia,
        Bradycardia,
        HeartBlock,
        AtrialFibrillation,
        VentricularFibrillation,
        Hypothermia,
        CardiacArrest
    }

    public class Guide : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Heart _heart;
        [SerializeField] private PatientSettings _settings;

        [Header("Vitals")]
        [SerializeField] private TMP_Text _bloodSuger;
        [SerializeField] private TMP_Text _bpm;
        [SerializeField] private TMP_Text _SpO2;
        [SerializeField] private TMP_Text _bloodPressure;
        [SerializeField] private TMP_Text _ekgTitle;

        [Header("Info")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _descripton;
        [SerializeField] private TMP_Text _traetment;
        [SerializeField] private SerializableDictionary<HeartCondition, GameObject> _cures;

        private int _pos = 0;
        private bool _revive = false;

        private HeartCondition _last;

        public void Show(HeartCondition condition)
        {
            _heart.SetHeartRate(58f / 60f);
            _heart.SetAtrialFibrillation(false);
            _heart.SetBlock(0);
            _heart.SetJWave(0);
            _heart.SetVentricularFibrillation(false, false, true);

            foreach (GameObject obj in _cures.Values)
            {
                obj.SetActive(false);
            }

            _cures[condition].SetActive(true);

            _SpO2.text = "NA";
            _bloodSuger.text = "NA";

            switch (condition)
            {
                case HeartCondition.Normal:
                    _descripton.text = $"Healthy patient. Nothing needs to be done.";
                    _traetment.text = $"NA";
                    break;
                case HeartCondition.Tachycardia:
                    _heart.SetHeartRate(58f / 210f);

                    _bloodSuger.text = $"<{_settings.bloodSugarDangerousHigh}";

                    _descripton.text = $"A elevated heart rate above 190 BPM. The condition can be caused by the following:\n �\tDiabetes, high blood sugar levels greater than {_settings.bloodSugarDangerousHigh} mg/dL. \n �\tPre-existing heart condition.\n\nBeing Tachycardic for too long will result in Cardiac Arrest or Atrial Fibrillation. See the respective entries for more information about treating these conditions.";
                    _traetment.text = $"\n� Heart rate can be lowered with Beta-Blockers.\n\n� If blood sugar is high treat with insulin.";
                    break;
                case HeartCondition.Bradycardia:
                    _heart.SetHeartRate(58f / 20f);

                    _SpO2.text = $">{_settings.bloodSugarDangerousLow}";
                    _bloodSuger.text = $">{_settings.oxygenDangerousLevel}";

                    _descripton.text = $"A slow heart rate below 22 BPM. The condition can be caused by the following:\n �\tStarving, low blood sugar levels less than {_settings.bloodSugarDangerousLow} mg/dL. \n �\tLow Oxygen (SpO2) levels less then {_settings.oxygenDangerousLevel}%.\n �\tPre-existing heart condition.\n\nBeing Bradycardic for too long will result in Cardiac Arrest or a Heat Block. See future entries for more information about treating these conditions.";
                    _traetment.text = $"\n� Heart rate can be raised with Adrenaline.\n\n� If blood sugar is low treat with food.\n\n � If SpO2 is low treat with oxygen.";
                    break;
                case HeartCondition.HeartBlock:
                    _heart.SetBlock(1);

                    _descripton.text = $"A condition that causes the heart to skip a beat. On the EKG this is seen as a one or two extra p-waves (tiny waves) between beats. A Heart Block is caused by low CRP levels. This condition can cause Cardiac Arrest if left untreated.";
                    _traetment.text = $"\n� Heart Block can be treated with Atropine.\n� After the patient must be treated with Ibuprofen treated to prevent further episodes.";

                    break;
                case HeartCondition.AtrialFibrillation:
                    _heart.SetAtrialFibrillation(true);

                    _descripton.text = $"A condition that causes the heart to beat in irregular intervals. Atrial Fibrillation is caused by a elevated heart rate from elevate CRP levels. This condition can cause Cardiac Arrest if left untreated.";
                    _traetment.text = $"\n� Atrial Fibrillation can be treated with Calcium Channel Blockers.\n� After the patient must be treated with Digoxin treated to prevent further episodes.";
                    break;
                case HeartCondition.VentricularFibrillation:
                    _heart.SetVentricularFibrillation(true);

                    _descripton.text = $"A condition that causes the heart to contract in a abnormal heart rhythm. This looks like random noise on the EKG. This condition immediately cause a Cardiac Arrest.";
                    _traetment.text = $"\n� Patients with Ventricular Fibrillation can must be revived with a defibrillator and then treated with then with Furosemide to prevent further episodes.";
                    break;
                case HeartCondition.Hypothermia:
                    _heart.SetJWave(1f);

                    _descripton.text = $"The patients body temperature is lower than 30 degrees. Hypothermia causes an abnormal EKG which can be seen as a second peak (J-Wave). This condition can cause a Cardiac Arrest if left untreated.";
                    _traetment.text = $"\n� Patients with Hypothermia can be treated by raising their room temperature using the thermostat in their room.";
                    break;
                case HeartCondition.CardiacArrest:
                    _heart.CauseCardiacArrest(true);

                    _descripton.text = $"The heart stops and is no longer functional.";
                    _traetment.text = $"\n� Patients with Cardiac Arrest can be revived with a defibrillator. The condition that caused the Cardiac Arrest must be treat to prevent further episodes.";
                    break;

            }

            if (_last == HeartCondition.CardiacArrest || _last == HeartCondition.VentricularFibrillation) _heart.Revive();

            _ekgTitle.text = $"{condition} EKG";
            _title.text = $"{condition}";

            _last = condition;
        }

        public bool Next()
        {
            _pos++;
            Show((HeartCondition)_pos);
            return _pos != 7;
        }

        public bool Back()
        {
            _pos--;
            Show((HeartCondition)_pos);
            return _pos != 0;
        }

        private void OnEnable()
        {
            Show((HeartCondition)_pos);
        }

        private void Update()
        {
            _bpm.text = ((int)_heart.Bpm()).ToString();
            _bloodPressure.text = $"{(int)_heart.Sbp()}/{(int)_heart.Dbp()}";

            if (_heart.IsDead() && _last != HeartCondition.CardiacArrest && _last != HeartCondition.VentricularFibrillation)
            {
                _heart.Revive();
            }
        }
    }
}
