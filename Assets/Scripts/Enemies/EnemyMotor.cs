using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EnemySpace
{
    [Serializable]
    public class EnemyMotor
    {
        public float moveSpeed;
        private Transform enemy;
        public void EnemyMotorInit(Transform Enmey)
        {
            enemy = Enmey;
        }
        public bool MoveTowards(Vector2 target)
        {
            if (Vector2.Distance(enemy.position, target) > 0.1f)
            {
                Vector2 dir = new Vector2(target.x - enemy.position.x, target.y - enemy.position.y).normalized;
                enemy.Translate(dir * moveSpeed * Time.deltaTime);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
