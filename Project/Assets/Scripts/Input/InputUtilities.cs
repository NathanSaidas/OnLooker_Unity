using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// InputUtilties houses constants for input strings as well as helper functions to parse input for a InputKey
/// </summary>
public static class InputUtilities 
{
    // begin constants
    // MOUSE
    public const string MOUSE_LEFT_BUTTON = "left";
    public const string MOUSE_RIGHT_BUTTON = "right";
    public const string MOUSE_MIDDLE_BUTTON = "middle";
    public const string MOUSE_X = "mousex";
    public const string MOUSE_Y = "mousey";
    public const string MOUSE_SCROLL_Y = "mousescrolly";

    // CONTROLLER
    public const string LEFT_STICK_X = "leftstickx";
    public const string LEFT_STICK_Y = "leftsticky";
    public const string RIGHT_STICK_X = "rightstickx";
    public const string RIGHT_STICK_Y = "rightsticky";
    public const string D_PAD_X = "dpadx";
    public const string D_PAD_Y = "dpady";
    public const string LEFT_TRIGGER = "lefttrigger";
    public const string RIGHT_TRIGGER = "righttrigger";
    public const string A = "a";
    public const string B = "b";
    public const string X = "x";
    public const string Y = "y";
    public const string LEFT_SHOULDER = "leftshoulder";
    public const string RIGHT_SHOULDER = "rightshoulder";
    public const string LEFT_STICK_IN = "leftstickin";
    public const string RIGHT_STICK_IN = "rightstickin";
    public const string BACK = "back";
    public const string START = "start";

    // KEYBOARD

    public const string SPACE = "space";
    public const string LEFT_CONTROL = "leftcontrol";
    public const string LEFT_SHIFT = "leftshift";
    public const string LEFT_ALT = "leftalt";
    public const string TAB = "tab";
    public const string ESC = "escape";
    public const string LEFT_ARROW = "leftarrow";
    public const string RIGHT_ARROW = "rightarrow";
    public const string UP_ARROW = "uparrow";
    public const string DOWN_ARROW = "downarrow";
    // end constants


    /// <summary>
    /// Returns true where the input string was successfully parsed.
    /// Returns false where the input string was a bad string
    /// </summary>
    /// <param name="aKey">The key to parse</param>
    /// <returns></returns>
    public static bool parseInputString(InputKey aKey)
    {
        if(aKey == null || aKey.owner == null)
        {
            return false;
        }
        //Make the string lower case
        string aInputString = aKey.input.ToLower();
        InputDevice aDeviceType = aKey.owner.deviceType;
        //Begin Parsing
        switch(aDeviceType)
        {
            case InputDevice.KEYBOARD:
                {
                    
                    if(aInputString == SPACE)
                    {
                        aKey.setAsKeyCode(KeyCode.Space);
                        return true;
                    }
                    else if(aInputString == LEFT_CONTROL)
                    {
                        aKey.setAsKeyCode(KeyCode.LeftControl);
                        return true;
                    }
                    else if(aInputString == LEFT_SHIFT)
                    {
                        aKey.setAsKeyCode(KeyCode.LeftShift);
                        return true;
                    }
                    else if(aInputString == LEFT_ALT)
                    {
                        aKey.setAsKeyCode(KeyCode.LeftAlt);
                        return true;
                    }
                    else if(aInputString == TAB)
                    {
                        aKey.setAsKeyCode(KeyCode.Tab);
                        return true;
                    }
                    else if(aInputString == ESC)
                    {
                        aKey.setAsKeyCode(KeyCode.Escape);
                        return true;
                    }
                    else if(aInputString == LEFT_ARROW)
                    {
                        aKey.setAsKeyCode(KeyCode.LeftArrow);
                        return true;
                    }
                    else if(aInputString == RIGHT_ARROW)
                    {
                        aKey.setAsKeyCode(KeyCode.RightArrow);
                        return true;
                    }
                    else if(aInputString == DOWN_ARROW)
                    {
                        aKey.setAsKeyCode(KeyCode.DownArrow);
                        return true;
                    }
                    else if(aInputString == UP_ARROW)
                    {
                        aKey.setAsKeyCode(KeyCode.UpArrow);
                        return true;
                    }
                    if (aInputString.Length > 0)
                    {
                        int keyID = aInputString[0];
                        if ((keyID >= 33 && keyID < 65) || (keyID >= 91 && keyID < 127))
                        {
                            aKey.setAsKeyCode((KeyCode)keyID);
                            return true;
                        }
                    }
                }
                break;
            case InputDevice.MOUSE:
                if(aInputString == MOUSE_LEFT_BUTTON)
                {
                    aKey.setAsMouseButton(MouseButton.LEFT);
                    return true;
                }
                else if(aInputString == MOUSE_RIGHT_BUTTON)
                {
                    aKey.setAsMouseButton(MouseButton.RIGHT);
                    return true;
                }
                else if(aInputString == MOUSE_MIDDLE_BUTTON)
                {
                    aKey.setAsMouseButton(MouseButton.MIDDLE);
                    return true;
                }
                else if(aInputString == MOUSE_X)
                {
                    aKey.setAsAxis(MOUSE_X);
                    return true;
                }
                else if(aInputString == MOUSE_Y)
                {
                    aKey.setAsAxis(MOUSE_Y);
                    return true;
                }
                else if(aInputString == MOUSE_SCROLL_Y)
                {
                    aKey.setAsAxis(MOUSE_SCROLL_Y);
                    return true;
                }

                break;

            case InputDevice.XBOX_CONTROLLER:
                if(aInputString == LEFT_STICK_X)
                {
                    aKey.setAsAxis(LEFT_STICK_X);
                    return true;
                }
                else if(aInputString == LEFT_STICK_Y)
                {
                    aKey.setAsAxis(LEFT_STICK_Y);
                    return true;
                }
                else if(aInputString == RIGHT_STICK_X)
                {
                    aKey.setAsAxis(RIGHT_STICK_X);
                    return true;
                }
                else if(aInputString == RIGHT_STICK_Y)
                {
                    aKey.setAsAxis(RIGHT_STICK_Y);
                    return true;
                }
                else if(aInputString == D_PAD_X)
                {
                    aKey.setAsAxis(D_PAD_X);
                    return true;
                }
                else if(aInputString == D_PAD_Y)
                {
                    aKey.setAsAxis(D_PAD_Y);
                    return true;
                }
                else if(aInputString == LEFT_TRIGGER)
                {
                    aKey.setAsAxis(LEFT_TRIGGER);
                    return true;
                }
                else if(aInputString == RIGHT_TRIGGER)
                {
                    aKey.setAsAxis(RIGHT_TRIGGER);
                    return true;
                }
                else if(aInputString == A)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == B)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == X)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == Y)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == LEFT_SHOULDER)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == RIGHT_SHOULDER)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == LEFT_STICK_IN)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if(aInputString == RIGHT_STICK_IN)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if (aInputString == BACK)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                else if (aInputString ==START)
                {
                    aKey.setAsKeyCode(getGamepadButton(aInputString, aKey.owner.player));
                    return true;
                }
                break;

            default:

                break;
        }
        return false;
    }


    /// <summary>
    /// Returns the correct keycode based on the name and the InputPlayer
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="aPlayer">The player whos  code t o get</param>
    /// <returns></returns>
    public static KeyCode getGamepadButton(string keyName, InputPlayer aPlayer)
    {
        switch (aPlayer)
        {
            case InputPlayer.PLAYER_1:
                if (keyName == A)
                {
                    return KeyCode.Joystick1Button0;
                }
                else if (keyName == B)
                {
                    return KeyCode.Joystick1Button1;
                }
                else if (keyName == X)
                {
                    return KeyCode.Joystick1Button2;
                }
                else if (keyName == Y)
                {
                    return KeyCode.Joystick1Button3;
                }
                else if (keyName == LEFT_SHOULDER)
                {
                    return KeyCode.Joystick1Button4;
                }
                else if (keyName == RIGHT_SHOULDER)
                {
                    return KeyCode.Joystick1Button5;
                }
                else if (keyName == LEFT_STICK_IN)
                {
                    return KeyCode.Joystick1Button8;
                }
                else if (keyName == RIGHT_STICK_IN)
                {
                    return KeyCode.Joystick1Button9;
                }
                else if (keyName == BACK)
                {
                    return KeyCode.Joystick1Button6;
                }
                else if (keyName == START)
                {
                    return KeyCode.Joystick1Button7;
                }
                break;
            case InputPlayer.PLAYER_2:
                if (keyName == A)
                {
                    return KeyCode.Joystick2Button0;
                }
                else if (keyName == B)
                {
                    return KeyCode.Joystick2Button1;
                }
                else if (keyName == X)
                {
                    return KeyCode.Joystick2Button2;
                }
                else if (keyName == Y)
                {
                    return KeyCode.Joystick2Button3;
                }
                else if (keyName == LEFT_SHOULDER)
                {
                    return KeyCode.Joystick2Button4;
                }
                else if (keyName == RIGHT_SHOULDER)
                {
                    return KeyCode.Joystick2Button5;
                }
                else if (keyName == LEFT_STICK_IN)
                {
                    return KeyCode.Joystick2Button8;
                }
                else if (keyName == RIGHT_STICK_IN)
                {
                    return KeyCode.Joystick2Button9;
                }
                else if (keyName == BACK)
                {
                    return KeyCode.Joystick2Button6;
                }
                else if (keyName == START)
                {
                    return KeyCode.Joystick2Button7;
                }
                break;
            case InputPlayer.PLAYER_3:
                if (keyName == A)
                {
                    return KeyCode.Joystick3Button0;
                }
                else if (keyName == B)
                {
                    return KeyCode.Joystick3Button1;
                }
                else if (keyName == X)
                {
                    return KeyCode.Joystick3Button2;
                }
                else if (keyName == Y)
                {
                    return KeyCode.Joystick3Button3;
                }
                else if (keyName == LEFT_SHOULDER)
                {
                    return KeyCode.Joystick3Button4;
                }
                else if (keyName == RIGHT_SHOULDER)
                {
                    return KeyCode.Joystick3Button5;
                }
                else if (keyName == LEFT_STICK_IN)
                {
                    return KeyCode.Joystick3Button8;
                }
                else if (keyName == RIGHT_STICK_IN)
                {
                    return KeyCode.Joystick3Button9;
                }
                else if (keyName == BACK)
                {
                    return KeyCode.Joystick3Button6;
                }
                else if (keyName == START)
                {
                    return KeyCode.Joystick3Button7;
                }
                break;
            case InputPlayer.PLAYER_4:
                if (keyName == A)
                {
                    return KeyCode.Joystick4Button0;
                }
                else if (keyName == B)
                {
                    return KeyCode.Joystick4Button1;
                }
                else if (keyName == X)
                {
                    return KeyCode.Joystick4Button2;
                }
                else if (keyName == Y)
                {
                    return KeyCode.Joystick4Button3;
                }
                else if (keyName == LEFT_SHOULDER)
                {
                    return KeyCode.Joystick4Button4;
                }
                else if (keyName == RIGHT_SHOULDER)
                {
                    return KeyCode.Joystick4Button5;
                }
                else if (keyName == LEFT_STICK_IN)
                {
                    return KeyCode.Joystick4Button8;
                }
                else if (keyName == RIGHT_STICK_IN)
                {
                    return KeyCode.Joystick4Button9;
                }
                else if (keyName == BACK)
                {
                    return KeyCode.Joystick4Button6;
                }
                else if (keyName == START)
                {
                    return KeyCode.Joystick4Button7;
                }
                break;
            case InputPlayer.ANY:
                if (keyName == A)
                {
                    return KeyCode.JoystickButton0;
                }
                else if (keyName == B)
                {
                    return KeyCode.JoystickButton1;
                }
                else if (keyName == X)
                {
                    return KeyCode.JoystickButton2;
                }
                else if (keyName == Y)
                {
                    return KeyCode.JoystickButton3;
                }
                else if (keyName == LEFT_SHOULDER)
                {
                    return KeyCode.JoystickButton4;
                }
                else if (keyName == RIGHT_SHOULDER)
                {
                    return KeyCode.JoystickButton5;
                }
                else if (keyName == LEFT_STICK_IN)
                {
                    return KeyCode.JoystickButton8;
                }
                else if (keyName == RIGHT_STICK_IN)
                {
                    return KeyCode.JoystickButton9;
                }
                else if (keyName == BACK)
                {
                    return KeyCode.JoystickButton6;
                }
                else if (keyName == START)
                {
                    return KeyCode.JoystickButton7;
                }
                break;
        }
        return 0;
    }
}
