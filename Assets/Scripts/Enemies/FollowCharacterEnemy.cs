using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EnemySpace
{
    public class FollowCharacterEnemy : Enemy
    {
        public EnemyMotor motor;
        private void Start()
        {
            motor.EnemyMotorInit(transform);
        }
        public override void EnemyDie()
        {
            EnemyControl.Instance.MinusEnemyFromDictionary(transform);
            GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load(deathEffectionPath) as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
            GameObjectPool.Instance.CollectObject(gameObject);
        }

        public override void GoReset()
        {
        }
        private void FollowCharacter()
        {
            motor.MoveTowards(BasicInformation.Instance.Character.position);
        }
        private void Update()
        {
            FollowCharacter();
        }
    }
}
