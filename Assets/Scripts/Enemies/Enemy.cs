using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, Resetable
{
    public abstract void GoReset();
    public abstract void EnemyDie();
}
