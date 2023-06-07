using UnityEngine;

public class SpellController : MonoBehaviour
{
    private bool _isDarkActive = false;
    private ColorManager _colorManager;
    private SpriteRenderer _spriteRenderer;
    public Color lightColor;
    public Color darkColor;

    private void Start()
    {
        _isDarkActive = _colorManager.IsDarkActive;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _colorManager = GameObject.Find("Color_Manager").GetComponent<ColorManager>();
    }

    private void Update()
    {
        if (_isDarkActive)
        {
            _spriteRenderer.color = darkColor;
        } 
        else
        {
            _spriteRenderer.color = lightColor;
        }
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
