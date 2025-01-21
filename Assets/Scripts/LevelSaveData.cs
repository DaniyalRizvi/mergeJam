using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BusSaveData
{
    public int slotIndex;
    public Colors busColor;
    public int capacity;
    public int currentSize;
    public Vector3 position;
}

[System.Serializable]
public class PassengerSaveData
{
    public int id;            // Unique ID for the passenger
    public Colors passengerColor;
    public bool hasBoarded;
}

[System.Serializable]
public class SlotSaveData
{
    public int slotIndex;
    public bool isLocked;
    public bool isEmpty;
}

[System.Serializable]
public class LevelSaveData
{
    public List<BusSaveData> buses = new List<BusSaveData>();
    public List<PassengerSaveData> passengers = new List<PassengerSaveData>();
    public List<SlotSaveData> slots = new List<SlotSaveData>();
    public List<ColorCount> colors = new List<ColorCount>();
}

