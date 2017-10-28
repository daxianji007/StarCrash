using UnityEngine;
using Assets.Script;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

    public AudioSource BackgroundSource;
    public AudioSource AudioEffect;
    
    public AudioClip Selected;
    public AudioClip DeSelect;
    public AudioClip Flash;
    public AudioClip Scoring;
    public AudioClip Moving;
    public AudioClip Growing;
    public AudioClip Exposing;
    public AudioClip Over;

    private Dictionary<Audio, AudioClip> AudioMap;
    private Dictionary<Audio, float> VolumnMap;
    private Dictionary<string, bool> IdMap;

    private bool pendingAudio;
    private float initVolumn;

    public enum Audio
    {
        SELECTED,
        DESELECT,
        FLASH,
        SCORING,
        MOVING,
        GROWING,
        EXPOSING,
        OVER
    }

    // Use this for initialization
    void Start () {
        IdMap = new Dictionary<string, bool>();
        AudioMap = new Dictionary<Audio, AudioClip>();
        VolumnMap = new Dictionary<Audio, float>();
        AudioMap[Audio.SELECTED] = Selected;    VolumnMap[Audio.SELECTED] = 1.0f;
        AudioMap[Audio.DESELECT] = DeSelect;    VolumnMap[Audio.DESELECT] = 1.0f;
        AudioMap[Audio.FLASH] = Flash;          VolumnMap[Audio.FLASH] = 1.0f;
        AudioMap[Audio.SCORING] = Scoring;      VolumnMap[Audio.SCORING] = 0.5f;
        AudioMap[Audio.MOVING] = Moving;        VolumnMap[Audio.MOVING] = 1.0f;
        AudioMap[Audio.GROWING] = Growing;      VolumnMap[Audio.GROWING] = 1.0f;
        AudioMap[Audio.EXPOSING] = Exposing;    VolumnMap[Audio.EXPOSING] = 1.0f;
        AudioMap[Audio.OVER] = Over;            VolumnMap[Audio.OVER] = 1.0f;
        pendingAudio = false;
        initVolumn = AudioEffect.volume;
    }
	
	// Update is called once per frame
	void Update () {
	    if(pendingAudio && !AudioEffect.isPlaying)
        {
            pendingAudio = false;
            StartAudioEffect(Audio.SCORING);
        }
	}

    public void StartBackgroundAudio()
    {
        if(PlayerPrefs.HasKey("Audio") && PlayerPrefs.GetInt("Audio") == 0)
        {
            if (!BackgroundSource.isPlaying)
            {
                BackgroundSource.Play();
            }
        }
        else
        {
            BackgroundSource.Stop();
        }        
    }

    public void StartAudioEffect(Audio audio, float delayMs = 0, string id=null)
    {
        //Disable audio effect
        if (!AudioMap.ContainsKey(audio))
        {
            return;
        }

        //Check if audio with id is triggered
        if(id != null)
        {
            if (IdMap.ContainsKey(id))
            {
                return;
            }
            else
            {
                IdMap[id] = true;

            }
        }

        //Play audio
        if (PlayerPrefs.HasKey("Audio") && PlayerPrefs.GetInt("Audio") == 0)
        {
            AudioEffect.clip = AudioMap[audio];
            if (audio == Audio.EXPOSING)
            {
                AudioEffect.time = 0.1f;
            }
            AudioEffect.volume = initVolumn * VolumnMap[audio];
            AudioEffect.PlayDelayed(delayMs / 1000f);
        }
    }

    public void PendingScoreAudio()
    {
        pendingAudio = true;
    }

    public void ClearId()
    {
        IdMap.Clear();
    }

    public void StopAudioEffect()
    {
        AudioEffect.Stop();
    }
}
