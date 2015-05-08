using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class StandaloneInputModuleExt : MonoBehaviour {

    public StandaloneInputModule module;
	
	// Update is called once per frame
	void Update () {
        module.horizontalAxis = InputManager.instance.FormatJoy(1, JoystickType.ControlStickH);
        module.verticalAxis = InputManager.instance.FormatJoy(1, JoystickType.ControlStickV);
        module.submitButton = InputManager.instance.FormatBtn(1, ButtonType.A);
        module.cancelButton = InputManager.instance.FormatBtn(1, ButtonType.B);
	}
}
