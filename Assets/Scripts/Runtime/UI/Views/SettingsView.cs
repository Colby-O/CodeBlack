
using CodeBlack;
using CodeBlack.ECG;
using CodeBlack.Events;
using CodeBlack.Player;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Utils;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBlack.UI
{
    public class SettingsView : View
    {
        [SerializeField] private TMP_Text _title;

        [SerializeField] private Slider _overall;
        [SerializeField] private Slider _sfx;
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _sensivity;
        [SerializeField] private Toggle _invertx;
        [SerializeField] private Toggle _inverty;
        [SerializeField] private Toggle _hints;
        [SerializeField] private Button _back;
        [SerializeField] private TMP_Dropdown _language;

        [SerializeField] private TMP_Text _overallT;
        [SerializeField] private TMP_Text _sfxT;
        [SerializeField] private TMP_Text _musicT;
        [SerializeField] private TMP_Text _sensivityT;
        [SerializeField] private TMP_Text _invertxT;
        [SerializeField] private TMP_Text _invertyT;
        [SerializeField] private TMP_Text _hintsT;
        [SerializeField] private TMP_Text _backT;
        [SerializeField] private TMP_Text _languageT;

        [SerializeField] private PlayerSettings _playerSettings;

        [SerializeField] private SerializableDictionary<Languages, List<string>> _titles;

        [SerializeField] private Heart _heart;

        public bool EnabledHints { get; set; }

        private void Back()
        {
            GameManager.EmitEvent(new CBEvents.CloseMenu());
        }

        private void Overall(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetOverallVolume(val);
        }

        private void SfX(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetSfXVolume(val);
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetAmbientVolume(val);
        }

        private void Music(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetMusicVolume(val);
        }

        private void Sensivity(float val)
        {
            _playerSettings.sensitivityX = Mathf.Lerp(12, 48, val);
            _playerSettings.sensitivityY = Mathf.Lerp(12, 48, val);
        }

        private void InvertX(bool val)
        {
            _playerSettings.invertedViewX = val;
        }

        private void InvertY(bool val)
        {
            _playerSettings.invertedViewY = !val;
        }

        private void Language(int val)
        {
            CodeBlackGameManager.language = (Languages)val;
        }

        private void Hints(bool val)
        {
            EnabledHints = val;
        }

        public override void Init()
        {
            DropdownUtilities.SetDropdownOptions<Languages>(ref _language);

            EnabledHints = true;

            _overall.onValueChanged.AddListener(Overall);
            _sfx.onValueChanged.AddListener(SfX);
            _music.onValueChanged.AddListener(Music);
            _sensivity.onValueChanged.AddListener(Sensivity);
            _back.onClick.AddListener(Back);
            _invertx.onValueChanged.AddListener(InvertX);
            _inverty.onValueChanged.AddListener(InvertY);
            _language.onValueChanged.AddListener(Language);
            _hints.onValueChanged.AddListener(Hints);

            _hints.isOn = EnabledHints;
            _overall.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _music.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetMusicVolume();
            _sfx.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetSfXVolume();
            _sensivity.value = Mathf.InverseLerp(12, 48, _playerSettings.sensitivityX);
            _invertx.isOn = _playerSettings.invertedViewX;
            _inverty.isOn = !_playerSettings.invertedViewY;
            _language.value = (int)CodeBlackGameManager.language;
        }

        private void Start()
        {
            _overall.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _music.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetMusicVolume();
            _sfx.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetSfXVolume();
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            _overall.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _music.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetMusicVolume();
            _sfx.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetSfXVolume();
            _sensivity.value = Mathf.InverseLerp(12, 48, _playerSettings.sensitivityX);
            _invertx.isOn = _playerSettings.invertedViewX;
            _inverty.isOn = !_playerSettings.invertedViewY;
            _language.value = (int)CodeBlackGameManager.language;
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _title.text = _titles[CodeBlackGameManager.language][0];
            _overallT.text = _titles[CodeBlackGameManager.language][1];
            _sfxT.text = _titles[CodeBlackGameManager.language][2];
            _musicT.text = _titles[CodeBlackGameManager.language][3];
            _sensivityT.text = _titles[CodeBlackGameManager.language][4];
            _invertxT.text = _titles[CodeBlackGameManager.language][5];
            _invertyT.text = _titles[CodeBlackGameManager.language][6];
            _languageT.text = _titles[CodeBlackGameManager.language][7];
            _backT.text = _titles[CodeBlackGameManager.language][8];
            _hintsT.text = _titles[CodeBlackGameManager.language][9];
        }
    }
}
