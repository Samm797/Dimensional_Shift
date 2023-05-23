using TMPro;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    private int _currentHealth;

    public int CurrentHealth {get {return _currentHealth;}}
    
    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void Damage(int amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
        }
    }
}
