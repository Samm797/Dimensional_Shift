using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorShift 
{
    /// <summary>
    /// Shifts a Vector3 by X degree angles. Use true for left, false for right. 
    /// </summary>
    /// <param name="direction">Initial Vector you are shifting from.</param>
    /// <param name="isLeft">True for shifting left, false for shifting right.</param>
    /// <param name="degrees">The number of degrees (as a float) you would like the Vector to be shifted either left or right.</param>
    /// <returns>A new Vector 3 with an X deg shift from the initial Vector3.</returns>
    public Vector3 ShiftVectorXDegreesLeft(Vector3 direction, bool isLeft, float degrees)
    {
        if (isLeft)
        {
            Vector3 shiftedDirection = (Quaternion.Euler(0f, 0f, degrees) * direction).normalized;
            return shiftedDirection;
        }
        else
        {
            Vector3 shiftedDirection = (Quaternion.Euler(0f, 0f, degrees) * direction).normalized;
            return shiftedDirection;
        }
    }
}
