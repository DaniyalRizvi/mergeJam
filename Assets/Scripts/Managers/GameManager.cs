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
    [SerializeField] private GameObject JumpPowerUpVFX;
    [SerializeField] private GameObject levelCompletedVFX;
    [SerializeField] private GameObject springVFX;
    public GameObject SlotVFX;
    private List<Slot> _slots = new();
    private List<Passenger> _passengers = new();
    public Level _level;
    public Action OnLevelComplete;
    public bool rocketPowerUp;
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

        Debug.Log("HEREE");
        // bool mergeHappened;
        // do
        // {
        //     mergeHappened = false;

        for (int i = _slots.Count - 1; i > 0; i--)
        {
            var leftSlot = _slots[i - 1];
            var rightSlot = _slots[i];

            //mergeHappened = true;

            if (CanMerge(leftSlot, rightSlot))
            {
                Debug.Log("HEREHRHEH");
                StartCoroutine(MergeAnimation(i - 1, leftSlot, rightSlot));
                //TryMergeBuses(leftSlot, rightSlot);
                //StartCoroutine(WaitToMergeBuses(leftSlot,rightSlot));

                //_level.SaveLevelStateToJson(); 
            }
            else
            {
                BoardPassengersToBus(i);
            }
        }

        //CheckAllSlots();


        // } while (mergeHappened);
    }

    private bool CanMerge(Slot leftSlot, Slot rightSlot)
    {
        return leftSlot?.CurrentBus != null &&
               rightSlot?.CurrentBus != null &&
               leftSlot.CurrentBus.busColor == rightSlot.CurrentBus.busColor &&
               leftSlot.CurrentBus.capacity == rightSlot.CurrentBus.capacity && rightSlot.vehiclePlaced && leftSlot.vehiclePlaced ;
    }

    // IEnumerator WaitToMergeBuses(Slot leftSlot, Slot rightSlot)
    // {
    //     yield return new WaitForSeconds(1f);
    //     Debug.Log("HEREREER");
    //     TryMergeBuses(leftSlot,rightSlot);
    // }
    
    public float mergeMoveSpeed = 20f; // Speed of movement during merge.
    public float moveSpeed = 30; // Speed of movement during merge.
    public float hoverHeight = 5f; // Height for hovering during merge.
    public float hoverDuration = 1f;

    // public void MergeVehicles(Slot leftSlot, Slot rightSlot)
    // {
    //     MergeAnimation(leftSlot, rightSlot);
    // }

    private IEnumerator MergeAnimation(int leftIndex, Slot leftSlot, Slot rightSlot)
    {
        Debug.Log("Merge animation started");
        var leftBus = leftSlot.CurrentBus;
        var rightBus = rightSlot.CurrentBus;
        if (leftBus == null || rightBus == null)
        {
            Debug.LogError("One of the buses is missing. Cannot merge.");
            yield break;
        }
        Vector3 leftInitialPosition = leftBus.transform.position;
        Vector3 leftHoverPosition = leftInitialPosition + Vector3.up * hoverHeight;
        Vector3 rightInitialPosition = rightBus.transform.position;
        Vector3 rightHoverPosition = rightInitialPosition + Vector3.up * hoverHeight;

        float moveSpeedMultiplier = 2f;
        while (Vector3.Distance(leftBus.transform.position, leftHoverPosition) > 0.1f || Vector3.Distance(rightBus.transform.position, rightHoverPosition) > 0.1f)
        {
            leftBus.transform.position = Vector3.MoveTowards(leftBus.transform.position, leftHoverPosition, mergeMoveSpeed * moveSpeedMultiplier * Time.deltaTime);
            rightBus.transform.position = Vector3.MoveTowards(rightBus.transform.position, rightHoverPosition, mergeMoveSpeed * moveSpeedMultiplier * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Buses hovered");

        while (Vector3.Distance(rightBus.transform.position, leftBus.transform.position) > 1f)
        {
            rightBus.transform.position = Vector3.MoveTowards(rightBus.transform.position,
                leftBus.transform.position, moveSpeed * moveSpeedMultiplier * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Buses merged");
        if (MergeVFX != null)
        {
            MergeVFX.SetActive(false);
            Vector3 mergePos = leftBus.transform.position;
            mergePos.y = MergeVFX.transform.position.y;
            MergeVFX.transform.position = mergePos;
            MergeVFX.SetActive(true);
        }
        Debug.Log("Explosion effect triggered");

        leftBus.capacity += rightBus.capacity;
        leftBus.VehicleRenderModels.ActiveVehicle(leftBus.capacity);
        leftBus.UpdateVisual();
        leftBus.currentSize += rightBus.currentSize;

        leftBus.AssignSlot(leftSlot);
        leftSlot.AssignMergeBus(leftBus);
        rightSlot.ClearSlot();

        if (!_level.colors.Exists(i => i.color == leftSlot.CurrentBus.busColor))
        {
            if (TutorialManager.Instance)
            {
                TutorialManager.Instance.tutorialCase++;
                TutorialManager.Instance.InitFanPanel();
                Debug.LogError("InitFan");
            }
            SoundManager.Instance.TrashItemDeletionSFX();
            MergeEffect(leftSlot.CurrentBus.transform);
            Destroy(leftSlot.CurrentBus.gameObject);
            leftSlot.ClearSlot();
            yield return null;
        }
        SoundManager.Instance.ItemMergeSoundSFX();

        Vector3 hoverTargetPosition = leftHoverPosition;
        float hoverStartTime = Time.time;

        while (Time.time - hoverStartTime < hoverDuration)
        {
            if (leftBus != null)
                leftBus.transform.position = Vector3.Lerp(leftBus.transform.position, hoverTargetPosition, Time.deltaTime * 4f); // Increase hover speed
            yield return null;
        }

        Vector3 slotPosition = leftSlot.transform.position;
        while (Vector3.Distance(leftBus.transform.position, slotPosition) > 0.1f)
        {
            leftBus.transform.position = Vector3.MoveTowards(leftBus.transform.position, slotPosition,
                mergeMoveSpeed * moveSpeedMultiplier * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Merged bus placed in slot");

        NotifyPassengersOfNewBus(leftBus);
        BoardPassengersToBus(leftIndex);
        Debug.Log("Merge animation completed");
    }

    // private void TryMergeBuses(Slot leftSlot, Slot rightSlot)
    // {
    //     var leftBus = leftSlot.CurrentBus;
    //     var rightBus = rightSlot.CurrentBus;
    //     
    //     leftBus.capacity += rightBus.capacity;
    //
    //     leftBus.VehicleRenderModels.ActiveVehicle(leftBus.capacity);
    //     leftBus.UpdateVisual();
    //     
    //     leftBus.currentSize += rightBus.currentSize;
    //     leftBus.AssignSlot(leftSlot);
    //     leftSlot.AssignMergeBus(leftBus);
    //     rightSlot.ClearSlot();
    //     if (!_level.colors.Exists(i=>i.color == leftSlot.CurrentBus.busColor))
    //     {
    //         if (TutorialManager.Instance)
    //         {
    //             TutorialManager.Instance.tutorialCase++;
    //             //TutorialManager.Instance.InitFan();
    //             TutorialManager.Instance.InitFanPanel();
    //             Debug.LogError("InitFan");
    //         }
    //         SoundManager.Instance.TrashItemDeletionSFX();
    //         //Rocket PowerUps
    //         MergeEffect(leftSlot.CurrentBus.transform);
    //         Destroy(leftSlot.CurrentBus.gameObject);
    //         leftSlot.ClearSlot();
    //         return;
    //     }
    //     SoundManager.Instance.ItemMergeSoundSFX();
    //     MergeEffect(leftBus.transform);
    //     NotifyPassengersOfNewBus(leftBus);
    //     BoardPassengersToBus(leftBus);
    // }
    private void MergeEffect(Transform target)
    {
        MergeVFX.SetActive(false);
        Vector3 MergePos = target.position;
        //MergePos.y = MergeVFX.transform.position.y;
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
    
    public void JumpPowerUps()
    {
        SoundManager.Instance.PlayFanPowerUpSFX();
        GameManager.Instance.JumpPowerUpVFX.SetActive(false);
        GameManager.Instance.JumpPowerUpVFX.SetActive(true);
    }

    private void NotifyPassengersOfNewBus(Bus newBus)
    {
        var passengersToRedirect = _passengers.Where(p => p.IsBoarding && p.passengerColor == newBus.busColor).ToList();
        foreach (var passenger in passengersToRedirect)
        {
            passenger.UpdateBusAfterMerge(newBus);
        }
    }

    public void BoardPassengersToBus(int i)
    {
        Debug.Log(i);
        if(i<0)
            return;
        Debug.Log(_passengers.Count);
        if(_passengers.Count<=0)
            return;

        // var matchingPassengers = _passengers
        //     .Where(p => p.passengerColor == bus.busColor && !p.hasBoarded && !p.IsBoarding)
        //     .ToList();


        // foreach (var passenger in _passengers)
        // {
        var bus = _slots[i].CurrentBus;
       
        if (bus != null)
        {
            Debug.Log(bus.currentSize);
            if (bus.currentSize > 0)
            {
                var passenger = _passengers[0];
                if (passenger.passengerColor == bus.busColor && !passenger.hasBoarded && !passenger.IsBoarding)
                {
                    passenger.TryBoardBus(bus, hasBoarded =>
                    {
                        if (hasBoarded)
                        {
                            _passengers.Remove(passenger);
                            Destroy(passenger.gameObject);
                            CheckLevelCompletion();
                            Debug.Log("HHH");
                            BoardPassengersToBus(i);
                        }
                        //QueuePassangers(passenger.gameObject.transform.position);
                    });

                }
                else
                {
                    BoardPassengersToBus(i-1);
                }
            }
            else
            {
                BoardPassengersToBus(i-1);
            }
        }
        else
        {
            BoardPassengersToBus(i-1);
        }
        

        // }
    }

    // public void CheckAllSlots()
    // {
    //     for(int i=0; i<_slots.Count;i++)
    //     {
    //         if(_slots[i].CurrentBus!=null)
    //             BoardPassengersToBus(i);
    //     }
    // }

    public void QueuePassangers(Vector3 position)
    {
        var newPosition = position;
        
        for (int i = 1; i < _passengers.Count; i++)
        {
            if(_passengers[i].IsBoarding)
                return;
            position = _passengers[i].transform.position;
            _passengers[i].MovePlayerToPosition(newPosition);
            newPosition = position;
        }
    }

    private void CheckLevelCompletion()
    {
        if (_passengers.Count == 0)
        {
            levelCompletedVFX.SetActive(false);
            GemsManager.Instance.AddGems(10);
            levelCompletedVFX.SetActive(true);
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
        if (clickedSlot != null && !rocketPowerUp)
        {
            selectedBus.AssignSlot(clickedSlot);
            clickedSlot.AssignBus(selectedBus);
            //TriggerCascadingMerge(clickedSlot, out Bus remainingBus);
            //StartCoroutine(BoardPassengersToBus(remainingBus));
            //InputManager.Instance.DeselectBus();
        }
         
        StartCoroutine(nameof(CheckLooseCondition));
    }
 
    // public void PlaceBusInSlot(Bus selectedBus, Slot clickedSlot)
    // {
    //     if (clickedSlot.isEmpty && !clickedSlot.isLocked)
    //     {
    //         selectedBus.AssignSlot(clickedSlot);
    //         clickedSlot.AssignBus(selectedBus);
    //         //TriggerCascadingMerge(clickedSlot, out Bus remainingBus);
    //         //StartCoroutine(BoardPassengersToBus(remainingBus));
    //         //InputManager.Instance.DeselectBus();
    //     }
    //     StartCoroutine(nameof(CheckLooseCondition));
    // }
    
    
    
    public void TriggerCascadingMerge(Slot clickedSlot, out Bus remainingBus)
    {
        Debug.Log("here");
        CheckForMerging(clickedSlot, out remainingBus);
        // foreach (var slot in _slots)
        // {
        //     if (slot.CurrentBus != null)
        //     {
        //         CheckForMerging(slot, out remainingBus);
        //     }
        // }
        //Invoke("BoardPassengersToBus",3f);
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

    public IEnumerator ApplySpringVFX(Transform transform)
    {
        var springObj = Instantiate(springVFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(springObj);
    }
}
