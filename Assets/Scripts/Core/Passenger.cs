using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Colors passengerColor;
    public bool hasBoarded;

    private void Start()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        GetComponent<Renderer>().material.color = passengerColor.GetColor();
    }

    public bool TryBoardBus(Slot slot)
    {
        if (slot.CurrentBus != null && slot.CurrentBus.busColor == passengerColor && slot.CurrentBus.Capacity > 0)
        {
            slot.CurrentBus.Capacity--; 
            hasBoarded = true;
            return true;
        }

        return false;
    }
}