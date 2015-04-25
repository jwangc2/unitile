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
        GUILayout.Label("Grid Transparency");
        alphaFV = EditorGUILayout.Slider(alphaFV, 0, 1);
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
        
        if (evcurrent.type == EventType.mouseUp) {
            Transform sel = Selection.activeTransform;
            if (sel)
            {
                // Snap to place only if it has a sprite
                if (sel.GetComponent<SpriteRenderer>())
                {
                    float sx = Mathf.Floor(sel.position.x / gridSize) * gridSize;
                    float sy = Mathf.Floor(sel.position.y / gridSize) * gridSize;
                    sel.transform.position = new Vector3(sx, sy);
                }
            }
        }
    }
}
