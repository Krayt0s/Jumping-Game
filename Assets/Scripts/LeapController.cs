using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandingController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class LeapController : MonoBehaviour {
    /** The Gameobject must have a trigger that acts as a landing spot sensor **/
    private LandingController lc;
    private Rigidbody2D rb2d;
    private Animator anim;

    public float turnSpeed = 420.0f;
    public float maxJumpDistance = 8.0f;
    public float fallTime = 0.3f;

    private bool _charging;
    private float jumpVelocity;
    private float heldTime;
    private const float maxHoldTime = 1.5f;

    private float lastAIH;

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
        lc = GetComponent<LandingController>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (lc.grounded) {
            if(charging) {
                if (Input.GetButton("Jump")) {
                    heldTime += Time.deltaTime;
                    if (heldTime > maxHoldTime) {
                        charging = false;
                        heldTime = 0;
                    }
                } else if (Input.GetButtonUp("Jump")) {
                    Jump((heldTime / maxHoldTime) * jumpVelocity);
                    Uncharge();
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

    private void Uncharge() {
        heldTime = 0;
        charging = false;
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

    private void Jump(float jumpStrength) {
        rb2d.velocity += jumpStrength * (Vector2)transform.up;
        lc.Unground();
        anim.SetTrigger("Jump");
    }

    private void Land() {
        if(lc.Land()) {
            anim.SetTrigger("Land");
        } else {
            lc.Sink();
        }
    }

    private void Lose() {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        //Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D coll) {
        Uncharge();
    }
}
