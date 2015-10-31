using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class DribbleTestMusicManager : MonoBehaviour {
	
	// for listening to, behaving with and FXing music
	public AudioMixer mixer;						// main mixer in scene
	public AudioMixerGroup masterTrack;				// all channels
	public AudioMixerGroup musicTrack;				// music channels
	[Range (64, 8192)] public int numSamples;		// break spectrum data into n sample slices (spectrumdata allows min=64,max=8192)
	public static float[] currentSpectrumData;		// array to hold spectrum data for current frame
	private float songPitch;						// used to get pitch of the song this frame
	private float songVolume;						// used to get volume of the song this frame
	private float songDistortion;					// used to get distortion level of the song this frame
	
	// for notifying everyone else what's going on with the song
	public static bool almostOver;
	
	// music to play through mixer
	public List<AudioClip> playlist = new List<AudioClip>();
	private AudioClip song;							// current music playing
	
	// UI display song stats
	public UnityEngine.UI.Text songTimerText;
	private float songTimer = 0f;					// how long since song started - will get compared to the clip length
	public static float songLeft = 0f;				// how much time left in song
	private float songLength;
	
	
	void Start () {
		// reset the music and UI to basic look and sound
		mixer.SetFloat ("MusicPitch", 1f);
		mixer.SetFloat ("MusicDistortion", 0f);
		mixer.SetFloat ("MusicVolume", 0f);
		songTimerText.color = Color.yellow;
		almostOver = false;
		
		// ensure samples is an even integer between 64 and 8192 as expected by spectrumdata
		numSamples = Mathf.Clamp (numSamples, 64, 8192);
		if (numSamples%2 == 1) {
			numSamples += 1;
		}
		
		// initialize spectrumdata array to how many samples we'll take each frame
		currentSpectrumData = new float[numSamples];
		
		// load and play first song
		song = playlist[0];
		GetComponent<AudioSource> ().clip = song;
		GetComponent<AudioSource> ().Play ();
		songLength = GetComponent<AudioSource> ().clip.length;

	}
	
	
	void Update () {
		// make current music calculations
		songTimer += Time.deltaTime;
		mixer.GetFloat ("MusicPitch", out songPitch);
		mixer.GetFloat ("MusicVolume", out songVolume);
		mixer.GetFloat ("MusicDistortion", out songDistortion);
		
		// calculate time remaining, factoring in how the pitch/tempo fx distort the time remaining
		songLeft = (songLength - songTimer) / songPitch >= 0f ? (songLength - songTimer) / songPitch : 0.0001f;
		
		// Update UI text with formatted song timer countdown
		string newText = System.TimeSpan.FromSeconds(songLeft).ToString();
		if (newText == "00:00:00") {
			songTimerText.text = "00:00";
		} else if (newText.Length < 10) {
			// we ran into a length calculation error - do not update the text this frame!
		} else {
			songTimerText.text = newText.Substring(newText.IndexOf(":")+1, newText.LastIndexOf(".")-3);
		}
		
		// make some UI changes during last minute of song
		if (songLeft < 60f) {
			// static bool that song is almost over
			almostOver = true;
			songTimerText.color = Color.Lerp (songTimerText.color, Color.red, Time.deltaTime);
		}
		
		// call function to adjust spectrum this frame
		currentSpectrumData = GetComponent<AudioSource> ().GetSpectrumData (numSamples, 0, FFTWindow.Blackman);

		// allow input to switch up songs
		if (Input.GetButtonDown ("Fire2")) {
			// reset time since song started
			songTimer = 0f;
			// adjust audiosource for this song
			GetComponent<AudioSource> ().clip = playlist[Random.Range(0, playlist.Count)];
			GetComponent<AudioSource> ().Play ();
			songLength = GetComponent<AudioSource> ().clip.length;
		}
		
	}

	/**
	 * 	Determine the average of this music's static spectrum data between two given samples in the spectrum
	 * 
	 * 	/!\	Does NOT verify that samples are within current spectrum samples taken
	 * 			e.g. this could try to take slices 64 to 128 of a 64 sample spectrum!
	 */
	public static float SpectrumAverage (int startSlice, int endSlice) {
		float avg = 0f;
		for (int i=startSlice; i < endSlice; i++) {
			avg += currentSpectrumData[i];
		}
		return avg/(endSlice-startSlice);
	}


	// music slowly goes to pitch 0 and kills the tune - the sound of failure
	public void PitchDown () {
		// check for pitch - currently already being done in Update
		//mixer.GetFloat ("MusicPitch", out songPitch);
		mixer.SetFloat ("MusicPitch", songPitch - (0.5f*Time.deltaTime));
	}
	
	// music gets distorted - called by levelmanager and boss scripts
	public void DistortionUp () {
		mixer.SetFloat ("MusicDistortion", Mathf.Lerp (songDistortion, 0.8f, 2f*Time.deltaTime));
	}
	
}