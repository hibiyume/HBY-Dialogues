using System;
using System.Collections.Generic;
using UnityEngine;

public class TagProcessor
{
    private readonly Dictionary<string, Action<string>> _tagHandlers;

    private string _currentSpeakerKey;
    private string _previousCharacterKey;
    
    private readonly InkTags _inkTags = new();
    
    public TagProcessor()
    {
        _tagHandlers = new Dictionary<string, Action<string>>()
        {
            // "speaker: name" - displays name, "speaker:" - hides name
            { "speaker", HandleSpeaker },
            
            // "emotion: name" - displays sprite, "emotion:" - hides sprite
            { "emotion", HandleCharacter }
        };
    }

    public void ProcessTags(List<string> tagsList)
    {
        foreach (var tagLine in tagsList)
        {
            var tags = tagLine.Split(',');

            foreach (var tag in tags)
            {
                var split = tag.Split(':', 2);

                string tagName = split[0].Trim();
                string tagContent = split[1].Trim();

                if (_tagHandlers.TryGetValue(tagName, out Action<string> handler))
                    handler.Invoke(tagContent);
                else
                    Debug.LogWarning($"Tag {tagName} does not have a handler.");
            }
        }
    }

    public void ClearTags()
    {
        _currentSpeakerKey = null;
        DialogueUI.Instance.HideSpeaker();
        
        _previousCharacterKey = null;
        DialogueUI.Instance.HideCharacter();
    }
    
    private void HandleSpeaker(string speakerKey)
    {
        if (string.IsNullOrWhiteSpace(speakerKey)) // if "speaker:"
        {
            _currentSpeakerKey = null;
            DialogueUI.Instance.HideSpeaker();
            return;
        }

        speakerKey = char.ToUpper(speakerKey[0]) + speakerKey.Substring(1); // e.g. "key" -> "Key"

        if (_inkTags.CharacterNames.TryGetValue(speakerKey, out string speakerName))
        {
            DialogueUI.Instance.DisplaySpeaker(speakerName);
            _currentSpeakerKey = speakerKey;
        }
        else
        {
            DialogueUI.Instance.DisplaySpeaker(speakerKey);
            _currentSpeakerKey = "";
        }
    }

    private void HandleCharacter(string emotion)
    {
        if (string.IsNullOrWhiteSpace(emotion))
        {
            DialogueUI.Instance.HideCharacter();
            _previousCharacterKey = null;
            return;
        }

        if (string.IsNullOrEmpty(_currentSpeakerKey))
        {
            Debug.LogWarning("No speaker name provided. Can't display character.");
            return;
        }
        
        if (_previousCharacterKey != null && _previousCharacterKey != _currentSpeakerKey)
        {
            DialogueUI.Instance.HideCharacter();
        }

        DialogueUI.Instance.DisplayCharacter(_currentSpeakerKey, emotion);
        _previousCharacterKey = _currentSpeakerKey;
    }
}
