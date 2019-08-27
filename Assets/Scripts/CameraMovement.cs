using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [Range(1f, 20f)]
    public float speed = 1.0f;

    private TimerUtils timerUtils = new TimerUtils(8.0f);
	private Vector3 targetDirection = Vector3.zero;

	void Update () {
		if (timerUtils.ReadyToUpdate()) {
            NewDirection();
		}
		transform.Rotate(targetDirection);
	}

	private void NewDirection() {
		targetDirection= new Vector3(Random.Range(-1f, 1f), 
		                             Random.Range(-1f, 1f),
						             Random.Range(-1f, 1f));
	} 
}
