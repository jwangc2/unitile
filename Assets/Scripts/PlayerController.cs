using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public TriggerCheck groundCheck;
    public float hspd = 10;
    public string hAxis;
    public string jumpButton;
    public string attackButton;

    public DamageMask[] attackSideMasks;

    private Rigidbody2D rig;
    private Animator anim;
    private float h;
    private int facing;

	// Use this for initialization
	void Start () {
        facing = 1;
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
        float dir = Mathf.Sign(rig.velocity.x);
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
            if (h != 0 && h != dir)
            {
                rig.velocity = new Vector2(rig.gravityScale * dir * hspd * 0.5f, rig.velocity.y);
            }
        }

        dir = Mathf.Sign(rig.velocity.x);
        if (dir != 0)
        {
            facing = (int)dir;
        }

    }

    bool OnGround() {
        if (groundCheck) {
            return groundCheck.hit != null;
        }

        return false;
    }

    public void React(DamageMask dm)
    {
        Debug.LogWarning("Hit!");
        float dir = Mathf.Sign(transform.position.x - dm.transform.position.x);
        Vector2 fdir = dm.direction.normalized * dir + Vector2.up * 0.5f;
        rig.AddForce(fdir * dm.impulseMag * rig.gravityScale, ForceMode2D.Impulse);
    }

    void EnableAttackMask(int frame) {
        if (frame >= 0 && frame < attackSideMasks.Length)
        {
            DamageMask dm = attackSideMasks[frame];
            if (dm)
            {
                dm.gameObject.SetActive(true);
                dm.owner = this;
                //dm.direction = Vector2.right * facing;
            }
        }
    }

    void DisableAttackMask(int frame) {
        if (frame >= 0 && frame < attackSideMasks.Length)
        {
            DamageMask dm = attackSideMasks[frame];
            if (dm)
            {
                dm.gameObject.SetActive(false);
                dm.owner = this;
            }
        }
    }

    void DisableAttackAll() {
        foreach (DamageMask dm in attackSideMasks)
        {
            if (dm)
            {
                dm.gameObject.SetActive(false);
                dm.owner = this;
            }
        }
    }

}
