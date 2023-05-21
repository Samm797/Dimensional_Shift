using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    public float outOfBoundsX, outOfBoundsY;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > outOfBoundsX || transform.position.x < -outOfBoundsX || transform.position.y > outOfBoundsY || transform.position.y < -outOfBoundsY)
        {
            Destroy(this.gameObject);
        }
    }
}
