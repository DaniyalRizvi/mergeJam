using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class GameManager : Singelton<GameManager>
{
    private List<Slot> _slots = new();
    private List<Passenger> _passengers = new();
    private Level _level;

    public void Init(List<Slot> slots, List<Passenger> passengers, Level level)
    {
        _slots = slots;
        _passengers = passengers;
        _level = level;
    }


    public void CheckForMerging(Slot clickedSlot, out Bus remainingBus)
    {
        var (leftSlot, rightSlot) = GetAdjacentSlots(clickedSlot);
        if (leftSlot?.CurrentBus != null && clickedSlot.CurrentBus != null &&
            leftSlot.CurrentBus.busColor == clickedSlot.CurrentBus.busColor &&
            leftSlot.CurrentBus.capacity == clickedSlot.CurrentBus.capacity)
        {
            remainingBus = TryMergeBuses(leftSlot.CurrentBus, clickedSlot.CurrentBus, leftSlot, clickedSlot);
            clickedSlot.CurrentBus = null;
        }
        else if (rightSlot?.CurrentBus != null && clickedSlot.CurrentBus != null &&
                 rightSlot.CurrentBus.busColor == clickedSlot.CurrentBus.busColor &&
                 rightSlot.CurrentBus.capacity == clickedSlot.CurrentBus.capacity)
        {
            remainingBus = TryMergeBuses(clickedSlot.CurrentBus, rightSlot.CurrentBus, clickedSlot, rightSlot);
            clickedSlot.CurrentBus = null;
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
            leftBus.currentSize += rightBus.currentSize;
            leftBus.capacity += rightBus.capacity;
            NotifyPassengersOfNewBus(leftSlot.CurrentBus);
            Destroy(rightBus.gameObject);
            rightSlot.ClearSlot();
            leftSlot.AssignBus(leftBus);
            if (!_level.colors.Contains(leftSlot.CurrentBus.busColor))
            {
                // TODO : Implement Animation For Bus Leaving
                Destroy(leftSlot.CurrentBus.gameObject);
                leftSlot.ClearSlot();
            }
            return leftBus;
        }

        return leftBus;
    }

    private void NotifyPassengersOfNewBus(Bus newBus)
    {
        var passengersToRedirect = _passengers.Where(p => p.IsBoarding && p.passengerColor == newBus.busColor).ToList();
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
            if (bus.currentSize > 0)
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
                if (bus.currentSize <= 0)
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

    public void PlaceBusInSlot(Bus selectedBus)
    {
        var clickedSlot = _slots.FirstOrDefault(slot => slot.isEmpty && !slot.isLocked);
        if (clickedSlot != null)
        {
            selectedBus.transform.localScale = new Vector3(10, 3, 4);
            selectedBus.AssignSlot(clickedSlot);
            clickedSlot.AssignBus(selectedBus);
            CheckForMerging(clickedSlot, out Bus finalBus);
            BoardPassengersToBus(finalBus);
            InputManager.Instance.DeselectBus();
        }

        StartCoroutine(nameof(CheckLooseCondition));
    }

    public void PlaceBusInSlot(Bus selectedBus, Slot clickedSlot)
    {
        if (clickedSlot.isEmpty && !clickedSlot.isLocked)
        {
            selectedBus.transform.localScale = new Vector3(10, 3, 4);
            selectedBus.AssignSlot(clickedSlot);
            clickedSlot.AssignBus(selectedBus);
            CheckForMerging(clickedSlot, out Bus finalBus);
            BoardPassengersToBus(finalBus);
            InputManager.Instance.DeselectBus();
        }

        StartCoroutine(nameof(CheckLooseCondition));
    }

    private IEnumerator CheckLooseCondition()
    {
        var isSlotEmpty = _slots.Any(slot => slot.isEmpty);
        if (!isSlotEmpty)
        {
            yield return new WaitUntil(() => _passengers.Where(p => p.IsBoarding).All(p => p.hasBoarded));
            if(_passengers.Count>0)
                UIManager.Instance.ShowLevelFailedUI();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
            DTAdsManager.Instance.ShowAd(Constants.InterstitialId);
    }
}
