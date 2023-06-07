using UnityEngine;
using UnityEngine.UI;

public class CooldownIcons : MonoBehaviour
{
    /// <summary>
    /// 0 = cast; 1 = dash; 2 = warp
    /// </summary>
    public int cooldownID;
    private float _cooldownLength;
    private bool _isCooldown;
    public KeyCode _ability;
    private Image _icon;
    private PlayerController _player;

    void Start()
    {
        _icon = GetComponent<Image>();

        if (_player == null)
        {
            return;
        }

        switch (cooldownID) 
        {
            default:
                Debug.LogError("Default case reached in cooldownID in Cooldown Icons.");
                break;
            case 0:
                _cooldownLength = _player.CastCooldown;
                break;
            case 1:
                _cooldownLength = _player.DashCooldown;
                break;
            case 2:
                _cooldownLength = _player.WarpCooldown;
                break;
        }
    }

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        if ( _player == null )
        {
            Debug.LogError("The CooldownIcons' PlayerController is NULL.");
        }
    }

    void Update()
    {
        if (Input.GetKey(_ability) && !_isCooldown)
        {
            _isCooldown = true;
            _icon.fillAmount = 1;
        }

        if (_isCooldown)
        {
            _icon.fillAmount -= 1 / _cooldownLength * Time.deltaTime;

            if (_icon.fillAmount <= 0)
            {
                _icon.fillAmount = 1;
                _isCooldown = false;
            }
        }
    }
}
