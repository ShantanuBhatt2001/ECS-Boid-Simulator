using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;



public class QuadrantSystem : SystemBase
{
    
    public NativeMultiHashMap<int,BoidAgent> cellMap;
    static int getHashMapKey(float3 pos,int cellSize)
    {
        return (int)((0*math.floor(pos.x/cellSize)) +(17*math.floor(pos.y/cellSize))+(23*math.floor(pos.z/cellSize)));
    }

    protected override void OnCreate()
    {
        cellMap=new NativeMultiHashMap<int, BoidAgent>(0,Allocator.Persistent);
    }
    protected override void OnUpdate()
    {
        EntityQuery entityQuery =GetEntityQuery(typeof(BoidAgent));
        cellMap.Clear();
        if(entityQuery.CalculateEntityCount()>cellMap.Capacity)
        {
            cellMap.Capacity=entityQuery.CalculateEntityCount();
        }
        NativeMultiHashMap<int,BoidAgent>.ParallelWriter cellMapParallel= cellMap.AsParallelWriter();
        
        
        
        
        
        Entities.ForEach((ref Translation translation,ref BoidAgent boidAgent) => {
            BoidAgent ba_data=new BoidAgent();
            ba_data=boidAgent;
            ba_data.boidPos=translation.Value;
            cellMapParallel.Add(getHashMapKey(translation.Value,ba_data.cellSize),ba_data);
            
        }).ScheduleParallel();

        float deltaTime= Time.DeltaTime;
        NativeMultiHashMap<int,BoidAgent> cellMapJob=cellMap;
        Entities.WithBurst().WithReadOnly(cellMapJob).ForEach((ref BoidAgent boidAgent,ref Translation translation,ref Rotation rotation)=>
        {
            int key =getHashMapKey(translation.Value,boidAgent.cellSize);
            NativeMultiHashMapIterator<int> mapIterator;
            int total=0;
            int seperationTotal=0;
            BoidAgent neighbour;
            float3 seperation=float3.zero;
            float3 cohesion=float3.zero;
            float3 alignment=float3.zero;

            if(cellMapJob.TryGetFirstValue(key,out neighbour,out mapIterator))
            {
                do
                {
                    if(!translation.Value.Equals(neighbour.boidPos) && math.distance(translation.Value,neighbour.boidPos)<boidAgent.perceptionRadius)
                    {
                        if(math.distance(translation.Value,neighbour.boidPos)<boidAgent.avoidanceRadius)
                        {
                            seperation+=neighbour.boidPos;
                            alignment+=neighbour.boidVel;
                            seperationTotal++;
                        }
                        else if(math.distance(translation.Value,neighbour.boidPos)<boidAgent.perceptionRadius)
                        {
                            alignment+=neighbour.boidVel;
                            cohesion+=neighbour.boidPos-translation.Value;
                            total++;
                        }
                          
                        
                        
                       
                        
                    }
                }while(cellMapJob.TryGetNextValue(out neighbour,ref mapIterator));

                if(total>0)
                {
                    cohesion=cohesion/total;
                    cohesion=cohesion-boidAgent.boidVel;
                    cohesion=math.normalize(cohesion)* boidAgent.cohesionForce;

                    
                    alignment=alignment/(total+seperationTotal);
                    alignment=alignment-boidAgent.boidVel;
                    alignment=math.normalize(alignment)*boidAgent.alignmentForce;
                }
                if(seperationTotal>0){
                    
                    seperation=seperation/total;
                    seperation=translation.Value-seperation-boidAgent.boidVel;
                    seperation=math.normalize(seperation)*boidAgent.avoidanceForce;
                     alignment=alignment/(total+seperationTotal);
                    alignment=alignment-boidAgent.boidVel;
                    alignment=math.normalize(alignment)*boidAgent.alignmentForce;
                    
                }
                
                    
                
                if(math.distance(translation.Value,boidAgent.target)>400f)
                boidAgent.boidAcel=math.normalize(boidAgent.target-translation.Value)*boidAgent.targetForce;
                else
                boidAgent.boidAcel=float3.zero;
                boidAgent.boidAcel+=(cohesion+alignment+seperation);
                
                rotation.Value=math.slerp(rotation.Value,quaternion.LookRotation(math.normalize(boidAgent.boidVel),math.up()),deltaTime*10);
                boidAgent.boidVel+=boidAgent.boidAcel;
                boidAgent.boidVel=math.normalize(boidAgent.boidVel)*boidAgent.speed;
                //if(math.length(boidAgent.boidVel)!=0 );
                translation.Value=math.lerp(translation.Value,(translation.Value+boidAgent.boidVel),deltaTime*boidAgent.step);
                //boidAgent.boidPos=translation.Value;
               
            }
        }).ScheduleParallel();

       
    }
    protected override void OnDestroy()
    {
        cellMap.Dispose();
    }
}
