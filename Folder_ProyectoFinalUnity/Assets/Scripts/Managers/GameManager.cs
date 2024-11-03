using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action OnStartGame;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
    }
}
