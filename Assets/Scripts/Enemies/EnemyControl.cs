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
            {EnemyType.Fixed,"Prafab/FixedEnemy" },
            {EnemyType.FollowCharacter,"Prafab/FollowCharacterEnemy" },
            {EnemyType.DirectMove,"Prafab/DirectMove" }
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
        public void GenerateOneEnemy(EnemyType type)
        {
            Vector2 pos = Vector2.Lerp(BasicInformation.Instance.WholeMapMinPoint.position, BasicInformation.Instance.WholeMapMaxPoint.position, Random.value);
            Transform item = GameObjectPool.Instance.CreateObject(type.ToString(), Resources.Load(enemyPrefabPath[type]) as GameObject, pos, Quaternion.identity).transform;
            item.SetParent(transform);
            enemyDictionary[type].Add(item);
        }
    }
}
