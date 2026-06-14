using System;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem centerThrusterParticleSystem;
    [SerializeField] private ParticleSystem rightThrusterParticleSystem;
    [SerializeField] private GameObject landerExplosionVFX;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();

        lander.OnUpForce += Lander_OnUpForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnRightForce += Lander_OnRightForce;
        lander.OnBeforeForce += Lander_OnBeforceForce;

        SetEnableTrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(centerThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Start()
    {
        lander.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.type)
        {
            case Lander.LandingType.TooSteepAngle:
            case Lander.LandingType.TooFastLanding:
            case Lander.LandingType.WrongLandingArea:
                Instantiate(landerExplosionVFX, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                break;
        }
    }

    private void Lander_OnBeforceForce(object sender, EventArgs e)
    {
        SetEnableTrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(centerThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Lander_OnUpForce(object sender, EventArgs e)
    {
        SetEnableTrusterParticleSystem(leftThrusterParticleSystem, true);
        SetEnableTrusterParticleSystem(centerThrusterParticleSystem, true);
        SetEnableTrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void Lander_OnLeftForce(object sender, EventArgs e)
    {
        SetEnableTrusterParticleSystem(leftThrusterParticleSystem, true);
        SetEnableTrusterParticleSystem(centerThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Lander_OnRightForce(object sender, EventArgs e)
    {
        SetEnableTrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(centerThrusterParticleSystem, false);
        SetEnableTrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void SetEnableTrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled;


    }
}
