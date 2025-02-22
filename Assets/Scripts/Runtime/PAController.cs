using CodeBlack;
using CodeBlack.ECG;
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
    [SerializeField] private DoubleDoor _door;

    private bool _isPlaying;

    [SerializeField] private GameObject _endingMap;


    private IEnumerator RepeatAnnouncement(Patient p)
    {
        while (GameManager.GetMonoSystem<IAudioMonoSystem>().IsPlaying(PlazmaGames.Audio.AudioType.Ambient))
        {
            yield return null;
        }

        if (!_pm.AllPatientsDeadForReal() && !p.IsDeadForReal()) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(p.GetDyingAudiio(), PlazmaGames.Audio.AudioType.Ambient, false, false);
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
        _door.SetLockedState(false);
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

        _door.SetLockedState(true);

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
