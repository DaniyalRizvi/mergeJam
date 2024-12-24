using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameLoadingController : MonoBehaviour
{ 
    [SerializeField] private float LoadingTime;
    [SerializeField] private Image LoadingImage;
    public static bool AttPermission;
    private IEnumerator Start()
    {
#if UNITY_ANDROID
        AttPermission = true;
#endif
        LoadingImage.fillAmount = 0f;
        yield return new WaitUntil(() => AttPermission);

        LoadingImage.DOFillAmount(1, LoadingTime).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });

    }

}
