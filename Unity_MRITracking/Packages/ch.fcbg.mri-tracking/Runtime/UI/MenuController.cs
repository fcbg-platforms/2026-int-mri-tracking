using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityAtoms.BaseAtoms;

public class MenuController : MonoBehaviour
{
	[Serializable]
	/// <summary>
	/// Function definition for a button click event.
	/// </summary>
	public class ButtonClickedEvent : UnityEvent { }

	[SerializeField] UIDocument _uiDoc;
	[SerializeField] VisualTreeAsset _settingsButtonsTemplate;

	[SerializeField] FloatReference _bodyWeightVariable;
	[SerializeField] IntReference _genderReference;
	[SerializeField] ColorReference _currentSkinColorReference;
	[SerializeField] ColorConstant[] _skinColors;


	private VisualElement _bodyWrapper;
	private VisualElement _settings;
	private VisualElement _mainMenu;

	// Settings elements
	private Button _playButton;
	private Button _settingsButton;
	private Button _exitButton;
	private TextField _participantTextField;
	private RadioButtonGroup _genderButtonGroup;
	private Slider _weightSlider;

	[SerializeField] private ButtonClickedEvent onPlayClick = new ButtonClickedEvent();

	private Button _currentlySelectedButton = null;

	private void Awake()
	{
		_bodyWrapper = _uiDoc.rootVisualElement.Q<VisualElement>("body");
		_mainMenu = _bodyWrapper.Q<VisualElement>("main-menu");

		// _participantTextField = _mainMenu.Q<TextField>("participant-text-field");
		_playButton = _mainMenu.Q<Button>("play-button");
		_settingsButton = _mainMenu.Q<Button>("settings-button");
		_exitButton = _mainMenu.Q<Button>("exit-button");

		_settings = _settingsButtonsTemplate.CloneTree();
		Button returnButton = _settings.Q<Button>("return-button");
		_genderButtonGroup = _settings.Q<RadioButtonGroup>("gender-button-group");
		_weightSlider = _settings.Q<Slider>("weight-slider");
		returnButton.clicked += ReturnButtonClicked;

		Button skinColorButton1 = _settings.Q<Button>("skin-color-button1");
		Button skinColorButton2 = _settings.Q<Button>("skin-color-button2");
		Button skinColorButton3 = _settings.Q<Button>("skin-color-button3");
		Button skinColorButton4 = _settings.Q<Button>("skin-color-button4");
		Button skinColorButton5 = _settings.Q<Button>("skin-color-button5");
		Button skinColorButton6 = _settings.Q<Button>("skin-color-button6");
		Button skinColorButton7 = _settings.Q<Button>("skin-color-button7");
		Button skinColorButton8 = _settings.Q<Button>("skin-color-button8");
		Button skinColorButton9 = _settings.Q<Button>("skin-color-button9");
		Button skinColorButton10 = _settings.Q<Button>("skin-color-button10");

		skinColorButton1.clicked += () => ColorButtonSelected(skinColorButton1, 1);
		skinColorButton2.clicked += () => ColorButtonSelected(skinColorButton2, 2);
		skinColorButton3.clicked += () => ColorButtonSelected(skinColorButton3, 3);
		skinColorButton4.clicked += () => ColorButtonSelected(skinColorButton4, 4);
		skinColorButton5.clicked += () => ColorButtonSelected(skinColorButton5, 5);
		skinColorButton6.clicked += () => ColorButtonSelected(skinColorButton6, 6);
		skinColorButton7.clicked += () => ColorButtonSelected(skinColorButton7, 7);
		skinColorButton8.clicked += () => ColorButtonSelected(skinColorButton8, 8);
		skinColorButton9.clicked += () => ColorButtonSelected(skinColorButton9, 9);
		skinColorButton10.clicked += () => ColorButtonSelected(skinColorButton10, 10);
	}

	private void OnEnable()
	{
		_playButton.clicked += PlayButtonClicked;
		_settingsButton.clicked += SettingsButtonClicked;
		_exitButton.clicked += ExitButtonClicked;

		_genderButtonGroup.value = _genderReference.Value;
		_genderButtonGroup.RegisterValueChangedCallback(GenderChangedCallback);
		_weightSlider.RegisterValueChangedCallback(WeightValueChangedCallback);

		Debug.Log($"MenuController: Start Menu with application version {Application.version}");
	}

	private void OnDisable()
	{
		_playButton.clicked -= PlayButtonClicked;
		_settingsButton.clicked -= SettingsButtonClicked;
		_exitButton.clicked -= ExitButtonClicked;

		_genderButtonGroup.UnregisterValueChangedCallback(GenderChangedCallback);
	}

	private void PlayButtonClicked()
	{
		onPlayClick.Invoke();
	}

	private void SettingsButtonClicked()
	{
		_bodyWrapper.Clear();
		_bodyWrapper.Add(_settings);
	}

	private void ReturnButtonClicked()
	{
		_bodyWrapper.Clear();
		_bodyWrapper.Add(_mainMenu);
	}

	private void ColorButtonSelected(Button skinColorButton, int colorIndex)
	{
		Debug.Log(string.Format("MenuController: Color button {0} selected.", colorIndex));
		// remove old selection
		if (_currentlySelectedButton != null)
		{
			ApplyColorToButton(_currentlySelectedButton, new Color(0.0705f, 0.3764f, 0.5019f));
		}
		_currentlySelectedButton = skinColorButton;
		_currentSkinColorReference.Value = GetColorFromIndex(colorIndex - 1);
		// Display new selection
		ApplyColorToButton(_currentlySelectedButton, new Color(0.5f, 0f, 0f));
	}

	private static void ApplyColorToButton(Button skinColorButton, StyleColor color)
	{
		skinColorButton.style.borderTopColor = color;
		skinColorButton.style.borderBottomColor = color;
		skinColorButton.style.borderLeftColor = color;
		skinColorButton.style.borderRightColor = color;
	}

	private void GenderChangedCallback(ChangeEvent<int> evt)
	{
		Debug.Log(string.Format("MenuController: Change gender to {0}.", evt.newValue));
		_genderReference.Value = evt.newValue;
	}

	private void WeightValueChangedCallback(ChangeEvent<float> evt)
	{
		_bodyWeightVariable.Value = evt.newValue;
	}

	private void ExitButtonClicked()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	/// <summary>
	/// Returns the color for a given index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	private Color GetColorFromIndex(int index)
	{
		// index check
		if (index < 0 || index >= _skinColors.Length)
		{
			Debug.LogError(string.Format("MenuController: Invalid color index {0}.", index));
			return Color.white;
		}
		return _skinColors[index].Value;
	}
}
