using CodeBlack.Events;
using CodeBlack.Player;
using PlazmaGames.Animation;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace CodeBlack
{
    public enum Languages
    {
        EN,
        FR
    }

    public class CodeBlackGameManager : GameManager
    {
        [Header("Holders")]
        [SerializeField] private GameObject _monoSystemParnet;

        [Header("MonoSystems")]
        [SerializeField] private UIMonoSystem _uiMonoSystem;
        [SerializeField] private AnimationMonoSystem _animationMonoSystem;
        [SerializeField] private AudioMonoSystem _audioMonoSystem;
        
        [SerializeField] private float _timeScale = 5f;
        public static bool firstStockCart = false;

        public static Languages language;
        public static PlayerController player;
        public static bool isPaused = true;
        public static bool hasStarted = false;
        public static bool ending = false;

        private static float _runningTime;
        public static float RunningTime() => _runningTime;

        public override string GetApplicationName()
        {
            return nameof(CodeBlackGameManager);
        }

        private void AttachMonoSystems()
        {
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiMonoSystem);
            AddMonoSystem<AnimationMonoSystem, IAnimationMonoSystem>(_animationMonoSystem);
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioMonoSystem);
        }

        private void AddEvents()
        {
            AddEventListener<CBEvents.OpenMenu>(UIGameEvents.OpenMenuResponse);
            AddEventListener<CBEvents.CloseMenu>(UIGameEvents.CloseMenuResponse);

            AddEventListener<CBEvents.Start>(GenericGameEvents.StartResponse);
            AddEventListener<CBEvents.Quit>(GenericGameEvents.QuitResponse);
        }

        private void AddListeners()
        {

        }

        public override string GetApplicationVersion()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInitalized()
        {
            // Ataches all MonoSystems to the GameManager
            AttachMonoSystems();

            // Adds Event Listeners
            AddEvents();

            // Adds Event Listeners
            AddListeners();

            // Ensures all MonoSystems call Awake at the same time.
            _monoSystemParnet.SetActive(true);
        }

        private void Start()
        {
           player = FindAnyObjectByType<PlayerController>();
        }

        protected void FixedUpdate()
        {
            if (
                !isPaused &&
                !(firstStockCart && (int)(_runningTime + Time.deltaTime * _timeScale) / (60 * 60) > 0) &&
                (int)_runningTime / (60 * 60) < 8
            ) _runningTime += Time.deltaTime * _timeScale;
        }
    }
}
