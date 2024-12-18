using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimeManager : MonoBehaviour
{
    [SerializeField] private LevelTimeTextContainer LevelTimeTextContainer;
    [Space(10)]
    [SerializeField] private float MaxTime;
    [SerializeField] private bool  CanStartTimer;
    [SerializeField] private float Min;
    [SerializeField] private float Sec;
    private void Start()
    {
        CanStartTimer = false;
        GameManager.Instance.OnLevelComplete += OnLevelCompleteCallback;
        LevelManager.Instance.OnTimeBaseLevel += OnTimeBaseLevelCallback;
    }

    private void OnLevelCompleteCallback()
    {
        CanStartTimer = false;
        LevelTimeTextContainer.GameobjectStatus(false);
    }

    private void OnTimeBaseLevelCallback(float time,bool Status)
    {
      
        SetupLevelTime(time);
        CanStartTimer = Status;
        LevelTimeTextContainer.GameobjectStatus(Status);
        LevelTimeTextContainer.SetText(Min, Sec);

    }
    private void OnDisable()
    {

        if(GameManager.Instance)
        GameManager.Instance.OnLevelComplete -= OnLevelCompleteCallback;
        if(LevelManager.Instance)
        LevelManager.Instance.OnTimeBaseLevel -= OnTimeBaseLevelCallback;
    }
 

    public void SetupLevelTime(float LevelTime)
    {
        MaxTime = LevelTime;
        Min = MaxTime / 60;
        Sec = MaxTime % 60;
    }
    private void Update()
    {
        if (!CanStartTimer) return;
       
            MaxTime -= Time.deltaTime;
            Min = Mathf.FloorToInt(MaxTime / 60f);
            Sec = Mathf.FloorToInt(MaxTime % 60f);
            LevelTimeTextContainer.SetText(Min, Sec);

        if (MaxTime <= 0)
        {
            CanStartTimer = false;
            Debug.LogError("Level Failed");
            UIManager.Instance.ShowLevelFailedUI();
            LevelTimeTextContainer.SetText(0, 0);
        }


    }
}
