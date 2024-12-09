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
    [SerializeField] private RectTransform winTxt;
    [SerializeField] private GameObject panel;
    [SerializeField] private Image loosePanel;
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
    [SerializeField] private CinemachineVirtualCamera cameraExplotion;

    [Header("Menu Navigation")]
    [SerializeField] private GameObject optionsSelected;
    [SerializeField] private GameObject menuSelected;

    [Header("References Skeleton and Bridge")]
    [SerializeField] private SkeletonController[] skeletons;
    [SerializeField] private FrogController[] frogs;
    [SerializeField] private GameObject bridgeCollider;        
    [SerializeField] private GameObject explotion;        
    [SerializeField] private GameObject rockCollider;        
    [SerializeField] private Material bridgeMaterial;          
    [SerializeField] private Material rockMaterial;          
    [SerializeField] private float dissolveSpeed;
    public bool IsOptionsMenuActive { get; private set; }
    public int skeletonKillCount = 0;
    public int frogKillCount = 0;
    private int TotalSkeletons = 6;
    private int TotalFrogs = 8;

    public static event Action OnExplotionStart;
    private void OnEnable()
    {
        for (int i = 0; i < skeletons.Length; i++)
        {
            skeletons[i].OnSkeletonKilled += ActivateBridge;
        }
        for (int i = 0; i < frogs.Length; i++)
        {
            frogs[i].OnFrogKilled += ActivateWall;
        }
        PlayerController.OnPlayerDeath += LooseGame;
        PlayerController.OnPlayerWin += WinGame;
    }
    private void OnDisable()
    {
        for (int i = 0; i < skeletons.Length; i++)
        {
            skeletons[i].OnSkeletonKilled -= ActivateBridge;
        }
        for (int i = 0; i < frogs.Length; i++)
        {
            frogs[i].OnFrogKilled -= ActivateWall;
        }
        PlayerController.OnPlayerDeath -= LooseGame;
        PlayerController.OnPlayerWin -= WinGame;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
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
        rockMaterial.SetFloat("_DissolveAmount", 0f);
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
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
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
        Cursor.lockState = CursorLockMode.Locked;   
        Cursor.visible = false;
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
        StartCoroutine(AnimateLoosePanel());
    }
    public void WinGame()
    {
        StartCoroutine(AnimateWinPanel());
    }
    private IEnumerator AnimateLoosePanel()
    {
        yield return new WaitForSeconds(3f);
        float duration = 1f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            loosePanel.fillAmount = progress; 
            yield return null;
        }
        RestartGame();
        loosePanel.fillAmount = 1f; 
    }
    private IEnumerator AnimateWinPanel()
    {
        yield return new WaitForSeconds(1f);
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            loosePanel.fillAmount = progress;
            yield return null;
        }
        loosePanel.fillAmount = 1f;
        
        yield return new WaitForSeconds(2f);
        winTxt.DOAnchorPos(new Vector3(0,0,0),1f).SetEase(Ease.InOutBounce);
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Menu");

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
            StartCoroutine(DissolveBridgeAndRocks());
        }
    }
    private void ActivateWall()
    {
        frogKillCount++;

        if (frogKillCount >= TotalFrogs)
        {
            OnExplotionStart?.Invoke();
            explotion.SetActive(false);
            StartCoroutine(ChangeCameraExplotion());
        }
    }
    private IEnumerator DissolveBridgeAndRocks()
    {
        cameraBridge.Priority = 11;
        yield return new WaitForSeconds(1.5f);
        float dissolveValue = 1f;
        float rockDissolveValue = 0f;

        bridgeCollider.SetActive(false);

        while (dissolveValue > 0f)
        {
            dissolveValue -= Time.deltaTime * dissolveSpeed;
            rockDissolveValue += Time.deltaTime * dissolveSpeed;
            bridgeMaterial.SetFloat("_DissolveAmount", dissolveValue);
            rockMaterial.SetFloat("_DissolveAmount", rockDissolveValue);
            yield return null;        
        }
        bridgeMaterial.SetFloat("_DissolveAmount", 0f);
        rockMaterial.SetFloat("_DissolveAmount", 1f);

        bridgeCollider.SetActive(true);
        rockCollider.SetActive(false);

        yield return new WaitForSeconds(1f);

        cameraBridge.Priority = 9;
    }
    private IEnumerator ChangeCameraExplotion()
    {
        cameraExplotion.Priority = 11;
        yield return new WaitForSeconds(3f);
        cameraExplotion.Priority = 9;
    }
}
