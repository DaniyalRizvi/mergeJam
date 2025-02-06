using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : Singelton<LevelManager>
{
    public List<Level> _levels;
    private int _levelNumber;
    public Action OnLevelComplete;
    public Action OnLevelRestart;
    public Action<float,bool> OnTimeBaseLevel;
    public Transform passengerRef;
    void Start()
    {
        if (PlayerPrefs.GetInt("LevelTutorialCompleted") == 1)
        {
            if (PlayerPrefs.HasKey("CurrentLevel"))
            {
                _levelNumber = PlayerPrefs.GetInt("CurrentLevel");
            }
        }

        _levels = FindObjectsOfType<Level>().ToList();
        _levels.SortByName();
        
        //if(_levels[_levelNumber].GetComponentInChildren<passengerRef>())
        //passengerRef = _levels[_levelNumber].GetComponentInChildren<passengerRef>().transform;

        Application.targetFrameRate = 1000;
        OnLevelComplete += OnLevelCompleted;
        OnLevelRestart += RestartLevel;
        Debug.LogError($"LevelNumber is: {_levelNumber}"); 
        //PlayerPrefs.SetInt("TrashTutorial", 0);
        //PlayerPrefs.SetInt("TrashTutorialPlayed", 0);
        if (_levelNumber == 1 && PlayerPrefs.GetInt("TrashTutorial") == 0)
        {
            PlayerPrefs.SetInt("TrashTutorial", 1);
           // SceneManager.LoadScene("MergeJamTutorial");
        }

        else if (_levelNumber == 2 && PlayerPrefs.GetInt("RocketTutorial") == 0)
        {
            PlayRocketTutorial();
            PlayerPrefs.SetInt("RocketTutorial", 1);

        }
        else if (_levelNumber == 3 && PlayerPrefs.GetInt("FanTutorial") == 0)
        {
            PlayFanTutorial();
            PlayerPrefs.SetInt("FanTutorial", 1);

        }

        else if (_levelNumber == 4 && PlayerPrefs.GetInt("JumpTutorial") == 0)
        {
            PlayJumpTutorial();
            PlayerPrefs.SetInt("JumpTutorial", 1);

        }

        else if (_levelNumber == 1 && PlayerPrefs.GetInt("TrashTutorial") == 1)
        {
            PlayTrashTutorial();
        }

        else
        {
            LoadLevel(_levelNumber);
        }
    }

    void PlayTrashTutorial()
    {
        LoadLevel(0);
    }
    
    void PlayRocketTutorial()
    {

    }
    
    void PlayFanTutorial()
    {

    }
    
    void PlayJumpTutorial()
    {

    }

    private void LoadLevel(int levelNumber)
    {
        foreach (var level in _levels)
        {
            level.SetActiveState(false);
        }
        _levels[levelNumber].SetActiveState(true);
        _levels[levelNumber].Init();

        if(_levels[levelNumber].GetComponent<LevelTimeComponent>())
        {
            OnTimeBaseLevel?.Invoke(_levels[levelNumber].GetComponent<LevelTimeComponent>().GetLevelTime(), true);
        }
        else
        {
            OnTimeBaseLevel?.Invoke(0,false);
        }
         

        if (GameObject.Find("LevelText"))
            GameObject.Find("LevelText").GetComponent<TMP_Text>().SetText($"Level No: {_levelNumber + 1}"); 
        UIManager.Instance.ResetUI();

        if (_levels[levelNumber].isHard)
        {
            UIManager.Instance.hardLevelUI.SetActive(true);
            Invoke("DisableHardLevelUI",1f);
        }
    }

    private void DisableHardLevelUI()
    {
        UIManager.Instance.hardLevelUI.SetActive(false);
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
            
            PlayerPrefs.SetInt("CurrentLevel", _levelNumber);
            LoadLevel(_levelNumber);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    private void RestartLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel", _levelNumber);
        GameManager.Instance.ClearSavedGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public Level GetCurrentLevel() => _levels[_levelNumber];

    public List<Colors> GetCurrentLevelColors()
    {
        var allColors = Enum.GetValues(typeof(Colors)).Cast<Colors>();
        var levelColors = _levels[_levelNumber].colors.Select(colorCount => colorCount.color);
        return allColors.Except(levelColors).ToList();
    }

    public void ApplyJump(Level level, Bus bus, Transform referencePoint)
    {
        StartCoroutine(GameManager.Instance.ApplySpringVFX(referencePoint));
        bus.GetComponent<SquashAndStretch>().enabled = true;
        var spawnPoint = level.gameObject.GetComponentInChildren<SpawnPoint>().transform;
        Vector3 tempPos = level.GetRandomSpawnPoint(spawnPoint);
        tempPos.y = 2.5f;
        StartCoroutine(MoveFromSlot(bus, tempPos, spawnPoint));
    }

    public IEnumerator MoveFromSlot(Bus bus, Vector3 targetPosition, Transform spawnPoint)
    {
        Vector3 initialPosition = bus.transform.position;
        Quaternion initialRotation = bus.transform.rotation;
        float moveDuration = 1f;
        float elapsedTime = 0f;
        float jumpHeight = 2f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            Vector3 position = Vector3.Lerp(initialPosition, targetPosition, t);
            position.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;
            bus.transform.position = position;

            yield return null;
        }

        bus.transform.position = targetPosition;
        bus.transform.SetParent(spawnPoint, true);
        bus.GetComponent<Rigidbody>().isKinematic = false;
        bus.GetComponent<SquashAndStretch>().enabled = false;
        GameManager.Instance.movingBack = false;
    }
}