using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameobjectWithTime : MonoBehaviour
{
    [SerializeField] private float DisableTime;
    private void Start()
    {

        Invoke(nameof(DisableObject), DisableTime);
    }
    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
