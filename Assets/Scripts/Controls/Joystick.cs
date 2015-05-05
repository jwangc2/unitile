using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Joystick : ControlItem {

    public JoystickType joyTypeH;
    public JoystickType joyTypeV;
    public RectTransform indicator;
    public float rad;
    public int joy1 = 1;
    public int dir1 = 1;
    public int joy2 = 2;
    public int dir2 = 1;
    private List<int> joyIgnore;

    private float prevTime;
    private bool firstStep;

    void Start()
    {
        prevTime = Time.time;
        joyIgnore = new List<int>();
        joyIgnore.Add(4);
        joyIgnore.Add(5);

        firstStep = true;
    }

    void Update()
    {
        int g = ControlManager.instance.GetCurrentGamepad();
        if (firstStep)
        {
            JoyValue jh = InputManager.instance.GetJoyValue(g, joyTypeH);
            joy1 = jh.joyID;
            dir1 = jh.dir;
            
            JoyValue jv = InputManager.instance.GetJoyValue(g, joyTypeV);
            joy2 = jv.joyID;
            dir2 = jv.dir;

            firstStep = false;
        }

        int getJoy;
        switch (calibStep)
        {
            case 0:
            getJoy = GetJoystick(10, joyIgnore);
            if (getJoy != -1 && Time.time - prevTime >= 3f)
            {
                joy1 = getJoy;
                dir1 = (int) Extensions.SignZero(InputManager.instance.GetAxisRaw(g, joy1));
                prevTime = Time.time;
                calibStep ++;
            }
            
            break;

            case 1:
            getJoy = GetJoystick(10, joyIgnore);
            if (getJoy != -1 && Time.time - prevTime >= 3f)
            {
                joy2 = getJoy;
                dir2 = (int) Extensions.SignZero(InputManager.instance.GetAxisRaw(g, joy2));
                prevTime = Time.time;
                calibStep = -1;
            }
            
            break;
        }

        if (indicator)
        {
            float h = InputManager.instance.GetAxis(g, joy1) * dir1;
            float v = InputManager.instance.GetAxis(g, joy2) * dir2;

            indicator.localPosition = new Vector3(h * rad, v * rad);
        }
    }

    private int GetJoystick(int maxJoy, List<int> ignore)
    {
        // Check only one joystick
        int g = ControlManager.instance.GetCurrentGamepad();

        // Sort out the strongest input
        int best = -1;
        float strength = 0.1f;

        // Gotta check 'em all
        for (int j = 1; j <= maxJoy; j ++)
        {
            if (ignore.Contains(j))
                continue;

            float get = Mathf.Abs(InputManager.instance.GetAxisRaw(g, j));
            if (get > strength)
            {
                best = j;
                strength = get;

            }
        }

        return best;
    }

    private int GetButton(int max)
    {
        // Check only one joystick
        int g = ControlManager.instance.GetCurrentGamepad();
        int best = -1;
        
        // Gotta check 'em all
        for (int b = 0; b < max; b ++)
        {
            bool get = InputManager.instance.GetButtonDown(g, b);
            if (get)
            {
                best = b;
                break;
            }
        }
        
        return best;
    }

	public override bool CanUnfocus()
    {
        return true;
    }

    public override void UpdateMapping()
    {
        int g = ControlManager.instance.GetCurrentGamepad();
        InputManager.instance.MapJoystick(g, joyTypeH, joy1, dir1);
        InputManager.instance.MapJoystick(g, joyTypeV, joy2, dir2);
    }

    public void OnClick()
    {
        if (calibStep == -1)
        {
            bool canMod = ControlManager.instance.RequestFocus(this);
            if (canMod)
            {
                // Enter into calibration
                calibStep = 0;
            }
        }
    }
}
