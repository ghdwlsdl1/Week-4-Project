using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponHandler : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] private float delay = 1f;
    public float Delay { get => delay; set => delay = value; }

    [SerializeField] private float weaponSize = 1f;
    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] private float power = 1f;
    public float Power { get => power; set => power = value; }

    [SerializeField] private float speed = 1f;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField] private float attackRange = 10f;
    public float AttackRange { get => attackRange; set => attackRange = value; }

    public LayerMask target;

    [Header("Knock Back Info")]
    [SerializeField] private bool isOnKnockback = false;
    public bool IsOnKnockback { get => isOnKnockback; set => isOnKnockback = value; }

    [SerializeField] private float knockbackPower = 0.1f;
    public float KnockbackPower { get => knockbackPower; set => knockbackPower = value; }

    [SerializeField] private float knockbackTime = 0.5f;
    public float KnockbackTime { get => knockbackTime; set => knockbackTime = value; }

    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    public BaseController Controller { get; private set; }

    private Animator animator;
    private SpriteRenderer weaponRenderer;

    protected virtual void Awake()
    {
        Controller = GetComponentInParent<BaseController>();
        animator = GetComponentInChildren<Animator>();
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();

        animator.speed = 1.0f / delay;
        transform.localScale = Vector3.one * weaponSize;
    }

    protected virtual void Start()
    {
    }

    // 무기 회전 처리 (방향에 따라 스프라이트 뒤집기)
    public virtual void Rotate(bool isLeft)
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.flipY = isLeft;
        }
    }

    // 무기 공격 실행 (방향 포함)
    public virtual void Attack(Vector2 direction)
    {
        // 기본 구현 (자식에서 오버라이드 가능)
        animator.SetTrigger(IsAttack);
    }
}
