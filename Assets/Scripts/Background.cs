using UnityEngine;

// Created by SHAU - 2019
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

public class Background : MonoBehaviour {

	private Material material;
	private Camera mainCamera;

	void Start () {
		material = transform.GetComponent<Renderer>().material; //background material
		mainCamera = Camera.main;
		Keyboard.changeBackground += UpdateShaderBackground;
	}

	void Update() {
		//opposite camera
		Vector3 direction = -mainCamera.transform.position.normalized;
		transform.position = direction * 20; 
		transform.rotation = mainCamera.transform.rotation;
	}
	
	void UpdateShaderBackground(int backgroundId) {
		material.SetInt("_Background", backgroundId);
	}

	void OnDisable() {
		Keyboard.changeBackground -= UpdateShaderBackground;
	} 
}
