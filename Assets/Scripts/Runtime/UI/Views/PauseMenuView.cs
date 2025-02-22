using CodeBlack;
using CodeBlack.ECG;
using CodeBlack.Events;
using CodeBlack.UI;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : View
{
    [SerializeField] private GameObject _bg;
    [SerializeField] private GameObject _ekg;
    [SerializeField] private Heart _heart;

    [SerializeField] private Button _resume;
    [SerializeField] private Button _setting;
    [SerializeField] private Button _quit;


    private void Resume()
    {
        _bg.SetActive(false);
        _ekg.SetActive(false);
        GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
    }

    private void Setting()
    {
        GameManager.EmitEvent(new CBEvents.OpenMenu(true, true, typeof(SettingsView)));
    }

    private void Quit()
    {
        Application.Quit();
    }

    public override void Show()
    {
        base.Show();
        CodeBlackGameManager.isPaused = true;
        CodeBlackGameManager.hasStarted = false;
        _bg.SetActive(true);
        _ekg.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (GameManager.HasMonoSystem<IAudioMonoSystem>()) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(2, PlazmaGames.Audio.AudioType.Music, true, false);

        _heart.Revive();
    }

    public override void Hide()
    {
        base.Hide();
        CodeBlackGameManager.isPaused = false;
        CodeBlackGameManager.hasStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (GameManager.HasMonoSystem<IAudioMonoSystem>()) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(1, PlazmaGames.Audio.AudioType.Music, true, false);
    }

    public override void Init()
    {
        _resume.onClick.AddListener(Resume);
        _setting.onClick.AddListener(Setting);
        _quit.onClick.AddListener(Quit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
        }
    }
}
