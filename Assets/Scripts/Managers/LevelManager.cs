using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singelton<LevelManager>
{
    private List<Level> _levels;
    private int _levelNumber;
    public Action OnLevelComplete;
    public Action OnLevelRestart;
    public Action<float,bool> OnTimeBaseLevel;
    void Start()
    {
        if (PlayerPrefs.HasKey("RestartLevel"))
        {
            _levelNumber = PlayerPrefs.GetInt("RestartLevel");
            PlayerPrefs.DeleteKey("RestartLevel");
        }

        _levels = FindObjectsOfType<Level>().ToList();
        _levels.SortByName();

        Application.targetFrameRate = 1000;
        OnLevelComplete += OnLevelCompleted;
        OnLevelRestart += RestartLevel;
        LoadLevel(_levelNumber);
        
    }

    private void LoadLevel(int levelNumber)
    {
        foreach (var level in _levels)
        {
            level.SetActiveState(false);
        }
        _levels[levelNumber].SetActiveState(true);
        _levels[levelNumber].Init();

        if(_levels[levelNumber].TryGetComponent<LevelTimeComponent>(out LevelTimeComponent Z))
        {
            OnTimeBaseLevel?.Invoke(Z.GetLevelTime(), true);
        }
        else
        {
            OnTimeBaseLevel?.Invoke(0,false);
        }
         

        if (GameObject.Find("LevelText"))
            GameObject.Find("LevelText").GetComponent<TMP_Text>().SetText($"Level No: {_levelNumber + 1}"); 
        UIManager.Instance.ResetUI();
    }


    private void OnLevelCompleted()
    {
        _levelNumber++;
        if (_levelNumber % 3 == 0)
        {
            DTAdsManager.Instance.ShowAd(Constants.InterstitialId);
        }
        if (_levelNumber < _levels.Count)
        {
            LoadLevel(_levelNumber);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    private void RestartLevel()
    {
        PlayerPrefs.SetInt("RestartLevel", _levelNumber);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public Level GetCurrentLevel() => _levels[_levelNumber];

    public List<Colors> GetCurrentLevelColors()
    {
        var allColors = Enum.GetValues(typeof(Colors)).Cast<Colors>();
        var levelColors = _levels[_levelNumber].colors.Select(colorCount => colorCount.color);
        return allColors.Except(levelColors).ToList();
    }
}