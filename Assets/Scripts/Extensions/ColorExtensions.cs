using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public static Color GetColor(this Colors color)
    {
        Color newColor = Color.clear;
        switch (color)
        {
            case Colors.Red:
                newColor = Color.red;
                break;
            case Colors.Blue:
                newColor = Color.blue;
                break;
            case Colors.Pink:
                newColor = Color.magenta;
                break;
            case Colors.Green:
                newColor = Color.green;
                break;
        }

        return newColor;
    } 
}
