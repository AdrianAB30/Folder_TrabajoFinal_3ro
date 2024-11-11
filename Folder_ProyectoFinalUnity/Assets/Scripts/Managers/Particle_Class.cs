using System.Collections;
using UnityEngine;
using System;
public class Particle_Class : MonoBehaviour
{
    [SerializeField] private ParticleSystem rollingPlayerParticle;

    private void OnEnable()
    {
        PlayerController.OnStartRollingParticle += PlayRollingParticlesPlayer;
        PlayerController.OnEndRollingParticle += StopRollingParticlesPlayer;
    }
    private void OnDisable()
    {
        PlayerController.OnStartRollingParticle -= PlayRollingParticlesPlayer;
        PlayerController.OnEndRollingParticle -= StopRollingParticlesPlayer;
    }
    public void PlayRollingParticlesPlayer()
    {
        if (rollingPlayerParticle != null && !rollingPlayerParticle.isPlaying)
        {
            rollingPlayerParticle.Play();
        }
    }
    public void StopRollingParticlesPlayer()
    {
        if (rollingPlayerParticle != null && rollingPlayerParticle.isPlaying)
        {
            rollingPlayerParticle.Stop();
        }
    }
}
