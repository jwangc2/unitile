using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlButton : ControlItem {

    public ButtonType buttonType;
    public Sprite sprUp;
    public Sprite sprDown;
    public ControlDisplay display;

    private Image thisImage;
    private string calibText;

    void Start()
    {
        calibText = "PRESS ANY BUTTON TO MAP IT TO THE [" + buttonType.ToString().ToUpper() + "] BUTTON.";
        thisImage = this.GetComponent<Image>();
    }

    void Update()
    {
        int g = ControlManager.instance.GetCurrentGamepad();
        if (calibStep == 0)
        {
            int getBtn = ControlManager.instance.GetButton();
            if (getBtn != -1)
            {
                int btnID = getBtn;
                InputManager.instance.MapButton(g, buttonType, btnID);
                calibStep = -1;
                ControlManager.instance.ReleaseFocus(this);
                display.ResetText();
            }
        }

        bool pressed = InputManager.instance.GetButton(g, buttonType);
        if (pressed)
        {
            thisImage.sprite = sprDown;
        }
        else
        {
            thisImage.sprite = sprUp;
        }
    }

    public override bool CanUnfocus()
    {
        return calibStep < 0;
    }

    public override void UpdateMapping()
    {
        int g = ControlManager.instance.GetCurrentGamepad();
        InputManager.instance.UpdateMappingButton(g, buttonType);
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
                display.SetText(calibText);
                Input.ResetInputAxes();
            }
        }
    }
}
