using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public struct SpawnVariables : IComponentData
{
    public float _spawnRadius;
    public float _spawnCount;
    public Entity _spawnPrefab;
    public float _currentSpawned;
    public float _perceptionRadius;
    public float _avoidanceRadius;
    public int _cellSize;
    public float _maxSpeed;
    public float _cohesionForce;
    public float _avoidanceForce;
    public float _alignmentForce;
    public float step;
    public float _targetForce;

}
