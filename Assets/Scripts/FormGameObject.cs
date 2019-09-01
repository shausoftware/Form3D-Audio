using UnityEngine;

// Created by SHAU - 2019
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

public class FormGameObject : MonoBehaviour {

    private int branchId, leafId;
	private Vector3 targetPosition, targetScale;
    private PatternType currentPattern;
	private float startTime;
	private Material frameMaterial, coreMaterial;
	private AudioPlayer audioPlayer;

	void Start () {
		frameMaterial = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
		coreMaterial = transform.GetChild(1).gameObject.GetComponent<Renderer>().material;
		audioPlayer = FindObjectOfType<AudioPlayer>();
		Keyboard.changeBackground += UpdateShaderBackground;
	}
	
 	void Update () {
		UpdateTargetPosition(); 
	    Animate();	 
	}

	void OnDisable() {
		Keyboard.changeBackground -= UpdateShaderBackground;
	}

	public void InitFormObject(int branchId, int leafId) {
		this.branchId = branchId;
		this.leafId = leafId;
	}

	public void UpdatePattern(PatternType pattern, Vector3 targetPosition, Vector3 targetScale) {
		currentPattern = pattern;
		this.targetPosition = targetPosition;
		this.targetScale = targetScale;
		startTime = Time.time;
	}

	public void UpdateShaderBackground(int backgroundId) {
		frameMaterial.SetInt("_Background", backgroundId);
		coreMaterial.SetInt("_Background", backgroundId);
	}

	public void Reset() {
		transform.position = Vector3.zero;
	}

    private void Animate() {

		float db = audioPlayer.GetDb();
		float rms = audioPlayer.GetRms();
		float hz = audioPlayer.GetHz();		
		float avs = 1 + Mathf.Pow(rms*2.4f, 4) * ((hz>80) ? 1 : 0);

		//position
        float targetDistance = Vector3.Distance(transform.position, targetPosition * avs);
		transform.position = Vector3.Lerp(transform.position, targetPosition * avs, 0.4f/targetDistance);
		//scale
        float targetScaleDelta = Vector3.Distance(transform.localScale, targetScale);
		transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.1f/targetScaleDelta);
		//rotation
		transform.Rotate(new Vector3(Time.deltaTime*leafId*4, Time.deltaTime*branchId*5, Time.deltaTime*-2*leafId));
		//shader
		coreMaterial.SetVector("_FormObject", new Vector4(transform.position.x, transform.position.y, transform.position.z));
        coreMaterial.SetVector("_Animation", new Vector4(db, hz, branchId, leafId));
	}

	private void UpdateTargetPosition() {
		
		float elapsedTime = (Time.time - startTime) * 0.3f;

		switch (currentPattern) {
			case PatternType.SHELL: {
				targetPosition = Quaternion.Euler(0,0, Mathf.Cos(elapsedTime) * 0.6f*branchId) * targetPosition; 
				break;
			}
			case PatternType.LOXODROME: {
				targetPosition = Quaternion.Euler(0, Mathf.Cos(elapsedTime) * 0.3f*leafId,0) * targetPosition; 
			    break;
			}
			case PatternType.SQUID: {
				targetPosition.z += leafId*0.008f * Mathf.Sin(elapsedTime*3);
				targetPosition = Quaternion.Euler(0,0, leafId*0.2f*Mathf.Cos(elapsedTime*2.5f)) * targetPosition;
				targetPosition = Quaternion.Euler(0, Mathf.Cos(elapsedTime) * 0.1f*leafId, 0) * targetPosition; 
			    break;
			}
			case PatternType.RING: {
				float a = Mathf.Cos(elapsedTime*0.8f) * 0.2f*leafId * Mathf.Sign((leafId%2) -1);
				float b = Mathf.Cos(elapsedTime*1.2f) * 0.4f*branchId;
				targetPosition = Quaternion.Euler(0,0,a) * targetPosition; 
				targetPosition = Quaternion.Euler(0,b,0) * targetPosition; 
			    break;
			}
			default: {
				break;
			}
		}
	}
}
