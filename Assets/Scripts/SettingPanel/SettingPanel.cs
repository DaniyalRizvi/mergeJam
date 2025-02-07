using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("Add Button         0=Off 1=On State")]
    [SerializeField] private GameObject[] SoundsSFXButtons;
    [SerializeField] private bool isSFXOn;
    [SerializeField] private int CurrentSFXIndex;
    [Space(15)]
    [Header("Add Button         0=Off 1=On State")]
    [SerializeField] private GameObject[] MusicButtons;
    [SerializeField] private bool isMusicOn;
    [SerializeField] private int CurrentMusicIndex;

    [Space(10)]
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button BackButton;
    void Start()
    {
        // Load saved preferences
        //isMusicOn = PlayerPrefs.GetInt(Constants.MusicKey, 1) == 1; // Default: On
        //isSFXOn = PlayerPrefs.GetInt(Constants.SoundsSFXKey, 1) == 1;     // Default: O
        
        BackButton.onClick.AddListener(() => {
            //LoadData();
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
            SoundsSFXButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();

        for (int i = 0; i < MusicButtons.Length; i++)
            MusicButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();

    }

    private void InitilizedButtons()
    {

        for (int i = 0; i < SoundsSFXButtons.Length; i++)
        {
            SoundSfxButtonsCallback();
            SoundsSFXButtons[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                if (PlayerPrefs.GetInt(Constants.SoundsSFXKey) == 0)
                    PlayerPrefs.SetInt(Constants.SoundsSFXKey,1);
                    else
                    PlayerPrefs.SetInt(Constants.SoundsSFXKey,0);
                SoundSfxButtonsCallback();
            });
        }


        for (int i = 0; i < MusicButtons.Length; i++)
        {
            MusicButtonsCallback();
            MusicButtons[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                if (PlayerPrefs.GetInt(Constants.MusicKey) == 0)
                    PlayerPrefs.SetInt(Constants.MusicKey,1);
                else
                    PlayerPrefs.SetInt(Constants.MusicKey,0);
                MusicButtonsCallback();
            });
        }

        LoadData();
    }
    private void SoundSfxButtonsCallback()
    {
        SoundManager.Instance.SfxAudioSourceState(PlayerPrefs.GetInt(Constants.SoundsSFXKey)==1);
        for (int i = 0; i < SoundsSFXButtons.Length; i++)
        {
            SoundsSFXButtons[i].gameObject.SetActive(PlayerPrefs.GetInt(Constants.SoundsSFXKey)==i);
        }
        
    }

    private void MusicButtonsCallback()
    {
        SoundManager.Instance.MusicAudioSourceState(PlayerPrefs.GetInt(Constants.MusicKey)==1);
        for (int i = 0; i < MusicButtons.Length; i++)
        {
                MusicButtons[i].gameObject.SetActive(PlayerPrefs.GetInt(Constants.MusicKey)==i);
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
        //SoundSfxButtonsCallback(PlayerPrefs.GetInt(Constants.SoundsSFXKey, 1));
        //MusicButtonsCallback(PlayerPrefs.GetInt(Constants.MusicKey, 1));

    }
}
