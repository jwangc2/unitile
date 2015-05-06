using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlButton : ControlItem {

    public ButtonType buttonType;
    public int btnID = 1;
    public Sprite sprUp;
    public Sprite sprDown;

    private bool firstStep;
    private Image thisImage;

    void Start()
    {
        thisImage = this.GetComponent<Image>();
        firstStep = true;
    }

    void Update()
    {
        int g = ControlManager.instance.GetCurrentGamepad();

        if (firstStep)
        {
            btnID = InputManager.instance.GetBtnID(g, buttonType);
            firstStep = false;
        }

        bool pressed = InputManager.instance.GetButton(g, btnID);
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
        return true;
    }

    public override void UpdateMapping()
    {
        throw new System.NotImplementedException();
    }
}
