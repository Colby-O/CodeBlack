using CodeBlack;
using CodeBlack.ECG;
using CodeBlack.Trigger;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger2 : Trigger
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _target2;
    [SerializeField] private Patient _you;
    [SerializeField] private GameObject _devil;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private AudioClip _final;
    [SerializeField] private Heart _heart;

    private bool _hasRan = false;

    private float _timer = 0;



    protected override void OnEnter()
    {
        _hasRan = true;
        CodeBlackGameManager.ending = true;
        CodeBlackGameManager.player.SetHeadPostion(_target.position);
        _you.SetAchRasing();
        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_final, PlazmaGames.Audio.AudioType.Music, false, false);
    }

    protected override void OnExit()
    {

    }

    private IEnumerator Jumpscare()
    {
        GameManager.GetMonoSystem<IAudioMonoSystem>().StopAudio(PlazmaGames.Audio.AudioType.Music);
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        CodeBlackGameManager.player.SetHeadPostion(_target2.position, true);
        yield return new WaitForSeconds(Random.Range(2f, 10f));
        _devil.SetActive(true);
        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_clip, PlazmaGames.Audio.AudioType.Sfx, false, true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        Application.Quit();
    }

    private void Update()
    {
        if (!_hasRan || !_isTriggeredEnter) return;

        _timer += Time.deltaTime;

        if (_timer > _final.length)
        {
            _heart.CauseCardiacArrest(true);
            StartCoroutine(Jumpscare());
        }
    }
}
