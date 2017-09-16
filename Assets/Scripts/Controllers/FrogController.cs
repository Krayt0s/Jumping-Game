using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collector))]
[RequireComponent(typeof(LandingController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FrogController : MonoBehaviour {
    /** The Gameobject must have a trigger that acts as a landing spot sensor **/
    private LandingController lc;
    private Rigidbody2D rb2d;
    private Animator anim;
    private AudioSource asrc;

    public GameObject respawnPoint;

    public AudioClip eatSound;

    public AudioClip chargeSound;
    private float startPitch = 0.1f;
    private float endPitch = 2.5f;
    private float volume = 0.6f;

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

        SubscribeEvents();
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
                lc.TryLand();
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

    private void Respawn() {
        if(lc.submerged) {
            lc.Surface();
        }
        transform.position = respawnPoint.transform.position;
        // Give time for collision to check
        Invoke("PostRespawn", 0.1f);
    }
    private void PostRespawn() {
        lc.TryLand();
    }

    #region Control Implementation

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

    #endregion

    private void SubscribeEvents() {
        GetComponent<Collector>().onCollect += OnCollect;
        lc.onLand += OnLand;
        lc.onSink += OnSink;
    }

    private void OnCollect(GameObject toCollect, string tag) {
        switch (tag) {
            case "Fly":
                AudioSource.PlayClipAtPoint(eatSound, transform.position);
                toCollect.tag = "X";
                break;
            default:
                break;
        }
    }

    private void OnLand() {
        anim.SetTrigger("Land");
        anim.ResetTrigger("Jump");
    }

    private void OnSink() {
        if (respawnPoint) {
            rb2d.velocity = Vector2.zero;
            Invoke("Respawn", 0.2f);
        }
    }

    void OnDestroy() {
        GetComponent<Collector>().onCollect -= OnCollect;
        lc.onLand -= OnLand;
        lc.onSink -= OnSink;
    }
}
