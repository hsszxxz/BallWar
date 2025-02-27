using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
namespace EnemySpace
{
    public class FixedEnemy : Enemy
    {
        private void Start()
        {
            type = EnemyType.Fixed;
            EnemyDie();
        }
        public override void EnemyDie()
        {
            explosionEffect.Play();
            GameObjectPool.Instance.CollectObject(gameObject,explosionEffect.main.duration);
            EnemyControl.Instance.enemyDictionary[type].Remove(transform);
        }
        public override void GoReset() { }
    }
}
