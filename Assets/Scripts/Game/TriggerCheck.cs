using UnityEngine;
using System.Collections;

public class TriggerCheck : MonoBehaviour {

    public Collider2D hit = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        hit = other;
    }

    void OnTriggerStay2D(Collider2D other) {
        hit = other;
    }

    void OnTriggerExit2D(Collider2D other) {
        hit = null;
    }
}
