using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class LeapController : MonoBehaviour {
    /** The Gameobject must have a trigger that acts as a landing spot sensor **/
    private Rigidbody2D rb2d;
    private Animator anim;

    public float turnSpeed = 420.0f;
    public float maxJumpDistance = 8.0f;
    public float fallTime = 0.3f;

    private bool _charging;
    private float jumpVelocity;
    private float heldTime;
    private const float maxHoldTime = 1.5f;

    private bool grounded = true;
    private Joint2D grounding;

    private float lastAIH;

    private GameObject landObject;

    private bool charging {
        get { return _charging; }
        set {
            _charging = value;
            anim.SetBool("Charging", _charging);
        }
    }

    void Awake() {
        heldTime = 0f;
        jumpVelocity = maxJumpDistance / fallTime;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if(grounded) {
            if(charging) {
                if (Input.GetButton("Jump")) {
                    heldTime += Time.deltaTime;
                    if (heldTime > maxHoldTime) {
                        charging = false;
                        heldTime = 0;
                    }
                } else if (Input.GetButtonUp("Jump")) {
                    Jump((heldTime / maxHoldTime) * jumpVelocity);
                    heldTime = 0;
                    charging = false;
                    Invoke("Land", fallTime);
                }
           } else {
               if (Input.GetButtonDown("Jump")) {
                   charging = true;
               }
            }
        }

        Rotate();
    }

    private void Rotate() {
        float ih = Input.GetAxis("Horizontal");
        float aih = Mathf.Abs(ih);
        if (aih > lastAIH || aih == 1f) {
            float rotation = Time.deltaTime * turnSpeed * -Mathf.Sign(ih);
            transform.Rotate(new Vector3(0, 0, rotation));
        }
        lastAIH = aih;
    }

    private void Unground() {
        if(grounding) {
            Destroy(grounding);
        }
        grounded = false;
    }

    private void Ground(GameObject ground) {
        grounding = ground.AddComponent<RelativeJoint2D>();
        grounding.connectedBody = rb2d;
        grounded = true;
    }

    private void Jump(float jumpStrength) {
        rb2d.velocity += jumpStrength * (Vector2)transform.up;
        Unground();
        anim.SetTrigger("Jump");
    }

    private void Land() {
        rb2d.velocity = Vector2.zero;
        if (landObject == null) {
            Lose();
        } else {
            Ground(landObject);
            anim.SetTrigger("Land");
        }
    }

    private void Lose() {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D coll) {
        Unground();
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if(coll.gameObject.layer == LayerMask.NameToLayer("Landable")) {
            landObject = coll.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D coll) {
        if(coll.gameObject == landObject) {
            landObject = null;
        }
    }
}
