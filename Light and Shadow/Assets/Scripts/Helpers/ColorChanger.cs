using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    // Set in the inspector for each individual item in the scene 
    [SerializeField] private Color _lightShift, _darkShift;
    private SpriteRenderer _rend;
    
    // Start is called before the first frame update
    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    public void ShiftToLight()
    {
        _rend.color = _lightShift;
    }

    public void ShiftToDark()
    {
        _rend.color = _darkShift;
    }
}
