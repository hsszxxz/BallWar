using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedEnemy : Enemy
{
    public override void EnemyDie()
    {
        GameObjectPool.Instance.CollectObject(gameObject);
    }

    public override void GoReset() {}
}
