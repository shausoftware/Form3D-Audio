using System.Collections;
using System.Collections.Generic;

// Created by SHAU - 2019
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

public class BPMUtils {
	public static float GetDuration(float bpm, int nBeats) {
		return 1/(bpm/60) * nBeats;
	}	 
}
