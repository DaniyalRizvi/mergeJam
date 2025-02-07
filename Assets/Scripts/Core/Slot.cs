using System;
using System.Collections;
using UnityEngine;

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
        StartCoroutine(MoveToSlot());
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

    private IEnumerator MoveToSlot()
    {
        CurrentBus.GetComponent<SquashAndStretch>().enabled = true;
        busMoving = true;
        Vector3 initialPosition = CurrentBus.transform.position;
        Quaternion initialRotation = CurrentBus.transform.rotation;
        Vector3 targetPosition = _referencePoint.transform.position;
        Quaternion targetRotation = Quaternion.Euler(0, _referencePoint.transform.eulerAngles.y, 0);

        float riseHeight = 4f;
        float riseDuration = 0.5f;
        float moveDuration = 1f;
        float elapsedTime = 0f;

        Vector3 raisedPosition = initialPosition + Vector3.up * riseHeight;
        while (elapsedTime < riseDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / riseDuration;
            CurrentBus.transform.position = Vector3.Lerp(initialPosition, raisedPosition, t);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime*2;
            float t = elapsedTime / moveDuration;
            CurrentBus.transform.position = Vector3.Lerp(raisedPosition, targetPosition, t);
            CurrentBus.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

            yield return null;
        }
        CurrentBus.transform.position = targetPosition;
        CurrentBus.transform.rotation = targetRotation;
        busMoving = false;
        GameManager.Instance.PlacingBus=false;
        CurrentBus.GetComponent<SquashAndStretch>().enabled = false;
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