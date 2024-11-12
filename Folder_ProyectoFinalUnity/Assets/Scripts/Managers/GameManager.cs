using System.Collections;
using UnityEngine;  
using System;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform panelOptions;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject loosePanel;

    [Header("Dotween")]

    [SerializeField] private Ease myEase;
    [SerializeField] private float duration;

    public event Action OnStartGame;

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        OnStartGame?.Invoke(); 
    }
    public void ShowOptions()
    {
        panel.gameObject.SetActive(true);
        panelOptions.DOAnchorPos(new Vector3(0, 0, 0), duration).SetEase(myEase);
    }
    public void ShowOptionsInGame()
    {
        panel.gameObject.SetActive(true);
        panelOptions.DOAnchorPos(new Vector3(0, 0, 0), duration).SetEase(myEase);
        StartCoroutine(TimeInGamePause());
    }
    public void HideOptions()
    {
        panel.gameObject.SetActive(false);
        panelOptions.DOAnchorPos(new Vector3(0,950,0), duration).SetEase(myEase);
    }
    public void HideOptionsInGame()
    {
        panel.gameObject.SetActive(false);
        panelOptions.DOAnchorPos(new Vector3(0, 950, 0), duration).SetEase(myEase);
        Time.timeScale = 1;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }
    IEnumerator TimeInGamePause()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
    }
    public void LooseGame()
    {
        loosePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
