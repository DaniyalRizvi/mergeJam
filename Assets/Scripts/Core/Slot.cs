using UnityEngine;

public class Slot : MonoBehaviour
{
    internal Bus CurrentBus;
    private Transform _referencePoint;


    public bool isLocked;

    private void Awake()
    {
        _referencePoint = transform.GetChild(0);
    }

    public bool IsEmpty => CurrentBus == null;


    public void UnlockSlot()
    {
        isLocked = false;
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
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ClearSlot()
    {
        CurrentBus = null;
    }
}