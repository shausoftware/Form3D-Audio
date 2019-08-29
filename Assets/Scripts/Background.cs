using UnityEngine;

public class Background : MonoBehaviour {

	private Material material;

	void Start () {
		material = transform.GetComponent<Renderer>().material; //background material
		Keyboard.changeBackground += UpdateShaderBackground;
	}
	
	void UpdateShaderBackground(int backgroundId) {
		material.SetInt("_Background", backgroundId);
	}

	void OnDisable() {
		Keyboard.changeBackground -= UpdateShaderBackground;
	} 
}
