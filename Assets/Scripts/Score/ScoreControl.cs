using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScoreSpace
{
    public class ScoreControl : MonoSingleton<ScoreControl>
    {
        [HideInInspector]
        public int Scores;
        private string scorePrefabPath = "Prefab/Score";
        private string multPrefabPath = "Prefab/Mult";
        public override void Init()
        {
            base.Init();
            Scores = 0;
        }
        public void RandomGenerateScoreItems(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Vector2 pos = Vector2.Lerp(BasicInformation.Instance.WholeMapMinPoint.position, BasicInformation.Instance.WholeMapMaxPoint.position, Random.value);
                Transform item = GameObjectPool.Instance.CreateObject("score", Resources.Load(scorePrefabPath) as GameObject, pos, Quaternion.identity).transform;
            }
        }
        public void PlusScore(int num)
        {
            Scores += num;
            UIControl.Instance.UIScorePlus(num);
        }
        public void PlusScore(int num,Vector3 pos,float lastTime)
        {
            if (num >1)
            {
                MultItem mult = GameObjectPool.Instance.CreateObject("mult", Resources.Load(multPrefabPath) as GameObject, pos, Quaternion.identity).GetComponentInChildren<MultItem>();
                mult.ShowMult(num);
                mult.ShutMult(lastTime);
            }
            Scores += num*num;
            UIControl.Instance.UIScorePlus(num*num);
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
