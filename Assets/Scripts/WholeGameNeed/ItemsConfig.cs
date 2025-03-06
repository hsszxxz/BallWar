using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemsConfig
{
    [Tooltip("是否随机生成三种敌人之一")]
    public bool randomEnemy;
    [Tooltip("随机生成的敌人数量")]
    public int randomEnemyNum;
    [Tooltip("固定敌人数量")]
    public int fixedEnemyNum;
    [Tooltip("直线移动敌人数量")]
    public int directMoveEnemyNum;
    [Tooltip("跟随主角敌人数量")]
    public int followCharacterEnemyNum;
    [Tooltip("奖励物数量")]
    public int scoreNum;
    [Tooltip("道具数量")]
    public int toolNum;
}
