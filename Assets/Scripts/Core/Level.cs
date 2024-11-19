using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<BusType> busTypes;
    private List<Slot> _slots;
    private List<Passenger> _passengers;
    
    public void Init()
    {
        InitLevel();
        GameManager.Instance.Init(_slots, _passengers);
    }

    private void InitLevel()
    {
        _slots = gameObject.GetComponentsInChildren<Slot>().ToList();
        _passengers = gameObject.GetComponentsInChildren<Passenger>().ToList();
    }

    public void SetActiveState(bool state)
    {
        gameObject.SetActive(state);
    }
}


[Serializable]
public class BusType
{
    public int capacity;
    public Colors colors;
}