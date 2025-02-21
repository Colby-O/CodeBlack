using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack
{
    [CreateAssetMenu(fileName = "PatientSettings", menuName = "Patient/Settings")]
    public sealed class PatientSettings : ScriptableObject
    {
        public float tickRate = 1.0f;
        
        [Header("Diabetes")]
        public float diabetesAttackChance = 160;
        public float diabetesAttackBloodSugarValue = 330;
        public float tooMuchInsulinHungerLevel = 0.2f;
        
        [Header("Temperature")]
        public float normalTemp = 37f;
        public float tempRateRand = 0.01f;
        public float tempRate = 0.1f;
        public float tempRecoverRate = 0.5f;
        public float tempDangerousLevel = 32f;
        public float tempDeadLevel = 25f;

        [Header("Oxygen")]
        public float oxygenAttackChance = 300f;
        public float oxygenDepletionRate = 0.5f;
        public float oxygenRestorationRate = 1.5f;
        public float oxygenMinValue = 20f;
        public float oxygenDangerousLevel = 80f;
        public float oxygenDeathValue = 52f;
        
        [Header("Hunger")]
        public float hungerDepletionRate = 0.01f;
        public float hungerCutoff = 0.7f;
        
        [Header("Blood Sugar")]
        public float bloodSugarHungerLow = 20;
        public float bloodSugarNormalValue = 120;
        public float bloodSugarRestorationRate = 18;
        public float bloodSugarMinRate = 0.1f;
        public float bloodSugarMaxRate = 7f;
        public float bloodSugarVariance = 0.2f;
        public int bloodSugarRateChangeRate = 7;
        public int bloodSugarNormalValueChangeRate = 15;
        public float bloodSugarHeartDropRate = 0.3f;
        public float bloodSugarDangerousLow = 80f;
        public float bloodSugarDangerousHigh = 200f;

        [Header("Heart")]
        public float reviveTime = 60 + 45;
        public float restingHeartRateLow = 70f;
        public float restingHeartRateHigh = 110f;
        [Header("Heart Stats")]
        public float eventProb = 0.1f;
        public float caProb = 0.1f;
        public float afProb = 0.1f;
        public float vfProb = 0.1f;
        public float blockProb = 0.1f;
        public float achRate = 0.01f;
        public float crpRate = 0.01f;
        public float bnpRate = 0.1f;
        public float achRateRand = 0.001f;
        public float crpRateRand = 0.001f;
        public float bnpRateRand = 0.01f;
    }
}
