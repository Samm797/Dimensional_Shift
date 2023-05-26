using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownIcons : MonoBehaviour
{
    public float coolDownLength;
    private bool _isCooldown;
    public KeyCode _ability;
    private Image _icon;

    // Start is called before the first frame update
    void Start()
    {
        _icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(_ability) && !_isCooldown)
        {
            _isCooldown = true;
            _icon.fillAmount = 1;
        }

        if (_isCooldown)
        {
            _icon.fillAmount -= 1 / coolDownLength * Time.deltaTime;

            if (_icon.fillAmount <= 0)
            {
                _icon.fillAmount = 1;
                _isCooldown = false;
            }
        }

    }


}
