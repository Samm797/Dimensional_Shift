using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    private int _currentHealth;
    public HealthBar healthBar;

    public int CurrentHealth {get {return _currentHealth;}}
    
    void Start()
    {
        _currentHealth = _maxHealth;
        if (healthBar != null)
        {
            healthBar.MaxHealth(CurrentHealth);
        }
    }

    public void Damage(int amount)
    {
        _currentHealth -= amount;
        if (healthBar != null)
        {
            healthBar.SetHealth(_currentHealth);
        }

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            if (healthBar != null)
            {
                healthBar.SetHealth(_currentHealth);
            }
        }
    }
}
