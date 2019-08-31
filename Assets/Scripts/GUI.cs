using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour {

	private const string title = "FORM-3D-Audio\n";
	private const string openHelp = "Press 'h' for Help";
	private const string helpText = "Keys 1,2,3 change background.\n" +
	                                "Keys 4,5 change shape.\n" +
									"Keys 6,7 change quality.\n" +
									"Key 'r' to reset.\n" +
									"Key 'h' to hide help text.";
	private static readonly string[] backgrounds = {"Background: Planes\n","Background: Kali Fractal\n","Background: Snow\n"};
	private static readonly string[] shapes = {"Shape: Cubes\n","Shape: Spheres\n"};
	private static readonly string[] qualities = {"Quality: Low\n", "Quality: High\n"};
	
	private Text guiText;
	private int currentBackgroundId = 1;
	private int currentShapeId = 1;
	private int currentQualityId = 1;
	private bool helpOpen = false;

	void Awake () {
		guiText = GetComponentInChildren<Text>();
		Keyboard.changeBackground += UpdateShaderBackground;
		Keyboard.changeShape += UpdateShape;
		Keyboard.changeQuality += UpdateQuality;		
		Keyboard.help += DisplayHelp;
	}

	void Start() {
	}
	
	void Update () {
		string message = title + backgrounds[currentBackgroundId - 1];
		message += shapes[currentShapeId - 1];
		message += qualities[currentQualityId - 1];
		message += helpOpen ? helpText : openHelp;
		guiText.text = message;
	}

	void OnDisable() {
		Keyboard.changeBackground -= UpdateShaderBackground;
		Keyboard.changeShape -= UpdateShape;
		Keyboard.changeQuality -= UpdateQuality;		
		Keyboard.help -= DisplayHelp;
	} 

	void UpdateShaderBackground(int backgroundId) {
		currentBackgroundId = backgroundId;
	}

	void UpdateShape(int shapeId) {
		currentShapeId = shapeId;
	}

	void UpdateQuality(int qualityId) {
		currentQualityId = qualityId;
	}

	void DisplayHelp() {
		helpOpen = !helpOpen;
	}
}
