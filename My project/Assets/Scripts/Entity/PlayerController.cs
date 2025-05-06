using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    // 메인 카메라를 참조할 변수
    private Camera camera;

    // 카메라 이동 속도
    [SerializeField] private float cameraFollowSpeed = 5f;

    // 카메라 y축 오프셋
    [SerializeField] private float cameraYOffset = 1.5f;

    private ProjectileController currentProjectile;

    protected override void Start()
    {
        base.Start();             // BaseController의 Start() 호출
        camera = Camera.main;     // 메인 카메라를 찾아 camera에 할당
    }

    protected override void Update()
    {
        base.Update();            // 입력/회전/점프 처리
        FollowCameraSmooth();     // 카메라가 부드럽게 플레이어를 따라오게 함
    }
    public void RegisterProjectile(ProjectileController proj)
    {
        currentProjectile = proj;
    }

    // 입력 해석 및 lookDirection 계산
    protected override void HandleAction()
    {
        base.HandleAction();

        if (camera != null)
        {
            Vector2 worldPos = camera.ScreenToWorldPoint(inputLook); // 화면 좌표 → 월드 좌표 변환
            lookDirection = (worldPos - (Vector2)transform.position); // 캐릭터 → 마우스 방향 벡터
            lookDirection = (lookDirection.magnitude < 0.9f) ? Vector2.zero : lookDirection.normalized;
        }
    }

    // 카메라가 플레이어를 따라오되, 부드럽게 이동하도록 처리하는 함수
    private void FollowCameraSmooth()
    {
        if (camera == null) return;

        Vector3 currentCamPos = camera.transform.position;
        Vector3 targetCamPos = new Vector3(
            transform.position.x,
            transform.position.y + cameraYOffset,
            currentCamPos.z
        );

        camera.transform.position = Vector3.Lerp(currentCamPos, targetCamPos, Time.deltaTime * cameraFollowSpeed);
    }

    // 입력 시스템: 이동 처리
    void OnMove(InputValue inputValue)
    {
        inputMove = inputValue.Get<Vector2>();
    }

    // 입력 시스템: 마우스 위치 처리
    void OnLook(InputValue inputValue)
    {
        inputLook = inputValue.Get<Vector2>();
    }

    // 입력 시스템: 점프 입력 처리
    void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            inputJump = true; // 점프 요청: 실제 점프는 BaseController가 처리
        }
    }
    // 입력 시스템: 공격 입력 처리
    void OnFire(InputValue inputValue)
    {
        isAttacking = inputValue.isPressed;
    }
    void OnRecover(InputValue inputValue)
    {
        if (inputValue.isPressed && currentProjectile != null)
        {
            currentProjectile.Recall();
            currentProjectile = null;
        }
    }
}
