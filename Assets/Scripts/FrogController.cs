using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandingController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FrogController : MonoBehaviour {
    /** The Gameobject must have a trigger that acts as a landing spot sensor **/
    private LandingController lc;
    private Rigidbody2D rb2d;
    private Animator anim;
    private AudioSource asrc;

    public AudioClip chargeSound;
    private float startPitch = 0.1f;
    private float endPitch = 2.5f;
    private float volume = 0.3f;

    public float turnSpeed = 420.0f;
    public float maxJumpDistance = 8.0f;
    public float fallTime = 0.3f;

    private bool _charging;
    private float jumpVelocity;
    private float heldTime;
    private const float maxHoldTime = 1.5f;

    private float fallTimer;

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
        asrc = GetComponent<AudioSource>();
    }
	
	void Update () {
        Rotate();

        if (charging) {
            if (ReleaseCharge()) {
                if (lc.grounded || (lc.airborne && lc.CanLand())) {
                    Jump((heldTime / maxHoldTime) * jumpVelocity);
                }
                Uncharge();
            } else {
                if(asrc.isPlaying) {
                    asrc.Stop();
                }
                asrc.pitch = Mathf.Lerp(startPitch, endPitch, heldTime / maxHoldTime);
                asrc.PlayOneShot(chargeSound, volume);
                heldTime += Time.deltaTime;
                if (heldTime > maxHoldTime) {
                    Uncharge();
                }
            }
        } else {
            if (BeginCharge()) {
                charging = true;
            }
        }

        if(lc.airborne) {
            fallTimer -= Time.deltaTime;
            if(fallTimer <= 0) {
                if (lc.CanLand()) {
                    anim.SetTrigger("Land");
                } else {
                    lc.Sink();
                }
            }
        }
    }

    private void Uncharge() {
        heldTime = 0;
        if(charging) {
            if (asrc.isPlaying) {
                asrc.Stop();
            }
            asrc.pitch = Mathf.Lerp(startPitch, endPitch, 0.1f);
            asrc.PlayOneShot(chargeSound, volume);
        }
        charging = false;
    }

    private void Jump(float jumpStrength) {
        rb2d.velocity += jumpStrength * (Vector2)transform.up;
        lc.Unground();
        anim.SetTrigger("Jump");
        fallTimer = fallTime;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        Uncharge();
    }

    // --- Input Controls Implementation
    
    private void Rotate() {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private bool BeginCharge() {
        return Input.GetButtonDown("Fire1");
    }

    private bool ReleaseCharge() {
        return Input.GetButtonUp("Fire1");
    }
}
