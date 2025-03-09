using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ScoreSpace
{
    public class ScoreControl : MonoSingleton<ScoreControl>
    {
        [HideInInspector]
        public int Scores;
        private string scorePrefabPath = "Prefab/Score";
        private string multPrefabPath = "Prefab/Mult";
        private List<Transform> scoreItems = new List<Transform>();
        public override void Init()
        {
            base.Init();
            Scores = 0;
        }
        public void RandomGenerateScoreItems(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Vector2 pos = new Vector2(Random.Range(BasicInformation.Instance.WholeMapMinPoint.position.x, BasicInformation.Instance.WholeMapMaxPoint.position.x), Random.Range(BasicInformation.Instance.WholeMapMinPoint.position.y, BasicInformation.Instance.WholeMapMaxPoint.position.y));
                Transform item = GameObjectPool.Instance.CreateObject("score", Resources.Load(scorePrefabPath) as GameObject, pos, Quaternion.identity).transform;
                scoreItems.Add(item);
            }
        }
        public void ClearAllScoreItems()
        {
            foreach (Transform item in scoreItems)
            {
                GameObjectPool.Instance.CollectObject(item.gameObject);
            }
            scoreItems.Clear();
        }
        public void PlusScore(int num)
        {
            Scores += num;
            UIManager.Instance.GetUIWindow<UIControl>().UIScorePlus(num);
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
            UIManager.Instance.GetUIWindow<UIControl>().UIScorePlus(num*num);
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
        public void FinishGameScoreShow()
        {
            UIManager.Instance.GetUIWindow<UIControl>().ShutAndOpen(false);
            FinishUIWindow finishUIWindow = UIManager.Instance.GetUIWindow<FinishUIWindow>();
            finishUIWindow.ShutAndOpen(true);
            finishUIWindow.score.text = Scores.ToString();
            StartCoroutine(BackToStartScene(finishUIWindow.FinishShow()));
        }
        IEnumerator BackToStartScene(float animationTime)
        {
            yield return new WaitForSeconds(animationTime);
            Time.timeScale = 0;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene("StartScene");
                    break;
                }
                yield return null;
            }
        }
    }
}
