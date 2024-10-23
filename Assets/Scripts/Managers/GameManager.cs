using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singelton<GameManager>
{
    private List<Slot> _slots = new(); 
    private List<Passenger> _passengers = new(); 
    private Transform _spawnPointBus;
    private Transform _spawnPointPassenger;
    
    public void Init(List<Slot> slots, List<Passenger> passengers)
    {
        _slots = slots;
        _passengers = passengers;
    }
    
    public bool PlaceBusInSlot(Bus selectedBus, Slot clickedSlot)
    {
        if (clickedSlot.IsEmpty && !clickedSlot.isLocked)
        {
            selectedBus.AssignSlot(clickedSlot);
            clickedSlot.AssignBus(selectedBus);
            return true;
        }

        if (clickedSlot.isLocked)
        {
            Debug.Log("Cannot place bus. Slot is locked.");
            return false;
        }

        Debug.Log("Cannot place bus. Slot is already filled.");
        return false;
    }
    
    public void UnlockSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _slots.Count)
        {
            _slots[slotIndex].UnlockSlot();
            Debug.Log($"Slot {slotIndex} is now unlocked.");
        }
        else
        {
            Debug.LogError("Invalid slot index.");
        }
    }
    
    public void StartBoarding()
    {
        foreach (Passenger passenger in _passengers)
        {
            bool boarded = false;
            foreach (Slot slot in _slots)
            {
                if (slot != null && passenger.TryBoardBus(slot)) 
                {
                    boarded = true;
                    Destroy(passenger.gameObject);
                    break;
                }
            }

            if (!boarded)
            {
                Debug.Log($"Passenger \"{passenger.name} {passenger.passengerColor}\" couldn't board!");
            }
        }

        CheckLevelCompletion(); 
    }
    
    private void CheckLevelCompletion()
    {
        bool allBoarded = _passengers.TrueForAll(p => p.hasBoarded); 
        if (allBoarded)
        {
            Debug.Log("Level Complete!");
            UIManager.Instance.ShowLevelCompleteUI();
            Invoke(nameof(LevelComplete), 2f);
        }
        else
        {
            Debug.Log("Level Failed!");
            UIManager.Instance.ShowLevelFailedUI(); 
        }
    }

    private void LevelComplete()
    {
        LevelManager.Instance.OnLevelComplete?.Invoke();
    }

    
    // private Vector3 GetRandomPositionBus()
    // {
    //     return _spawnPointBus.position + new Vector3(Random.Range(-10f, 10f), 10, 0); 
    // }
    //
    // private Vector3 GetRandomPositionPassenger()
    // {
    //     return _spawnPointPassenger.position + new Vector3(9, 0, Random.Range(-10f, 10f)); 
    // }
    //
    // private Colors GetRandomColor()
    // {
    //     return (Colors)Random.Range(0, Enum.GetValues(typeof(Colors)).Length);
    // }
    //
    //
    // private Vector3 GetSlotPosition(int index)
    // {
    //     float xOffset = 2.0f; 
    //     return new Vector3(index * xOffset, 0, 0); 
    // }
    //
    //
    //
    // void InitializeSlots()
    // {
    //     foreach (Slot slot in slots)
    //     {
    //         slot.ClearSlot();
    //     }
    // }
    //
    // void InitializePile()
    // {
    //     for (int i = 0; i < 10; i++)
    //     {
    //         GameObject busGO = Instantiate(busPrefab, GetRandomPositionBus(), Quaternion.identity);
    //         var bus = busGO.GetComponent<Bus>();
    //         bus.SetBus(2, GetRandomColor());
    //         pileBuses.Add(bus);
    //     }
    // }
    //
    //
    // void SpawnPassengers()
    // {
    //     for (int i = 0; i < _totalPassengers; i++)
    //     {
    //         GameObject newPassenger = Instantiate(passengerPrefab, GetRandomPositionPassenger(), Quaternion.identity);
    //         var passenger = newPassenger.GetComponent<Passenger>();
    //         passenger.SetPassenger(GetRandomColor());
    //         _passengers.Add(passenger);
    //     }
    // }


}
