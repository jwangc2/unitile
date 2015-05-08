using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Thrust.Singleton;

public enum JoystickType {
    ControlStickH, ControlStickV, DPadH, DPadV, CStickH, CStickV
}

public enum ButtonType {
    A, B, X, Y, Z, LT, RT, Start
}

public struct JoyValue{
    public int joyID;
    public bool inverted;

    public JoyValue(int joyID)
    {
        this.joyID = joyID;
        this.inverted = false;
    }

    public JoyValue(int joyID, bool inverted)
    {
        this.joyID = joyID;
        this.inverted = inverted;
    }
}

public class InputManager : PersistentSingleton<InputManager> {

    public int numGamepads;
    private Dictionary<JoystickType, JoyValue>[] joyMaps;
    private Dictionary<ButtonType, int>[] btnMaps;

    private JoystickType[] joyTypes;
    private ButtonType[] buttonTypes;

    void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        ResetInputs();
    }

    public void ResetInputs()
    {
        // Define empty dictionaries
        joyMaps = new Dictionary<JoystickType, JoyValue>[numGamepads];
        btnMaps = new Dictionary<ButtonType, int>[numGamepads];

        // Define a list of the types to iterate over
        joyTypes = (JoystickType[])Enum.GetValues(typeof(JoystickType));
        buttonTypes = (ButtonType[])Enum.GetValues(typeof(ButtonType));

        // Default Joystick Mapping
        Dictionary<JoystickType, JoyValue> defaultJoyMap = new Dictionary<JoystickType, JoyValue>();
        
        defaultJoyMap.Add(JoystickType.ControlStickH, new JoyValue(1));
        defaultJoyMap.Add(JoystickType.ControlStickV, new JoyValue(2, true));        
        defaultJoyMap.Add(JoystickType.DPadH, new JoyValue(7));        
        defaultJoyMap.Add(JoystickType.DPadV, new JoyValue(8));        
        defaultJoyMap.Add(JoystickType.CStickH, new JoyValue(6));        
        defaultJoyMap.Add(JoystickType.CStickV, new JoyValue(3, true));

        // Default Button Mapping
        Dictionary<ButtonType, int> defaultBtnMap = new Dictionary<ButtonType, int>();

        defaultBtnMap.Add(ButtonType.A, 1);
        defaultBtnMap.Add(ButtonType.B, 2);
        defaultBtnMap.Add(ButtonType.X, 0);
        defaultBtnMap.Add(ButtonType.Y, 3);
        defaultBtnMap.Add(ButtonType.Z, 7);
        defaultBtnMap.Add(ButtonType.LT, 4);
        defaultBtnMap.Add(ButtonType.RT, 5);
        defaultBtnMap.Add(ButtonType.Start, 9);


        // Make shallow copies across the board
        for (int g = 0; g < numGamepads; g ++)
        {
            joyMaps[g] = new Dictionary<JoystickType, JoyValue>(defaultJoyMap);
            foreach (JoystickType jt in joyTypes)
            {
                string key = FormatJoyKey(g + 1, jt);
                if (PlayerPrefs.HasKey(key))
                {
                    Debug.Log("Read [" + key + "]");
                    int dir = 1;
                    if (PlayerPrefs.HasKey(key + "Dir"))
                    {
                        dir = PlayerPrefs.GetInt(key + "Dir");
                    }

                    (joyMaps[g])[jt] = new JoyValue(PlayerPrefs.GetInt(key), dir < 0);
                }
            }

            btnMaps[g] = new Dictionary<ButtonType, int>(defaultBtnMap);
            foreach (ButtonType bt in buttonTypes)
            {
                string key = FormatBtnKey(g + 1, bt);
                if (PlayerPrefs.HasKey(key))
                {
                    (btnMaps[g])[bt] = PlayerPrefs.GetInt(key);
                }
            }
        }
    }

    /* Handles the internal naming of axes */
    public string FormatJoy(int gamepadID, JoystickType joyKey)
    {
        JoyValue jv = (joyMaps[gamepadID - 1])[joyKey];
        return FormatJoy(gamepadID, jv.joyID, jv.inverted);
    }

    private string FormatJoy(int gamepadID, int joyID, bool inverted)
    {
        return "myPad" + gamepadID + "J" + joyID + (inverted ? "-" : "+");
    }

    public string FormatBtn(int gamepadID, ButtonType btnKey)
    {
        return "myPad" + gamepadID + "B" + (btnMaps[gamepadID - 1])[btnKey];
    }

    private string FormatBtn(int gamepadID, int btnID)
    {
        return "myPad" + gamepadID + "B" + btnID;
    }

    private string FormatJoyKey(int gamepadID, JoystickType joyKey)
    {
        return "G" + gamepadID + "J" + joyKey.ToString();
    }

    private string FormatBtnKey(int gamepadID, ButtonType btnKey)
    {
        return "G" + gamepadID + "B" + btnKey.ToString();
    }

    /* Mutators that will edit the joymaps, and the player prefs respectively */
    public void MapJoystick(int gamepadID, JoystickType joyKey, int joyID, bool inverted)
    {
        (joyMaps[gamepadID - 1])[joyKey] = new JoyValue(joyID, inverted);
    }

    public void MapButton(int gamepadID, ButtonType btnKey, int btnID)
    {
        (btnMaps[gamepadID - 1])[btnKey] = btnID;

    }

    public void UpdateMappingJoystick(int gamepadID, JoystickType joyKey)
    {
        JoyValue joy = (joyMaps[gamepadID - 1])[joyKey];
        string key = FormatJoyKey(gamepadID, joyKey);
        PlayerPrefs.SetInt(key, joy.joyID);
        PlayerPrefs.SetInt(key + "Dir", (joy.inverted ? -1 : 1));
        Debug.Log("Set [" + key + "]");
    }

    public void UpdateMappingButton(int gamepadID, ButtonType btnKey)
    {
        int btnID = (btnMaps[gamepadID - 1])[btnKey];
        string key = FormatBtnKey(gamepadID, btnKey);
        PlayerPrefs.SetInt(key, btnID);
    }

    public JoyValue GetJoyValue(int gamepadID, JoystickType joyKey)
    {
        return (joyMaps[gamepadID - 1])[joyKey];
    }

    public int GetBtnID(int gamepadID, ButtonType btnKey)
    {
        return (btnMaps[gamepadID - 1])[btnKey];
    }

    /* Series of accessors that mimic the Input class */
    public float GetAxisRaw(int gamepadID, JoystickType joyKey)
    {
        JoyValue jv = GetJoyValue(gamepadID, joyKey);
        return GetAxisRaw(gamepadID, jv.joyID, jv.inverted);
    }

    public float GetAxis(int gamepadID, JoystickType joyKey)
    {
        JoyValue jv = GetJoyValue(gamepadID, joyKey);
        return GetAxis(gamepadID, jv.joyID, jv.inverted);
    }

    public bool GetButton(int gamepadID, ButtonType btnKey)
    {
        return GetButton(gamepadID, GetBtnID(gamepadID, btnKey));
    }

    public bool GetButtonDown(int gamepadID, ButtonType btnKey)
    {
        return GetButtonDown(gamepadID, GetBtnID(gamepadID, btnKey));
    }

    public bool GetButtonUp(int gamepadID, ButtonType btnKey)
    {
        return GetButtonUp(gamepadID, GetBtnID(gamepadID, btnKey));
    }

    /* Handles ALL inputs, not just the internal GC style */
    public float GetAxisRaw(int gamepadID, int joyID, bool inverted)
    {
        return Input.GetAxisRaw(FormatJoy(gamepadID, joyID, inverted));
    }
    
    public float GetAxis(int gamepadID, int joyID, bool inverted)
    {
        return Input.GetAxis(FormatJoy(gamepadID, joyID, inverted));
    }
    
    public bool GetButton(int gamepadID, int btnID)
    {
        return Input.GetButton(FormatBtn(gamepadID, btnID));
    }
    
    public bool GetButtonDown(int gamepadID, int btnID)
    {
        return Input.GetButtonDown(FormatBtn(gamepadID, btnID));
    }
    
    public bool GetButtonUp(int gamepadID, int btnID)
    {
        return Input.GetButtonUp(FormatBtn(gamepadID, btnID));
    }
}