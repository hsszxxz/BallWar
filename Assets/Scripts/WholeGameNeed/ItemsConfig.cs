using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemsConfig
{
    [Tooltip("�Ƿ�����������ֵ���֮һ")]
    public bool randomEnemy;
    [Tooltip("������ɵĵ�������")]
    public int randomEnemyNum;
    [Tooltip("�̶���������")]
    public int fixedEnemyNum;
    [Tooltip("ֱ���ƶ���������")]
    public int directMoveEnemyNum;
    [Tooltip("�������ǵ�������")]
    public int followCharacterEnemyNum;
    [Tooltip("����������")]
    public int scoreNum;
    [Tooltip("��������")]
    public int toolNum;
}
