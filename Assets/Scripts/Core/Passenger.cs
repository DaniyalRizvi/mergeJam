using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;

public class Passenger : MonoBehaviour
{
    [SerializeField] private PassengerAnimator PassengerAnimator;
    public Colors passengerColor;
    public bool hasBoarded;
    internal bool IsBoarding;
    public GameObject vfx;
    public int id;
    public Bus _selectedBus;
    public Vector3 myPosition;

    private void Start()
    {
        myPosition = this.transform.position;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        //GetComponent<Renderer>().material.color = passengerColor.GetColor();
        PassengerAnimator.transform.GetChild(1).GetComponent<Renderer>().material.color = passengerColor.GetColor();
    }

    public void UpdateBusAfterMerge(Bus bus)
    {
        _selectedBus = bus;
    }

    public void ClearSelectedBus()
    {
        if (IsBoarding)
        {
            _selectedBus.currentSize++;
            _selectedBus = null;
            IsBoarding = false;
        }
    }

    public void TryBoardBus(Bus bus, Action<bool> onComplete)
    {
        if (IsBoarding && _selectedBus != null)
            return;
        StartCoroutine(TryBoardBus(bus, 8f, onComplete));
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
        float moveSpeed = 10f;
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / moveSpeed;

        // Use DOTween to move the passenger to the target position with linear easing.
        transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);

        // Calculate the final rotation that faces the target.
        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion finalRotation = Quaternion.LookRotation(direction);
            // Tween the rotation concurrently with the movement.
            transform.DORotateQuaternion(finalRotation, duration).SetEase(Ease.Linear);
        }
    }
}
