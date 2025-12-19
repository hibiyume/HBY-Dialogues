using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private PlayerInput playerInput;

    /*[Header("Dialogue UI")]
    [SerializeField] private GameObject speakerPanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject oneChoicePanel;
    [SerializeField] private GameObject twoChoicesPanel;
    [SerializeField] private GameObject threeChoicesPanel;*/

    private GameObject[] _choices;
    private TextMeshProUGUI[] _choicesText;

    private Story _currentStory;
    private TagProcessor _tagProcessor;
    private InkExternalFunctions _inkExternalFunctions;

    private string _storyText;
    public bool IsDialoguePlaying { get; private set; }
    private bool _isMakingChoice;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager, destroying one.");
            Destroy(gameObject);
        }
        else
            Instance = this;

        playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        
        //Adding PointerClick trigger on DialoguePanel
        /*EventTrigger trigger = dialoguePanel.GetComponent<EventTrigger>() ?? dialoguePanel.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((eventData) => ContinueDialogue());
        trigger.triggers.Add(entry);*/
    }
    private void Start()
    {
        _inkExternalFunctions = new InkExternalFunctions();
        _tagProcessor = new TagProcessor();

        IsDialoguePlaying = false;
    }

    private void OnEnable()
    {
        playerInput.actions["Submit"].performed += OnSubmit;
    }
    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.actions["Submit"].performed -= OnSubmit;
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (IsDialoguePlaying)
            ContinueDialogue();
    }

    /// <summary>
    /// Default Dialogue Mode
    /// </summary>
    public void EnterDialogueMode(TextAsset dialogueJson)
    {
        if (IsDialoguePlaying)
            return;

        _currentStory = new Story(dialogueJson.text);
        _inkExternalFunctions.Bind(_currentStory);
        
        DialogueUI.Instance.DisplayDialogueCanvas();
        //GameEventsManager.Instance.DialogueEvents.DialogueStart();
        IsDialoguePlaying = true;
        ContinueDialogue();
    }
    
    //TODO: Instead of overloading the function, make door send event and subscribe here. Then check if any doors send event, and in such case load scene after dialogue
    /// <summary>
    /// Dialogue Mode that will load scene after completion (doors)
    /// </summary>
    public void EnterDialogueMode(TextAsset dialogueJson, string sceneToLoad)
    {
        EnterDialogueMode(dialogueJson);
        _currentStory.variablesState["sceneToLoad"] = sceneToLoad;
    }

    public void ExitDialogueMode()
    {
        DialogueUI.Instance.HideSpeaker();
        DialogueUI.Instance.HideCharacter();
        DialogueUI.Instance.HideDialogueCanvas();
        
        _inkExternalFunctions.Unbind(_currentStory);
        //GameEventsManager.Instance.DialogueEvents.DialogueEnd();
        IsDialoguePlaying = false;
    }
    
    //Can also be called from Dialogue Panel click
    public void ContinueDialogue()
    {
        if (_isMakingChoice)
            return;

        if (_currentStory.canContinue)
        {
            Debug.Log("Can Continue");
            _storyText = _currentStory.Continue();
            
            /*if (_storyText == "")
                ExitDialogueMode();*/

            DisplayDialogue();
            DisplayChoices();
            Debug.Log("Displayed Dialogue and Choices");
            _tagProcessor.ProcessTags(_currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }
    private void DisplayDialogue()
    {
        if (_storyText != "")
            DialogueUI.Instance.DisplayDialogue(_storyText);
        else
            DialogueUI.Instance.HideDialogue();
    }
    
    private void DisplayChoices()
    {
        int currentChoicesAmount = _currentStory.currentChoices.Count;

        if (currentChoicesAmount == 0)
        { DialogueUI.Instance.HideChoices(); return;}
        if (currentChoicesAmount is < 0 or > 3)
        { Debug.LogError("Unexpected number of choices."); return; }

        List<Choice> currentChoices = _currentStory.currentChoices;
        string[] currentChoicesText = new string[currentChoices.Count];
            
        for (int i = 0; i < currentChoices.Count; i++)
        {
            currentChoicesText[i] = currentChoices[i].text;
        }

        _isMakingChoice = true;
        
        DialogueUI.Instance.DisplayChoices(currentChoicesText);
    }

    //Used by Choice Buttons
    public void MakeChoice(int choiceIndex)
    {
        _currentStory.ChooseChoiceIndex(choiceIndex);
        _isMakingChoice = false;
        ContinueDialogue();
    }
}