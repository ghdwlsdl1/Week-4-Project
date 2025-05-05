using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    // 캐릭터의 외형 반전을 위한 스프라이트 렌더러
    [SerializeField] private SpriteRenderer characterRenderer;

    // 무기 회전 축 기준 (총구나 검 방향을 회전할 때 사용)
    [SerializeField] private Transform weaponPivot;

    // 입력 저장용
    protected Vector2 inputMove = Vector2.zero;
    protected Vector2 inputLook = Vector2.zero;
    protected bool inputJump = false;

    // 이동 입력 방향을 저장하는 벡터
    protected Vector2 movementDirection = Vector2.zero;

    // 외부에서 movementDirection을 읽기 위한 프로퍼티
    public Vector2 MovementDirection { get { return movementDirection; } }

    // 마우스를 기준으로 바라보는 방향
    protected Vector2 lookDirection = Vector2.zero;

    // 외부에서 lookDirection을 읽기 위한 프로퍼티
    public Vector2 LookDirection { get { return lookDirection; } }

    // 점프 입력 여부 저장
    protected bool jumpInput = false;

    // 외부에서 jumpInput 상태를 읽기 위한 프로퍼티
    public bool JumpInput { get { return jumpInput; } }

    // 넉백 벡터 (캐릭터를 밀어내는 힘)
    private Vector2 knockback = Vector2.zero;

    // 넉백 지속 시간
    private float knockbackDuration = 0.0f;

    // 애니메이션 핸들러
    protected AnimationHandler animationHandler;

    // 스탯 핸들러
    protected StatHandler statHandler;

    [SerializeField] private WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;

    protected bool isAttacking;
    private float timeSinceLastAttack = float .MaxValue;
    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();

        if(WeaponPrefab != null)
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        else
            weaponHandler = GetComponentInChildren<WeaponHandler>();
    }

    // Start는 필요 시 오버라이드 가능하도록 빈 구현
    protected virtual void Start() { }

    // 매 프레임마다 호출. 입력 처리 및 회전 처리
    protected virtual void Update()
    {
        HandleAction();            // 입력 및 행동 처리
        Rotate(lookDirection);     // lookDirection 방향으로 캐릭터 및 무기 회전

        if (jumpInput && IsGrounded())
        {
            Jump();         // 점프 실행 함수 호출
            jumpInput = false;
        }

        HandleAttackDelay();
    }

    // FixedUpdate는 물리 갱신 프레임마다 호출됨
    protected virtual void FixedUpdate()
    {
        Movment(movementDirection); // 이동 처리 호출

        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
            _rigidbody.velocity += knockback;
        }
    }

    // 입력 저장값을 바탕으로 동작 상태 갱신
    protected virtual void HandleAction()
    {
        movementDirection = new Vector2(inputMove.x, 0);

        if (inputJump)
        {
            jumpInput = true;
            inputJump = false;
        }
    }

    // 이동 처리 함수: 이동 속도와 넉백을 적용하여 Rigidbody2D에 반영
    private void Movment(Vector2 direction)
    {
        float moveSpeed = statHandler.Speed;
        direction = direction * moveSpeed;

        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        Vector2 velocity = _rigidbody.velocity;
        velocity.x = direction.x;
        _rigidbody.velocity = velocity;

        animationHandler.Move(direction);
    }

    // 캐릭터와 무기 회전 처리 함수
    private void Rotate(Vector2 direction)
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;
        characterRenderer.flipX = isLeft;

        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }

    // 외부에서 넉백을 적용하기 위한 함수
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
    }

    // 바닥에 있는지 확인하는 함수
    protected virtual bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }

    // 점프 실행 함수
    protected virtual void Jump()
    {
        _rigidbody.AddForce(Vector2.up * statHandler.JumpPower, ForceMode2D.Impulse);
    }

    private void HandleAttackDelay()
    {
        if(weaponHandler == null) return;

        if(timeSinceLastAttack <= weaponHandler.Delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if(isAttacking && timeSinceLastAttack >= weaponHandler.Delay)
        {
            timeSinceLastAttack = 0f;
            AttackAnimation();
        }
    }

    protected virtual void AttackAnimation()
    {
        if (weaponHandler != null)
        {
            weaponHandler.Attack(lookDirection);
        }
    }
}
