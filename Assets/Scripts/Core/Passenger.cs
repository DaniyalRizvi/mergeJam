using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Passenger : MonoBehaviour
{
    [SerializeField] private PassengerAnimator PassengerAnimator;
    public Colors passengerColor;
    public bool hasBoarded;
    internal bool IsBoarding;
    public GameObject vfx;
    public int id;
    public Bus _selectedBus;

    private void Start()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        //GetComponent<Renderer>().material.color = passengerColor.GetColor();
        PassengerAnimator.transform.GetChild(1).GetComponent<Renderer>().material.color = passengerColor.GetColor();
    }

    public void TryBoardBus(Bus bus, Action<bool> onComplete)
    {
        if (IsBoarding && _selectedBus != null)
            return;
        StartCoroutine(TryBoardBus(bus, 8f, onComplete));
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

                    Vector3 direction = _selectedBus.gateTransform.position - transform.position;

                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 100f * Time.deltaTime);

                    PassengerAnimator.IsWalking(true);
                }
                else
                {
                    PassengerAnimator.IsWalking(false);
                    break;
                }
            }

            yield return null;
        }
        Debug.Log("JJJJJJJ");

        while (true)
        {
            if (_selectedBus)
            {
                if (Vector3.Distance(transform.position, _selectedBus.transform.GetChild(0).position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _selectedBus.transform.GetChild(0).position, speed * Time.deltaTime);

                    Vector3 direction = _selectedBus.transform.GetChild(0).position - transform.position;

                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 100f * Time.deltaTime);
                    PassengerAnimator.IsWalking(true);
                }
                else
                {
                    PassengerAnimator.IsWalking(false);
                    break;
                }
            }

            yield return null;
        }

        if (TutorialManager.Instance)
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
        UIManager.Instance.UpdateHolder(passengerColor);
        hasBoarded = true;
        onComplete?.Invoke(hasBoarded);
    }

    public void MovePlayerToPosition(Vector3 targetPosition)
    {
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float moveSpeed = 5f;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
