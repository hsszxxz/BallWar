using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EnemySpace
{
    public abstract class Enemy : MonoBehaviour, Resetable
    {
        [HideInInspector]
        public EnemyType type;
        public abstract void GoReset();
        public abstract void EnemyDie();
    }
}
