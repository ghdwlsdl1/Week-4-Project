using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask levelCollisionLayer;

    private RangeWeaponHandler rangeWeaponHandler;

    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;

    public bool fxOnDestory = true;

    private DistanceJoint2D ropeJoint;
    private LineRenderer lineRenderer;

    public static bool ArrowIsActive { get; private set; } = false;
    public float ScrollInputY { get; set; }

    [SerializeField] private float reelSpeed = 1f;
    [SerializeField] private float minRopeLength = 1f;
    [SerializeField] private float maxRopeLength = 10f;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady)
            return;

        currentDuration += Time.deltaTime;

        if (currentDuration > rangeWeaponHandler.Duration)
        {
            Recall();
        }

        if (_rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            transform.right = _rigidbody.velocity;
        }

        // 줄 위치 갱신
        if (lineRenderer != null && ropeJoint != null && ropeJoint.connectedBody != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ropeJoint.connectedBody.position);
        }

        if (ropeJoint != null && Mathf.Abs(ScrollInputY) > 0.01f)
        {
            float newDistance = ropeJoint.distance - ScrollInputY * reelSpeed * Time.deltaTime;
            ropeJoint.distance = Mathf.Clamp(newDistance, minRopeLength, maxRopeLength);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 먼저 벽/땅/타일에 닿았는지 확인
        if ((levelCollisionLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            StickToWall();
            return;
        }

        // 2. 적(타겟)에 닿았는지 확인
        if ((rangeWeaponHandler.target.value & (1 << collision.gameObject.layer)) != 0)
        {
            Recall();
        }
    }


    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler)
    {
        ArrowIsActive = true;
        this.direction = direction;
        rangeWeaponHandler = weaponHandler;
        _rigidbody = GetComponent<Rigidbody2D>();
        isReady = true;

        _rigidbody.velocity = direction.normalized * rangeWeaponHandler.Speed;

        // 줄 연결
        ropeJoint = gameObject.AddComponent<DistanceJoint2D>();
        ropeJoint.autoConfigureDistance = false;
        ropeJoint.enableCollision = false;
        ropeJoint.maxDistanceOnly = true;

        Rigidbody2D playerRb = weaponHandler.Controller.GetComponent<Rigidbody2D>();
        ropeJoint.connectedBody = playerRb;

        float dist = Vector2.Distance(transform.position, playerRb.position);
        ropeJoint.distance = Mathf.Max(dist, 10f); // 최소 줄 길이

        // 줄 시각화
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        if (rangeWeaponHandler.Controller is PlayerController player)
        {
            player.RegisterProjectile(this);
        }
    }

    private void StickToWall()
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.bodyType = RigidbodyType2D.Static;
    }

    public void Recall()
    {
        ArrowIsActive = false;

        if (ropeJoint != null)
        {
            Destroy(ropeJoint);
        }

        if (lineRenderer != null)
        {
            Destroy(lineRenderer);
        }

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        ArrowIsActive = false;

        if (rangeWeaponHandler.Controller is PlayerController player)
        {
            player.RegisterProjectile(null);
        }
    }
}

