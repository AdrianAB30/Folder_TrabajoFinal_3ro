using System.Collections;
using UnityEngine;
using System;
public class Particle_Class : MonoBehaviour
{
    [SerializeField] private ParticleSystem rollingPlayerParticle;
    [SerializeField] private ParticleSystem[] explotionParticle;

    private void Start()
    {
        for (int i = 0; i < explotionParticle.Length; i++)
        {
            explotionParticle[i].Stop();
            explotionParticle[i].gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        PlayerController.OnStartRollingParticle += PlayRollingParticlesPlayer;
        PlayerController.OnEndRollingParticle += StopRollingParticlesPlayer;
        GameManager.OnExplotionStart += PlayParticleExplotion;
    }
    private void OnDisable()
    {
        PlayerController.OnStartRollingParticle -= PlayRollingParticlesPlayer;
        PlayerController.OnEndRollingParticle -= StopRollingParticlesPlayer;
        GameManager.OnExplotionStart -= PlayParticleExplotion;
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
    public void PlayParticleExplotion()
    {
        if (explotionParticle != null)
        {
            for (int i = 0; i < explotionParticle.Length; i++)
            {
                explotionParticle[i].gameObject.SetActive(true);
                explotionParticle[i].Play();
            }
        }
    }
}
