using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private bool _isDarkActive;
    private ColorChanger[] _colorChangers;
    // private List<ColorChanger> _colorChangers;
    
    // Start is called before the first frame update
    void Start()
    {
        _isDarkActive = false;
        _colorChangers = FindObjectsOfType<ColorChanger>();
    }

    /// <summary>
    /// Called by the player on LeftShift (Axis: Shift) to swap the dimensions. Starts the coroutine so the change can happen with a small transition.  
    /// </summary>
    public void ChangeDimension()
    {
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

        // Goes through the array of any object that has a ColorChanger item and will 
        foreach (ColorChanger colorChanger in _colorChangers)
        {
            if (_isDarkActive == true)
            {
                colorChanger.ShiftToLight();
            }
            else
            {
                colorChanger.ShiftToDark();
            }
        }
        
        _isDarkActive = !_isDarkActive;
        yield break;
    }

    public bool IsDarkActive()
    {
        return _isDarkActive;
    }
}
