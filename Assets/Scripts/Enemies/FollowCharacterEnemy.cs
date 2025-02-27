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
            type = EnemyType.FollowCharacter;
        }
        public override void EnemyDie()
        {
            GameObjectPool.Instance.CollectObject(gameObject);
            EnemyControl.Instance.enemyDictionary[type].Remove(transform);
        }

        public override void GoReset()
        {
            throw new System.NotImplementedException();
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
