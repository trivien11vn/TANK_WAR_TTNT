using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class IncreAIScore : MonoBehaviour
{
    public TextMeshProUGUI ai_scoreText;

    void Start()
    {

    }
    void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        var ai_score = entityManager.CreateEntityQuery(typeof(AIScore)).GetSingleton<AIScore>();
        ai_scoreText.text = ai_score.score.ToString();
    }
}