using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private AudioMixer myAudioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Dotween Audio")]
    [SerializeField] private AudioSource[] backgroundAudios;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float maxVolume;
    private int currentIndex = 0;

    private void Start()
    {
        LoadVolume();
        if (backgroundAudios.Length > 0)
        {
            PlayNextSong();
        }
    }
    public void LoadVolume()
    {
        masterSlider.value = audioSettings.masterVolume;
        musicSlider.value = audioSettings.musicVolume;
        sfxSlider.value = audioSettings.sfxVolume;
    }

    public void SetMasterVolume()
    {
        audioSettings.masterVolume = masterSlider.value;
        myAudioMixer.SetFloat("MasterVolume", Mathf.Log10(audioSettings.masterVolume) * 20);
    }

    public void SetMusicVolume()
    {
        audioSettings.musicVolume = musicSlider.value;
        myAudioMixer.SetFloat("MusicVolume", Mathf.Log10(audioSettings.musicVolume) * 20);
    }

    public void SetSfxVolume()
    {
        audioSettings.sfxVolume = sfxSlider.value;
        myAudioMixer.SetFloat("SfxVolume", Mathf.Log10(audioSettings.sfxVolume) * 20);
    }
    private void PlayNextSong()
    {
        if (currentIndex >= backgroundAudios.Length)
        {
            currentIndex = 0;
        }
        AudioSource currentAudio = backgroundAudios[currentIndex];
        currentAudio.volume = 0;
        currentAudio.Play();

        currentAudio.DOFade(maxVolume, fadeDuration).OnComplete(() =>
        {
            currentAudio.DOFade(0, fadeDuration).SetDelay(currentAudio.clip.length - fadeDuration).OnComplete(() =>
            {
                currentAudio.Stop();
                ++currentIndex;
                PlayNextSong();
            });
        });
    }
}
