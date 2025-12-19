using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset dialogueJson;

    public void Interact()
    {
        DialogueManager.Instance.EnterDialogueMode(dialogueJson);
    }
}