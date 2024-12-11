using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnimator : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    public  void IsWalking(bool State)
    {
        Animator.SetBool("IsWalking", State);
    }
 
}
