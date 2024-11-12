using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private AudioMixer myAudioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private float volume;

    public static AudioManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        LoadVolume();
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
}
