using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singelton<GameManager>
{
    private List<Slot> _slots = new();
    private List<Passenger> _passengers = new();

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
            CheckForMerging(clickedSlot, out Bus finalBus);
            BoardPassengersToBus(finalBus);
            return true;
        }

        return false;
    }

    private void CheckForMerging(Slot clickedSlot, out Bus remainingBus)
    {
        var (leftSlot, rightSlot) = GetAdjacentSlots(clickedSlot);
        if (leftSlot?.CurrentBus != null && clickedSlot.CurrentBus != null &&
            leftSlot.CurrentBus.busColor == clickedSlot.CurrentBus.busColor &&
            leftSlot.CurrentBus.capacity == clickedSlot.CurrentBus.capacity)
        {
            remainingBus = TryMergeBuses(leftSlot.CurrentBus, clickedSlot.CurrentBus, leftSlot, clickedSlot);
        }
        else if (rightSlot?.CurrentBus != null && clickedSlot.CurrentBus != null &&
                 rightSlot.CurrentBus.busColor == clickedSlot.CurrentBus.busColor &&
                 rightSlot.CurrentBus.capacity == clickedSlot.CurrentBus.capacity)
        {
            remainingBus = TryMergeBuses(clickedSlot.CurrentBus, rightSlot.CurrentBus, clickedSlot, rightSlot);
        }
        else
        {
            remainingBus = clickedSlot.CurrentBus;
        }
    }

    private (Slot leftSlot, Slot rightSlot) GetAdjacentSlots(Slot currentSlot)
    {
        int currentIndex = _slots.IndexOf(currentSlot);
        if (currentIndex == -1)
        {
            return (null, null);
        }

        Slot leftSlot = currentIndex > 0 ? _slots[currentIndex - 1] : null;
        Slot rightSlot = currentIndex < _slots.Count - 1 ? _slots[currentIndex + 1] : null;
        return (leftSlot, rightSlot);
    }


    private Bus TryMergeBuses(Bus leftBus, Bus rightBus, Slot leftSlot, Slot rightSlot)
    {
        if (leftBus.busColor == rightBus.busColor && leftBus.capacity == rightBus.capacity)
        {
            leftBus.CurrentSize += rightBus.CurrentSize;
            leftBus.capacity += rightBus.capacity;
            NotifyPassengersOfNewBus(leftSlot.CurrentBus);
            Destroy(rightBus.gameObject);
            rightSlot.ClearSlot();
            leftSlot.AssignBus(leftBus);
            return leftBus;
        }

        return leftBus;
    }

    private void NotifyPassengersOfNewBus(Bus newBus)
    {
        var passengersToRedirect = _passengers.Where(p => p.IsBoarding && p.passengerColor == newBus.busColor).ToList();
        Debug.Log(passengersToRedirect.Count);
        foreach (var passenger in passengersToRedirect)
        {
            passenger.UpdateBusAfterMerge(newBus);
        }
    }

    private void BoardPassengersToBus(Bus bus)
    {
        var matchingPassengers = _passengers
            .Where(p => p.passengerColor == bus.busColor)
            .ToList();


        foreach (var passenger in matchingPassengers)
        {
            if (bus.CurrentSize > 0)
            {
                passenger.TryBoardBus(bus, hasBoarded =>
                {
                    if (hasBoarded)
                    {
                        _passengers.Remove(passenger);
                        Destroy(passenger.gameObject);
                        CheckLevelCompletion();
                    }
                });
                if (bus.CurrentSize <= 0)
                {
                    break;
                }
            }
        }
    }

    private void CheckLevelCompletion()
    {
        if (_passengers.Count == 0)
        {
            UIManager.Instance.ShowLevelCompleteUI();
            Invoke(nameof(LevelComplete), 2f);
        }
    }

    private void LevelComplete()
    {
        LevelManager.Instance.OnLevelComplete?.Invoke();
    }
}
