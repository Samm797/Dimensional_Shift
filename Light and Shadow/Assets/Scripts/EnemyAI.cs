using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform _target;
    [SerializeField] private float _moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Find the player and walk toward them 
        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Currently just kills the enemies
        // TODO: Have these functions implement the health system
        if (other.CompareTag("Spell"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        // Damage the player 
        if (other.CompareTag("Player"))
        {
            Debug.Log("Damaged the player!");
        }
    }

}
