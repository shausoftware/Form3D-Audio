using UnityEngine;

public class AudioPlayer : MonoBehaviour {

	private const int SAMPLES = 512;
    private const float REF_VALUE = 0.1f; // RMS value for DB
    private const float THRESHOLD = 0.02f;

	public Sound sound;

	private float db;
	private float rms;
	private float hz;
	private float sampleRate;

	void Awake () {
		sound.source = gameObject.AddComponent<AudioSource>();
		sound.source.clip = sound.clip;
		sound.source.volume = sound.volume;
		sound.source.loop = true;
	}

	void Start() {
		if (sound == null) {
			Debug.LogWarning("Audio file not found.");
		} else {
			sound.source.Play();
		}
		sampleRate = AudioSettings.outputSampleRate;
	}	
	
	// Update is called once per frame
	void Update () {
		if (sound != null) {
			AnalyseAudio();
		}
	}

	public float GetRms() {
		return rms;
	}

	public float GetDb() {
		return db;
	}

	public float GetHz() {
		return hz;
	}

	private float maxDB = -160;
	private float maxHz = 0;

	//https://answers.unity.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
	private void AnalyseAudio() {

		//amplitude
		float[] samples = new float[SAMPLES];
		sound.source.GetOutputData(samples, 0); // fill array with samples
        float sum = 0;
        for (int i = 0; i < SAMPLES; i++) {
             sum += samples[i]*samples[i]; // sum squared samples
        }
		rms = Mathf.Sqrt(sum/SAMPLES); // rms = square root of average
     	db = 20*Mathf.Log10(rms/REF_VALUE); // calculate dB
     	if (db < -160) db = -160; // clamp it to -160dB min

		//frequency
		float[] spectrum = new float[SAMPLES];
		sound.source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float maxV = 0;
		int maxN = 0;
		for (int i = 0; i < SAMPLES; i++) { // find max 
            if (spectrum[i] > maxV && spectrum[i] > THRESHOLD){
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
		}
		float freqN = maxN; // pass the index to a float variable
		if (maxN > 0 && maxN < SAMPLES-1){ // interpolate index using neighbours
         	float dL = spectrum[maxN-1]/spectrum[maxN];
         	float dR = spectrum[maxN+1]/spectrum[maxN];
         	freqN += 0.5f*(dR*dR - dL*dL);
		}
		hz = freqN*(sampleRate/2)/SAMPLES; // convert index to frequency

		/* 
        if (db>maxDB) { 
			maxDB = db;
		}
		if (hz>maxHz) {
			maxHz = hz;
		}
		Debug.Log("DB:"+db+" maxDB:"+maxDB);
		Debug.Log("Hz:"+hz+" maxHz:"+maxHz);
		//*/
	}
}
