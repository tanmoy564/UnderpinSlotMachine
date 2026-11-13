
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource spinSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip spinLoop;
    [SerializeField] private AudioClip reelStop;
    [SerializeField] private AudioClip coinPayout;
    [SerializeField] private AudioClip jackpot;
    [SerializeField] private AudioClip bonus;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClick);
    }

    public void StartSpinSound()
    {
        if (spinSource == null || spinLoop == null)
            return;

        spinSource.clip = spinLoop;
        spinSource.loop = true;
        spinSource.Play();
    }

    public void StopSpinSound()
    {
        if (spinSource != null)
            spinSource.Stop();
    }

    public void PlayReelStop()
    {
        PlaySound(reelStop);
    }

    public void PlayCoinPayout()
    {
        PlaySound(coinPayout);
    }

    public void PlayJackpot()
    {
        PlaySound(jackpot);
    }

    public void PlayBonus()
    {
        PlaySound(bonus);
    }

    private void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
