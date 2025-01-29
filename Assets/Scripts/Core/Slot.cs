using System;
using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    internal Bus CurrentBus;
    private Transform _referencePoint;
    private Transform _standTransform;
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

    public void AssignBus(Bus bus)
    {
        if (isLocked)
        {
            Debug.Log("Slot is locked. Cannot assign a bus.");
            return;
        }
        Debug.Log("KKK");

        CurrentBus = bus;
        StartCoroutine(MoveToSlot());
        //CurrentBus.transform.position = _referencePoint.transform.position;
        //CurrentBus.transform.rotation = _referencePoint.transform.rotation;
        if(bus.Rb!=null)
            bus.Rb.isKinematic = true;

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
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
    
    private IEnumerator MoveToSlot()
    {

        yield return new WaitForSeconds(0.5f);
        
        
        // Get random starting rotation and move to upright position
        Quaternion initialRotation = CurrentBus.transform.rotation;
        Quaternion uprightRotation = Quaternion.Euler(0, initialRotation.eulerAngles.y, 0);
        
        Vector3 initialPosition = CurrentBus.transform.position;
        initialPosition.y += 4f;

        CurrentBus.transform.position = initialPosition;

        yield return new WaitForSeconds(0.5f);
        
        InputManager.Instance.DeselectBus();
        
        //Debug.Log(gameObject.name+" V: "+Vector3.Distance(CurrentBus.transform.position, _referencePoint.transform.position));
        
        // Move and rotate to the vehicle slot
        while (Vector3.Distance(CurrentBus.transform.position, _referencePoint.transform.position) > 1f && !vehiclePlaced) 
                //|| Quaternion.Angle(CurrentBus.transform.rotation, uprightRotation) > 1f)
            {
                //Debug.Log(gameObject.name+" V: "+Vector3.Distance(CurrentBus.transform.position, _referencePoint.transform.position));
                // Move towards the slot
                CurrentBus.transform.position = Vector3.MoveTowards(CurrentBus.transform.position,
                    _referencePoint.transform.position, moveSpeed * Time.deltaTime*3);

                // Rotate to upright position
                //CurrentBus.transform.rotation = Quaternion.RotateTowards(CurrentBus.transform.rotation, uprightRotation, rotateSpeed * Time.deltaTime);

                yield return null;
            }

        Vector3 position = _referencePoint.transform.position;
        position.y += 2;
        CurrentBus.transform.position = position;


        Quaternion targetRotation = Quaternion.Euler(0, _referencePoint.transform.eulerAngles.y, 0);
        while (Quaternion.Angle(CurrentBus.transform.rotation, targetRotation) > 1f )
        {
            CurrentBus.transform.rotation = Quaternion.RotateTowards(CurrentBus.transform.rotation, 
                targetRotation, rotateSpeed * Time.deltaTime*2);
            yield return null;
        }
        
        CurrentBus.transform.rotation = targetRotation;
        CurrentBus.transform.position = _referencePoint.transform.position;

        // Align to the slot's rotation
        
        // Snap to the final position and rotation
        
        busMoving = true;

    }


    public void ClearSlot()
    {
        if (CurrentBus != null)
        { 
            //Debug.Log($"Clearing slot: {name}, Bus: {CurrentBus.name}");
            Destroy(CurrentBus.gameObject);
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
        GameManager.Instance. TriggerCascadingMerge(this, out Bus remainingBus);
        //GameManager.Instance.BoardPassengersToBus(remainingBus);
    }
}