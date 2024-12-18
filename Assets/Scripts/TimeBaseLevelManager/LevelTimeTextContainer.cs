using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimeTextContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI LevelTimeText;
    public void GameobjectStatus(bool Status)
    {
        gameObject.SetActive(Status);
    }
    public void SetText(float Min,float Sec)
    {
        //LevelTimeText.text = string.Format("00:00", $"{Min} : {Sec} ");
        LevelTimeText.text = string.Format("{0:00}:{1:00}", Min, Sec);
    }
}
