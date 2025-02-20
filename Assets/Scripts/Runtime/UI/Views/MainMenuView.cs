using CodeBlack.ECG;
using CodeBlack.Events;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBlack.UI
{
    public class MainMenuView : View
    {
        [SerializeField] private TMP_Text _title;

        [SerializeField] private Button _play;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit;

        [SerializeField] private TMP_Text _playT;
        [SerializeField] private TMP_Text _settingsT;
        [SerializeField] private TMP_Text _exitT;

        [SerializeField] private GameObject _bg;
        [SerializeField] private GameObject _ekg;
        [SerializeField] private Heart _heart;

        [SerializeField] private SerializableDictionary<Languages, List<string>> _titles;

        private void Play()
        {
            _bg.SetActive(false);
            _ekg.SetActive(false);
            GameManager.EmitEvent(new CBEvents.Start());
        }

        private void Settings()
        {
            GameManager.EmitEvent(new CBEvents.OpenMenu(true, true, typeof(SettingsView)));
        }

        private void Exit()
        {
            GameManager.EmitEvent(new CBEvents.Quit());
        }

        public override void Init()
        {
            _play.onClick.AddListener(Play);
            _settings.onClick.AddListener(Settings);
            _exit.onClick.AddListener(Exit);

            _bg.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            _heart.SetBlock(0);
            _bg.SetActive(true);
            _ekg.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(0, PlazmaGames.Audio.AudioType.Music, true, false);
        }

        private void Update()
        {
            _title.text = _titles[CodeBlackGameManager.language][0];
            _playT.text = _titles[CodeBlackGameManager.language][1];
            _settingsT.text = _titles[CodeBlackGameManager.language][2];
            _exitT.text = _titles[CodeBlackGameManager.language][3];
        }
    }
}
