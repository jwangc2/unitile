using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlJoystick : ControlItem {

    public JoystickType joyTypeH;
    public JoystickType joyTypeV;
    public RectTransform indicator;
    public float rad;
    public ControlDisplay display;

    private float prevTime;
    private bool firstStep;
    private string[] calibText;

    void Start()
    {
        prevTime = Time.time;

        calibText = new string[2];
        calibText[0] = "MOVE THE STICK (TO MAP TO THE HORIZONTAL AXIS) TO THE RIGHT AND PRESS THE [SMASH] BUTTON.";
        calibText[1] = "MOVE THE STICK (TO MAP TO THE VERTICAL AXIS) UPWARDS AND PRESS THE [SMASH] BUTTON.";
    }

    void Update()
    {
        int g = ControlManager.instance.GetCurrentGamepad();
        int getJoy;
        //bool canCalib = Time.time - prevTime >= 3f;
        bool canCalib = InputManager.instance.GetButtonDown(g, ButtonType.A);
        switch (calibStep)
        {
            case 0:
            getJoy = ControlManager.instance.GetJoystick();
            if (getJoy != -1 && canCalib)
            {
                int joy1 = getJoy;
                int dir1 = (int) Extensions.SignZero(InputManager.instance.GetAxisRaw(g, joy1, false));
                InputManager.instance.MapJoystick(g, joyTypeH, joy1, dir1 < 0);
                prevTime = Time.time;
                calibStep ++;
                display.SetText(calibText[calibStep]);
            }
            
            break;

            case 1:
            getJoy = ControlManager.instance.GetJoystick();
            if (getJoy != -1 && canCalib)
            {
                int joy2 = getJoy;
                int dir2 = (int) Extensions.SignZero(InputManager.instance.GetAxisRaw(g, joy2, false));
                InputManager.instance.MapJoystick(g, joyTypeV, joy2, dir2 < 0);
                prevTime = Time.time;
                calibStep = -1;
                ControlManager.instance.ReleaseFocus(this);
                display.ResetText();
            }
            
            break;
        }

        if (indicator)
        {
            float h = InputManager.instance.GetAxis(g, joyTypeH);
            float v = InputManager.instance.GetAxis(g, joyTypeV);

            indicator.localPosition = new Vector3(h * rad, v * rad);
        }
    }

	public override bool CanUnfocus()
    {
        return calibStep < 0;
    }

    public override void UpdateMapping()
    {
        int g = ControlManager.instance.GetCurrentGamepad();
        InputManager.instance.UpdateMappingJoystick(g, joyTypeH);
        InputManager.instance.UpdateMappingJoystick(g, joyTypeV);
    }

    public override void Calibrate()
    {
        if (calibStep == -1)
        {
            bool canMod = ControlManager.instance.RequestFocus(this);
            if (canMod)
            {
                // Enter into calibration
                calibStep = 0;
                display.SetText(calibText[calibStep]);
                Input.ResetInputAxes();
            }
        }
    }
}
