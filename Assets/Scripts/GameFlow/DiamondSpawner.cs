using Items;
using UnityEngine;

namespace GameFlow
{
    public class DiamondSpawner : CollectableSpawner<Diamond>
    {
        [SerializeField] private DiamondType diamondType;

        public override void Spawn()
        {
            base.Spawn();
            objectSpawned.GetComponent<Diamond>().Type = diamondType;
        }
    }
}