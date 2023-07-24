using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class IncrePlayerScore : MonoBehaviour
{
    public TextMeshProUGUI player_scoreText;

    void Start()
    {

    }
    void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        var player_score = entityManager.CreateEntityQuery(typeof(PlayerScore)).GetSingleton<PlayerScore>();
        player_scoreText.text = player_score.score.ToString();
    }
}