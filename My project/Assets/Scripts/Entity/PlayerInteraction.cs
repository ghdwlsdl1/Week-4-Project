using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private GameObject guidePanel;

    private Transform currentNpc;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ShowInteractionPrompt();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        UpdateInteractionPrompt();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }
    private void UpdateInteractionPrompt()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, npcLayer);

        if (hit != null)
        {
            currentNpc = hit.transform;
            interactionPrompt.SetActive(true);

            Vector3 worldPos = currentNpc.position + promptOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            interactionPrompt.transform.position = screenPos;
        }
        else
        {
            currentNpc = null;
            interactionPrompt.SetActive(false);
        }
    }
    private void ShowInteractionPrompt()
    {
        if (interactionPrompt == null) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, npcLayer);
        interactionPrompt.SetActive(hit != null);
    }

    public void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, npcLayer);
        if (hit == null) return;

        NpcController npc = hit.GetComponent<NpcController>();
        if (npc == null) return;

        HandleInteraction(npc.npcType);
    }

    private void HandleInteraction(NpcType type)
    {
        switch (type)
        {
            case NpcType.Guide:
                ShowControlGuide();
                break;

            case NpcType.Minigame:
                LaunchMinigame();
                break;

            default:
                Debug.LogWarning("정의되지 않은 NPC 타입입니다: " + type);
                break;
        }
    }

    private void ShowControlGuide()
    {
        Debug.Log("조작법 UI 표시");
        guidePanel?.SetActive(true);
    }
    public void CloseGuidePanel()
    {
        guidePanel.SetActive(false);
    }
    private void LaunchMinigame()
    {
        Debug.Log("미니게임 실행");
        SceneManager.LoadScene("MinigameScene");
    }
}
