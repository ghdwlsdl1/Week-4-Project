using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    // 메인 카메라를 참조할 변수
    private Camera camera;

    // 바닥 감지를 위한 레이어 마스크 (Ground로 설정된 타일만 감지하도록 사용)
    [SerializeField] private LayerMask groundLayer;

    // 플레이어 이동 속도
    [SerializeField] private float moveSpeed = 5f;

    // 점프 시 적용할 힘의 크기
    [SerializeField] private float jumpForce = 7f;

    // 카메라 이동 속도
    [SerializeField] private float cameraFollowSpeed = 5f;

    // 카메라 y축
    [SerializeField] private float cameraYOffset = 1.5f;

    protected override void Start()
    {
        base.Start(); // BaseController의 Start() 호출
        camera = Camera.main; // 메인 카메라를 찾아 camera에 할당
    }

    protected override void Update()
    {
        base.Update();         // 이동 및 회전 처리 (BaseController)
        FollowCameraSmooth();  // 카메라가 부드럽게 플레이어를 따라오게 함
    }

    // 플레이어 행동
    protected override void HandleAction()
    {
        // 좌우 입력 값을 받아 movementDirection 설정
        float horizontal = Input.GetAxisRaw("Horizontal");
        movementDirection = new Vector2(horizontal, 0).normalized;

        // 현재 리지드바디 속도 가져와서 x축만 이동 속도로 덮어씀
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = movementDirection.x * moveSpeed;
        _rigidbody.velocity = velocity;

        // 점프 키 입력 + 바닥에 닿아 있는 경우 점프
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 마우스 위치 기준으로 캐릭터가 바라볼 방향을 계산
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition); // 화면 좌표 → 월드 좌표 변환
        lookDirection = (worldPos - (Vector2)transform.position);    // 캐릭터 → 마우스 방향 벡터

        // 너무 가까우면 0, 아니면 정규화된 방향 벡터로 설정
        lookDirection = (lookDirection.magnitude < 0.9f) ? Vector2.zero : lookDirection.normalized;
    }

    // 플레이어가 바닥에 닿아 있는지 감지하는 함수
    private bool IsGrounded()
    {
        // 아래 방향으로 Raycast를 쏴서 일정 거리 내에 groundLayer와 충돌하는 오브젝트가 있으면 true
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }

    // 카메라가 플레이어를 따라오되, 부드럽게 이동하도록 처리하는 함수
    private void FollowCameraSmooth()
    {
        if (camera == null) return; // 카메라가 없으면 중단

        // 현재 카메라 위치
        Vector3 currentCamPos = camera.transform.position;

        // 따라올 목표 위치 (플레이어 위치 + y 오프셋)
        Vector3 targetCamPos = new Vector3(
            transform.position.x,
            transform.position.y + cameraYOffset,
            currentCamPos.z // 카메라의 z 위치 유지
        );

        // 현재 위치 → 목표 위치로 부드럽게 보간 이동
        camera.transform.position = Vector3.Lerp(currentCamPos, targetCamPos, Time.deltaTime * cameraFollowSpeed);
    }
}
