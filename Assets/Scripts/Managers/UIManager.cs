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
        GameManager.Instance.StartBoarding();  
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