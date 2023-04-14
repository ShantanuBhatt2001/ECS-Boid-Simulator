using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
[GenerateAuthoringComponent]

public struct BoidAgent :IComponentData
{
     public float3 boidVel;
     public float3 boidPos;
     public float3 boidAcel;
     public float perceptionRadius;
     public float avoidanceRadius;
     public int cellSize;
     public float cohesionForce;
     public float alignmentForce;
     public float avoidanceForce;
     public float speed;
     public float step;
     public float3 target;
     public float targetForce;
     public float maxRadius;

}


