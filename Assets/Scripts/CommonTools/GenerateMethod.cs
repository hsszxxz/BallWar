using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class  GenerateMethod
{
    public static bool InitRandomObject(List<int> num,Vector2 minPoints, Vector2 maxPoints,string PrefabPath,string poolTag)
    {
        Vector3 pos = new Vector3(Random.Range(minPoints.x, maxPoints.x), Random.Range(minPoints.y, maxPoints.y), 0);
        Transform item = GameObjectPool.Instance.CreateObject(poolTag, Resources.Load(PrefabPath) as GameObject, pos, Quaternion.identity).transform;
        return true;
    }
}
