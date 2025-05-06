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

    private GameObject rope;

    public static bool ArrowIsActive { get; private set; } = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady)
        {
            return;
        }

        currentDuration += Time.deltaTime;

        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false);
        }

        if (_rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            transform.right = _rigidbody.velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 먼저 벽/땅/타일에 닿았는지 확인
        if ((levelCollisionLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            StickToWall();      // 벽에 붙고
            return;             // 다른 처리는 무시
        }

        // 2. 적(타겟)에 닿았는지 확인
        if ((rangeWeaponHandler.target.value & (1 << collision.gameObject.layer)) != 0)
        {
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }


    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler)
    {
        ArrowIsActive = true;
        rangeWeaponHandler = weaponHandler;

        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * weaponHandler.BulletSize;
        spriteRenderer.color = weaponHandler.ProjectileColor;

        transform.right = this.direction;

        if (this.direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0);
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0);
        _rigidbody.AddForce(this.direction.normalized * rangeWeaponHandler.Speed, ForceMode2D.Impulse);
        isReady = true;

        if (rangeWeaponHandler.Controller is PlayerController player)
        {
            player.RegisterProjectile(this);
        }

    }

    public void SetRope(GameObject ropeInstance)
    {
        rope = ropeInstance;
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (rope != null)
            Destroy(rope);

        Destroy(this.gameObject);
    }

    private void StickToWall()
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.bodyType = RigidbodyType2D.Static;
    }

    public void Recall()
    {
        ArrowIsActive = false;
        Destroy(gameObject);
    }
}

