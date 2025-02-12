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
        StartCoroutine(TryBoardBus(bus, 5f, onComplete));
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
        // Wait until merging is done and tween the passenger to the gate.
        yield return StartCoroutine(TweenPassengerToGate(speed));

        Debug.Log("MergingBus: "+GameManager.Instance.MergingBus);

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

    // This coroutine replaces the while-loop that moves the passenger to the bus gate.
    private IEnumerator TweenPassengerToGate(float speed)
    {
        // Function to (re)create a tween sequence toward the currently assigned bus's gate.
        Sequence CreateMoveTween()
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = _selectedBus.gateTransform.position;
            float distance = Vector3.Distance(startPos, targetPos);
            float duration = distance / speed;
            
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => PassengerAnimator.IsWalking(true));
            seq.Append(transform.DOMove(targetPos, duration).SetEase(Ease.Linear));
            seq.Join(transform.DORotateQuaternion(Quaternion.LookRotation(targetPos - transform.position), duration)
                .SetEase(Ease.Linear));
            seq.AppendCallback(() => PassengerAnimator.IsWalking(false));
            return seq;
        }
        
        // Create initial tween sequence.
        Sequence moveSeq = CreateMoveTween();
        
        // Loop until the tween completes.
        while (moveSeq != null && moveSeq.IsActive() && !moveSeq.IsComplete())
        {
            // First, check merging status.
            if (GameManager.Instance.MergingBus)
            {
                if (moveSeq.IsPlaying())
                {
                    moveSeq.Pause();
                    PassengerAnimator.IsWalking(false);
                }
                yield return new WaitUntil(() => !GameManager.Instance.MergingBus);
                if (!moveSeq.IsPlaying())
                {
                    moveSeq.Play();
                    PassengerAnimator.IsWalking(true);
                }
            }
            // Additionally, check if the current bus or its gateTransform has been destroyed.
            if (_selectedBus == null || _selectedBus.gateTransform == null)
            {
                // Kill the current tween.
                moveSeq.Kill();
                // Wait until _selectedBus becomes valid again.
                yield return new WaitUntil(() => _selectedBus != null && _selectedBus.gateTransform != null);
                // Create a new tween from the current position to the new _selectedBus's gate.
                moveSeq = CreateMoveTween();
            }
            yield return null;
        }
    }
}
