using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public TriggerCheck groundCheck;
    public float hspd = 10;
    public string hAxis;
    public string jumpButton;
    public string attackButton;
    private Rigidbody2D rig;
    private Animator anim;
    private float h;

	// Use this for initialization
	void Start () {
        rig = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetInteger("h", (int)h);
        anim.SetBool("OnGround", OnGround());
        anim.SetBool("Attack", Input.GetButton(attackButton));
	}

    void FixedUpdate() {
        h = Input.GetAxisRaw(hAxis);
        if (h != 0)
        {
            transform.localScale = new Vector3(h, transform.localScale.y);
        }

        if (OnGround())
        {
            // On the ground
            if (h != 0)
            {
                rig.velocity = new Vector2(rig.gravityScale * h * hspd, rig.velocity.y);
            }

            if (Input.GetButtonDown(jumpButton)) {
                rig.AddForce(new Vector2(0, rig.gravityScale * 3.5f), ForceMode2D.Impulse);
            }
        }
        else
        {
            float dir = Mathf.Sign(rig.velocity.x);
            if (h != 0 && h != dir)
            {
                rig.velocity = new Vector2(rig.gravityScale * dir * hspd * 0.5f, rig.velocity.y);
            }
        }

    }

    bool OnGround() {
        if (groundCheck) {
            return groundCheck.hit != null;
        }

        return false;
    }

}
