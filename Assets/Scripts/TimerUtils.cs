using UnityEngine;

// Created by SHAU - 2019
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

public class TimerUtils {

    private float timer;
	private float animationInterval;
    private int callCount = 0;

	public TimerUtils(float animationInterval) {
		this.animationInterval = animationInterval;
        callCount = 0;
	}

	public bool ReadyToUpdate() {
        callCount++;
        if (callCount == 1) return true;
        if (timer>0.0) {
            timer -= Time.deltaTime;
        } else {
            timer = animationInterval;
            return true;
        }
        return false;
	}
}
