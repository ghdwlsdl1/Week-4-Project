using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt;

    public void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}
