using UnityEngine;

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
		float bassTreble = (hz<200.0f) ? 1 : 0; //treble  = 1 - bassTreble
		
		float avs = 1.0f;
		if (branchId%2 == 0) {
	        //avs += Mathf.Pow(rms*2.0f,6) * bassTreble;
			//avs += Mathf.Clamp(db*0.5f, 0, 1) * 0.05f * bass;
		} else {
		}

		//position
        float targetDistance = Vector3.Distance(transform.position, targetPosition);
		transform.position = Vector3.Lerp(transform.position, targetPosition, 0.4f/targetDistance);
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
		float rms = audioPlayer.GetRms();
		float hz = audioPlayer.GetHz();
		float bassTreble = (hz<200) ? 1 : 0; //treble  = 1 - bassTreble

		switch (currentPattern) {
			case PatternType.SHELL: {
				targetPosition = Quaternion.Euler(0,0, Mathf.Cos(elapsedTime) * 0.6f*branchId) * targetPosition; 
				break;
			}
			case PatternType.LOXODROME: {
				targetPosition = Quaternion.Euler(0, Mathf.Cos(elapsedTime) * 0.4f*leafId,0) * targetPosition; 
			    break;
			}
			case PatternType.SQUID: {
				targetPosition.z += leafId*0.008f * Mathf.Sin(elapsedTime*3);
				targetPosition = Quaternion.Euler(0,0, leafId*0.2f*Mathf.Cos(elapsedTime*2.5f)) * targetPosition;
				targetPosition = Quaternion.Euler(0, Mathf.Cos(elapsedTime) * 0.1f*leafId, 0) * targetPosition; 
			    break;
			}
			case PatternType.RING: {
				float a = Mathf.Cos(elapsedTime) * 0.2f*leafId * Mathf.Sign((leafId%2) -1);
				float b = Mathf.Cos(elapsedTime*3.2f) * 0.4f*branchId;
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
