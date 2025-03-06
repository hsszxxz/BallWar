using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EnemySpace
{
    public class DirectMoveEnemy : Enemy
    {
        public float HalfMaxMoveDistance;
        private int isHorizontal;
        public EnemyMotor motor;
        private Vector2 target;
        private int face;
        private void Start()
        {
            type = EnemyType.DirectMove;
            isHorizontal = Random.Range(0, 2);
            motor.EnemyMotorInit(transform);
            target = transform.position + new Vector3(isHorizontal * HalfMaxMoveDistance, (1 - isHorizontal) * HalfMaxMoveDistance, 0);
            face = 1;
        }
        public override void EnemyDie()
        {
            GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load(deathEffectionPath) as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
            GameObjectPool.Instance.CollectObject(gameObject);
            EnemyControl.Instance.MinusEnemyFromDictionary(transform);
        }

        public override void GoReset()
        {
            isHorizontal = Random.Range(0, 2);
            target = transform.position + new Vector3(isHorizontal * HalfMaxMoveDistance, (1 - isHorizontal) * HalfMaxMoveDistance, 0);
            face = 1;
        }
        private void TurnDirection()
        {
            target -= face * 2 * new Vector2(isHorizontal * HalfMaxMoveDistance, (1 - isHorizontal) * HalfMaxMoveDistance);
            face = -face;
        }
        private void DirectMove()
        {
            if (motor.MoveTowards(target) || BasicInformation.Instance.isOutMap(transform.position))
            {
                TurnDirection();
            }
        }
        private void Update()
        {
            DirectMove();
        }
    }
}
