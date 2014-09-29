using UnityEngine;
using System.Collections;



public static class Utilities
{

    /// <summary>
    /// Clamps an angle between -360 degrees and 360 degrees and keeps it within the min and max ranges.
    /// </summary>
    /// <param name="aAngle">The angle to clamp.</param>
    /// <param name="aMin">The minimum value of the angle.</param>
    /// <param name="aMax">The maximum value of the angle.</param>
    /// <returns></returns>
    public static float clampAngle(float aAngle, float aMin, float aMax)
    {
        if (aAngle < -360)
            aAngle += 360;
        if (aAngle > 360)
            aAngle -= 360;
        return Mathf.Clamp(aAngle, aMin, aMax);


    }

    /// <summary>
    /// Moves a value from start to the end based on speed given.
    /// </summary>
    /// <param name="aStart">The starting value.</param>
    /// <param name="aEnd">The target value.</param>
    /// <param name="aSpeed">The speed to move at. (This is clamped to 0 to 1 value).</param>
    /// <returns></returns>
    public static float exponentialEase(float aStart, float aEnd, float aSpeed)
    {
        float diff = aEnd - aStart;

        diff *= Mathf.Clamp(aSpeed, 0.0f, 1.0f);

        return diff + aStart;
    }

    /// <summary>
    /// Moves a value from start to the end based on speed given.
    /// </summary>
    /// <param name="aStart">The starting value.</param>
    /// <param name="aEnd">The target value.</param>
    /// <param name="aSpeed">The speed to move at. (This is clamped to 0 to 1 value).</param>
    /// <returns></returns>
    public static Vector3 exponentialEase(Vector3 aStart, Vector3 aEnd, float aSpeed)
    {
        Vector3 diff = aEnd - aStart;

        diff *= Mathf.Clamp(aSpeed, 0.0f, 1.0f);

        return diff + aStart;
    }
}
