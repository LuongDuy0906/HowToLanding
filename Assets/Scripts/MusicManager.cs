using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private const int MAX_MUSIC_VOLUME = 10;

    private static float musicTime;
    private static int musicVolume = 6;

    private AudioSource musicAudioSource;

    private event EventHandler onMusicVolumeChange;

    private void Awake()
    {
        Instance = this;

        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.time = musicTime;
    }

    private void Start()
    {
        musicAudioSource.volume = GetMusicVolumeNormalize();
    }

    private void Update()
    {
        musicTime = musicAudioSource.time;
    }

    public void ChangeMusicVolume()
    {
        musicVolume = (musicVolume + 1) % MAX_MUSIC_VOLUME;
        musicAudioSource.volume = GetMusicVolumeNormalize();
        onMusicVolumeChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetMusicVolumeNormalize()
    {
        return ((float)musicVolume) / MAX_MUSIC_VOLUME;
    }
}
