using System.Collections;
using UnityEngine;  
using System;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform panelOptions;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject loosePanel;
    [SerializeField] private Image fadeImage;

    [Header("Dotween")]
    [SerializeField] private Ease myEase;
    [SerializeField] private float duration;
    [SerializeField] private float fadeDuration;

    [Header("Luces")]
    [SerializeField] private Light bonfireLight;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;


    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera cameraMenu;

    [Header("Menu Navigation")]
    [SerializeField] private GameObject optionsSelected;
    [SerializeField] private GameObject menuSelected;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (cameraMenu != null)
            {
                StartCoroutine(ChangeCameraInGame());
            }
        }
        bonfireLight.DOIntensity(maxIntensity, 1f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutQuart);
        FadeStart();
    }
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    private void FadeStart()
    {
        if (fadeImage != null)
        {
            fadeImage.DOFade(0f, fadeDuration);
        }
    }
    public void ShowOptions()
    {
        panel.gameObject.SetActive(true);
        panelOptions.DOAnchorPos(new Vector3(0, 0, 0), duration).SetEase(myEase);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsSelected);
    }
    public void ShowOptionsInGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            panel.gameObject.SetActive(true);
            panelOptions.DOAnchorPos(new Vector3(0, 0, 0), duration).SetEase(myEase);
            StartCoroutine(TimeInGamePause());
        }
        else if (context.canceled)
        {
            panel.gameObject.SetActive(false);
            panelOptions.DOAnchorPos(new Vector3(0, 950, 0), duration).SetEase(myEase);
            Time.timeScale = 1;
        }
    }
    public void HideOptions()
    {
        panel.gameObject.SetActive(false);
        panelOptions.DOAnchorPos(new Vector3(0,950,0), duration).SetEase(myEase);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuSelected);
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
        yield return new WaitForSeconds(1.05f);
        Time.timeScale = 0;
    }
    public void LooseGame()
    {
        loosePanel.SetActive(true);
        Time.timeScale = 0;
    }
    private IEnumerator ChangeCameraInGame()
    {
        yield return new WaitForSeconds(1f);
        cameraMenu.Priority = 9;
    }
}
