using System.Collections;
using GameFlow;
using Pool;
using Unity.VisualScripting;
using UnityEngine;
using IPoolable = Pool.IPoolable;

namespace Items
{
    public enum DiamondType {
        Small,
        Big,
        Red
    }
    public class Diamond : Collectable
    {
        [SerializeField] private int smallDiamondValue;
        [SerializeField] private int bigDiamondValue;
        [SerializeField] private int redDiamondValue;
        private int _value;
        // private float _initialGravityScale;

        private static readonly int Big = Animator.StringToHash("Big");
        private static readonly int Small = Animator.StringToHash("Small");
        private static readonly int Red = Animator.StringToHash("Red");

        public int Value => _value;
        
        public DiamondType Type
        {
            set
            {
                switch (value)
                {
                    case DiamondType.Small:
                        _value = smallDiamondValue;
                        Animator.SetTrigger(Small);
                        break;
                    case DiamondType.Big:
                        _value = bigDiamondValue;
                        Animator.SetTrigger(Big);
                        break;
                    case DiamondType.Red:
                        _value = redDiamondValue;
                        Animator.SetTrigger(Red);
                        break;
                    default:
                        _value = bigDiamondValue;
                        Animator.SetTrigger(Big);
                        break;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(LifeCoutdown());
        }

        private IEnumerator LifeCoutdown()
        { 
            yield return new WaitForSeconds(4f);
            Renderer renderer = GetComponent<Renderer>();
            for (int i = 0; i < 6; i++)
            {
                renderer.enabled = !renderer.isVisible;
                yield return new WaitForSeconds(.25f);
            }
            DiamondMonoPool.Instance?.Return(this);
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (!isActiveAndEnabled)
            {
                DiamondMonoPool.Instance?.Return(this);
            }
        }
    }
}