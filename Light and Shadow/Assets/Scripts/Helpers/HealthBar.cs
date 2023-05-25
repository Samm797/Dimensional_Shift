using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider _healthSlider;

    public void SetHealth(int value)
    {
        _healthSlider.value = value;
    }

    public void MaxHealth(int value)
    {
        _healthSlider.maxValue = value;
        _healthSlider.value = value;
    }
}
