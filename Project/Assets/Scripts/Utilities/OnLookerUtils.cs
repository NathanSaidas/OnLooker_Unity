using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    public enum MouseButton
    {
        LEFT,
        RIGHT,
        MIDDLE,
        NONE
    }


    [ExecuteInEditMode()]
    public class OnLookerUtils
    {

        public static bool anyMouseButtonDown()
        {

            return anyMouseButtonDown(false);
        }
        public static bool anyMouseButtonDown(bool aContinous)
        {
            if (aContinous == true)
            {
                if (Input.GetMouseButton((int)MouseButton.LEFT))
                {
                    return true;
                }
                if (Input.GetMouseButton((int)MouseButton.MIDDLE))
                {
                    return true;
                }
                if (Input.GetMouseButton((int)MouseButton.RIGHT))
                {
                    return true;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown((int)MouseButton.LEFT))
                {
                    return true;
                }
                if (Input.GetMouseButtonDown((int)MouseButton.MIDDLE))
                {
                    return true;
                }
                if (Input.GetMouseButtonDown((int)MouseButton.RIGHT))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool pointInRect(Vector2 aPoint, Rect aRect)
        {
            return pointInRect(aPoint, aRect, true);
        }
        public static bool pointInRect(Vector2 aPoint, Rect aRect, bool yUp)
        {
            //Is the Y Direction Up or down
            if (yUp == true)
            {
                if (aPoint.x < aRect.x || aPoint.y < aRect.y || aPoint.x > aRect.x + aRect.width || aPoint.y > aRect.y + aRect.height)
                {
                    return false;
                }
            }
            else
            {
                if (aPoint.x < aRect.x || aPoint.y > aRect.y + aRect.height || aPoint.x > aRect.x + aRect.width || aPoint.y < aRect.y)
                {
                    return false;
                }
            }
            return true;
        }

        public static float clampAngle(float angle, float min, float max)
        {
            if (angle < -360.0f)
            {
                angle += 360.0f;
            }
            else if (angle > 360.0f)
            {
                angle -= 360.0f;
            }
            return Mathf.Clamp(angle, min, max);
        }

        
        
    }
}