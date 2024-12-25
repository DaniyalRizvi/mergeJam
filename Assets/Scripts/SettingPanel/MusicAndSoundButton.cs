using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicAndSoundButton : MonoBehaviour
{
    public Button Button;
    [SerializeField] private Image SelectedImage;

    public void SelectedImageState(bool State)
    {
        SelectedImage.gameObject.SetActive(State);
    }
}
