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
        }
        public override void EnemyDie()
        {
            GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load(deathEffectionPath) as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
            GameObjectPool.Instance.CollectObject(gameObject);
            EnemyControl.Instance.enemyDictionary[type].Remove(transform);
        }
        public override void GoReset() { }
    }
}
