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
        }
        public override void EnemyDie()
        {
            EnemyControl.Instance.MinusEnemyFromDictionary(transform);
            GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load(deathEffectionPath) as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
            GameObjectPool.Instance.CollectObject(gameObject);
        }
        public override void GoReset() { type = EnemyType.Fixed; }
    }
}
