using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Thrust.Singleton;

public class ControlManager : Singleton<ControlManager> {

    public ControlItem[] controlItems;
    public int maxAxes = 10;
    public int maxButtons = 10;
    public ControlDisplay display;
    public StandaloneInputModule module;

    private ControlItem focusItem;
    public int currentGamepad;
    private int setupStep;
    private List<int> joyIgnore;

	// Use this for initialization
	void Start () {
        focusItem = null;
        currentGamepad = 1;
        setupStep = 0;
        module.gameObject.SetActive(false);
        display.SetText("PLEASE RELEASE ALL AXIS STICKS AND PRESS ANY BUTTON.");
        joyIgnore = new List<int>();
	}

    void Update() {
        if (setupStep == 0)
        {
            int btnID = GetButton();
            if (btnID != -1)
            {
                joyIgnore = GetJoystickAll();
                setupStep = -1;
                module.gameObject.SetActive(true);
                display.ResetText();
                Input.ResetInputAxes();
            }
        }
    }

    public bool RequestFocus(ControlItem item)
    {
        bool success = false;
        if (setupStep == -1)
        {
            if (focusItem)
            {
                if (focusItem.CanUnfocus())
                {
                    focusItem.calibStep = -1;
                    focusItem = item;
                    success = true;
                }
            }
            else
            {
                focusItem = item;
                success = true;
            }
        }

        if (success)
        {
            module.gameObject.SetActive(false);
        }

        return success;
    }

    public void ReleaseFocus(ControlItem item)
    {
        item.calibStep = -1;
        focusItem = null;
        module.gameObject.SetActive(true);
    }

    public List<int> GetJoystickAll()
    {
        List<int> found = new List<int>();

        // Check only one joystick
        int g = currentGamepad;
        
        // Filter out baddies
        float thresh = 0.1f;
        
        // Gotta check 'em all
        for (int j = 1; j <= maxAxes; j ++)
        {
            float get = Mathf.Abs(InputManager.instance.GetAxisRaw(g, j, false));
            if (get > thresh)
            {
                found.Add(j);
            }
        }
        
        return found;
    }

    public int GetJoystick()
    {
        // Check only one joystick
        int g = currentGamepad;
        
        // Sort out the strongest input
        int best = -1;
        float strength = 0.1f;
        
        // Gotta check 'em all
        for (int j = 1; j <= maxAxes; j ++)
        {
            if (joyIgnore.Contains(j))
                continue;
            
            float get = Mathf.Abs(InputManager.instance.GetAxisRaw(g, j, false));
            if (get > strength)
            {
                best = j;
                strength = get;
                
            }
        }
        
        return best;
    }
    
    public int GetButton()
    {
        // Check only one joystick
        int g = currentGamepad;
        int best = -1;
        
        // Gotta check 'em all
        for (int b = 0; b < maxButtons; b ++)
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

    public ControlItem GetFocus()
    {
        return focusItem;
    }

    public int GetCurrentGamepad()
    {
        return currentGamepad;
    }

    public void UpdateControls()
    {
        foreach (ControlItem item in controlItems)
        {
            item.UpdateMapping();
        }
    }

    public void ResetControls()
    {
        InputManager.instance.ResetInputs();
    }

}
