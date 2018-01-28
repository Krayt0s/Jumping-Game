using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LandStateController : MonoBehaviour {
    [SerializeField] private Vector2 anchorPoint;

    private Rigidbody2D rb2d;
    private AudioSource asrc;

    private SpriteRenderer[] srs;
    private Color[] cols;
    private const int sinkOffset = 500;
    private const float sinkScale = 0.8f;

    private const float ascendScale = 1.1f;

    
    [SerializeField] private GameObject splashParticleSystem;
    [SerializeField] private AudioClip splashSound;

    private AnchoredJoint2D grounding;
    public AnchoredJoint2D Grounding { get { return grounding; } } 
    private List<GameObject> landObjects = new List<GameObject>();

    private float fallTimer;

    private State state;
    public bool Airborne { get { return state == State.AIRBORNE; } }
    public bool Grounded { get { return state == State.GROUNDED; } }
    public bool Underwater { get { return state == State.UNDERWATER; } }

    public delegate void StateChange();
    public event StateChange OnSink;
    public event StateChange OnSurface;
    public event StateChange OnLand;

    private enum State {
        GROUNDED,
        UNDERWATER,
        AIRBORNE
    }

    void Awake() {
        srs = gameObject.GetComponentsInChildren<SpriteRenderer>();
        cols = new Color[srs.Length];
        rb2d = GetComponent<Rigidbody2D>();
        asrc = GetComponent<AudioSource>();
    }

    private void Start() {
        if (Grounded) {
            Invoke("TryLand", 0.1f);
        }
    }

    private void Update() {
        if (state == State.AIRBORNE) {
            fallTimer -= Time.deltaTime;
            if (fallTimer <= 0) {
                TryLand();
            }
        }
    }

    public bool CanLand() {
        return landObjects.Count != 0;
    }

    public bool LandIfCan() {
        bool canLand = CanLand();
        if (canLand) {
            Ground(landObjects[0]);
            if (OnLand != null) {
                OnLand();
            }
        }
        return canLand;
    }

    #region State Entry
    public bool TryLand() {
        bool landed = LandIfCan();
        if (!landed) {
            Sink();
        }
        return landed;
    }
    private void Ground(GameObject ground) {
        LeaveState();
        grounding = ground.AddComponent<HingeJoint2D>();
        grounding.autoConfigureConnectedAnchor = true;
        Quaternion invrot = Quaternion.Inverse(ground.transform.rotation);
        grounding.anchor = invrot * (transform.position - ground.transform.position);
        grounding.connectedBody = rb2d;
        grounding.enableCollision = true;
        state = State.GROUNDED;

        anchorPoint = grounding.anchor;
    }

    public void Sink() {
        LeaveState();
        state = State.UNDERWATER;
        
        transform.localScale *= sinkScale;
        Instantiate(splashParticleSystem, transform.position, Quaternion.identity);
        asrc.PlayOneShot(splashSound);
        for (int i = 0; i < srs.Length; i++) {
            cols[i] = srs[i].color;
            srs[i].color = Color.grey;
            
            srs[i].sortingOrder -= sinkOffset;
        }

        if(OnSink != null) {
            OnSink();
        }
    }
    
    public void Ascend(float timer) {
        LeaveState();
        state = State.AIRBORNE;

        transform.localScale *= ascendScale;
        fallTimer = timer;
    }
    #endregion


    #region State Exit
    private void LeaveState() {
        switch (state) {
            case State.AIRBORNE:
                Descend();
                break;
            case State.UNDERWATER:
                Surface();
                break;
            case State.GROUNDED:
                Unground();
                break;
        }
    }

    private void Unground() {
        if (grounding) {
            Destroy(grounding);
        }
    }

    private void Surface() {
        transform.localScale /= sinkScale;
        for (int i = 0; i < srs.Length; i++) {
            srs[i].color = cols[i];
            srs[i].sortingOrder += sinkOffset;
        }

        if(OnSurface != null) {
            OnSurface();
        }
    }
    
    private void Descend() {
        transform.localScale /= ascendScale;
    }
    #endregion

    // The ground detector is a trigger, so all collisions are entity-entity.
    void OnCollisionEnter2D(Collision2D coll) {
        Unground();
    }

    #region Land Collision
    private bool IsLandCollider(Collider2D coll) {
        return coll.gameObject.layer == LayerMask.NameToLayer("Landable");
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (IsLandCollider(coll)) {
            landObjects.Add(coll.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D coll) {
        if (IsLandCollider(coll)) {
            landObjects.Remove(coll.gameObject);

            if(Grounded && !CanLand()) {
                Sink();
            }
        }
    }
    #endregion
}
