using UnityEngine;

public class Slot : MonoBehaviour
{
    internal Bus CurrentBus;
    private Transform _referencePoint;
    private Transform _standTransform;
    public bool isLocked;
    private GameObject _lockedIcon;

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


    public void ClearSlot()
    {
        if (CurrentBus != null)
        { 
            //Debug.Log($"Clearing slot: {name}, Bus: {CurrentBus.name}");
            Destroy(CurrentBus.gameObject);
            CurrentBus = null;
           
        }
    }


}