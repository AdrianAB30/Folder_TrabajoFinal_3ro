using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.UI;  
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Image imageFade;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float logoDisplayTime;

    public event Action OnStartGame;


    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); 
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start()
    {
        StartCoroutine(ShowLogoAndMenu());
    }

    private IEnumerator ShowLogoAndMenu()
    {
        if (imageFade != null)
        {
            imageFade.color = new Color(0, 0, 0, 1);

            yield return StartCoroutine(Fade(1, 0));

            yield return new WaitForSeconds(logoDisplayTime);

            yield return StartCoroutine(Fade(0, 1));

            OnStartGame?.Invoke();
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            imageFade.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        imageFade.color = new Color(0, 0, 0, endAlpha);
    }
}
