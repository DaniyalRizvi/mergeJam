using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameLoadingController : MonoBehaviour
{ 
    public GameObject privacyPanel;
    public Button TermsAndConditionsBttn;
    public Button AcceptPolicyBttn;
    private static bool PolicyAccepted;
    [SerializeField] private float LoadingTime;
    [SerializeField] private Image LoadingImage;
    [SerializeField] private TMP_Text percentageText;
    public static bool AttPermission;
    private IEnumerator Start()
    {
        // ByPassTutorials();
        if (PlayerPrefs.GetInt("firstTimeUser") == 0)
        {
            privacyPanel.SetActive(true);
            
            AcceptPolicyBttn.onClick.RemoveAllListeners();
            AcceptPolicyBttn.onClick.AddListener(()=>PlayerPrefs.SetInt("firstTimeUser",1));
            AcceptPolicyBttn.onClick.AddListener(()=>privacyPanel.SetActive(false));
            
            TermsAndConditionsBttn.onClick.RemoveAllListeners();
            TermsAndConditionsBttn.onClick.AddListener(()=>TermAndCondition());
        }
#if UNITY_ANDROID
        AttPermission = true;
#endif
        LoadingImage.fillAmount = 0f;
        yield return new WaitUntil(() => AttPermission);
        yield return new WaitUntil(() => PlayerPrefs.GetInt("firstTimeUser") == 1);
        float elapsedTime = 0f;
        while (elapsedTime < LoadingTime)
        {
            elapsedTime += Time.deltaTime;
            LoadingImage.fillAmount = elapsedTime / LoadingTime;

            // Update loading percentage
            int percentage = Mathf.Clamp((int)((elapsedTime / LoadingTime) * 100), 0, 100);
            percentageText.text = "Loading..."+percentage + "%"; // Assuming you have a Text or TMP_Text for percentage

            yield return null;
        }

        LoadingImage.DOFillAmount(1, LoadingTime).OnComplete(() =>
        {
            //ByPassTutorials();
            Debug.Log("LevelTutorialCompleted: "+PlayerPrefs.GetInt("LevelTutorialCompleted"));

            Debug.Log("Actual Current LEvel: "+PlayerPrefs.GetInt("ActualCurrentLevel"));
            if (PlayerPrefs.GetInt("ActualCurrentLevel") != 0)
                PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("ActualCurrentLevel"));
            
            
            if (PlayerPrefs.GetInt("LevelTutorialCompleted")==0 || 
                (PlayerPrefs.GetInt("CurrentLevel")==25 && PlayerPrefs.GetInt("TrashTutorial")==0)|| 
                (PlayerPrefs.GetInt("RocketTutorial")==0 && PlayerPrefs.GetInt("CurrentLevel")==27) ||
                (PlayerPrefs.GetInt("FanTutorial")==0 && PlayerPrefs.GetInt("CurrentLevel")==35) ||
                (PlayerPrefs.GetInt("JumpTutorial") == 0 &&  PlayerPrefs.GetInt("CurrentLevel") == 13))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                Debug.Log(SceneManager.GetActiveScene().buildIndex+2);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
            }
        });

    }
    
    public void ByPassTutorials()
    {
        PlayerPrefs.SetInt("ActualCurrentLevel",0);
        PlayerPrefs.SetInt("CurrentLevel", 39);
        PlayerPrefs.SetInt("Gems", 1000000);
        PlayerPrefs.SetInt("LevelTutorialCompleted", 1);
        PlayerPrefs.SetInt("TrashTutorial", 1);
        PlayerPrefs.SetInt("RocketTutorial", 1);
        PlayerPrefs.SetInt("FanTutorial", 1);
        PlayerPrefs.SetInt("JumpTutorial", 1);
    }

    public void TermAndCondition()
    {
        Application.OpenURL("http://cherrytop.games/privacy-policy.html");
    }

}
