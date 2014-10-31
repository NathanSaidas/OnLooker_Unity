using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#region CHANGE LOG
/* October,30,2014 - Nathan Hanlan, Added a ParseToWords method.
 * 
 */
#endregion

public static class Utilities
{

    /// <summary>
    /// Clamps an angle between -360 degrees and 360 degrees and keeps it within the min and max ranges.
    /// </summary>
    /// <param name="aAngle">The angle to clamp.</param>
    /// <param name="aMin">The minimum value of the angle.</param>
    /// <param name="aMax">The maximum value of the angle.</param>
    /// <returns></returns>
    public static float ClampAngle(float aAngle, float aMin, float aMax)
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
    public static float ExponentialEase(float aStart, float aEnd, float aSpeed)
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
    public static Vector3 ExponentialEase(Vector3 aStart, Vector3 aEnd, float aSpeed)
    {
        Vector3 diff = aEnd - aStart;

        diff *= Mathf.Clamp(aSpeed, 0.0f, 1.0f);

        return diff + aStart;
    }
    /// <summary>
    /// Parses a string for all of its words, uses a space character as a separator.
    /// </summary>
    /// <param name="aContext">The string to parse</param>
    /// <returns>A list of words from the context. </returns>
    public static List<string> ParseToWords(string aContext, bool aToLower)
    {
        List<string> words = new List<string>();
        if (aContext.Length == 0)
        {
            return words;
        }
        if(aToLower == true)
        {
            aContext = aContext.ToLower();
        }
        if (aContext.Length == 1)
        {
            words.Add(aContext);
            return words;
        }
        int startIndex = -1;
        int endIndex = 0;
        int spaceIndex = 0;

        while (aContext.Length > 0)
        {
            startIndex = -1;
            endIndex = 0;
            spaceIndex = 0;
            if (aContext.Length == 1)
            {
                words.Add(aContext);
                break;
            }

            for (int i = 0; i < aContext.Length; i++)
            {
                if (startIndex == -1 && aContext[i] != ' ')
                {
                    startIndex = i;
                    continue;
                }
                if (startIndex != -1 && aContext[i] == ' ')
                {
                    spaceIndex = i;
                    break;
                }
                if (i == aContext.Length - 1)
                {
                    spaceIndex = aContext.Length;
                    break;
                }
            }

            endIndex = spaceIndex - 1;
            int wordLength = Mathf.Clamp(endIndex - startIndex + 1, 0, aContext.Length);
            int removeLength = Mathf.Clamp(spaceIndex - startIndex + 1, 0, aContext.Length);
            string word = aContext.Substring(startIndex, wordLength);
            aContext = aContext.Remove(startIndex, removeLength);
            words.Add(word);
        }


        return words;
    }

    /// <summary>
    /// Parses a string for all of its words using the specified separtor.
    /// </summary>
    /// <param name="aContext">The string to parse</param>
    /// <param name="aSeparator">The value which determines a space in the sentence</param>
    /// <returns>A list of words from the context. </returns>
    public static List<string> ParseToWords(string aContext,char aSeparator, bool aToLower)
    {
        List<string> words = new List<string>();
        if (aContext.Length == 0)
        {
            return words;
        }
        if(aToLower == true)
        {
            aContext = aContext.ToLower();
        }
        if (aContext.Length == 1)
        {
            words.Add(aContext);
            return words;
        }
        int startIndex = -1;
        int endIndex = 0;
        int spaceIndex = 0;

        while (aContext.Length > 0)
        {
            startIndex = -1;
            endIndex = 0;
            spaceIndex = 0;
            if (aContext.Length == 1)
            {
                words.Add(aContext);
                break;
            }

            for (int i = 0; i < aContext.Length; i++)
            {
                if (startIndex == -1 && aContext[i] != aSeparator)
                {
                    startIndex = i;
                    continue;
                }
                if (startIndex != -1 && aContext[i] == aSeparator)
                {
                    spaceIndex = i;
                    break;
                }
                if (i == aContext.Length - 1)
                {
                    spaceIndex = aContext.Length;
                    break;
                }
            }

            endIndex = spaceIndex - 1;
            int wordLength = Mathf.Clamp(endIndex - startIndex + 1, 0, aContext.Length);
            int removeLength = Mathf.Clamp(spaceIndex - startIndex + 1, 0, aContext.Length);
            string word = aContext.Substring(startIndex, wordLength);
            aContext = aContext.Remove(startIndex, removeLength);
            words.Add(word);
        }


        return words;
    }
}
