using EnemySpace;
using ScoreSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsControl : MonoSingleton<LevelsControl>
{
    [Tooltip("��һ����ѧ��֮��ÿ����ѧ�ظ���Ʒ���ɵ�����")]
    public List<ItemsConfig> levelConfig;
    [Tooltip("��ѧ��֮��ÿ�����ӵ���Ʒ����")]
    public ItemsConfig levelIncrease;
    [Tooltip("��ѧ��֮��ÿ����ʼ��Ʒ����")]
    public ItemsConfig levelInit;
    [Tooltip("��Ҷ��ٷ�Χ�ڲ����ɵ���")]
    public float enemyDistanceToCharacter;
    public float levelShowTime;
    private int levelIndexValue;
    public GameObject tiShi;
    [HideInInspector]
    public int levelIndex//��ǰ��
    {
        get
        {
            return levelIndexValue;
        }
        set
        {
            if (value ==2)
            {
                tiShi.gameObject.SetActive(false);
            }
            levelIndexValue = value;
            ShowLevelProcess(value);
        }
    }

    private void Generate(int value)
    {
        if (value > 1 && value <= levelConfig.Count + 1)
        {
            GenerateAccordingItemsConfig(levelConfig[value - 2]);
        }
        else if (value > levelConfig.Count + 1)
        {
            GenerateAccordingItemsConfig(levelInit);
            for (int i = 0; i < value - levelConfig.Count - 1; i++)
            {
                GenerateAccordingItemsConfig(levelIncrease);
            }
        }
    }

    private void Start()
    {
        levelIndex = 1;
    }
    private void ShowLevelProcess(int level)
    {
        Generate(level);
        UIManager.Instance.GetUIWindow<UIControl>().LevelShow(level);
        StartCoroutine(CloseShowLevelNum(level));
    }
    private IEnumerator CloseShowLevelNum(int level)
    {
        yield return new WaitForSeconds(levelShowTime);
        UIManager.Instance.GetUIWindow<UIControl>().LevelFade();
    }
    private void GenerateAccordingItemsConfig(ItemsConfig config)
    {
        if (config.randomEnemy)
        {
            for (int i = 0; i < config.randomEnemyNum; i++)
            {
                GenenrateEnemy(EnemyType.Fixed, true);
            }
        }
        else
        {
            for (int i = 0;i < config.fixedEnemyNum; i++)
            {
                GenenrateEnemy(EnemyType.Fixed, false);
            }
            for (int i = 0; i < config.directMoveEnemyNum; i++)
            {
                GenenrateEnemy(EnemyType.DirectMove, false);
            }
            for (int i = 0; i < config.followCharacterEnemyNum; i++)
            {
                GenenrateEnemy(EnemyType.FollowCharacter, false);
            }
        }
        ScoreControl.Instance.ClearAllScoreItems();
        GameToolControl.Instance.ClearAllGameToolItems();
        ScoreControl.Instance.RandomGenerateScoreItems(config.scoreNum);
        GameToolControl.Instance.RandomGenerateGameToolItems(config.toolNum);
    }
    private Transform GenenrateEnemy(EnemyType type, bool isRandom = false)
    {
        Transform temp = null;
        if (isRandom)
        {
            temp = EnemyControl.Instance.GenerateOneRandomEnemy();
        }
        else
        {
            temp = EnemyControl.Instance.GenerateOneEnemy(type);
        }
        if (Vector2.Distance(temp.position,BasicInformation.Instance.Character.position) <=enemyDistanceToCharacter)
        {
            EnemyControl.Instance.MinusEnemyFromDictionary(temp,false);
            GameObjectPool.Instance.CollectObject(temp.gameObject);
            return GenenrateEnemy(type, isRandom);
        }
        else
        {
            return temp;
        }
    }
}
