using UnityEngine;
using System.Collections;

public abstract class ControlItem : MonoBehaviour {

    public int calibStep = -1;

    public abstract bool CanUnfocus();
    public abstract void UpdateMapping();
    public abstract void Calibrate();
}
