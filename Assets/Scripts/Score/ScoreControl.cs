using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScoreSpace
{
    public class ScoreControl : MonoSingleton<ScoreControl>
    {
        [HideInInspector]
        public int Scores;
        private string scorePrefabPath = "Prafab/Score";
        private string multPrefabPath = "Prefab/Mult";
        public override void Init()
        {
            base.Init();
            Scores = 0;
        }
        public void RandomGenerateScoreItems(int num)
        {
            Vector2 pos = Vector2.Lerp(BasicInformation.Instance.WholeMapMinPoint.position, BasicInformation.Instance.WholeMapMaxPoint.position, Random.value);
            Transform item = GameObjectPool.Instance.CreateObject("score", Resources.Load(scorePrefabPath) as GameObject, pos, Quaternion.identity).transform;
        }
        public void PlusScore(int num)
        {
            Scores += num;
        }
        public void PlusScore(int num,Vector3 pos)
        {
            if (num >=1)
            {
                GameObjectPool.Instance.CreateObject("mult", Resources.Load(multPrefabPath) as GameObject, pos, Quaternion.identity).GetComponentInChildren<MultItem>().ShowMult(num);
            }
            Scores += num;
        }
        public void MinusScore(int num)
        {
            Scores -= num;
            if (Scores < 0)
            {
                Debug.Log("您的得分小于0");
                Scores = 0;
            }
        }
    }
}
