using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class UTWindow : EditorWindow {

    private int gridSizeFV;
    private int roomWidthFV;
    private int roomHeightFV;
    private float alphaFV;
    
	private int gridSize;
    private int roomWidth;
    private int roomHeight;
    private float alpha;
    private bool placePrefab;
    private Object prefabObj;
    private string prefabStatus = "Error: No prefab selected.";
    private GameObject tempPlaceGO;
    
    private float depth;
    private Vector3 p1;
    private Vector3 p2;
    private List<Vector3> pList;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/UTWindow")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(UTWindow));
    }

    void OnEnable() {
        if (EditorPrefs.HasKey("UT Grid Size")) {
            gridSizeFV = EditorPrefs.GetInt("UT Grid Size");
        } else {
            gridSizeFV = 128;
        }
        
        if (EditorPrefs.HasKey("UT Grid Alpha")) {
            alphaFV = EditorPrefs.GetFloat("UT Grid Alpha");
        } else {
            alphaFV = 1;
        }
        
        if (EditorPrefs.HasKey("UT Room Width")) {
            roomWidthFV = EditorPrefs.GetInt("UT Room Width");
        } else {
            roomWidthFV = 640;
        }
        
        if (EditorPrefs.HasKey("UT Room Height")) {
            roomHeightFV = EditorPrefs.GetInt("UT Room Height");
        } else {
            roomHeightFV = 480;
        }
        
        placePrefab = false;
        prefabObj = null;
        tempPlaceGO = null;
        depth = -10;
        pList = new List<Vector3>();
        UpdateFieldValues();
        UpdatePList();
    }

	void OnGUI() {
        roomWidthFV = EditorGUILayout.IntField("Room Width:", roomWidthFV);
        roomWidthFV = Mathf.Max(0, roomWidthFV);
        
        roomHeightFV = EditorGUILayout.IntField("Room Height:", roomHeightFV);
        roomHeightFV = Mathf.Max(0, roomHeightFV);
        
        gridSizeFV = EditorGUILayout.IntField("Grid Size:", gridSizeFV);
        gridSizeFV = Mathf.Max(gridSizeFV, 1);
        
        EditorGUILayout.LabelField("Grid Transparency");
        alphaFV = EditorGUILayout.Slider(alphaFV, 0, 1);
        
        placePrefab = EditorGUILayout.Toggle("Place Prefab:", placePrefab);
        
        Object newObj = EditorGUILayout.ObjectField("Instance:", prefabObj, typeof(GameObject), false);
        if (newObj != prefabObj) 
        {
            if (newObj == null)
            {
                prefabStatus = "Error: Select GO's with SpriteRenderer.";
            }
            else
            {
                GameObject GO = (GameObject)newObj;
                if (GO.GetComponent<SpriteRenderer>())
                {
                    prefabStatus = "Gucci.";
                    prefabObj = newObj;
                }
                else
                {
                    prefabStatus = "Error: Select GO's with SpriteRenderer.";
                }
            }
        }
        EditorGUILayout.LabelField(prefabStatus);
        
        bool update = GUILayout.Button("Update");
        if (update) {
            UpdateFieldValues();
            UpdatePList();
        }
        if(SceneView.onSceneGUIDelegate != this.OnSceneGUI)
        {
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }
	}
    
    void UpdateEditorPrefs()
    {
        EditorPrefs.SetFloat("UT Grid Alpha", alpha);
        EditorPrefs.SetInt("UT Grid Size", gridSize);
        EditorPrefs.SetInt("UT Room Width", roomWidth);
        EditorPrefs.SetInt("UT Room Height", roomHeight);
    }
    
    void UpdateFieldValues()
    {
        gridSize = gridSizeFV;
        alpha = alphaFV;
        roomWidth = roomWidthFV;
        roomHeight = roomHeightFV;
        
        UpdateEditorPrefs();
    }

    void UpdatePList()
    {
        pList = new List<Vector3>();
        for (int i = 0; i < Mathf.Ceil((float)roomWidth / gridSize); i ++)
        {
            pList.Add(new Vector3(i * gridSize, 0, depth));
            pList.Add(new Vector3(i * gridSize, -roomHeight, depth));
        }

        for (int n = 0; n < Mathf.Ceil((float)roomHeight / gridSize); n ++)
        {
            pList.Add(new Vector3(0, -n * gridSize, depth));
            pList.Add(new Vector3(roomWidth, -n * gridSize, depth));
        }
        
        pList.Add(new Vector3(roomWidth, 0, depth));
        pList.Add(new Vector3(roomWidth, -roomHeight, depth));
        pList.Add(new Vector3(0, -roomHeight, depth));
        pList.Add(new Vector3(roomWidth, -roomHeight, depth));
    }

    void OnDestroy() {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
    
    public Vector3 ScreenToWorld(Camera cam, Vector3 sp)
    {
        Vector3 place = cam.ScreenToWorldPoint(sp);
        place.y = place.y + 34;
        place.y = place.y * -1f;
        place.z = 0;
        
        return place;
    }
    
    public Vector3 SnapPos(Vector3 pos)
    {
        float sx = Mathf.Floor(pos.x / gridSize) * gridSize;
        float sy = Mathf.Floor(pos.y / gridSize) * gridSize;
        return new Vector3(sx, sy);
    }

    public void OnSceneGUI (SceneView scnView)
    {        
        GUI.depth = 1000;
        Handles.color = new Color(1, 1, 1, alpha);
        Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 16);
        if (pList != null)
        {
            for (int p = 0; p < pList.Count; p += 2)
            {
                Handles.DrawLine(pList[p], pList[p + 1]);
            }
        }
        
        Event evcurrent = Event.current;
        Camera cam = Camera.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        
        switch (evcurrent.type)
        {            
            case EventType.MouseDown:
                if (placePrefab && cam && !Selection.activeTransform)
                {
                    tempPlaceGO = PrefabUtility.InstantiatePrefab((GameObject)prefabObj) as GameObject;
                    Vector3 mp = (Vector3) evcurrent.mousePosition;
                    Vector3 place = ScreenToWorld(cam, mp);
                    tempPlaceGO.transform.position = SnapPos(place);
                    GameObject[] gos = new GameObject[] {tempPlaceGO};
                    Selection.objects = gos;
                }
                break;
            
            case EventType.MouseUp:
                foreach (GameObject go in Selection.objects)
                {
                    if (go.GetComponent<SpriteRenderer>())
                    {
                        go.transform.position = SnapPos(go.transform.position);
                    }
                }
                break;
        }
            
    }
}
