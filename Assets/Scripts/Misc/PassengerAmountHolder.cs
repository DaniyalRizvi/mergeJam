using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PassengerAmountHolder : MonoBehaviour
{
    public Image image;
    public TMP_Text amountText;
    internal Colors Color;
    private int _amount;

    public void Init(Colors color, int amount)
    {
        Color = color;
        _amount = amount;
        image.color = color.GetColor();
        amountText.text = amount.ToString();
    }
   bool IsInAnimaiton;
    public void UpdateAmount()
    {

        _amount--;
        _amount = Mathf.Clamp(_amount, 0, int.MaxValue);
        amountText.text = _amount.ToString();

        if (!IsInAnimaiton)
        {
            IsInAnimaiton = true;
            amountText.transform.DOScale(Vector3.one * 1.2f, .8f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                amountText.transform.localScale = Vector3.one;
                IsInAnimaiton = false;
            });
        }

    }
}
