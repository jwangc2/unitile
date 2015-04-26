using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlacerScript : MonoBehaviour {

	public delegate void Placing();
	public event Placing OnExitPlacing;
	public event Placing OnPlacing;

	private float snapDist;
	public float SnapDistance {
		
		get {return snapDist;}
		set {if (value > 0) snapDist = value;}
	}


	// Use this for initialization
	public void TriggerExitPlacing () {
		OnExitPlacing();
	
	}
	public void TriggerPlaced () {
		OnPlacing();
		
	}
	
	// Update is called once per frame
	public void MoveObject (Vector3 placement) {
		transform.position = placement;
	}
}
