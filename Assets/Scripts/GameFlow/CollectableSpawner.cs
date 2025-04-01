using Items;
using Pool;
using UnityEngine;
using IPoolable = Unity.VisualScripting.IPoolable;

namespace GameFlow
{
    public abstract class CollectableSpawnerBase : MonoBehaviour
    {
        public abstract void Spawn();
    }
    
    public  abstract class CollectableSpawner<T> : CollectableSpawnerBase where T : Collectable
    {
        protected GameObject objectSpawned;
        public override void Spawn()
        { 
            objectSpawned = MonoPool<T>.Instance.Get().gameObject;
            objectSpawned.transform.position = transform.position;
        }
    }
}