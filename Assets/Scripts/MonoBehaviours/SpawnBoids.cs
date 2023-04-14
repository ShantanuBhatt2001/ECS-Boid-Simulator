using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class SpawnBoids : MonoBehaviour
{
    [SerializeField] float _spawnRadius=100f;
    [SerializeField] GameObject _boidPrefab;
    [SerializeField] int _spawnCount=10000;
    [SerializeField] int _currentSpawned=0;

    BlobAssetStore blob;
    void Start()
    {
        blob= new BlobAssetStore();
        var settings=GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,blob);
        var entity=GameObjectConversionUtility.ConvertGameObjectHierarchy(_boidPrefab,settings);
        var entityManager=World.DefaultGameObjectInjectionWorld.EntityManager;
        for(int i=0;i<_spawnCount;i++)
        {
            _currentSpawned++;
            var instance=entityManager.Instantiate(entity);
            Vector3 pos=UnityEngine.Random.insideUnitSphere*_spawnRadius;
            entityManager.SetComponentData(instance,new Translation{Value=pos});
            Vector3 rot=UnityEngine.Random.insideUnitSphere*100;
            Debug.Log(Quaternion.Euler(rot));
            entityManager.SetComponentData(instance,new Rotation{Value=Quaternion.Euler(rot)});

        }
    }
    private void OnDestroy() {
        blob.Dispose();
    }
}

    
