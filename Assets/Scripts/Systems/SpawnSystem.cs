using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnSystem : SystemBase
{
    private RandomSystem randomSystem;
    BeginInitializationEntityCommandBufferSystem ECB;
    protected override void OnCreate()
    {
        randomSystem=World.GetExistingSystem<RandomSystem>();
        ECB=World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    
    
    protected override void OnUpdate()
    {
        var randomArray=randomSystem.RandomArray;
       
        
        var ecb=ECB.CreateCommandBuffer().AsParallelWriter();
        Entities.WithBurst(synchronousCompilation:true).WithNativeDisableParallelForRestriction(randomArray).ForEach((Entity e,int entityInQueryIndex,int nativeThreadIndex,in SpawnVariables spawner, in LocalToWorld ltw) => {
            for(int i=0;i<spawner._spawnCount;i++)
            {
                 var random=randomArray[nativeThreadIndex];
                Entity tempEntity= ecb.Instantiate(entityInQueryIndex,spawner._spawnPrefab);
                float3 pos=random.NextFloat3Direction()*random.NextFloat();
                float3 rot=random.NextFloat3Direction();
                
                pos*=spawner._spawnRadius;
                ecb.SetComponent(entityInQueryIndex,tempEntity,new Translation{Value=pos});
                ecb.SetComponent(entityInQueryIndex,tempEntity,new Rotation{Value=quaternion.Euler(rot)});
                ecb.AddComponent(entityInQueryIndex,tempEntity,new BoidAgent
                {
                    boidVel=random.NextFloat3Direction(),
                    boidPos=pos,
                    boidAcel=0,
                    perceptionRadius=spawner._perceptionRadius,
                    avoidanceRadius=spawner._avoidanceRadius,
                    cellSize=spawner._cellSize,
                    speed=spawner._maxSpeed,
                    avoidanceForce=spawner._avoidanceForce,
                    alignmentForce=spawner._alignmentForce,
                    cohesionForce=spawner._cohesionForce,
                    step=spawner.step,
                    targetForce=spawner._targetForce,
                    target=float3.zero,
                    maxRadius=spawner._spawnRadius
                    


                    
                });
                randomArray[nativeThreadIndex]=random;
            }
           ecb.DestroyEntity(entityInQueryIndex,e);
            
        }).ScheduleParallel();
        ECB.AddJobHandleForProducer(Dependency);
    }
}
