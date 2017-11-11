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
    [SerializeField] private JumpCursor jumpCursor;

    [SerializeField] private AudioClip eatSound;

    public AudioClip chargeSound;
    private float startPitch = 0.1f;
    private float endPitch = 2.5f;
    private float volume = 0.6f;

    [SerializeField] private float maxJumpDistance = 8.0f;
    [SerializeField] private float fallTime = 0.3f;

    private bool _charging;
    private float jumpVelocity;
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
        jumpVelocity = maxJumpDistance / fallTime;
        lc = GetComponent<LandStateController>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        asrc = GetComponent<AudioSource>();

        boingAsrc = gameObject.AddComponent<AudioSource>();

        if(jumpCursor) {
            jumpCursor.SetHomeObject(gameObject);
        }

        SubscribeEvents();
    }

    void Update () {
        Rotate();

        if (Charging) {
            if (ReleaseCharge()) {
                if (lc.Grounded || (lc.Airborne && lc.CanLand())) {
                    Jump();
                }
                Uncharge();
                jumpCursor.StartContraction();
            } else {
                if(boingAsrc.isPlaying) {
                    boingAsrc.Stop();
                }
                float chargeFrac = heldTime / maxHoldTime;
                if(jumpCursor) {
                    Vector3 worldv = (transform.up * jumpVelocity) + (Vector3)rb2d.velocity;
                    jumpCursor.SetTowardsDisp(worldv * fallTime * chargeFrac);
                }
                boingAsrc.pitch = Mathf.Lerp(startPitch, endPitch, chargeFrac);
                boingAsrc.PlayOneShot(chargeSound, volume);
                heldTime += Time.deltaTime;
                if (heldTime > maxHoldTime) {
                    Uncharge();
                }
            }
        } else {
            if (BeginCharge()) {
                Charging = true;
            }
        }
    }

    private void Uncharge() {
        heldTime = 0;
        if(Charging) {
            if (boingAsrc.isPlaying) {
                boingAsrc.Stop();
            }
            boingAsrc.pitch = Mathf.Lerp(startPitch, endPitch, 0.1f);
            boingAsrc.PlayOneShot(chargeSound, volume);
        }
        Charging = false;
    }

    private void Jump() {
        rb2d.velocity = ChargeRatio * jumpVelocity * (Vector2)transform.up + rb2d.velocity;
        lc.Ascend(fallTime);
        anim.SetTrigger("Jump");
    }

    void OnCollisionEnter2D(Collision2D coll) {
        Uncharge();
    }

    public void Respawn() {
        transform.position = respawnPoint.transform.position;
        Instantiate(spawnSetEffect, respawnPoint.transform.position, Quaternion.identity);
        // Give time for collision to check
        Invoke("PostRespawn", 0.1f);
        if (jumpCursor) {
            jumpCursor.EndContraction();
        }
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
            jumpCursor.EndContraction();
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
