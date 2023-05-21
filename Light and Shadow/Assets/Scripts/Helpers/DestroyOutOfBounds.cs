using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    public float outOfBoundsX, outOfBoundsY;

    void Update()
    {
        if (transform.position.x > outOfBoundsX || transform.position.x < -outOfBoundsX || transform.position.y > outOfBoundsY || transform.position.y < -outOfBoundsY)
        {
            Destroy(this.gameObject);
        }
    }
}
