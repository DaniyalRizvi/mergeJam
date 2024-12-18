using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimeComponent : MonoBehaviour
{
    [SerializeField] private float Time;
    public float GetLevelTime() => this.Time;

}
