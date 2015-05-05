using UnityEngine;
using System.Collections;
using Thrust.Singleton;

public class ControlManager : Singleton<ControlManager> {

    public ControlItem[] controlItems;
    private ControlItem focusItem;
    private int currentGamepad;

	// Use this for initialization
	void Start () {
        focusItem = null;
        currentGamepad = 1;
	}

    public bool RequestFocus(ControlItem item)
    {
        bool success = false;
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

        return success;
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

}
