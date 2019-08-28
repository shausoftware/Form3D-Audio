using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private TimerUtils timerUtils = new TimerUtils(6.857142f); //chane camera direction every 8 beats at 140 BPM
	private Vector3 targetDirection = Vector3.zero;
	private AudioPlayer audioPlayer;
	private GameObject background;

	void Start() {
		transform.LookAt(Vector3.zero);
		audioPlayer = FindObjectOfType<AudioPlayer>();
		background = GameObject.Find("Background");
	}

	void Update () {
		if (timerUtils.ReadyToUpdate()) {
            NewDirection();
		}

		Vector3 bounceDirection = transform.position.normalized;
		float db = audioPlayer.GetDb();
		float hz = audioPlayer.GetHz();		
		float at64 = Time.time % 27.4285753f; //64 beats
		float st = Mathf.Max(at64 - 13.7142877f, 0); //start time
        float wobble = Mathf.Sin(at64*28) * 1.0f * //frequency and amplitude 
		               Mathf.Min(1, st) * //start time ramps up to 1
		               Mathf.Max(1 - st*0.6f, 0); //decay
		//TODO: multiply bu amplitude
		transform.Translate(bounceDirection*wobble);
		transform.RotateAround(Vector3.zero, targetDirection, 20 * Time.deltaTime);
		background.transform.RotateAround(Vector3.zero, targetDirection, 20 * Time.deltaTime);
		transform.LookAt(Vector3.zero);
	}

	public Vector3 GetTargetDirection() {
		return targetDirection;
	}

	private void NewDirection() {
		targetDirection= new Vector3(Random.Range(-1f, 1f), 
		                             Random.Range(-1f, 1f),
						             Random.Range(-1f, 1f));
	} 
}
