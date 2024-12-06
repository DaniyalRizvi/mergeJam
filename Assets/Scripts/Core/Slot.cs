using UnityEngine;

public class Slot : MonoBehaviour
{
    internal Bus CurrentBus;
    private Transform _referencePoint;
    public bool isLocked;
    private GameObject _lockedIcon;

    private void Awake()
    {
        _referencePoint = transform.GetChild(0);
        name = $"{name} {transform.GetSiblingIndex()}";
        if (isLocked)
        {
            _lockedIcon = Instantiate(Resources.Load<GameObject>("Unlock Icon"), transform, false);
        }
    }

    public bool isEmpty => CurrentBus == null;


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

        CurrentBus = bus;
        CurrentBus.transform.position = _referencePoint.transform.position;
        CurrentBus.transform.rotation = _referencePoint.transform.rotation;
        bus.Rb.isKinematic = true;
    }


    public void ClearSlot()
    {
        if (CurrentBus != null)
        {
            Debug.Log($"Clearing slot: {name}, Bus: {CurrentBus.name}");
            Destroy(CurrentBus.gameObject);
            CurrentBus = null;
        }
    }


}