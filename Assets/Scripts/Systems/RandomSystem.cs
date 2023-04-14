using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RandomSystem : SystemBase
{
    public NativeArray<Unity.Mathematics.Random> RandomArray{get;private set;}
    protected override void OnCreate()
    {
       var randomArray= new Unity.Mathematics.Random[JobsUtility.MaxJobThreadCount];

       for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
       {
            var seed= new System.Random();
            randomArray[i]=new Unity.Mathematics.Random((uint)seed.Next());
       }
       RandomArray= new NativeArray<Unity.Mathematics.Random>(randomArray,Allocator.Persistent);
    }
    protected override void OnUpdate()
    {
        
    }

    protected override void OnDestroy()
    {
        RandomArray.Dispose();
    }
}
