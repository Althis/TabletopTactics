using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.World;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]

public class UnitSoundHandler : MonoBehaviour {

	public AudioSource audioSource;
	public AudioClip cavalryHitAudio;
	public AudioClip infantryHitAudio;
	public AudioClip artillaryHitAudio;
	public AudioClip deathAudio;
	public static Sing singleton;
	public class Sing{
		
		AudioSource audioSource;
		AudioClip cavalryHitAudio;
		AudioClip infantryHitAudio;
		AudioClip artillaryHitAudio;
		AudioClip deathAudio;
		int artillarySoundsBeingPlayed;
		int infantrySoundsBeingPlayed;
		int cavalrySoundsBeingPlayed;

		public void playHitAudio(Unit unit){
			switch (unit.Type) {
			case Unit.ClassType.Cavalry:
				audioSource.PlayOneShot (cavalryHitAudio);
				break;
			case Unit.ClassType.Infantry:
				audioSource.PlayOneShot (infantryHitAudio, (float)0.3);
				break;
			case Unit.ClassType.Artillary:
				audioSource.PlayOneShot (artillaryHitAudio);
				break;
			default:
				break;
			}
		}
		public void playDeathAudio(){
			audioSource.PlayOneShot (deathAudio);
		}

			public Sing (AudioSource audioSource, AudioClip cavalryHitAudio, AudioClip infantryHitAudio, AudioClip artillaryHitAudio, AudioClip deathAudio){
			this.audioSource=audioSource;
			this.cavalryHitAudio=cavalryHitAudio;
			this.infantryHitAudio = infantryHitAudio;
			this.artillaryHitAudio = artillaryHitAudio;
			this.deathAudio= deathAudio;
			artillarySoundsBeingPlayed=0;
			infantrySoundsBeingPlayed=0;
			cavalrySoundsBeingPlayed=0;
		}
	}

	void Awake()
	{
		if (UnitSoundHandler.singleton == null) {
			singleton = new Sing (audioSource, cavalryHitAudio, infantryHitAudio, artillaryHitAudio, deathAudio);
		}
	}



}
