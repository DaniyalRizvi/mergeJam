using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singelton<SoundManager>
{
    [SerializeField]
    private SoundsClipsCollectionSO SoundsClipsCollectionSO;

    [SerializeField]
    private AudioSource effectsAudioSource;

    [SerializeField]
    private AudioSource musicAudioSource;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
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
    public void PlayRocketPowerUpSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.RocketPowerUp);
    public void PlayFanPowerUpSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.FanPowerUp);
    public void TrashItemDeletionSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.TrashItemDeletion);
    public void ItemMergeSoundSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.ItemMergeSound);
    public void AddingVehiclesToSlotsSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.AddingVehiclesToSlots);
    public void LevelCompleteSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.LevelComplete);
    public void LevelFailSFX()=>effectsAudioSource.PlayOneShot(SoundsClipsCollectionSO.LevelFail);

    private void PlaySFXClip(AudioClip audio)
    {
        if (audio == null) return;
        effectsAudioSource.PlayOneShot(audio);

    }
}
