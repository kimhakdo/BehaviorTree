using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Dictionary<int, Monster2> monsters = new Dictionary<int, Monster2>();
    public Character userCharacter;

    public bool isPowerShot = false;

    public void Init()
    {
        Application.targetFrameRate = 60;
        userCharacter = ResourceManager.Instance.InstantiateAsset<Character>("Character", ResourceType.Prefab);
        userCharacter.Init();
        //Observable.Timer(System.TimeSpan.FromSeconds(0), System.TimeSpan.FromSeconds(5))
        //    .Subscribe(_ => CreateMonster());

        StartCoroutine(Test()); 
    }

    private IEnumerator Test()
    {
        yield return new WaitUntil(() => DataManager.Instance.isInit);
        CreateMonster(); 
    }



    public void CreateMonster()
    {
        var monster = PoolManager.Instance.Spawn<Monster2>();
        monsters.Add(monster.gameObject.GetInstanceID(), monster);
        var monsterDatas = DataManager.Instance.GetMonsterDatas();
        var monsterData = monsterDatas.OrderBy(obj => Random.Range(0, monsterDatas.Count)).ToList()[0];
        monster.Init((EnemyData)monsterData);
        var randomNormal = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        var randomRange = Random.Range(3, 5);
        monster.SetPosition(randomNormal * randomRange);
    }

    public void ReleaseMonster(Monster2 monster)
    {
        monsters.Remove(monster.GetInstanceID());
    }
}