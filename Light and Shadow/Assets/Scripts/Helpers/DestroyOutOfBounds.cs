using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    public float outOfBoundsX, outOfBoundsY;

    void Update()
    {
        // If this gameObject ever leaves the bounds, it is destroyed
        // Bounds are set in the inspector for each gameObject
        if (transform.position.x > outOfBoundsX || transform.position.x < -outOfBoundsX || transform.position.y > outOfBoundsY || transform.position.y < -outOfBoundsY)
        {
            Destroy(this.gameObject);
        }
    }
}
