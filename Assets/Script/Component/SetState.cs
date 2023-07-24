using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using Enum;
public class SetState : MonoBehaviour
{
    public TextMeshProUGUI stateText;

    void Start()
    {

    }
    void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        var state = entityManager.CreateEntityQuery(typeof(EndGame)).GetSingleton<EndGame>();
        if (state.state == Value.GameResult.Win)
        {
            // Gán giá trị "YOU WIN" cho stateText
            stateText.text = "YOU WIN";
        }
        if (state.state == Value.GameResult.Lose)
        {
            // Gán giá trị "YOU WIN" cho stateText
            stateText.text = "YOU LOSE";
        }
        if (state.state == Value.GameResult.Playing)
        {
            // Gán giá trị "YOU WIN" cho stateText
            stateText.text = "";
        }
    }
}