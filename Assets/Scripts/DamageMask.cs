using UnityEngine;
using System.Collections;

public class DamageMask : MonoBehaviour {

    public DamageType damageType = DamageType.Directional;
    public Vector2 direction = Vector2.right;
    public float impulseMag = 3;
    public PlayerController owner;

    public enum DamageType {
        Directional, Node
    };

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc)
        {
            if (pc != owner)
                pc.React(this);
        }
    }
}
