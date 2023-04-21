using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public interface IAudioManager
{
    void PlayOneShotSound(AudioClip audioClipToPlay);
    void PlayDefaultMusic();

    void ToggleMusicMute();
    void ToggleSFXMute();

    public bool MusicMuted { get; }
    public bool SFXMuted { get; }
}

public class AudioManager : MonoBehaviour, IAudioManager
{
    public AudioClip deafultMusicToPlay;
    AudioSource musicAudioSource;
    AudioSource sfxAudioSource;

    public AudioMixer audioMixer;
    public AudioMixerGroup musicAudioMixerGroup;
    public AudioMixerGroup sfxAudioMixerGroup;

    bool musicMuted;
    public bool MusicMuted { get { return musicMuted; } }

    bool sfxMuted;
    public bool SFXMuted { get { return sfxMuted; } }

    float defaultMusicAudioMixerVolume;
    float defaultSFXAudioMixerVolume;

    private void Start()
    {
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.playOnAwake = false;
        sfxAudioSource.playOnAwake = false;

        musicAudioSource.outputAudioMixerGroup = musicAudioMixerGroup;
        sfxAudioSource.outputAudioMixerGroup = sfxAudioMixerGroup;

        LoadAudioSettings();

        PlayDefaultMusic();
    }

    public void LoadAudioSettings() 
    {
        audioMixer.GetFloat("MusicVolume", out defaultMusicAudioMixerVolume);
        audioMixer.GetFloat("SFXVolume", out defaultSFXAudioMixerVolume);

        musicMuted = PlayerPrefs.GetFloat("MusicMuted") == 1;
        sfxMuted = PlayerPrefs.GetFloat("SFXMuted") == 1;

        if (musicMuted)
        {
            audioMixer.SetFloat("MusicVolume", -80);
        }
        else 
        {
            audioMixer.SetFloat("MusicVolume", defaultMusicAudioMixerVolume);
        }

        if (sfxMuted)
        {
            audioMixer.SetFloat("SFXVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", defaultSFXAudioMixerVolume);
        }

    }

    public void PlayDefaultMusic() 
    {
        if (deafultMusicToPlay != null)
        {
            musicAudioSource.clip = deafultMusicToPlay;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }

    public void PlayMusic(AudioClip audioClipToPlay) 
    {
        if (musicAudioSource.isPlaying) 
        {
            musicAudioSource.Stop();
        }

        musicAudioSource.clip = audioClipToPlay;
        musicAudioSource.Play();
    }

    public void PlayOneShotSound(AudioClip audioClipToPlay) 
    {
        sfxAudioSource.PlayOneShot(audioClipToPlay);
    }

    public void ToggleMusicMute() 
    {
        musicMuted = !musicMuted;

        if (musicMuted)
        {
            audioMixer.SetFloat("MusicVolume", -80);
            PlayerPrefs.SetFloat("MusicMuted", 1);
        }
        else 
        {
            audioMixer.SetFloat("MusicVolume", defaultMusicAudioMixerVolume);
            PlayerPrefs.SetFloat("MusicMuted", 0);
        }
    }

    public void ToggleSFXMute() 
    {
        sfxMuted = !sfxMuted;

        if (sfxMuted)
        {
            audioMixer.SetFloat("SFXVolume", -80);
            PlayerPrefs.SetFloat("SFXMuted", 1);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", defaultSFXAudioMixerVolume);
            PlayerPrefs.SetFloat("SFXMuted", 0);
        }
    }
}
