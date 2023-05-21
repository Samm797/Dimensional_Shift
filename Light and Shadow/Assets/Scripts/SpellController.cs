using UnityEngine;

public class SpellController : MonoBehaviour
{
    private void Start()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: Have the spells "blow up" and create damage around the walls 

        if (other.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
    }
}
