using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public Vector2 groundMaskCenter;
    public Vector2 groundMaskSize;
    public string groundLayerName = "Ground";
    public float hspd = 10;
    public string hAxis;
    public string jumpButton;
    public string attackButton;

    public DamageMask[] attackSideMasks;

    private Rigidbody2D rig;
    private Animator anim;
    private float h;
    private int facing;
    private int groundLayer;
    private bool onGround;

	// Use this for initialization
	void Start () {
        onGround = UpdateOnGround();
        facing = 1;
        rig = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
        groundLayer = LayerMask.NameToLayer(groundLayerName);
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetInteger("h", (int)h);
        anim.SetBool("OnGround", onGround);
        anim.SetBool("Attack", Input.GetButton(attackButton));
	}

    void FixedUpdate() {
        h = Input.GetAxisRaw(hAxis);
        float dir = Mathf.Sign(rig.velocity.x);
        if (h != 0)
        {
            transform.localScale = new Vector3(h, transform.localScale.y);
        }

        if (onGround)
        {
            // On the ground
            if (h != 0)
            {
                rig.velocity = new Vector2(rig.gravityScale * h * hspd, rig.velocity.y);
            }

            if (Input.GetButtonDown(jumpButton)) {
                rig.AddForce(new Vector2(0, rig.mass * rig.gravityScale * 3.5f), ForceMode2D.Impulse);
            }
        }
        else
        {
            dir = Extensions.SignZero(rig.velocity.x);
            if (h != 0 && h != dir && dir != 0)
            {
                rig.velocity = new Vector2(rig.velocity.x * 0.8f, rig.velocity.y);
            }
        }

        dir = Extensions.SignZero(rig.velocity.x);
        if (dir != 0)
        {
            facing = (int)dir;
        }

        UpdateOnGround();
    }

    bool UpdateOnGround() {
        LayerMask groundMask = 1 << groundLayer;
        Vector2 pos = (Vector2)transform.position + groundMaskCenter;
        Vector2 halfSize = groundMaskSize * 0.5f;
        Collider2D hit = Physics2D.OverlapArea(pos - halfSize, pos + halfSize, groundMask);
        onGround = hit;
        return onGround;
    }

    public void React(DamageMask dm)
    {
        Debug.LogWarning("Hit!");
        float dir = Mathf.Sign(transform.position.x - dm.transform.position.x);
        Vector2 fdir = dm.direction.normalized;
        fdir.x = fdir.x * dir;
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundMaskCenter, new Vector3(groundMaskSize.x, groundMaskSize.y, 1f));
    }

}
