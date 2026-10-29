
using UnityEngine;

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
    [SerializeField] private AudioClip correctAnswer;
    [SerializeField] private AudioClip incorrectAnswer;
    [SerializeField] private AudioClip doorUnlock;

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
    }

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(buttonClick);
    }

    public void PlayCorrectAnswer()
    {
        sfxSource.PlayOneShot(correctAnswer);
    }

    public void PlayIncorrectAnswer()
    {
        sfxSource.PlayOneShot(incorrectAnswer);
    }

    public void PlayDoorUnlock()
    {
        sfxSource.PlayOneShot(doorUnlock);
    }
}
