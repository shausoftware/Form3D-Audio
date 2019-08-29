using UnityEngine;

public class FormGameObject : MonoBehaviour {

    private int branchId;
	private int leafId;
	private Vector3 targetPosition;
	private Vector3 targetScale;
    private PatternType currentPattern;
	private float startTime;
	private Material frameMaterial;
	private Material coreMaterial;
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
		float bassTreble = (hz<200.0f) ? 1 : 0; //treble  = 1 - bassTreble
		
		float avs = 1.0f;
		if (branchId%2 == 0) {
	        avs += Mathf.Pow(rms*1.2f,4) * bassTreble;
			//avs += Mathf.Clamp(db*0.5f, 0, 1) * 0.05f * bass;
		} else {
		}

		//position
        float targetDistance = Vector3.Distance(transform.position, targetPosition * avs);
		transform.position = Vector3.Lerp(transform.position, targetPosition * avs, 0.4f/targetDistance);
		//scale
        float targetScaleDelta = Vector3.Distance(transform.localScale, targetScale);
		transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.1f/targetScaleDelta);
		//rotation
		transform.Rotate(new Vector3(Time.deltaTime*leafId*4f, Time.deltaTime*branchId*5f, Time.deltaTime*-2f*leafId));
		//shader
		coreMaterial.SetVector("_FormObject", new Vector4(transform.position.x, transform.position.y, transform.position.z));
        coreMaterial.SetVector("_Animation", new Vector4(db, hz, branchId, leafId));
	}

	private void UpdateTargetPosition() {
		float elapsedTime = (Time.time - startTime) * 0.3f;
		switch (currentPattern) {
			case PatternType.SHELL: {
				targetPosition = Quaternion.Euler(0f, 0f, Mathf.Cos(elapsedTime) * 0.6f * (float) branchId) * targetPosition; 
				break;
			}
			case PatternType.LOXODROME: {
				targetPosition = Quaternion.Euler(0f, Mathf.Cos(elapsedTime) * 0.4f * (float) leafId, 0f) * targetPosition; 
			    break;
			}
			case PatternType.SQUID: {
				targetPosition.z += leafId*0.008f * Mathf.Sin(elapsedTime*3f);
				targetPosition = Quaternion.Euler(0f, 0f, leafId*0.2f*Mathf.Cos(elapsedTime*2.5f)) * targetPosition;
				targetPosition = Quaternion.Euler(0f, Mathf.Cos(elapsedTime) * 0.1f * (float) leafId, 0f) * targetPosition; 
			    break;
			}
			case PatternType.RING: {
				float a = Mathf.Cos(elapsedTime) * 0.1f * (float) leafId;
				if ((float) leafId % 2f == 0f) {
                    a *= -1f; 
				}
				targetPosition = Quaternion.Euler(0f, 0f, a) * targetPosition; 
			    break;
			}
			case PatternType.GRID: {
				targetPosition.z += (Mathf.Sin((float) leafId + elapsedTime*2f) +
				           			 Mathf.Sin((float) branchId + elapsedTime*2f)) * 0.02f;
			    break;
			}
			default: {
				break;
			}
		}
	}
}
