using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmersiveMusic : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource bonfireAudio; 
    [SerializeField] private float maxVolume;      
    [SerializeField] private float fadeSpeed;
    [SerializeField] private Transform playerTransform;                   
    private SphereCollider audioRangeCollider;

    private void Start()
    {
        audioRangeCollider = GetComponent<SphereCollider>();
    }
    private void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);

            float normalizedVolume = Mathf.Clamp01(1 - (distance / audioRangeCollider.radius));

            bonfireAudio.volume = Mathf.Lerp(bonfireAudio.volume, normalizedVolume * maxVolume, Time.deltaTime * fadeSpeed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!bonfireAudio.isPlaying)
            {
                bonfireAudio.Play();
            }
        }
    }
}
