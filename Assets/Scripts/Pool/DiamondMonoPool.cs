using Items;
using Unity.VisualScripting;
using UnityEngine;

namespace Pool
{
    public class DiamondMonoPool : MonoPool<Diamond>
    {
        public override Diamond Get()
        {
            Debug.Log("Diamond Pool get");
            return base.Get();
        }
    }
}