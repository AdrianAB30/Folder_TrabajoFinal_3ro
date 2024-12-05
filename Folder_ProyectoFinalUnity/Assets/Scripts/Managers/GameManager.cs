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

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera cameraMenu;
    [SerializeField] private CinemachineVirtualCamera cameraBridge;

    [Header("Menu Navigation")]
    [SerializeField] private GameObject optionsSelected;
    [SerializeField] private GameObject menuSelected;

    [Header("References Skeleton and Bridge")]
    [SerializeField] private SkeletonController[] skeletons;
    [SerializeField] private GameObject bridgeCollider;        
    [SerializeField] private Material bridgeMaterial;          
    [SerializeField] private float dissolveSpeed;
    public int skeletonKillCount = 0;
    private int TotalSkeletons = 5;

    private void OnEnable()
    {
        for (int i = 0; i < skeletons.Length; i++)
        {
            skeletons[i].OnEnemyKilled += ActivateBridge;
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < skeletons.Length; i++)
        {
            skeletons[i].OnEnemyKilled -= ActivateBridge;
        }
    }
    public bool IsOptionsMenuActive { get; private set; }
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (cameraMenu != null)
            {
                StartCoroutine(ChangeCameraInGame());
            }
        }
        if (bonfireLight != null)
        {
            bonfireLight.DOIntensity(maxIntensity, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
        }
        FadeStart();
        bridgeMaterial.SetFloat("_DissolveAmount", 1f);
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
            IsOptionsMenuActive = true;
            panel.gameObject.SetActive(true);
            panelOptions.DOAnchorPos(new Vector3(0, 0, 0), duration).SetEase(myEase);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionsSelected);
            StartCoroutine(TimeInGamePause());
        }
    }
    public void HideOptionsInGame()
    {
        IsOptionsMenuActive = false;
        panel.gameObject.SetActive(false);
        panelOptions.DOAnchorPos(new Vector3(0, 950, 0), duration).SetEase(myEase);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1;
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
    private void ActivateBridge()
    {
        skeletonKillCount++;

        if (skeletonKillCount >= TotalSkeletons)
        {
            StartCoroutine(DissolveBridge());
            Debug.Log("¡Puente activado!");
        }
    }
    private IEnumerator DissolveBridge()
    {
        cameraBridge.Priority = 11;
        yield return new WaitForSeconds(1.5f);
        float dissolveValue = 1f;

        bridgeCollider.SetActive(false);

        while (dissolveValue > 0f)
        {
            dissolveValue -= Time.deltaTime * dissolveSpeed;
            bridgeMaterial.SetFloat("_DissolveAmount", dissolveValue);
            yield return null;
        }
        bridgeMaterial.SetFloat("_DissolveAmount", 0f);

        bridgeCollider.SetActive(true);
        Debug.Log("¡Collider del puente activado!");

        yield return new WaitForSeconds(1f);

        cameraBridge.Priority = 9;
    }
}
