using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotation;

    void Update()
    {
        transform.Rotate(rotation );
    }
}
