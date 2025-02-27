using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacterEnemy : Enemy
{
    public EnemyMotor motor;
    private void Start()
    {
        motor.EnemyMotorInit(transform);
    }
    public override void EnemyDie()
    {
        GameObjectPool.Instance.CollectObject(gameObject);
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
