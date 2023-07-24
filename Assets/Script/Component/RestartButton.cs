using System;
using System.Collections;
using System.Collections.Generic;
using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        Debug.Log("awake") ;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ReStartGame);
    }

    
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(ReStartGame);
    }

    private void ReStartGame()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        MessageBroadcaster
            .PrepareMessage()
            .AliveForUnlimitedTime()
            .PostImmediate(entityManager,
                new RestartCommand
                {
                    IsStart = true
                });

    }
}
