using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class FirstPMusicManager : MonoBehaviour {
	
	// for lining up objects and moving them with spectrum while playing song
	public GameObject spectrumBlock;
	public GameObject songManagerEmpty; 			// to instantiate for each song as parent of that song's spectrum
	
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
	private List<Song> songs = new List<Song>();	// all song objects in game right now
	private Song thisSong;							// current music playing

	// UI display song stats
	public UnityEngine.UI.Text songTimerText;
	private float songTimer = 0f;					// how long since song started - will get compared to the clip length
	public static float songLeft = 0f;				// how much time left in song
	private float songLength;


	/**
	 *	store information about each song and create an in-world spectrum representation of it 
	 */
	public class Song {
		GameObject songManager;
		AudioClip clip;
		float[] spectrum;
		int samples;
		List<GameObject> spectrumObjects;

		public Song (AudioClip track, GameObject spectrumBrick, int samplesCount, GameObject audioManager) {
			clip = track;
			samples = samplesCount;
			spectrumObjects = new List<GameObject>();

			songManager = Instantiate (audioManager, Vector3.zero, Quaternion.identity) as GameObject;
			songManager.GetComponent<AudioSource>().clip = clip;
			songManager.GetComponent<AudioSource>().Play ();

			// lineup blocks in spectrum from origin until placing total number of sample slices
			for (int i = 0; i < samples; i++) {
				spectrumObjects.Add (Instantiate (spectrumBrick, new Vector3 (i*0.2f, 0f, 0f), Quaternion.identity) as GameObject);
				spectrumObjects[i].transform.parent = songManager.transform;
			}

			// disable first and last blocks, since right now they just get 0 from spectrumData
			//spectrumObjects[spectrumObjects.Count-1].SetActive (false);
			//spectrumObjects[0].SetActive (false);
		}

		public void UpdateSpectrum () {
			spectrum = songManager.GetComponent<AudioSource>().GetSpectrumData (samples, 0, FFTWindow.Blackman);
			// go through each spectrum slice and move a corresponding object
			for (int i = 1; i < spectrum.Length-1; i++) {
				if (songManager.GetComponent<AudioSource>().isPlaying) {
					// change y-scale according to spectrum data, accounting for volume changes
					spectrumObjects[i].transform.localScale = Vector3.Lerp (spectrumObjects[i].transform.localScale, new Vector3 ( spectrumObjects[i].transform.localScale.x, 0.2f + 5f*spectrum[i]*i*Mathf.Log(spectrum[i]*spectrum[i]*i*i), 1f ), 9f * Time.deltaTime);
				// slowly stop moving objects if no music is playing
				} else {
					spectrumObjects[i].transform.localScale = Vector3.Lerp (spectrumObjects[i].transform.localScale, Vector3.one, Time.deltaTime);
				}
			}
		}

	}


	void Start () {
		// reset the music and UI to basic look and sound
		mixer.SetFloat ("MusicPitch", 1f);
		mixer.SetFloat ("MusicDistortion", 0f);
		mixer.SetFloat ("MusicVolume", 0f);
		songTimerText.color = Color.yellow;
		almostOver = false;
		

		// load and play first song
		thisSong = new Song (playlist[0], spectrumBlock, numSamples, songManagerEmpty);

		//GetComponent<AudioSource> ().clip = thisSong;
		//GetComponent<AudioSource> ().Play ();
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
		if (songLeft < 55f) {
			// static bool that song is almost over
			almostOver = true;
			songTimerText.color = Color.Lerp (songTimerText.color, Color.red, Time.deltaTime);
		}
		
		thisSong.UpdateSpectrum();
		
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
		if (Input.GetAxis ("Vertical") != 0f) {
			mixer.SetFloat ("MusicVolume", Mathf.Lerp(songVolume, Mathf.Clamp(songVolume+Input.GetAxis("Vertical"), -3f, 3f), 6f*Time.deltaTime));
		} else {
			mixer.SetFloat ("MusicVolume", Mathf.Lerp(songVolume, 0f, 6f*Time.deltaTime));
		}
		
	}

	/**
	 *	Music mixer variables 
	 */
	
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