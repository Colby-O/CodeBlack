using CodeBlack;
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

    private bool _hasRan = false;
    private bool _hasJumpscared;

    private float _timer = 0;

    protected override void OnEnter()
    {
        _hasRan = false;
        CodeBlackGameManager.player.SetHeadPostion(_target.position);
        _you.SetAchRasing();
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
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        CodeBlackGameManager.player.SetHeadPostion(_target2.position, true);
        yield return new WaitForSeconds(Random.Range(2f, 10f));
        _devil.SetActive(true);
        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_clip, PlazmaGames.Audio.AudioType.Sfx, true, true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void Update()
    {
        if (_you.IsDead() && !_hasRan)
        {
            _hasRan = true;
            Debug.Log("Trigger Jumpscare");
            StartCoroutine(Jumpscare());
        }

        if (_hasRan && !_hasJumpscared && _timer > 60f)
        {
            RestartGame();
        }
        else if (_hasRan && !_hasJumpscared)
        {
            _timer += Time.deltaTime;
        }
    }
}
