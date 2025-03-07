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
            Vector2 pos = new Vector2(Random.Range(BasicInformation.Instance.WholeMapMinPoint.position.x, BasicInformation.Instance.WholeMapMaxPoint.position.x), Random.Range(BasicInformation.Instance.WholeMapMinPoint.position.y, BasicInformation.Instance.WholeMapMaxPoint.position.y));
            Transform item = GameObjectPool.Instance.CreateObject("score", Resources.Load(gameToolPath) as GameObject, pos, Quaternion.identity).transform;
        }
    }
}
