using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    // Rigidbody2D는 protected로 선언되어 있어 상속 클래스에서 접근 가능
    protected Rigidbody2D _rigidbody;

    // 캐릭터의 외형 반전을 위한 스프라이트 렌더러
    [SerializeField] private SpriteRenderer characterRenderer;

    // 무기 회전 축 기준 (총구나 검 방향을 회전할 때 사용)
    [SerializeField] private Transform weaponPivot;

    // 이동 입력 방향을 저장하는 벡터
    protected Vector2 movementDirection = Vector2.zero;

    // 외부에서 movementDirection을 읽기 위한 프로퍼티
    public Vector2 MovementDirection { get { return movementDirection; } }

    // 마우스를 기준으로 바라보는 방향
    protected Vector2 lookDirection = Vector2.zero;

    // 외부에서 lookDirection을 읽기 위한 프로퍼티
    public Vector2 LookDirection { get { return lookDirection; } }

    // 넉백 벡터 (캐릭터를 밀어내는 힘)
    private Vector2 knockback = Vector2.zero;

    // 넉백 지속 시간
    private float knockbackDuration = 0.0f;

    // 오브젝트 생성 시 호출. Rigidbody2D를 캐싱
    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start는 필요 시 오버라이드 가능하도록 빈 구현
    protected virtual void Start()
    {
    }

    // 매 프레임마다 호출. 입력 처리 및 회전 처리
    protected virtual void Update()
    {
        HandleAction();            // 입력 및 행동 처리
        Rotate(lookDirection);     // lookDirection 방향으로 캐릭터 및 무기 회전
    }

    // FixedUpdate는 물리 갱신 프레임마다 호출됨
    protected virtual void FixedUpdate()
    {
        // 넉백 지속 중일 경우 넉백 힘을 계속 적용
        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
            _rigidbody.velocity += knockback;
        }
    }

    // 입력 처리를 위한 가상 함수. 상속받는 클래스에서 구현
    protected virtual void HandleAction()
    {
    }

    // 캐릭터와 무기 회전 처리 함수
    private void Rotate(Vector2 direction)
    {
        // 벡터 → 각도 (Z축 회전각 계산)
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 좌우 반전 판단 기준: rotZ 절댓값이 90 이상이면 왼쪽
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        // 스프라이트 좌우 반전 적용
        characterRenderer.flipX = isLeft;

        // 무기 회전 적용 (Pivot이 존재할 경우)
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }

    // 외부에서 넉백을 적용하기 위한 함수
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        // 넉백 지속 시간 설정
        knockbackDuration = duration;

        // other에서 나 자신을 향하는 방향으로 넉백 적용
        knockback = -(other.position - transform.position).normalized * power;
    }
}
