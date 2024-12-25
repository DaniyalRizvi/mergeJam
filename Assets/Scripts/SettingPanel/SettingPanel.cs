using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("Add Button         0=Off 1=On State")]
    [SerializeField] private MusicAndSoundButton[] SoundsSFXButtons;
    [SerializeField] private bool isSFXOn;
    [SerializeField] private int CurrentSFXIndex;
    [Space(15)]
    [Header("Add Button         0=Off 1=On State")]
    [SerializeField] private MusicAndSoundButton[] MusicButtons;
    [SerializeField] private bool isMusicOn;
    [SerializeField] private int CurrentMusicIndex;

    [Space(10)]
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button BackButton;
    void Start()
    {
        // Load saved preferences
        //isMusicOn = PlayerPrefs.GetInt(Constants.MusicKey, 1) == 1; // Default: On
        //isSFXOn = PlayerPrefs.GetInt(Constants.SoundsSFXKey, 1) == 1;     // Default: On
        SaveButton.onClick.AddListener(() => {
            SaveData();
        });
        BackButton.onClick.AddListener(() => {
            LoadData();
            SettingPanelState(false);
        });

        InitilizedButtons();
    }
    public void SettingPanelState(bool State)
    {
        gameObject.SetActive(State);
    }
    private void OnDestroy()
    {
        for (int i = 0; i < SoundsSFXButtons.Length; i++)
            SoundsSFXButtons[i].Button.onClick.RemoveAllListeners();

        for (int i = 0; i < MusicButtons.Length; i++)
            MusicButtons[i].Button.onClick.RemoveAllListeners();

    }

    private void InitilizedButtons()
    {

        for (int i = 0; i < SoundsSFXButtons.Length; i++)
        {
            int Count = i;
            SoundsSFXButtons[i].Button.onClick.AddListener(() =>
            {
                SoundSfxButtonsCallback(Count);
            });
        }


        for (int i = 0; i < MusicButtons.Length; i++)
        {
            int Count = i;
            MusicButtons[i].Button.onClick.AddListener(() =>
            {
                MusicButtonsCallback(Count);
            });
        }

        LoadData();
    }
    private void SoundSfxButtonsCallback(int Index)
    {
        isSFXOn = Index != 0;
        CurrentSFXIndex = Index;
        SoundManager.Instance.SfxAudioSourceState(isSFXOn);
        for (int i = 0; i < SoundsSFXButtons.Length; i++)
        {
            if (Index == i)
                SoundsSFXButtons[i].SelectedImageState(true);
            else
                SoundsSFXButtons[i].SelectedImageState(false);
        }
    }

    private void MusicButtonsCallback(int Index)
    {
        isMusicOn = Index != 0;
        CurrentMusicIndex = Index;
        SoundManager.Instance.MusicAudioSourceState(isMusicOn);
        for (int i = 0; i < MusicButtons.Length; i++)
        {
            if (Index == i)
                MusicButtons[i].SelectedImageState(true);
            else
                MusicButtons[i].SelectedImageState(false);
        }
    }
    private void SaveData()
    {
        PlayerPrefs.SetInt(Constants.SoundsSFXKey, CurrentSFXIndex);
        PlayerPrefs.SetInt(Constants.MusicKey, CurrentMusicIndex);
        LoadData();
    }
    private void LoadData()
    {
        SoundSfxButtonsCallback(PlayerPrefs.GetInt(Constants.SoundsSFXKey, 1));
        MusicButtonsCallback(PlayerPrefs.GetInt(Constants.MusicKey, 1));

    }
}
