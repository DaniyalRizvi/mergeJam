using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singelton<UIManager>
{
    private GameObject _levelCompleteUI;  
    private GameObject _levelFailedUI;  
    private Button _goButton;  

    void Start()
    {
        Init();
        _levelCompleteUI.SetActive(false);
        _levelFailedUI.SetActive(false);
        _goButton.onClick.AddListener(OnGoButtonPressed);
    }

    private void Init()
    {
        _levelCompleteUI = GameObject.Find("LevelCompleted");
        _levelFailedUI = GameObject.Find("LevelFailed");
        _goButton = GameObject.Find("StartBoardingButton").GetComponent<Button>();
    }


    private void OnGoButtonPressed()
    {
        Debug.Log("Go button pressed, starting boarding.");
        GameManager.Instance.StartBoarding();  
    }

    
    public void ShowLevelCompleteUI()
    {
        _levelCompleteUI.SetActive(true);  
        Debug.Log("Level Complete!");
    }

    
    public void ShowLevelFailedUI()
    {
        _levelFailedUI.SetActive(true);  
        Debug.Log("Level Failed!");
    }

    
    public void ResetUI()
    {
        _levelCompleteUI.SetActive(false);
        _levelFailedUI.SetActive(false);
        Debug.Log("UI reset.");
    }

    public void RestartLevel()
    {
        LevelManager.Instance.OnLevelRestart?.Invoke();
    }
}