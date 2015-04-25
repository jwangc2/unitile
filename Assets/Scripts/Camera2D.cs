using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

	Camera cam;
    public float scale = 1.0f;

	// Use this for initialization
    void Awake() {
        cam = GetComponent<Camera>();
        FixSize();
    }

	void Start() {
		FixSize();
	}
	
	// Update is called once per frame
	void Update() {
		FixSize();
	}

	void FixSize() {
		if (cam)
			cam.orthographicSize = (Screen.height / 2) / scale;
	}
}
