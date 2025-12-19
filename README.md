# Dialogue System
Easily integrated dialogues based on Ink package. Contains both dialogue system and UI based on Unity Input System.
### Usage
- Drag *DialogueCanvas* prefab to the scene;
- Drag *DialogueGameObject* prefab to the scene;
  - Assign .json file (compiled .ink) to *InteractableDialogue* component.

# Modification
To add .ink custom tags and functions you must modify *Mods* folder.

### TagProcessor
Modify *TagProcessor* to add custom tags:
- Create "Handle{FunctionName}" function that will be called upon tag;
- Add newly created function to *_tagHandlers* initialization in class's constructor.
### InkTags
Modify *InkTags* to create lists of possible assignments (f.e. character name's) for already defined tags.

### InkExternalFunctions
To add external functions, you must modify *InkExternalFunctions* file:
- Create function that will be called from .ink file;
- Add binding to Bind();
- Add unbinding to Unbind().