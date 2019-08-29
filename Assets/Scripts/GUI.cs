using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour {

	private const string title = "FORM-3D-Audio\n";
	private const string openHelp = "Press 'h' for Help";
	private const string helpText = "Keys 1,2,3 change background.\n" +
									"Key 'r' to reset.\n" +
									"Key 'h' to hide help text.";
	private static readonly string[] backgrounds = {"Background: Planes\n","Background: Kali Fractal\n","Background: Snow\n"};
	
	private Text guiText;
	private int currentBackgroundId = 1;
	private bool helpOpen = false;

	void Awake () {
		guiText = GetComponentInChildren<Text>();
		Keyboard.changeBackground += UpdateShaderBackground;
		Keyboard.help += DisplayHelp;		
	}

	void Start() {
	}
	
	void Update () {
		string message = title + backgrounds[currentBackgroundId - 1];
		message += helpOpen ? helpText : openHelp;
		guiText.text = message;
	}

	void OnDisable() {
		Keyboard.changeBackground -= UpdateShaderBackground;
		Keyboard.help -= DisplayHelp;
	} 

	void UpdateShaderBackground(int backgroundId) {
		currentBackgroundId = backgroundId;
	}

	void DisplayHelp() {
		helpOpen = !helpOpen;
	}
}
