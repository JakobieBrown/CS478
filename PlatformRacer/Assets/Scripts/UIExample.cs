using UnityEngine;
using UnityEngine.UIElements;

public class CompleteUIExample : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.paddingLeft = 20;
        root.style.paddingRight = 20;
        root.style.paddingTop = 20;
        root.style.paddingBottom = 20;

        // Title
        var title = new Label("Character Creator");
        title.style.fontSize = 28;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 20;
        root.Add(title);

        // Profile Image Section
        var imageContainer = new Box();
        imageContainer.style.marginBottom = 20;
        imageContainer.style.paddingBottom = 15;
        imageContainer.style.borderBottomWidth = 2;
        imageContainer.style.borderBottomColor = Color.gray;

        var imageLabel = new Label("Profile Picture");
        imageLabel.style.fontSize = 16;
        imageLabel.style.marginBottom = 10;
        imageContainer.Add(imageLabel);

        // Image element
        var profileImage = new Image();
        profileImage.style.width = 150;
        profileImage.style.height = 150;
        profileImage.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        // Load an image from Resources folder
        profileImage.image = Resources.Load<Texture2D>("YourImageName");
        imageContainer.Add(profileImage);

        root.Add(imageContainer);

        // Character Name Section
        var nameLabel = new Label("Character Name");
        nameLabel.style.fontSize = 14;
        nameLabel.style.marginTop = 15;
        nameLabel.style.marginBottom = 5;
        root.Add(nameLabel);

        var nameField = new TextField();
        nameField.style.marginBottom = 20;
        nameField.RegisterValueChangedCallback(evt =>
            Debug.Log("Character name: " + evt.newValue));
        root.Add(nameField);

        // Description Section
        var descLabel = new Label("Description");
        descLabel.style.fontSize = 14;
        descLabel.style.marginBottom = 5;
        root.Add(descLabel);

        var descField = new TextField();
        descField.multiline = true;
        descField.style.height = 80;
        descField.style.marginBottom = 20;
        descField.RegisterValueChangedCallback(evt =>
            Debug.Log("Description: " + evt.newValue));
        root.Add(descField);

        // Class Selection (Radio Buttons)
        var classLabel = new Label("Class");
        classLabel.style.fontSize = 14;
        classLabel.style.marginBottom = 10;
        classLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(classLabel);

        var classGroup = new GroupBox();
        classGroup.style.marginBottom = 20;
        classGroup.style.paddingLeft = 10;

        string[] classes = { "Warrior", "Mage", "Rogue", "Paladin" };
        foreach (var className in classes)
        {
            var radioButton = new RadioButton(className);
            radioButton.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                    Debug.Log("Selected class: " + className);
            });
            classGroup.Add(radioButton);
        }

        // Set default class
        root.Add(classGroup);

        // Abilities Section (Toggles)
        var abilitiesLabel = new Label("Abilities");
        abilitiesLabel.style.fontSize = 14;
        abilitiesLabel.style.marginBottom = 10;
        abilitiesLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(abilitiesLabel);

        var abilitiesContainer = new Box();
        abilitiesContainer.style.marginBottom = 20;
        abilitiesContainer.style.paddingLeft = 10;

        string[] abilities = { "Double Attack", "Fireball", "Invisibility", "Shield Bash" };
        foreach (var ability in abilities)
        {
            var toggle = new Toggle(ability);
            toggle.style.marginBottom = 8;
            toggle.RegisterValueChangedCallback(evt =>
                Debug.Log(ability + " enabled: " + evt.newValue));
            abilitiesContainer.Add(toggle);
        }

        root.Add(abilitiesContainer);

        // Additional Settings Section
        var settingsLabel = new Label("Settings");
        settingsLabel.style.fontSize = 14;
        settingsLabel.style.marginBottom = 10;
        settingsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(settingsLabel);

        var settingsContainer = new Box();
        settingsContainer.style.paddingLeft = 10;
        settingsContainer.style.marginBottom = 20;

        var soundToggle = new Toggle("Enable Sound");
        soundToggle.value = true;
        soundToggle.style.marginBottom = 8;
        settingsContainer.Add(soundToggle);

        var difficultyLabel = new Label("Difficulty");
        difficultyLabel.style.fontSize = 12;
        difficultyLabel.style.marginTop = 10;
        difficultyLabel.style.marginBottom = 8;
        settingsContainer.Add(difficultyLabel);

        var difficultyGroup = new GroupBox();
        difficultyGroup.style.paddingLeft = 10;

        string[] difficulties = { "Easy", "Normal", "Hard" };
        foreach (var difficulty in difficulties)
        {
            var radioButton = new RadioButton(difficulty);
            radioButton.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                    Debug.Log("Difficulty: " + difficulty);
            });
            difficultyGroup.Add(radioButton);
        }

        settingsContainer.Add(difficultyGroup);

        root.Add(settingsContainer);

        // Buttons
        var buttonContainer = new Box();
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.justifyContent = Justify.FlexEnd;

        var createButton = new Button { text = "Create Character" };
        createButton.style.width = 150;
        createButton.style.height = 40;
        createButton.clicked += () => Debug.Log("Character created!");
        buttonContainer.Add(createButton);

        var cancelButton = new Button { text = "Cancel" };
        cancelButton.style.width = 100;
        cancelButton.style.height = 40;
        cancelButton.clicked += () => Debug.Log("Cancelled");
        buttonContainer.Add(cancelButton);

        root.Add(buttonContainer);
    }
}
