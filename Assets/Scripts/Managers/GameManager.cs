using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Managers;
using UnityEngine;

public class GameManager : Singelton<GameManager>
{
    //public VehicleDataManager VehicleDataManager;
    [SerializeField] private GameObject RocketPowerupsVFX;
    [SerializeField] private GameObject MergeVFX;
    [SerializeField] private GameObject FanPowerUpVFX;
    private List<Slot> _slots = new();
    private List<Passenger> _passengers = new();
    public Level _level;
    public Action OnLevelComplete;
    public int maxCount;
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => DTAdsManager.Instance && DTAdsManager.Instance.isInitialised);
        Debug.Log("Ads Initialized: "+DTAdsManager.Instance.isInitialised);
        DTAdsManager.Instance.ShowAd(Constants.BannerID);
    }

    public bool CurrentBusExistInGame(Colors CurrentBusColor)
    {
        bool result = true;
        if (!_level.colors.Exists(i => i.color == CurrentBusColor))
        {
            result = false;
        }
        return result;
    }
    public void Init(List<Slot> slots, List<Passenger> passengers, Level level)
    {
        _slots = slots;
        _passengers = passengers;
        _level = level;
    }


    private void CheckForMerging(Slot clickedSlot, out Bus remainingBus)
    {
        remainingBus = clickedSlot.CurrentBus;
        bool mergeHappened;
        do
        {
            mergeHappened = false;

            for (int i = _slots.Count - 1; i > 0; i--)
            {
                var leftSlot = _slots[i - 1];
                var rightSlot = _slots[i];

                if (CanMerge(leftSlot, rightSlot))
                {
                    TryMergeBuses(leftSlot, rightSlot);
                    mergeHappened = true;
                    _level.SaveLevelStateToJson(); 
                }
            }

        } while (mergeHappened);
    }

    private bool CanMerge(Slot leftSlot, Slot rightSlot)
    {
        return leftSlot?.CurrentBus != null &&
               rightSlot?.CurrentBus != null &&
               leftSlot.CurrentBus.busColor == rightSlot.CurrentBus.busColor &&
               leftSlot.CurrentBus.capacity == rightSlot.CurrentBus.capacity && leftSlot.CurrentBus;
    }

    private void TryMergeBuses(Slot leftSlot, Slot rightSlot)
    {
        var leftBus = leftSlot.CurrentBus;
        var rightBus = rightSlot.CurrentBus;
        
        leftBus.capacity += rightBus.capacity;

        leftBus.VehicleRenderModels.ActiveVehicle(leftBus.capacity);
        leftBus.UpdateVisual();
        
        leftBus.currentSize += rightBus.currentSize;
        leftBus.AssignSlot(leftSlot);
        leftSlot.AssignBus(leftBus);
        rightSlot.ClearSlot();
        if (!_level.colors.Exists(i=>i.color == leftSlot.CurrentBus.busColor))
        {
            if (TutorialManager.Instance)
            {
                TutorialManager.Instance.tutorialCase++;
                //TutorialManager.Instance.InitFan();
                TutorialManager.Instance.InitFanPanel();
                Debug.LogError("InitFan");
            }
            SoundManager.Instance.TrashItemDeletionSFX();
            //Rocket PowerUps
            MergeEffect(leftSlot.CurrentBus.transform);
            Destroy(leftSlot.CurrentBus.gameObject);
            leftSlot.ClearSlot();
            return;
        }
        SoundManager.Instance.ItemMergeSoundSFX();
        MergeEffect(leftBus.transform);
        NotifyPassengersOfNewBus(leftBus);
        BoardPassengersToBus(leftBus);
    }
    private void MergeEffect(Transform target)
    {
        MergeVFX.SetActive(false);
        Vector3 MergePos = target.position;
        MergePos.y = MergeVFX.transform.position.y;
        MergeVFX.transform.position = MergePos;
        MergeVFX.SetActive(true);
    }
    public void RocketPowerUps(Transform Target)
    {
        SoundManager.Instance.PlayRocketPowerUpSFX();
        RocketPowerupsVFX.SetActive(false);
        Vector3 Pos = Target.position;
        Pos.y = MergeVFX.transform.position.y;
        RocketPowerupsVFX.transform.position = Pos;
        RocketPowerupsVFX.SetActive(true);
    }
    public void FanPowerUps()
    {
        SoundManager.Instance.PlayFanPowerUpSFX();
        GameManager.Instance.FanPowerUpVFX.SetActive(false);
        GameManager.Instance.FanPowerUpVFX.SetActive(true);

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
            .Where(p => p.passengerColor == bus.busColor && !p.hasBoarded && !p.IsBoarding)
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
            GemsManager.Instance.AddGems(10);
            UIManager.Instance.ShowLevelCompleteUI();
            OnLevelComplete?.Invoke();
            SoundManager.Instance.LevelCompleteSFX();
            ClearSavedGameState();
        }
    }
    
    public void ClearSavedGameState()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "LevelSaveData.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Saved game state cleared.");
        }
    }

    public void LevelComplete()
    {
        LevelManager.Instance.OnLevelComplete?.Invoke();
    }

    public void PlaceBusInSlot(Bus selectedBus)
    {
        var clickedSlot = _slots.FirstOrDefault(slot => slot.isEmpty && !slot.isLocked);
        if (clickedSlot != null)
        {
            selectedBus.AssignSlot(clickedSlot);
            clickedSlot.AssignBus(selectedBus);
            TriggerCascadingMerge(clickedSlot, out Bus remainingBus);
            BoardPassengersToBus(remainingBus);
            InputManager.Instance.DeselectBus();
        }
         
        StartCoroutine(nameof(CheckLooseCondition));
    }
 
    public void PlaceBusInSlot(Bus selectedBus, Slot clickedSlot)
    {
        if (clickedSlot.isEmpty && !clickedSlot.isLocked)
        {
            selectedBus.AssignSlot(clickedSlot);
            clickedSlot.AssignBus(selectedBus);
            TriggerCascadingMerge(clickedSlot, out Bus remainingBus);
            BoardPassengersToBus(remainingBus);
            InputManager.Instance.DeselectBus();
        }
        StartCoroutine(nameof(CheckLooseCondition));
    }
    
    public void TriggerCascadingMerge(Slot clickedSlot, out Bus remainingBus)
    {
        CheckForMerging(clickedSlot, out remainingBus);
        foreach (var slot in _slots)
        {
            if (slot.CurrentBus != null)
            {
                CheckForMerging(slot, out remainingBus);
            }
        }
    }

    private IEnumerator CheckLooseCondition()
    {
        var isSlotEmpty = _slots.Any(slot => slot.isEmpty && !slot.isLocked);
        if (!isSlotEmpty)
        {
            yield return new WaitUntil(() => _passengers.Where(p => p.IsBoarding).All(p => p.hasBoarded));
            if(_passengers.Count>0)
            {
                ClearSavedGameState();
                UIManager.Instance.ShowLevelFailedUI();
                SoundManager.Instance.LevelCompleteSFX();
            }
        }
    }
}
