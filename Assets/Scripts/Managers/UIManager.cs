using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singelton<UIManager>
{
    private GameObject _levelCompleteUI;  
    private GameObject _levelFailedUI;  

    void Start()
    {
        Init();
        _levelCompleteUI.SetActive(false);
        _levelFailedUI.SetActive(false);
    }

    private void Init()
    {
        _levelCompleteUI = GameObject.Find("LevelCompleted");
        _levelFailedUI = GameObject.Find("LevelFailed");
    }
    
    public void ShowLevelCompleteUI()
    {
        _levelCompleteUI.SetActive(true);  
    }
    
    public void ShowLevelFailedUI()
    {
        _levelFailedUI.SetActive(true);  
    }
    
    public void ResetUI()
    {
        _levelCompleteUI.SetActive(false);
        _levelFailedUI.SetActive(false);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.OnLevelRestart?.Invoke();
    }
}