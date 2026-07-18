using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {  get; private set; }

    [SerializeField] private AudioClip fuelPickupAudioClip;
    [SerializeField] private AudioClip coinPickupAudioClip;
    [SerializeField] private AudioClip crashAudioClip;
    [SerializeField] private AudioClip landingSuccessAudioClip;

    private const int MAX_SOUND_VOLUME = 10;
    private static int soundVolume = 5;

    public event EventHandler onSoundVolumeChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.OnFuelPickup += Lander_OnFuelPickup;
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLander;
    }

    private void Lander_OnLander(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.type) {
            case Lander.LandingType.Success:
                AudioSource.PlayClipAtPoint(landingSuccessAudioClip, Camera.main.transform.position, GetSoundVolumeNormalize());
                break;
            default:
                AudioSource.PlayClipAtPoint(crashAudioClip, Camera.main.transform.position, GetSoundVolumeNormalize());
                break;
        }
    }

    private void Lander_OnCoinPickup(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(coinPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalize());
    }

    private void Lander_OnFuelPickup(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(fuelPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalize());
    }

    public void ChangeSoundVolume()
    {
        soundVolume = (soundVolume + 1) % MAX_SOUND_VOLUME;
        onSoundVolumeChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetSoundVolume()
    {
        return soundVolume;
    }

    public float GetSoundVolumeNormalize()
    {
        return ((float)soundVolume) / MAX_SOUND_VOLUME;
    }
}
