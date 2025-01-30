using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [Header("Squash and Stretch Settings")]
    public GameObject[] objects; // Assign manually in Inspector
    public float duration = 0.4f; // Smooth transition time
    public float xFactor = 0.2f;  // Reduction in X scale
    public float yFactor = 0.2f;  // Reduction in Y scale
    public float zFactor = 0.2f;  // Reduction in Z scale

    private Vector3[] _originalScales; // Stores original scales
    private Coroutine _effectCoroutine; // Reference to running coroutine

    private void OnEnable()
    {
        if (objects == null || objects.Length == 0) return;

        _originalScales = new Vector3[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                _originalScales[i] = objects[i].transform.localScale;
        }

        _effectCoroutine = StartCoroutine(SquashAndStretchLoop());
    }

    private IEnumerator SquashAndStretchLoop()
    {
        while (true) // Infinite loop
        {
            yield return StartCoroutine(SmoothTransition(1));  // Squash
            yield return StartCoroutine(SmoothTransition(-1)); // Stretch
        }
    }

    private IEnumerator SmoothTransition(int direction)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth easing function

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    Vector3 newScale = _originalScales[i];
                    newScale.x += direction * xFactor * t;
                    newScale.y += direction * yFactor * t;
                    newScale.z += direction * zFactor * t;
                    objects[i].transform.localScale = newScale;
                }
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (_effectCoroutine != null)
            StopCoroutine(_effectCoroutine); // Stop the effect loop

        // Reset all objects to their original scale
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                objects[i].transform.localScale = _originalScales[i];
        }
    }
}
