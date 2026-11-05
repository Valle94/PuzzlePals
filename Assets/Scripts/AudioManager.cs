
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip incorrectAnswer;
    [SerializeField] private AudioClip doorUnlock;


    [Header("Volume Settings")]
    [SerializeField, Range (0f, 1f)] 
    private float musicVolume = 0.4f;

    [SerializeField, Range (0f, 1f)] 
    private float sfxVolume = 0.8f;
    private bool musicMuted = false; 
    private bool sfxMuted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Start background music automatically
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
        UpdateVolumes();
        
    }

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(buttonClick);
    }

    public void PlayIncorrectAnswer()
    {
        sfxSource.PlayOneShot(incorrectAnswer);
    }

    public void PlayDoorUnlock()
    {
        sfxSource.PlayOneShot(doorUnlock, 0.3f);
    }

    public void UpdateVolumes()
    {
        musicSource.volume = musicMuted ? 0f : musicVolume;
        sfxSource.volume = sfxMuted? 0f : sfxVolume;
    }


    public void SetMusicVolume(float vaule)
    {
        musicVolume = vaule;
        Debug.Log("Music Slider Value: " + vaule);
        UpdateVolumes();
    }

    public void SetSFXVolume(float vaule)
    {
        sfxVolume = vaule;
        UpdateVolumes();
    }

    public void ToggleMusicMute()
    {
        musicMuted = !musicMuted;
        UpdateVolumes();
    }

    public void ToggleSFXMute()
    {
        sfxMuted = !sfxMuted;
        UpdateVolumes();
    }
}
