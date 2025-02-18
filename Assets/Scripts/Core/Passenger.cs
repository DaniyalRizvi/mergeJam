using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class Passenger : MonoBehaviour
{
    [SerializeField] private PassengerAnimator PassengerAnimator;
    public Colors passengerColor;
    public bool hasBoarded;
    public bool IsBoarding;
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
        if (bus == null)
        {
            return;
        }

        if (_selectedBus == null)
        {
            _selectedBus = bus;
        }

        if (IsBoarding) return;

        StartCoroutine(TryBoardBus(bus, 8f, onComplete));
    }


    private IEnumerator TryBoardBus(Bus bus, float speed, Action<bool> onComplete)
    {
        hasBoarded = false;
        if (bus.currentSize > 0)
        {
            _selectedBus = bus;
            _selectedBus.currentSize--;
            Debug.Log("Passenger: "+id+" boarded bus: "+bus.name+" current size: "+bus.currentSize);
        }
        else
        {
            onComplete?.Invoke(hasBoarded);
            yield break;
        }

        IsBoarding = true;
        yield return StartCoroutine(TweenPassengerToGate(speed));

        /*if (TutorialManager.Instance)
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
        }*/
        UIManager.Instance.UpdateHolder(passengerColor);
        hasBoarded = true;
        //_selectedBus.SetCurrentSize();
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

    private IEnumerator TweenPassengerToGate(float speed)
    {
        Sequence moveSeq = null;

        void StartMoveSequence()
        {
            if (_selectedBus == null || _selectedBus.gateTransform == null) return;

            Vector3 startPos = transform.position;
            Vector3 targetPos = _selectedBus.gateTransform.position;
            float distance = Vector3.Distance(startPos, targetPos);
            float duration = distance / speed;

            moveSeq = DOTween.Sequence();
            moveSeq.AppendCallback(() => PassengerAnimator.IsWalking(true));
            moveSeq.Append(transform.DOMove(targetPos, duration).SetEase(Ease.Linear));
            moveSeq.Join(transform.DORotateQuaternion(Quaternion.LookRotation(targetPos - transform.position), duration)
                .SetEase(Ease.Linear));
            moveSeq.AppendCallback(() =>
            {
                PassengerAnimator.IsWalking(false);
                hasBoarded = true;
                IsBoarding = false;

                Debug.Log($"Passenger {id} boarded bus {_selectedBus.name} and calling RemovePassenger");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RemovePassenger(this);
                }
            });
        }

        StartMoveSequence();

        while (true)
        {
            if (this == null)
            {
                yield break;
            }

            if (GameManager.Instance == null)
            {
                yield break;
            }

            if (GameManager.Instance.MergingBus)
            {
                if (moveSeq.IsPlaying())
                {
                    moveSeq.Pause();
                    PassengerAnimator.IsWalking(false);
                }

                yield return new WaitUntil(() => !GameManager.Instance.MergingBus);
                Slot busSlot = GameManager.Instance._slots.FirstOrDefault(slot => slot.CurrentBus == _selectedBus);

                if (busSlot != null && busSlot.vehiclePlaced && !GameManager.Instance.movingBack)
                {
                    Debug.Log($"Passenger {id} resumes boarding to the same bus {_selectedBus.name} after merge.");
                    StartMoveSequence(); // Resume movement to the same bus
                }
                else
                {
                    Bus foundBus = GameManager.Instance.FindBestBusForPassenger(this);
                    if (foundBus != null)
                    {
                        _selectedBus = foundBus;
                        Debug.Log($"Passenger {id} found a new bus after merge: {_selectedBus.name}");
                        StartMoveSequence();
                    }
                    else
                    {
                        Debug.LogError($"Passenger {id} couldn't find any bus after merge!");
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }


    public void StartMovingToBus()
    {
        if (_selectedBus == null)
        {
            Bus latestBus = GameManager.Instance.GetLatestBusForPassenger(this);
            if (latestBus != null)
            {
                _selectedBus = latestBus;
            }
            else
            {
                return;
            }
        }

        StopAllCoroutines();
        StartCoroutine(TweenPassengerToGate(5f));
    }
}
