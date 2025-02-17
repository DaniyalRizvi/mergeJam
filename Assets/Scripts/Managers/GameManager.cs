using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Managers;
using UnityEngine;
using DG.Tweening;

public class GameManager : Singelton<GameManager>
{
    //public VehicleDataManager VehicleDataManager;
    [SerializeField] private GameObject RocketPowerupsVFX;
    [SerializeField] public GameObject MergeVFX;
    [SerializeField] private GameObject FanPowerUpVFX;
    [SerializeField] private GameObject JumpPowerUpVFX;
    [SerializeField] private GameObject levelCompletedVFX;
    [SerializeField] private GameObject springVFX;
    public GameObject SlotVFX;
    public List<Slot> _slots = new();
    [SerializeField] public List<Passenger> _passengers = new();
    public Level _level;
    public Action OnLevelComplete;
    public bool PlacingBus;
    public bool movingBack;
    public bool MergingBus;
    public bool rocketPowerUp;
    public int maxCount;
    public List<Vector3> myQueuePositions = new List<Vector3>();
    bool levelCompleted = false; 
    public bool isBoardingInProgress = false;

    // Flag to cancel pending board tween calls.
    //private bool _cancelBoardTween = false;

    private IEnumerator Start()
    {
        SoundManager.Instance.SetSlotVFX();
        yield return new WaitUntil(() => DTAdsManager.Instance && DTAdsManager.Instance.isInitialised);
        Debug.Log("Ads Initialized: " + DTAdsManager.Instance.isInitialised);
        DTAdsManager.Instance.ShowAd(Constants.BannerID);
        InitializePassengerPositions();
    }


    public void InitializePassengerPositions()
    {
        myQueuePositions.Clear();
        foreach (var passenger in _passengers)
        {
            myQueuePositions.Add(passenger.myPosition);
        }
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
        InitializePassengerPositions();
    }


    private void CheckForMerging(Slot clickedSlot, out Bus remainingBus)
    {
        remainingBus = clickedSlot.CurrentBus;
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
                MergingBus = true;
                MergeAnimation(i - 1, leftSlot, rightSlot);
                //TryMergeBuses(leftSlot, rightSlot);
                //StartCoroutine(WaitToMergeBuses(leftSlot,rightSlot));

                //_level.SaveLevelStateToJson(); 
            }
            else
            {
                BoardPassengersToBusTween(i - 1);
                //boardCoroutine = StartCoroutine(BoardPassengerCoroutin(i - 1));
                //BoardPassengersToBus(i-1);
            }
            // else
            // {
            //     BoardPassengersToBus(i);
            // }
        }


        //CheckAllSlots();


        // } while (mergeHappened);
    }

    private bool CanMerge(Slot leftSlot, Slot rightSlot)
    {
        return leftSlot?.CurrentBus != null &&
               rightSlot?.CurrentBus != null &&
               leftSlot.CurrentBus.busColor == rightSlot.CurrentBus.busColor &&
               leftSlot.CurrentBus.capacity == rightSlot.CurrentBus.capacity && rightSlot.vehiclePlaced && leftSlot.vehiclePlaced;
    }

    // IEnumerator WaitToMergeBuses(Slot leftSlot, Slot rightSlot)
    // {
    //     yield return new WaitForSeconds(1f);
    //     Debug.Log("HEREREER");
    //     TryMergeBuses(leftSlot,rightSlot);
    // }

    public float mergeMoveSpeed = 20f; // Speed of movement during merge.
    public float moveSpeed = 0.15f; // Speed of movement during merge.
    public float hoverHeight = 5f; // Height for hovering during merge.
    public float hoverDuration = 1f;

    // public void MergeVehicles(Slot leftSlot, Slot rightSlot)
    // {
    //     MergeAnimation(leftSlot, rightSlot);
    // }

    private void MergeAnimation(int leftIndex, Slot leftSlot, Slot rightSlot)
    {
        Debug.Log("Merge animation started");
        var leftBus = leftSlot.CurrentBus;
        var rightBus = rightSlot.CurrentBus;
        if (leftBus == null || rightBus == null)
        {
            Debug.LogError("One of the buses is missing. Cannot merge.");
            return;
        }

        // Force the right bus to begin at the same Y as the left bus's starting Y.
        Vector3 rightStart = rightBus.transform.position;
        rightStart.y = leftBus.transform.position.y;
        rightBus.transform.position = rightStart;

        // 1. Hover both buses upward by 4 units relative to the left bus's starting Y.
        float commonHoverY = leftBus.transform.position.y + 4f;
        Vector3 leftHover = new Vector3(leftBus.transform.position.x, commonHoverY, leftBus.transform.position.z);
        Vector3 rightHover = new Vector3(rightBus.transform.position.x, commonHoverY, rightBus.transform.position.z);

        Sequence seq = DOTween.Sequence();
        // Both buses animate upward concurrently.
        seq.Append(leftBus.transform.DOMove(leftHover, 0.3f).SetEase(Ease.OutQuad));
        seq.Join(rightBus.transform.DOMove(rightHover, 0.3f).SetEase(Ease.OutQuad));

        // 2. After a brief pause, have both buses move concurrently to a middle point.
        seq.AppendInterval(0.05f);
        Vector3 midPoint = new Vector3((leftHover.x + rightHover.x) * 0.5f, commonHoverY, (leftHover.z + rightHover.z) * 0.5f);
        seq.Append(leftBus.transform.DOMove(midPoint, 0.3f).SetEase(Ease.Linear));
        seq.Join(rightBus.transform.DOMove(midPoint, 0.3f).SetEase(Ease.Linear));

        // 3. Merge the buses: update left bus properties and destroy right bus.
        seq.AppendCallback(() =>
        {
            // Update left bus: merge capacities, update visuals, etc.
            leftBus.capacity += rightBus.capacity;
            leftBus.VehicleRenderModels.ActiveVehicle(leftBus.capacity);
            leftBus.UpdateVisual();
            leftBus.currentSize += rightBus.currentSize;

            // Reassign left bus into the slot and destroy right bus.
            leftBus.AssignSlot(leftSlot);
            leftSlot.AssignMergeBus(leftBus);
            Destroy(rightBus.gameObject);

            // If after merging the bus color is not available in the level, trigger extra behavior.
            if (!_level.colors.Exists(i => i.color == leftBus.busColor))
            {
                if (TutorialManager.Instance)
                {
                    TutorialManager.Instance.tutorialCase++;
                    Debug.LogError("InitFan");
                }
                SoundManager.Instance.TrashItemDeletionSFX();
                MergeEffect(leftBus.transform);
                Destroy(leftBus.gameObject);
                leftSlot.ClearSlot();
                return;
            }
            SoundManager.Instance.ItemMergeSoundSFX();
        });

        // 4. After a brief hover, move the merged left bus to the left slot's final position.
        seq.AppendInterval(hoverDuration);
        Vector3 slotPos = leftSlot.transform.position;
        seq.Append(leftBus.transform.DOMove(slotPos, 0.3f).SetEase(Ease.Linear));

        // 5. Final callback: clear merge flag, notify passengers, and resume boarding.
        seq.AppendCallback(() =>
        {
            if (leftIndex - 1 >= 0)
                MergingBus = CanMerge(_slots[leftIndex - 1], _slots[leftIndex]);
            else
                MergingBus = false;
            NotifyPassengersOfNewBus(leftBus);
            //StartCoroutine(BoardPassengersToBus(leftIndex));
            BoardPassengersToBusTween(leftIndex);

            // Debug.Log("Merge animation completed");
        });

        seq.Play();
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
        foreach (var passenger in _passengers)
        {
            if (passenger.IsBoarding && passenger.passengerColor == newBus.busColor)
            {
                passenger.UpdateBusAfterMerge(newBus);
                passenger.StartMovingToBus();
            }
        }
    }


    public IEnumerator BoardPassengerCoroutin(int index)
    {
        yield return new WaitWhile(() => PlacingBus);
        BoardPassengersToBusTween(index);
    }

    public void BoardPassengersToBusTween(int index)
    {
        if (index < 0 || index >= _slots.Count || isBoardingInProgress || movingBack) return;

        var bus = _slots[index].CurrentBus;
        if (bus == null || bus.currentSize <= 0) return;

        if (PlacingBus || MergingBus)
        {
            DOVirtual.DelayedCall(0.1f, () => BoardPassengersToBusTween(index));
            return;
        }

        if (bus.currentSize > 0)
        {
            List<Passenger> passengers = new List<Passenger>();

            int count = 0;
            foreach (var p in _passengers)
            {
                if (!p.hasBoarded && !p.IsBoarding && p._selectedBus == null)
                {
                    if (p.passengerColor == bus.busColor)
                    {
                        passengers.Add(p);
                        count++;
                        if (count == bus.currentSize) break;
                    }
                    else break;
                }
            }

            if (passengers.Count > 0)
            {
                isBoardingInProgress = true; // Block next batch
                DOVirtual.DelayedCall(0.65f, () =>
                {
                    TryBoardPassengerTween(passengers, bus, index);
                });
            }
            else
            {
                Debug.Log("No available passengers to board.");
            }
        }
    }

    public void RemovePassenger(Passenger passenger)
    {
        if (passenger == null || !_passengers.Contains(passenger))
        {
            return;
        }

        _passengers.Remove(passenger);
        UIManager.Instance.UpdateHolder(passenger.passengerColor);

        if (passenger.gameObject != null)
        {
            passenger.gameObject.SetActive(false);
            Destroy(passenger.gameObject);
        }
        else
        {
            Debug.LogError($"Passenger {passenger.id} was already missing before removal!");
        }

        if (!_passengers.Any(p => p.IsBoarding))
        {
            isBoardingInProgress = false;

            for (int i = 0; i < _slots.Count; i++)
            {
                BoardPassengersToBusTween(i);
            }
        }

        CheckLevelCompletion();
    }



    public void TryBoardPassengerTween(List<Passenger> passengers, Bus bus, int currentIndex)
    {
        List<Passenger> boardedPassengers = new List<Passenger>();

        foreach (var p in passengers)
        {
            if (!p.IsBoarding)
            {
                p.TryBoardBus(bus, boarded =>
                {
                    if (boarded)
                    {
                        boardedPassengers.Add(p);
                        p.gameObject.SetActive(false);
                        Destroy(p.gameObject);
                    }
                });
            }
        }

        DOVirtual.DelayedCall(0.3f, () =>
        {
            foreach (var p in boardedPassengers)
            {
                _passengers.Remove(p);
            }

            if(PlayerPrefs.GetInt("LevelTutorialCompleted") == 1)
                MoveAllPassengersForward();

            if (TutorialManager.Instance && PlayerPrefs.GetInt("LevelTutorialCompleted") == 0)
        {
            switch (TutorialManager.Instance.tutorialCase)
            {
                case 5:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.InitSecondBus();
                        break;
                    }
                case 6:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.InitPanel(
                            "When two vehicles of the same color and size merge, they form a higher-capacity vehicle!");
                        break;
                    }
            }
        }



            if (bus.currentSize > 0)
            {
                BoardPassengersToBusTween(currentIndex);
            }
            
        });
    }

    private void MoveAllPassengersForward()
    {
        if (_passengers.Count == 0)
        {
            return;
        }
        List<Passenger> waitingPassengers = _passengers.FindAll(p => !p.hasBoarded && !p.IsBoarding);
        for (int i = 0; i < waitingPassengers.Count; i++)
        {
            if (i < myQueuePositions.Count)
            {
                Vector3 targetPosition = myQueuePositions[i];
                waitingPassengers[i].MovePlayerToPosition(targetPosition);
            }
        }
    }

    public Bus GetLatestBusForPassenger(Passenger passenger)
    {
        foreach (var slot in _slots)
        {
            if (slot.CurrentBus != null && slot.CurrentBus.busColor == passenger.passengerColor)
            {
                return slot.CurrentBus;
            }
        }

        return null;
    }


    public Bus FindBestBusForPassenger(Passenger passenger)
    {
        foreach (var slot in _slots)
        {
            if (slot.CurrentBus != null && slot.CurrentBus.busColor == passenger.passengerColor && slot.CurrentBus.currentSize > 0)
            {
                return slot.CurrentBus;
            }
        }

        return null; // ? No valid bus found
    }

    private void CheckLevelCompletion()
    {
        if (_passengers.Count == 0 && !levelCompleted)
        {
            levelCompletedVFX.SetActive(false);
            //GemsManager.Instance.AddGems(10);
            levelCompletedVFX.SetActive(true);
            SoundManager.Instance.ConfettiSoundSFX();
            Invoke("SetLevelCompleteUI", 1f);
            levelCompleted = true;
        }
        else
        {
            Debug.Log("Level completion check");
            DOVirtual.DelayedCall(0.1f, () => { BoardPassengersToBusTween(_slots.Count - 1); });
            //BoardPassengersToBusTween(_slots.Count - 1);
            //StartCoroutine(BoardPassengersToBus(_slots.Count - 1));
        }
    }

    private void SetLevelCompleteUI()
    {
        UIManager.Instance.ShowLevelCompleteUI();
        OnLevelComplete?.Invoke();
        SoundManager.Instance.LevelCompleteSFX();
        ClearSavedGameState();
        levelCompleted = false;
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
            PlacingBus = true;
            selectedBus.AssignSlot(clickedSlot);
            StartCoroutine(clickedSlot.AssignBus(selectedBus));
            //TriggerCascadingMerge(clickedSlot, out Bus remainingBus);
            //StartCoroutine(BoardPassengersToBus(remainingBus));
            //InputManager.Instance.DeselectBus();
        }

        Debug.Log("PlacingBus: " + PlacingBus);

        StartCoroutine(CheckLooseCondition());
    }

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
        Debug.Log("Checking loose condition");

        yield return new WaitUntil(() => !MergingBus);
        yield return new WaitUntil(() => !_passengers.Any(p => p.IsBoarding));

        yield return new WaitForSeconds(0.5f);
        // If there is any empty slot available, we should not trigger a loose condition.
        bool isSlotEmpty = _slots.Any(slot => slot.isEmpty && !slot.isLocked);
        if (!isSlotEmpty)
        {
            // First, wait until no bus is being placed or merged.

            // Enter a loop that continuously checks:
            // 1. Whether any merging is still active.
            // 2. Whether any bus on a slot, which still has boarding space, can board a waiting passenger.
            // 3. Whether any merge is possible between adjacent slots.
            while (true)
            {
                Debug.Log("MergingBus: " + MergingBus);

                bool canAnyBusBoard = false;
                foreach (var slot in _slots)
                {
                    if (slot.CurrentBus != null && slot.CurrentBus.currentSize > 0)
                    {
                        if (_passengers[0].passengerColor == slot.CurrentBus.busColor
                            && !_passengers[0].hasBoarded && !_passengers[0].IsBoarding)
                        {
                            canAnyBusBoard = true;
                            break;
                        }
                    }
                }

                bool anyMergePossible = false;
                // Check for a possible merge among adjacent slots.
                for (int i = 1; i < _slots.Count; i++)
                {
                    if (CanMerge(_slots[i - 1], _slots[i]))
                    {
                        anyMergePossible = true;
                        break;
                    }
                }

                Debug.Log("canAnyBusBoard: " + canAnyBusBoard);
                Debug.Log("anyMergePossible: " + anyMergePossible); 
                Debug.Log("Passengers Count: " + _passengers.Any(p => p.IsBoarding));

                //yield return new WaitUntil(() => !_passengers.Any(p => p.IsBoarding));

                // If at least one boarding opportunity exists or if merging is possible, wait a bit and re-check.
                if (canAnyBusBoard || anyMergePossible || _passengers.Any(p => p.IsBoarding))
                {
                    yield return new WaitForSeconds(0.5f);
                    CheckLevelCompletion();
                }
                else
                {
                    // If neither boarding nor merging opportunities exist, break the loop.
                    break;
                }
            }
            // After leaving the loop, if there are still passengers waiting, trigger level failure.
            if (_passengers.Count > 0)
            {
                ClearSavedGameState();
                UIManager.Instance.ShowLevelFailedUI();
                SoundManager.Instance.LevelCompleteSFX();
            }
        }
    }

    public void MovePassengerBack(Bus bus)
    {
        // Cancel any pending boarding tween logic.
        //_cancelBoardTween = true;

        int count = 0;
        foreach (var p in _passengers)
        {
            if (p.passengerColor == bus.busColor && p.IsBoarding)
            {
                count++;
                p.IsBoarding = false;
                p._selectedBus = null;
            }
        }
        bus.currentSize += count;
        // Optionally, if you want to resume boarding later, reset _cancelBoardTween.
        // _cancelBoardTween = false;
    }
    public IEnumerator ApplySpringVFX(Transform transform)
    {
        var position = transform.position;
        //position.x += 0.5f;
        position.z += 0.5f;

        if (springVFX)
        {
            var springObj = Instantiate(springVFX, position, transform.rotation);
            yield return new WaitForSeconds(2f);
            Destroy(springObj);
        }
    }

    // New method: Moves the bus from its current position to the target slot using DOTween.
    public void MoveBusToSlot(Bus bus, Transform slotTransform, float duration = 1f)
    {
        // Create a DOTween sequence to move and rotate the bus simultaneously.
        Sequence moveSequence = DOTween.Sequence();

        // Tween the position to the slot's position.
        moveSequence.Append(bus.transform.DOMove(slotTransform.position, duration).SetEase(Ease.InOutQuad));

        // Tween the rotation to match the slot's rotation.
        moveSequence.Join(bus.transform.DORotateQuaternion(slotTransform.rotation, duration).SetEase(Ease.InOutQuad));

        // Optionally, put an OnComplete callback to trigger any extra logic once the move is done.
        moveSequence.OnComplete(() =>
        {
            // For example, you might set a flag that the bus is now in position.
            //Debug.Log("Bus move complete");
            // Additional code, if required.
        });

        moveSequence.Play();
    }
}
