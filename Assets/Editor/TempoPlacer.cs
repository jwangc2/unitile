using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlacerScript))] 
public class TempoPlacer : Editor {
	
	GameObject thingToPlace = null;


	// Use this for initialization

	void OnSceneGUI () {
		PlacerScript script = (PlacerScript)target;

		float snapDist = script.SnapDistance;

		Vector3 eventMousePosition = Event.current.mousePosition;
		eventMousePosition.y = -eventMousePosition.y;

		//if (Event.current.type == EventType.MouseDown) Debug.Log ("Fiskelifisk");
		Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y-40, 0.0f));
		if (mouseRay.direction.y < 0.0f)
		{
			float t = -mouseRay.origin.y / mouseRay.direction.y;
			Vector3 mouseWorldPos = mouseRay.origin + t * mouseRay.direction;
			mouseWorldPos.x = ((mouseWorldPos.x % snapDist) < snapDist*0.5f) ? mouseWorldPos.x - (mouseWorldPos.x % snapDist) : mouseWorldPos.x + (snapDist-(mouseWorldPos.x % snapDist));
			mouseWorldPos.z = ((mouseWorldPos.z % snapDist) < snapDist*0.5f) ? mouseWorldPos.z - (mouseWorldPos.z % snapDist) : mouseWorldPos.z + (snapDist-(mouseWorldPos.z % snapDist));

			script.MoveObject(mouseWorldPos);


		}
		if(Event.current.type == EventType.MouseDown) 
		{
			Event.current.Use();
			script.TriggerPlaced();
			DestroyImmediate(script);
		}
		if(Event.current.isKey)
		{
			Debug.Log (Event.current.keyCode);
			script.TriggerExitPlacing();
			Event.current.Use();
			DestroyImmediate(script.gameObject);
		}

	}
}
