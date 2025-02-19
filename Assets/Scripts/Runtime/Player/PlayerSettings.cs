using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack.Player
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Settings")]
    public sealed class PlayerSettings : ScriptableObject
    {
        [Header("View Settings")]
        public float sensitivityMax;
        public float sensitivityMin;
        public float sensitivityX;
        public float sensitivityY;
        public bool invertedViewX;
        public bool invertedViewY;
        [Range(0, 90)] public float maxViewY = 80;
        [Range(-90, 0)] public float minViewY = -80;

        [Header("Movement -- Walking")]
        public float walkingForwardSpeed;
        public float walkingBackwardSpeed;
        public float walkingStrateSpeed;
        public float movementSmoothing;
        public float cartSpeed;
        public float cartTurnSpeed;
        public float cartSmoothing;

        [Header("Speed Effects")]
        public float speedEffector = 1.0f;
        public float crouchSpeedEffector;
    }
}
