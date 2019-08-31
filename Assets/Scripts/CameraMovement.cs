using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float BPM = 140;

    private TimerUtils timerUtils;
	private Vector3 targetDirection = Vector3.zero;
	private AudioPlayer audioPlayer;

	void Start() {
		timerUtils  = new TimerUtils(BPMUtils.GetDuration(BPM, 8));
		audioPlayer = FindObjectOfType<AudioPlayer>();
	}

	void Update () {
		//change camera rotation direction
		if (timerUtils.ReadyToUpdate()) {
            NewDirection();
		}

		//camera wobble
		Vector3 bounceDirection = transform.position.normalized;
		float rms = audioPlayer.GetRms();
		float at32 = Time.time % BPMUtils.GetDuration(BPM, 32);
		float wobble = Mathf.Sin(at32*28) * rms*0.6f;
        wobble += Mathf.Sin(at32*28)*0.8f * //frequency and amplitude 
		          Mathf.Max(1 - at32*0.5f, 0); //decay

		transform.Translate(bounceDirection*wobble);
		transform.RotateAround(Vector3.zero, targetDirection, 20 * Time.deltaTime);
		transform.LookAt(Vector3.zero);
	}

	public Vector3 GetTargetDirection() {
		return targetDirection;
	}

	private void NewDirection() {
		targetDirection= new Vector3(NewRandom(), 
		                             NewRandom(),
						             NewRandom());
	} 

	/* Always want at least a bit of camera movement */
	private float NewRandom() {
		float nr = 0;
		while (Mathf.Abs(nr) < 0.3) {
			nr = Random.Range(-2, 2);
		}
		return nr;
	}
}
