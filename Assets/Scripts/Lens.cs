using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lens : MonoBehaviour {

	private Material material;
	private float countdown;

	void Awake() {
		material = transform.GetComponent<Renderer>().material; //lens material
		Keyboard.changeBackground += UpdateShaderBackground;
		Keyboard.reset += Reset;
	}

	void Start () {
	}
	
	void Update () {
		if (countdown>0) {
			material.SetFloat("_GlitchAmount", countdown);
			countdown -= Time.deltaTime*1.0f;
		}
	}

	void OnDisable() {
		Keyboard.changeBackground -= UpdateShaderBackground;
		Keyboard.reset -= Reset;
	} 

	void UpdateShaderBackground(int backgroundId) {
		countdown = 1.0f;
	}

	void Reset() {
		UpdateShaderBackground(0);
	}
}
