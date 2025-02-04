using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singelton<SoundManager>
{
    [SerializeField]
    private SoundsClipsCollectionSO SoundsClipsCollectionSO;

    [SerializeField]
    private AudioSource effectsAudioSource;

    [SerializeField]
    private AudioSource musicAudioSource;

    [SerializeField]
    private List<AudioSource> _audioSources;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        _audioSources = null;
    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey(Constants.SoundsSFXKey))
            PlayerPrefs.SetInt(Constants.SoundsSFXKey, 1);
        if (!PlayerPrefs.HasKey(Constants.MusicKey))
            PlayerPrefs.SetInt(Constants.MusicKey, 1);

        SfxAudioSourceState(PlayerPrefs.GetInt(Constants.SoundsSFXKey)==1);
        MusicAudioSourceState(PlayerPrefs.GetInt(Constants.MusicKey) == 1);
        PlayBackgroundMusic();
    }
    public void PlayBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
            return;

        musicAudioSource.clip = SoundsClipsCollectionSO.backgroundMusic;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicAudioSource.Stop();
    }

    public void PlayRocketPowerUpSFX()
    {
        if (!effectsAudioSource.mute)
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.RocketPowerUp);
    }

    public void PlayFanPowerUpSFX()
    {
        if (!effectsAudioSource.mute)
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.FanPowerUp);
    }

    public void TrashItemDeletionSFX()
    {
        if (!effectsAudioSource.mute)
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.TrashItemDeletion);
    }

    public void ItemMergeSoundSFX()
    {
        if (!effectsAudioSource.mute)
        {
            Debug.Log("Played");
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.ItemMergeSound);
        }
    }
    
    public void ConfettiSoundSFX()
    {
        if (!effectsAudioSource.mute)
        {
            Debug.Log("Played");
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.Confetti);
        }
    }

    public void AddingVehiclesToSlotsSFX()
    {
        Debug.Log("Effective Audio Source: "+effectsAudioSource.mute);
        if (!effectsAudioSource.mute)
        {
            Debug.Log("Played");
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.AddingVehiclesToSlots);
        }
    }

    public void LevelCompleteSFX()
    {
        if(!effectsAudioSource.mute)
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.LevelComplete);
    }

    public void LevelFailSFX()
    {
        if(!effectsAudioSource.mute)
            effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.LevelFail);
    }

    private void PlaySFXClip(AudioClip audio)
    {
        if (audio == null) return;
        effectsAudioSource.PlayOneShot(audio);

    }

    public void SetSlotVFX()
    {
        GameManager.Instance.SlotVFX.GetComponent<AudioSource>().mute = effectsAudioSource.mute;
    }


    public  void SfxAudioSourceState(bool isSFXOn)
    {
        effectsAudioSource.mute = !isSFXOn;
        if(GameManager.Instance!=null)
            GameManager.Instance.SlotVFX.GetComponent<AudioSource>().mute = !isSFXOn;
        // if (_audioSources != null)
        // {
        //     foreach (var audio in _audioSources)
        //     {
        //         audio.mute = !isSFXOn;
        //     }
        // }
    }

    public void MusicAudioSourceState(bool isMusicOn)
    {
        musicAudioSource.mute = !isMusicOn;
        
    }
}
