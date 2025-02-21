using CodeBlack;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAController : MonoBehaviour
{
    private PatientManager _pm;
    [SerializeField] private AudioSource _as;
    [SerializeField] private float _repeatTime = 10f;
    [SerializeField] private AudioClip _everyoneDiedClip;

    private bool _isPlaying;

    [SerializeField] private GameObject _endingMap;


    private IEnumerator RepeatAnnouncement(Patient p)
    {
        if (!_pm.AllPatientsDeadForReal() && !p.IsDeadForReal()) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(p.GetDyingAudiio(), PlazmaGames.Audio.AudioType.Ambient, false, false);
        yield return new WaitForSeconds(_repeatTime  + Random.Range(0f, 15f));
        if (p.IsDead() && !p.IsDeadForReal()) StartCoroutine(RepeatAnnouncement(p));
    }

    private void AnnouncementDying(Patient p)
    {
        _isPlaying = true;
        _as.Play();
        StartCoroutine(RepeatAnnouncement(p));
    }

    private void AnnouncementDeath(Patient p)
    {
        if (_pm.AllPatientsDeadForReal()) GameManager.GetMonoSystem<IAudioMonoSystem>().StopAudio(PlazmaGames.Audio.AudioType.Ambient);
        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(p.GetDeathAudiio(), PlazmaGames.Audio.AudioType.Ambient, false, false);
    }

    private void End()
    {
        FindObjectOfType<LightController>().SetMainLightLevel(0);
        _endingMap.SetActive(true);
    }

    private void EveryoneDied()
    {
        StopAllCoroutines();
        GameManager.GetMonoSystem<IAudioMonoSystem>().StopAudio(PlazmaGames.Audio.AudioType.Ambient);
        GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_everyoneDiedClip, PlazmaGames.Audio.AudioType.Ambient, false, false);
        End();
    }

    private void Start()
    {
        if (_pm == null) _pm = FindAnyObjectByType<PatientManager>();
        if (_as == null) _as = GetComponent<AudioSource>();

        _endingMap.SetActive(false);

        _pm.SubscribePatientDead(AnnouncementDying);
        _pm.SubscribePatientDeadForReal(AnnouncementDeath);
        _pm.SubscribePatientsAllDead(EveryoneDied);
    }

    private void Update()
    {
        if (!_pm.AnyPatientsFlatLine())
        { 
            _as.Stop();
        }
    }
}
