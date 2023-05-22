using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    private int _currentHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
    }


    public void Damage(int amount)
    {
        _currentHealth -= amount;
        Debug.Log($"{this.name}'s current health: {_currentHealth}.");  

        if (_currentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log($"{this.name} has died. :(");
    }

    public int CurrentHealth()
    {
        return _currentHealth;
    }
}
