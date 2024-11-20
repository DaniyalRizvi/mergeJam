using System;
using System.Collections;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Colors passengerColor;
    public bool hasBoarded;
    internal bool IsBoarding;
    private Bus _selectedBus;

    private void Start()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        GetComponent<Renderer>().material.color = passengerColor.GetColor();
    }

    public void TryBoardBus(Bus bus, Action<bool> onComplete)
    {
        if(IsBoarding)
            return;
        StartCoroutine(TryBoardBus(bus, 5f, onComplete));
    }

    public void UpdateBusAfterMerge(Bus bus)
    {
        _selectedBus = bus;
    }

    private IEnumerator TryBoardBus(Bus bus, float speed, Action<bool> onComplete)
    {
        hasBoarded = false;
        if (bus.currentSize > 0)
        {
            _selectedBus = bus;
            _selectedBus.currentSize--;
        }
        else
        {
            onComplete?.Invoke(hasBoarded);
            yield break;
        }

        IsBoarding = true;
        while (true)
        {
            if (_selectedBus)
            {
                if (Vector3.Distance(transform.position, _selectedBus.gateTransform.position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _selectedBus.gateTransform.position,
                        speed * Time.deltaTime);
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }

        while (true)
        {
            if (_selectedBus)
            {
                if (Vector3.Distance(transform.position, _selectedBus.transform.GetChild(0).position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _selectedBus.transform.GetChild(0).position, speed * Time.deltaTime);
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }

        hasBoarded = true;
        onComplete?.Invoke(hasBoarded);
    }

}
