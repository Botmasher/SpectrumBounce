using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {

	// for lining up objects and moving them with spectrum while playing song
	public GameObject spectrumBlock;
	private List<GameObject> spectrumBlocks = new List<GameObject>();
	public float spectrumXOrigin;					// where to start laying blocks, from left to right along world x-axis

	// for listening to, behaving with and FXing music
	public AudioMixer mixer;						// main mixer in scene
	public AudioMixerGroup masterTrack;				// all channels
	public AudioMixerGroup musicTrack;				// music channels
	[Range (64, 8192)] public int numSamples;		// break spectrum data into n sample slices (spectrumdata allows min=64,max=8192)
	private float[] currentSpectrumData;			// array to hold spectrum data for current frame
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

		// lineup blocks in spectrum from origin until placing total number of sample slices
		for (int i = 0; i < numSamples; i++) {
			spectrumBlocks.Add (Instantiate (spectrumBlock, new Vector3 (spectrumXOrigin+i*0.2f, -3f, 0f), Quaternion.identity) as GameObject);
		}

		// load and play first song
		song = playlist[1];
		GetComponent<AudioSource> ().clip = song;
		GetComponent<AudioSource> ().Play ();
		songLength = GetComponent<AudioSource> ().clip.length;

		// disable first and last blocks, since right now they just get 0 from spectrumData
		spectrumBlocks[spectrumBlocks.Count-1].SetActive (false);
		spectrumBlocks[0].SetActive (false);
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
		MoveSpectrumWithMusic ();

		// allow input to switch up songs
		if (Input.GetButtonDown ("Fire2")) {
			// reset time since song started
			songTimer = 0f;
			// adjust audiosource for this song
			GetComponent<AudioSource> ().clip = playlist[Random.Range(0, playlist.Count)];
			GetComponent<AudioSource> ().Play ();
			songLength = GetComponent<AudioSource> ().clip.length;
		}

		// allow y input to change volume
//		if (Input.GetAxis ("Vertical") != 0f) {
//			mixer.SetFloat ("MusicVolume", Mathf.Lerp(songVolume, Mathf.Clamp(songVolume+Input.GetAxis("Vertical"), -3f, 3f), 6f*Time.deltaTime));
//		} else {
//			mixer.SetFloat ("MusicVolume", Mathf.Lerp(songVolume, 0f, 6f*Time.deltaTime));
//		}

	}
	

	/**
	 * 	Calculate music spectrum across a given number of samples and manipulate the same number of bars using spectrum data
	 */
	void MoveSpectrumWithMusic () {
		// grab the spectrum data for this component's track
		currentSpectrumData = GetComponent<AudioSource> ().GetSpectrumData (numSamples, 0, FFTWindow.Blackman);
		//currentSpectrumData = GetComponent<AudioSource> ().GetOutputData(numSamples, 0);

		// go through each spectrum slice and move a corresponding object
		for (int i = 1; i < currentSpectrumData.Length-1; i++) {
			// move along with the music if it's playing
			if (GetComponent<AudioSource>().isPlaying) {

				// change scale according to getoutputdata instead of getspectrumdata
				//spectrumBlocks[i].transform.localScale = Vector2.Lerp (spectrumBlocks[i].transform.localScale, new Vector2 ( spectrumBlocks[i].transform.localScale.x, 5f*currentSpectrumData[i]), 9f * Time.deltaTime);

//				// change position according to spectrum data
//				if (songVolume > 0f) {
//					spectrumBlocks[i].transform.position = Vector3.Lerp (spectrumBlocks[i].transform.position, new Vector3 (spectrumBlocks[i].transform.position.x, 1f + currentSpectrumData[i]*(1+songVolume) * i + Mathf.Log (currentSpectrumData[i]*i), spectrumBlocks[i].transform.position.z), 8f * Time.deltaTime);
//				} else if (songVolume < 0f) {
//					spectrumBlocks[i].transform.position = Vector3.Lerp (spectrumBlocks[i].transform.position, new Vector3 (spectrumBlocks[i].transform.position.x, 1f + currentSpectrumData[i]/(1+Mathf.Abs(songVolume)) * i + Mathf.Log (currentSpectrumData[i]*i), spectrumBlocks[i].transform.position.z), 8f * Time.deltaTime);
//				} else {
//					spectrumBlocks[i].transform.position = Vector3.Lerp (spectrumBlocks[i].transform.position, new Vector3 (spectrumBlocks[i].transform.position.x, 1f + 0.04f * i + Mathf.Log (currentSpectrumData[i]*i), spectrumBlocks[i].transform.position.z), 8f * Time.deltaTime);
//				}

				// change y-scale according to spectrum data, accounting for volume changes
				if (songVolume > 0f) {
					spectrumBlocks[i].transform.localScale = Vector2.Lerp (spectrumBlocks[i].transform.localScale, new Vector2 ( spectrumBlocks[i].transform.localScale.x, 0.2f + 5f*(currentSpectrumData[i]*(1+songVolume))*i*Mathf.Log(currentSpectrumData[i]*currentSpectrumData[i]*i*i) ), 9f * Time.deltaTime);
				} else if (songVolume < 0f) {
					spectrumBlocks[i].transform.localScale = Vector2.Lerp (spectrumBlocks[i].transform.localScale, new Vector2 ( spectrumBlocks[i].transform.localScale.x, 0.2f + 5f*(currentSpectrumData[i]/(1+Mathf.Abs(songVolume)))*i*Mathf.Log(currentSpectrumData[i]*currentSpectrumData[i]*i*i) ), 9f * Time.deltaTime);
				} else {
					spectrumBlocks[i].transform.localScale = Vector2.Lerp (spectrumBlocks[i].transform.localScale, new Vector2 ( spectrumBlocks[i].transform.localScale.x, 0.2f + 5f*currentSpectrumData[i]*i*Mathf.Log(currentSpectrumData[i]*currentSpectrumData[i]*i*i) ), 9f * Time.deltaTime);
				}
				//spectrumBlocks[i].transform.localScale = Vector2.Lerp (spectrumBlocks[i].transform.localScale, new Vector2 ( spectrumBlocks[i].transform.localScale.x, 0.2f + 5f*currentSpectrumData[i]*Mathf.Log(currentSpectrumData[i]/i) ), 10f * Time.deltaTime);
		
			// slowly stop moving objects if no music is playing
			} else {
				//spectrumBlocks[i].transform.localScale = Vector3.Lerp (spectrumBlocks[i].transform.localScale, Vector3.one, Time.deltaTime);
			}
		}
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