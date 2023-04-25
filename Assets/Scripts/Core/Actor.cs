using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface ISelectable
{
    Transform selectableTransform { get; }
    Rigidbody2D selectableRigidbody { get; }
    void OnSelect();
    void OnDiselect();
    bool IsSelectable { get; }
}

public class Actor : MonoBehaviour, ISelectable
{
    protected Coroutine bounceCouroutine;
    bool bouncing = false;

    protected Rigidbody2D rb;
    protected Collider2D actorCollider;
    Vector3 startScale;
    int currentBounces = 0;

    [Header("Actor Properties")]

    public bool canBeSelected = true;
    public bool isOnGround = true;
    [SerializeField] int maxBounces;
    public float bounceSpeeds;

    private IObjectPool<Actor> actorPool;

    public Transform selectableTransform { get { return transform; } }
    public Rigidbody2D selectableRigidbody { get { return rb; } }
    public bool IsSelectable { get { return canBeSelected; } }

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        actorCollider = GetComponent<Collider2D>();

        startScale = transform.localScale;
    }

    public virtual void Start() 
    { 
        DropActor(); 
    }

    public virtual void OnSelect()
    {
        if (!canBeSelected) { return; }

        StopBounceActor();
        PickUpActor();
    }

    public virtual void PickUpActor()
    {
        isOnGround = false;
        transform.localScale = startScale * 1.5f;
    }

    public virtual void OnDiselect()
    {
        DropActor();
        canBeSelected = true;
    }

    public virtual void DropActor() 
    {
        bounceCouroutine = StartCoroutine(BounceActor());
    }

    public void StopBounceActor() 
    {
        if (bounceCouroutine != null)
        {
            StopCoroutine(bounceCouroutine);
        }

        bouncing = false;
    }

    public virtual IEnumerator BounceActor()
    {
        bouncing = true;
        float timeElapsed = 0;

        currentBounces = 0;

        float bounceHeight = 1.5f;
        Vector3 fromScale = startScale * bounceHeight;
        Vector3 toScale = startScale;

        while (currentBounces != maxBounces + 1 && bouncing)
        {
            while (transform.localScale != toScale && bouncing)
            {
                transform.localScale = Vector3.Lerp(fromScale, toScale, timeElapsed);
                timeElapsed += Time.deltaTime * bounceSpeeds;

                yield return null;
            }

            currentBounces++;

            isOnGround = transform.localScale == startScale;

            if (isOnGround)
            {
                //Debug.Log("OnGround");
            }

            bounceHeight -= 0.25f;
            bounceHeight = Mathf.Clamp(bounceHeight, 1, bounceHeight);

            Vector3 temp = toScale;
            toScale = fromScale / bounceHeight;
            fromScale = temp;
            //Debug.Log("To Scale: " + toScale);

            timeElapsed = 0;

            yield return null;
        }

        //Debug.Log("Bounces done");
        currentBounces = 0;

        yield return null;
    }

    void SetColliderEnable(bool newEnabled)
    {
        actorCollider.enabled = newEnabled;
    }

    public void SetPool(IObjectPool<Actor> pool)
    {
        actorPool = pool;
    }

    public IEnumerator ReleaseToPoolAfterSeconds(float seconds) 
    {
        yield return new WaitForSeconds(seconds);

        ReleaseToPool();

        yield return null;
    }

    public void ReleaseToPool() 
    {
        if (actorPool != null)
        {
            actorPool.Release(this);
        }
        else 
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void OnEnable() 
    {
        canBeSelected = true;
        //When enabled, we runt he drop actor animation
        DropActor();
    }

    public virtual void OnDisable() 
    {
        isOnGround = false;
        StopBounceActor();
    }

    public virtual void ResetTransformScale() 
    {
        transform.localScale = startScale;
    }
}
