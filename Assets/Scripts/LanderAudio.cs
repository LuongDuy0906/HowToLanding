using System;
using UnityEngine;

public class LanderAudio : MonoBehaviour
{
    [SerializeField] private AudioSource thrusterAudioSource;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();
    }

    private void Start()
    {
        lander.OnBeforeForce += Lander_OnBeforeForce;
        lander.OnUpForce += Lander_OnUpForce;
        lander.OnRightForce += Lander_OnRightForce;
        lander.OnLeftForce += Lander_OnLeftForce;

        SoundManager.Instance.onSoundVolumeChange += SoundManager_OnSoundVolumeChange;

        thrusterAudioSource.Pause();
    }

    private void SoundManager_OnSoundVolumeChange(object sender, EventArgs e)
    {
        thrusterAudioSource.volume = SoundManager.Instance.GetSoundVolumeNormalize();
    }

    private void Lander_OnLeftForce(object sender, EventArgs e)
    {
        if(!thrusterAudioSource.isPlaying) {
            thrusterAudioSource.Play();   
        }
    }

    private void Lander_OnRightForce(object sender, EventArgs e)
    {
        if(!thrusterAudioSource.isPlaying) {
            thrusterAudioSource.Play();   
        }
    }

    private void Lander_OnUpForce(object sender, EventArgs e)
    {
        if(!thrusterAudioSource.isPlaying) {
            thrusterAudioSource.Play();   
        }
    }

    private void Lander_OnBeforeForce(object sender, EventArgs e)
    {
        thrusterAudioSource.Pause();
    }
}
