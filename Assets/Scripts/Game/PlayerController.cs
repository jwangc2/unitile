using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public Vector2 groundMaskCenter;
    public Vector2 groundMaskSize;
    public string groundLayerName = "Ground";
    public float pixPerfThresh = 2f;
    public float hspd = 10;
    public string hAxis;
    public string jumpButton;
    public string attackButton;
    public string shieldButton;
    public int playerID = 1;

    public DamageMask[] attackSideMasks;
    public GameObject shieldGO;

    public AnimationClip idleClip;
    public AnimationClip jumpClip;
    public AnimationClip runClip;
    public AnimationClip shieldClip;

    private Rigidbody2D rig;
    private Animator anim;
    private float h;
    private int facing;
    private int groundLayer;
    private bool onGround;
    private bool tryShielding; // Whether the player is trying to shield
    private bool isShielding;  // Whether the shield is actually on

	// Use this for initialization
	void Start () {
        onGround = UpdateOnGround();
        facing = 1;
        isShielding = false;
        tryShielding = false;
        rig = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
        groundLayer = LayerMask.NameToLayer(groundLayerName);
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetInteger("h", (int)h);
        anim.SetBool("OnGround", onGround);
        anim.SetBool("Attack", InputManager.instance.GetButton(playerID, ButtonType.A));
        anim.SetBool("Shield", tryShielding);
	}

    void FixedUpdate() {
        AnimatorClipInfo[] info = anim.GetCurrentAnimatorClipInfo(0);

        // Determine input and current conditions
        // h = Input.GetAxisRaw(hAxis);
        h = InputManager.instance.GetAxisRaw(playerID, JoystickType.ControlStickH);
        bool hasControl = true;

        // Essentially a control lock
        if (h != 0)
            hasControl = (ClipMatches(info, idleClip) || ClipMatches(info, jumpClip) || ClipMatches(info, runClip));

        if (hasControl && h != 0)
        {
            facing = (int) Extensions.SignZero(h);
        }

        float dir = Mathf.Sign(rig.velocity.x);

        // Face in the direction going if input is provided
        if (h != 0 && hasControl)
            transform.localScale = new Vector3(facing, transform.localScale.y);

        // Ground-based moves
        if (onGround)
        {
            // Run / Walk
            if (h != 0 && hasControl)
                rig.velocity = new Vector2(rig.gravityScale * h * hspd, rig.velocity.y);

            // Jump
            if (InputManager.instance.GetButton(playerID, ButtonType.X) && hasControl) {
                rig.AddForce(new Vector2(0, rig.gravityScale * 8f), ForceMode2D.Impulse);
            }
            else if (InputManager.instance.GetButton(playerID, ButtonType.RT))
            {
                if (hasControl)
                {
                    //else if (Input.GetButton(shieldButton) && hasControl) {
                    tryShielding = true;
                }
            }
            else
            {
                tryShielding = false;
            }
        }
        else
        {
            // Control movemement mid-air
            dir = Extensions.SignZero(rig.velocity.x);
            if (h != 0 && h != dir && dir != 0 && hasControl)
            {
                rig.velocity = new Vector2(rig.velocity.x * 0.8f, rig.velocity.y);
            }
        }

        // Ground-specific collisions
        UpdateOnGround();
    }

    bool UpdateOnGround() {
        // Ensure that the rig already exists...
        if (rig)
        {
            // Settings
            LayerMask groundMask = 1 << groundLayer;
            float vy = rig.velocity.y * Time.fixedDeltaTime;
            float dist = Mathf.Abs(vy);

            // Pixel-perfect vertical collision
            if (vy * -1f > pixPerfThresh && dist > 0)
            {
                Vector2 pos = (Vector2)transform.position;
                RaycastHit2D hitRay = Physics2D.BoxCast(pos, groundMaskSize, 0f, Vector2.up * -1f, dist, groundMask);
                if (hitRay.transform)
                {
                    transform.position = new Vector2(transform.position.x, hitRay.point.y + 1f);
                    rig.velocity = new Vector2(rig.velocity.x, 0f);
                }
                onGround = hitRay.transform;
            }
            else
            {
                // Check if we are currently touching the ground
                Vector2 pos = (Vector2)transform.position + groundMaskCenter;
                Vector2 halfSize = groundMaskSize * 0.5f;
                Collider2D hitCol = Physics2D.OverlapArea(pos - halfSize, pos + halfSize, groundMask);
                onGround = hitCol;
            }

        }

        return onGround;

    }

    // Method for reacting to damage-mask collisions. This is initiated and registered by the damage-mask itself.
    public void React(DamageMask dm)
    {
        Debug.LogWarning("Hit!");
        float dir = Mathf.Sign(transform.position.x - dm.transform.position.x);
        float impulseMag = dm.impulseMag;
        Vector2 fdir = dm.direction.normalized;

        if (isShielding) {
            fdir = Vector2.right;
            impulseMag = 0.5f * rig.mass;
        }


        fdir.x = fdir.x * dir;
        rig.AddForce(fdir * impulseMag * rig.gravityScale, ForceMode2D.Impulse);
    }

    // Determine whether a clip matches one in the clip info (used for checking the current state)
    bool ClipMatches(AnimatorClipInfo[] info, AnimationClip check)
    {
        foreach (AnimatorClipInfo clipInfo in info)
        {
            if (clipInfo.clip == check)
                return true;
        }
        return false;
    }

    void EnableShield()
    {
        SetShield(true);
    }

    void DisableShield()
    {
        SetShield(false);
    }

    void SetShield(bool state)
    {
        if (!isShielding && isShielding != state)
        {
            rig.velocity = Vector2.zero;
        }
        isShielding = state;
        if (shieldGO)
            shieldGO.SetActive(tryShielding);
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
