using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlDisplay : MonoBehaviour {

    public Text display;
    private string defaultText = null;

	// Use this for initialization
	void Start () {
        SetDefault();
	}
	
	public void SetText(string newText)
    {
        SetDefault();
        display.text = newText;
    }

    public void ResetText()
    {
        SetDefault();
        display.text = defaultText;
    }

    void SetDefault()
    {
        if (defaultText == null && display)
        {
            defaultText = display.text;
        }
    }
}
