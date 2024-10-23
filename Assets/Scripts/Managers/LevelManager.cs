using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singelton<LevelManager>
{
    private List<Level> _levels;
    
    
    private int _levelNumber;
    public Action OnLevelComplete;
    public Action OnLevelRestart;
    
    void Start()
    {
        if (PlayerPrefs.HasKey("RestartLevel"))
        {
            _levelNumber = PlayerPrefs.GetInt("RestartLevel");
            PlayerPrefs.DeleteKey("RestartLevel");
        }

        _levels = FindObjectsOfType<Level>().ToList();
        _levels.Reverse();

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
        UIManager.Instance.ResetUI();
    }


    private void OnLevelCompleted()
    {
        _levelNumber++;
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
}

//
// internal class LevelComparer : IComparer<Level>
// {
//     public int Compare(Level x, Level y)
//     {
//     }
// }
