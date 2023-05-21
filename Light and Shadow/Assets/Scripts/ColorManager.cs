using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private bool _isDarkActive;
    // private List<ColorChanger> _colorChangers;
    
    // Start is called before the first frame update
    void Start()
    {
        _isDarkActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called by the player on LeftShift (Axis: Shift) to swap the dimensions. Starts the coroutine so the change can happen with a small transition.  
    /// </summary>
    public void ChangeDimension()
    {
        // Swaps true and false
        StartCoroutine(ChangeDimensionRoutine());
    }

    IEnumerator ChangeDimensionRoutine()
    {
        // TODO - Camera affect / shake? 
        // Potential fade of some sort
        // If the bool just ends up swapping, this will be done in the function above
        // Swaps to shift dimensions

        // SUPER performant heavy, will likely not work for a bigger project
        // Currently a 2 second CD on this for the player and it will shift the background and the obstacles
        ColorChanger[] colorChangers = FindObjectsOfType<ColorChanger>();

        // Goes through the array of any object that has a ColorChanger item and will 
        foreach (ColorChanger colorChanger in colorChangers)
        {
            if (_isDarkActive == false)
            {
                colorChanger.ShiftToLight();
            }
            else
            {
                colorChanger.ShiftToDark();
            }
        }
        
        // Clears the array so we can shift back later
        for (int i = 0; i < colorChangers.Length; i++)
        {
            colorChangers[i] = null;
        }

        _isDarkActive = !_isDarkActive;
        yield break;
    }

    public bool IsDarkActive()
    {
        return _isDarkActive;
    }
}
