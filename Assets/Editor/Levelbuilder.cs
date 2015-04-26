using UnityEditor;
using UnityEngine;
using System.Collections;


// This code is holed and buggy and stripped down
// Some of it might be from some random place online
// I tried adding comments but there are alot of lose ends here

public class Levelbuilder : EditorWindow
{
	GameObject testObj;
	string dragDropIdentifier;
	public bool isCreating = false;

	private Texture2D currentAssetShow;

	[MenuItem("Window/LevelBuilder")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(Levelbuilder));

	}

	void OnFocus() 
	{

		//THis is to fill in the preview when no game object has been selected

		currentAssetShow = new Texture2D(100, 100, TextureFormat.ARGB32, false);
		int y = 0;
		while (y < currentAssetShow.height) {
			int x = 0;
			while (x < currentAssetShow.width) {
				Color color = Color.gray;
				currentAssetShow.SetPixel(x, y, color);
				++x;
			}
			++y;
		}
		currentAssetShow.Apply();
	}

	void OnGUI()
	{
		//For testing object previews I just draw what object is selected here
		testObj = EditorGUILayout.ObjectField(testObj, typeof(GameObject), true) as GameObject;
		if(testObj != null)
		{
			currentAssetShow = AssetPreview.GetAssetPreview(testObj);
		}

		//This is for checking event type when testing
		//EditorGUILayout.LabelField(Event.current.type.ToString ());


		Rect dndRect = new Rect(20,110,100,100);
		EditorGUI.DrawPreviewTexture(dndRect,currentAssetShow);
		//If I am clicking in the window and it is within the preview rect. Since this is not a button, you have to test yourself if mouse is being clicked and if it is within the preview.
		if (Event.current.type == EventType.MouseDown && dndRect.Contains (Event.current.mousePosition) && !isCreating) 
		{
			GameObject newObj = PrefabUtility.InstantiatePrefab(testObj) as GameObject;

			//Adds the script that is basically just a pointer for the editor to run the TempoPlacer.cs script and let you place in hte viewport
			PlacerScript placer = newObj.AddComponent<PlacerScript>();
			placer.SnapDistance = 1f;
			placer.OnExitPlacing += ChangeIsCreating;
			placer.OnPlacing += ChangeIsCreating;
			ChangeIsCreating();
			//SUBSCRIBE TO CANCEL EVENT HERE!!
			Selection.activeGameObject = newObj;
		}
	}

	//Debugging...

	void ChangeIsCreating() {
		isCreating = !isCreating;
		Debug.Log ("is creating is: " + isCreating);

	}


}