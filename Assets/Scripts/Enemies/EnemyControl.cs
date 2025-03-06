using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace EnemySpace
{
    public enum EnemyType
    {
        Fixed,
        DirectMove,
        FollowCharacter,
    }
    public class EnemyControl : MonoSingleton<EnemyControl>
    {
        public Dictionary <EnemyType, List<Transform>> enemyDictionary = new Dictionary<EnemyType, List<Transform>>();
        private Dictionary<EnemyType, string> enemyPrefabPath = new Dictionary<EnemyType, string>()
        {
            {EnemyType.Fixed,"Prefab/FixedEnemy" },
            {EnemyType.FollowCharacter,"Prefab/FollowCharacterEnemy" },
            {EnemyType.DirectMove,"Prefab/DirectMoveEnemy" }
        };
        public override void Init()
        {
            base.Init();
            enemyDictionary.Add(EnemyType.Fixed, new List<Transform>());
            enemyDictionary.Add(EnemyType.FollowCharacter, new List<Transform>());
            enemyDictionary.Add(EnemyType.DirectMove, new List<Transform>());
            foreach (Transform enemy in transform)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript!=null)
                {
                    enemyDictionary[enemyScript.type].Add(enemy);
                }
            }
        }
        //ø’‘Ú∑µªÿtrue
        private bool DetectDictionaryIsEmpty()
        {
            bool flag = true;
            foreach (List<Transform> enemies in enemyDictionary.Values)
            {
                if (enemies.Count > 0) { flag = false; }
            }
            //Debug.Log("Fixed:"+enemyDictionary[EnemyType.Fixed].Count);
            //Debug.Log("Direct:"+enemyDictionary[EnemyType.DirectMove].Count);
            //Debug.Log("Follow:" + enemyDictionary[EnemyType.FollowCharacter].Count);
            return flag;
        }
        public void MinusEnemyFromDictionary(Transform enemy)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript!=null)
            {
                enemyDictionary[enemyScript.type].Remove(enemy);
                if (DetectDictionaryIsEmpty())
                {
                    LevelsControl.Instance.levelIndex += 1;
                }
            }
        }
        public Transform GenerateOneEnemy(EnemyType type)
        {
            Vector2 pos = Vector2.Lerp(BasicInformation.Instance.WholeMapMinPoint.position, BasicInformation.Instance.WholeMapMaxPoint.position, Random.value);
            Transform item = GameObjectPool.Instance.CreateObject(type.ToString(), Resources.Load(enemyPrefabPath[type]) as GameObject, pos, Quaternion.identity).transform;
            item.SetParent(transform);
            enemyDictionary[type].Add(item);
            return item;
        }
        public Transform GenerateOneRandomEnemy()
        {
            EnemyType[] types = (EnemyType[])System.Enum.GetValues(typeof(EnemyType));
            EnemyType random = types[Random.Range(0, types.Length)];
            return GenerateOneEnemy(random);
        }
    }
}
