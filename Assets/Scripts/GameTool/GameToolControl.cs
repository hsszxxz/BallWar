using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameToolControl : MonoSingleton<GameToolControl>
{
    private string gameToolPath = "Prefab/GameTool";
    public void RandomGenerateGameToolItems(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Vector2 pos = Vector2.Lerp(BasicInformation.Instance.WholeMapMinPoint.position, BasicInformation.Instance.WholeMapMaxPoint.position, Random.value);
            Transform item = GameObjectPool.Instance.CreateObject("score", Resources.Load(gameToolPath) as GameObject, pos, Quaternion.identity).transform;
        }
    }
}
