using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Slot : MonoBehaviour
{
    internal Bus CurrentBus;
    public Transform _referencePoint;
    public Transform _standTransform;
    public float moveSpeed = 200f; // Speed at which the vehicle moves.
    public float rotateSpeed = 200f; // Speed of rotation during alignment.
    public bool isLocked;
    public bool vehiclePlaced;
    private GameObject _lockedIcon;
    public bool busMoving=false;

    private void Awake()
    {
        if (transform.GetChild(0).name == "ReferencePoint")
            _referencePoint = transform.GetChild(0);
        else
            _referencePoint = transform.GetChild(1);
        
        name = $"{name} {transform.GetSiblingIndex()}";
        if (isLocked)
        {
            _lockedIcon = Instantiate(Resources.Load<GameObject>("Unlock Icon"), transform, false);
            _lockedIcon = Instantiate(Resources.Load<GameObject>("TrafficCones"), transform, false);
        }
    }

    public bool isEmpty
    {
        get => CurrentBus == null;
        set
        {
            if(value)
            CurrentBus =null;
        }
    }


    public void UnlockSlot()
    {
        if (GemsManager.Instance.GetGems() >= Constants.UnlockSlotRequirement)
        {
            isLocked = false;
            GemsManager.Instance.UseGems(Constants.UnlockSlotRequirement);
            if(_lockedIcon)
                Destroy(_lockedIcon);
        }
    }

    public IEnumerator AssignBus(Bus bus)
    {
        if (isLocked)
        {
            Debug.Log("Slot is locked. Cannot assign a bus.");
            yield return null;
        }
        Debug.Log("KKK");

        CurrentBus = bus;
        MoveToSlot();
        //CurrentBus.transform.position = _referencePoint.transform.position;
        //CurrentBus.transform.rotation = _referencePoint.transform.rotation;
        if(bus.Rb!=null)
            bus.Rb.isKinematic = true;

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        if (LevelManager.Instance._levels[currentLevel])
        {
            if (!LevelManager.Instance._levels[currentLevel].loadingData)
            {
                if (!GameManager.Instance.CurrentBusExistInGame(CurrentBus.busColor))
                {
                    Debug.LogError("Bus NOT Exist");
                    if (TutorialManager.Instance)
                    {
                        if (TutorialManager.Instance.IsFirstTrashDone)
                        {
                            yield return new WaitForSeconds(4f);
                            TutorialManager.Instance.InitSecondTrashItems();
                        }
                    }
                }
            }
        }
    }
    public void AssignMergeBus(Bus bus)
    {
        if (isLocked)
        {
            Debug.Log("Slot is locked. Cannot assign a bus.");
            return;
        }
        Debug.Log("KKK");

        CurrentBus = bus;
        //StartCoroutine(MoveToSlot());
        CurrentBus.transform.position = _referencePoint.transform.position;
        CurrentBus.transform.rotation = _referencePoint.transform.rotation;
        if(bus.Rb!=null)
            bus.Rb.isKinematic = true;

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        if (LevelManager.Instance._levels[currentLevel])
        {
            if (!LevelManager.Instance._levels[currentLevel].loadingData)
            {
                if (!GameManager.Instance.CurrentBusExistInGame(CurrentBus.busColor))
                {
                    Debug.LogError("Bus NOT Exist");
                    if (TutorialManager.Instance)
                    {
                        if (TutorialManager.Instance.IsFirstTrashDone)
                            TutorialManager.Instance.InitSecondTrashItems();
                    }
                }
            }
        }
    }

    private void MoveToSlot()
    {
        //CurrentBus.GetComponent<SquashAndStretch>().enabled = true;
        busMoving = true;
        Vector3 initialPosition = CurrentBus.transform.position;
        Quaternion initialRotation = CurrentBus.transform.rotation;
        Vector3 targetPosition = _referencePoint.transform.position;
        // Use the _referencePoint's y rotation for a consistent facing direction
        Quaternion targetRotation = Quaternion.Euler(0, _referencePoint.transform.eulerAngles.y, 0);

        // Use fixed durations to ensure consistent movement
        // Calculate durations based on a desired speed if necessary:
        // For example, if you want 10 units upward at 30 units/sec, duration = 10 / 30 ≈ 0.33 sec.
        float verticalDuration = 0.33f; // approximate duration for 10 units upward
        float horizontalDuration = 0.5f; // set duration for horizontal movement (adjust as needed)

        // Create a DOTween sequence using duration-based tweens
        Sequence seq = DOTween.Sequence();

        // 1. Vertical tween: move upward by 10 units.
        seq.Append(CurrentBus.transform
            .DOMove(initialPosition + Vector3.up * 10f, verticalDuration)
            .SetEase(Ease.OutQuad));

        // 2. Horizontal tween: move from the raised position to the target slot.
        seq.Append(CurrentBus.transform
            .DOMove(targetPosition, horizontalDuration)
            .SetEase(Ease.Linear));

        // 3. Concurrently rotate to face the slot.
        seq.Join(CurrentBus.transform
            .DORotateQuaternion(targetRotation, horizontalDuration)
            .SetEase(Ease.Linear));

        seq.OnComplete(() =>
        {
            CurrentBus.transform.position = targetPosition;
            CurrentBus.transform.rotation = targetRotation;
            busMoving = false;
            GameManager.Instance.PlacingBus = false;
            //CurrentBus.GetComponent<SquashAndStretch>().enabled = false;
        });

        seq.Play();
    }


    public void ClearSlot()
    {
        if (CurrentBus != null)
        { 
            //Debug.Log($"Clearing slot: {name}, Bus: {CurrentBus.name}");
            //Destroy(CurrentBus.gameObject);
            CurrentBus = null;
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==6)
        {
            GameManager.Instance.SlotVFX.SetActive(false);
            Vector3 MergePos = gameObject.transform.position;
            MergePos.y = GameManager.Instance.SlotVFX.transform.position.y;
            GameManager.Instance.SlotVFX.transform.position = MergePos;
            GameManager.Instance.SlotVFX.SetActive(true);
            vehiclePlaced = true;
            MergeBus();
        }
    }

    void MergeBus()
    {
        GameManager.Instance.TriggerCascadingMerge(this, out Bus remainingBus);
        //GameManager.Instance.BoardPassengersToBus(remainingBus);
    }
}