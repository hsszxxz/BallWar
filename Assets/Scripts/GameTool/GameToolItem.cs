using ScoreSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameToolItem : MonoBehaviour
{
    public void CollectTool()
    { 
        GameObjectPool.Instance.CollectObject(gameObject);
    }
}
