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
    private float _timer2 = 0;

    private bool _hasDied = false;


    protected override void OnEnter()
    {
        _hasRan = true;
        _hasDied = false;
        _timer2 = 0;
        CodeBlackGameManager.player.SetHeadPostion(_target.position);
        _you.SetAchRasing();
        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_final, PlazmaGames.Audio.AudioType.Music, false, false);
    }

    protected override void OnExit()
    {

    }

    private void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
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
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void Update()
    {
        if (!_hasRan || !_isTriggeredEnter) return;

        if (_you.IsDead())
        {
            _hasDied = true;
            StartCoroutine(Jumpscare());
        }

        if (!_hasDied && _timer2 > _final.length)
        {
            _hasDied = true;
            _heart.CauseCardiacArrest(true);
            StartCoroutine(Jumpscare());
        }
        else _timer2 += Time.deltaTime;
    }
}
