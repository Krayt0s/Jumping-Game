using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandStateController))]
[RequireComponent(typeof(Collector))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FrogController : MonoBehaviour {
    /** The Gameobject must have a trigger that acts as a landing spot sensor **/
    [SerializeField] private GameObject spawnSetEffect;

    private LandStateController lc;
    private Rigidbody2D rb2d;
    private Animator anim;
    private AudioSource asrc;
    private AudioSource boingAsrc;
    private JumpCursor jumpCursor;

    [SerializeField] private AudioClip eatSound;

    public AudioClip chargeSound;
    private float startPitch = 0.1f;
    private float endPitch = 2.5f;
    private float volume = 0.6f;

    private float maxJumpVelocity = 30.0f;
    private float baseJumpVelocity = 12.0f;
    private float maxJumpDistance = 7.0f;

    private bool _charging;
    private float heldTime;
    private const float maxHoldTime = 1.5f;
    public float ChargeRatio { get { return (heldTime / maxHoldTime); } }

    private float fallTimer;

    private GameObject respawnPoint;

    private bool Charging {
        get { return _charging; }
        set {
            _charging = value;
            anim.SetBool("Charging", _charging);
        }
    }

    void Awake() {
        heldTime = 0f;
        lc = GetComponent<LandStateController>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        asrc = GetComponent<AudioSource>();

        boingAsrc = gameObject.AddComponent<AudioSource>();

        var g = GameObject.FindGameObjectWithTag("Assistant");
        if(g) {
            jumpCursor = g.GetComponentInChildren<JumpCursor>();
            if(jumpCursor) {
                jumpCursor.SetHomeObject(gameObject);
            }
        }

        SubscribeEvents();
    }

    void Update () {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotateTo(target);
        // Limit jump distance
        Vector3 dir = target - (Vector2)transform.position;
        if(dir.magnitude > maxJumpDistance) {
            target = dir.normalized * maxJumpDistance + transform.position;
        }

        if (Charging) {
            float chargeFrac = heldTime / maxHoldTime;

            if (ReleaseCharge()) {
                if (lc.Grounded || (lc.Airborne && lc.CanLand())) {
                    jumpCursor.Aim(JumpDisp(target, chargeFrac));
                    Jump(target, JumpVelocity(chargeFrac));
                }
                BreakCharge();
                if(jumpCursor) {
                    jumpCursor.Freeze();
                }
            } else {
                if(jumpCursor) {
                    jumpCursor.Aim(JumpDisp(target, chargeFrac));
                }
                // Update sound
                if(boingAsrc.isPlaying) {
                    boingAsrc.Stop();
                }
                boingAsrc.pitch = Mathf.Lerp(startPitch, endPitch, chargeFrac);
                boingAsrc.PlayOneShot(chargeSound, volume);
                // Update Charging Progress
                heldTime += Time.deltaTime;
                if (heldTime > maxHoldTime) {
                    BreakCharge();
                }
            }
        } else {
            if (BeginCharge()) {
                Charging = true;
            }
        }
    }

    private void BreakCharge() {
        heldTime = 0;
        if(Charging) {
            if (boingAsrc.isPlaying) {
                boingAsrc.Stop();
            }
            boingAsrc.pitch = Mathf.Lerp(startPitch, endPitch, 0.1f);
            boingAsrc.PlayOneShot(chargeSound, volume);
        }
        Charging = false;

        if (jumpCursor) {
            jumpCursor.StopAiming();
        }
    }

    private Vector2 JumpDisp(Vector2 target, float chargeFrac) {
        // Predict landing position
        Vector3 dir = (target - (Vector2)transform.position).normalized;
        Vector3 worldv = (dir * JumpVelocity(chargeFrac)) + (Vector3)rb2d.velocity;
        return worldv * FallTime(target, JumpVelocity(chargeFrac));
    }

    private void Jump(Vector2 target, float jumpVelocity) {
        rb2d.velocity = jumpVelocity * (Vector2)transform.up + rb2d.velocity;
        lc.Ascend(FallTime(target, jumpVelocity));
        anim.SetTrigger("Jump");
    }

    private float JumpVelocity(float chargeFrac) {
        return Mathf.Lerp(baseJumpVelocity, maxJumpVelocity, chargeFrac);
    }

    private float FallTime(Vector2 target, float jumpVelocity) {
        return (target - (Vector2)(transform.position)).magnitude / jumpVelocity;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        BreakCharge();
    }

    public void Respawn() {
        transform.position = respawnPoint.transform.position;
        Instantiate(spawnSetEffect, respawnPoint.transform.position, Quaternion.identity);
        // Give time for collision to check
        Invoke("PostRespawn", 0.1f);
        if (jumpCursor) {
            jumpCursor.Deaim();
        }
    }
    private void PostRespawn() {
        lc.TryLand();
    }

    #region Control Implementation

    private void RotateTo(Vector3 target) {
        Vector3 dir = target - transform.position;
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
        lc.OnLand += OnLand;
        lc.OnSink += OnSink;
    }

    private void OnCollect(GameObject toCollect, string tag) {
        switch (tag) {
            case "Secret":
            case "Fly":
                asrc.PlayOneShot(eatSound);
                toCollect.tag = "X";
                break;
            case "Lily":
                if(toCollect == respawnPoint) {
                    break;
                }
                // Restore old lily
                if(respawnPoint) {
                    respawnPoint.GetComponent<SpriteRenderer>().color = Color.white;
                }
                // Take new lily
                respawnPoint = toCollect;
                respawnPoint.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.4f, 0.4f, 1.0f);

                Instantiate(spawnSetEffect, respawnPoint.transform.position, Quaternion.identity);
                break;
            default:
                break;
        }
    }

    private void OnLand() {
        anim.SetTrigger("Land");
        anim.ResetTrigger("Jump");
        if (jumpCursor) {
            jumpCursor.Deaim();
        }
    }

    private void OnSink() {
        if (respawnPoint) {
            rb2d.velocity = Vector2.zero;
            Invoke("Respawn", 0.2f);
        }
    }

    void OnDestroy() {
        GetComponent<Collector>().onCollect -= OnCollect;
        lc.OnLand -= OnLand;
        lc.OnSink -= OnSink;
    }
}
