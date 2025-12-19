using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.EventSystems;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;
    
    [SerializeField] private GameObject dialogueCanvas;

    [Header("Speaker")]
    [SerializeField] private GameObject speakerPanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private Transform characterPlaceHolder;
    [Header("Text")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("Choices")]
    [SerializeField] private GameObject oneChoicePanel;
    [SerializeField] private GameObject twoChoicesPanel;
    [SerializeField] private GameObject threeChoicesPanel;
    [Header("Other")]
    [SerializeField] private string charactersPrefabFolderPath = "Assets/Level/Prefabs/UI/Characters";
    [SerializeField] private Animator uiEffectsAnimator;
    
    private GameObject _instantiatedCharacter;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void DisplaySpeaker(string speakerName)
    {
        speakerText.text = speakerName;
        speakerPanel.SetActive(true);
    }
    public void HideSpeaker()
    {
        speakerPanel.SetActive(false);
    }

    public void DisplayDialogueCanvas()
    {
        dialogueCanvas.SetActive(true);
    }
    public void HideDialogueCanvas()
    {
        dialogueCanvas.SetActive(false);
    }
    
    public void DisplayDialogue(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
    }
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void DisplayCharacter(string characterName, string emotion)
    {
        string key = $"{charactersPrefabFolderPath}/{characterName}/{emotion}.prefab";
        
        Addressables.LoadAssetAsync<GameObject>(key).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _instantiatedCharacter = Instantiate(handle.Result, characterPlaceHolder);
            }
        };
    }
    public void HideCharacter()
    {
        Destroy(_instantiatedCharacter);
    }
    
    public void DisplayChoices(string[] currentChoicesText)
    {
        int choicesAmount = currentChoicesText.Length;

        if (choicesAmount is < 1 or > 3)
        {
            Debug.LogError("Unexpected number of choices.");
            return;
        }

        GameObject[] choices = new GameObject[choicesAmount];
        TextMeshProUGUI[] choicesText = new TextMeshProUGUI[choicesAmount];
        
        oneChoicePanel.SetActive(false);
        twoChoicesPanel.SetActive(false);
        threeChoicesPanel.SetActive(false);
        
        switch (choicesAmount)
        {
            case 1:
                choices[0] = oneChoicePanel.transform.GetChild(0).gameObject;
                choicesText[0] = oneChoicePanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                oneChoicePanel.SetActive(true);
                break;
            case 2:
                for (int i = 0; i < choicesAmount; i++)
                {
                    choices[i] = twoChoicesPanel.transform.GetChild(i).gameObject;
                    choicesText[i] = twoChoicesPanel.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                    twoChoicesPanel.SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < choicesAmount; i++)
                {
                    choices[i] = threeChoicesPanel.transform.GetChild(i).gameObject;
                    choicesText[i] = threeChoicesPanel.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                    threeChoicesPanel.SetActive(true);
                }
                break;
        }
        
        for (int i = 0; i < choicesAmount; i++)
        {
            choicesText[i].text = currentChoicesText[i];
        }

        StartCoroutine(SelectFirstChoice(choices));
    }
    public void HideChoices()
    {
        oneChoicePanel.SetActive(false);
        twoChoicesPanel.SetActive(false);
        threeChoicesPanel.SetActive(false);
    }
    private IEnumerator SelectFirstChoice(GameObject[] choices) //Required to fix Unity bug
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }
    
    /*public void PlayFadeOut()
    {
        uiEffectsAnimator.SetTrigger("FadeOut");
        GameEventsManager.Instance?.UIEvents.FadeOutStart();
    }
    public void PlayFadeIn()
    {
        uiEffectsAnimator.SetTrigger("FadeIn");
        GameEventsManager.Instance?.UIEvents.FadeInStart();
    }*/
}