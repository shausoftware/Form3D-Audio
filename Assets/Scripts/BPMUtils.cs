using System.Collections;
using System.Collections.Generic;

public class BPMUtils {
	public static float GetDuration(float bpm, int nBeats) {
		return 1/(bpm/60) * nBeats;
	}	 
}
