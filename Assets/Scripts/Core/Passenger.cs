using System;
using System.Collections;
using System.Threading.Tasks;
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

    public void TryBoardBus(Slot slot, Action<bool> onComplete)
    {
        StartCoroutine(TryBoardBus(slot, 5f, onComplete));
    }

    private IEnumerator TryBoardBus(Slot slot, float speed, Action<bool> onComplete)
    {
        if (slot.CurrentBus.Capacity > 0)
        {
            Debug.Log("Passenger boarded the bus!");
            slot.CurrentBus.Capacity--;
            hasBoarded = true;
        }
        else
        {
            Debug.Log("Bus is full!");
            onComplete?.Invoke(hasBoarded);
            yield break;
        }
        Transform gateTransform = slot.CurrentBus.gateTransform;
        yield return StartCoroutine(MoveToPosition(gateTransform.position, speed));
        yield return StartCoroutine(MoveToPosition(slot.CurrentBus.transform.GetChild(0).position, speed));
        onComplete?.Invoke(hasBoarded);
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; 
        }
    }
}
