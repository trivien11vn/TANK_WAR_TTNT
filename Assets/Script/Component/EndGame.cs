using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Enum;

public struct EndGame : IComponentData{
    public Value.GameResult state;
}