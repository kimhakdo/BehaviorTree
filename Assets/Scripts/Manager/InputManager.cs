using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
    //        if (hit)
    //        {
    //            var monster = GameManager.Instance.monsters[hit.collider.gameObject.GetInstanceID()];
    //            if (monster != null)
    //            {
    //                var monsterData = DataManager.Instance.GetData<MonsterData>(monster.key);
    //                UIManager.Instance.uiMain.SetMonsterInfo(monster.name, monster.Hp);
    //            }
    //        }
    //        else
    //        {
    //            UIManager.Instance.uiMain.SetMonsterInfo("", 0);
    //        }
    //    }
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        GameManager.Instance.userCharacter.Shoot();
    //    }
    //}

    #region NewInput
    Dictionary<int, UnityAction> keyDown = new Dictionary<int, UnityAction>();
    Dictionary<int, UnityAction> keyUp = new Dictionary<int, UnityAction>();
    [SerializeField] private List<KeyCode> useKeyCodes = new List<KeyCode>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var keycode in useKeyCodes)
        {
            keyDown.Add((int)keycode, () => { });
            keyUp.Add((int)keycode, () => { });
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(keycode))
                .Subscribe(_ => keyDown[(int)keycode].Invoke());
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyUp(keycode))
                .Subscribe(_ => keyUp[(int)keycode].Invoke());
        }
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Select(_ => Input.mousePosition)
            .Subscribe(x => OnClickMouse(x));
    }

    public void OnClickMouse(Vector3 mousePos)
    {
        var ray = Camera.main.ScreenPointToRay(mousePos);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        { 
            var monster = GameManager.Instance.monsters[hit.collider.gameObject.GetInstanceID()];
            if (monster != null)
            {
                var monsterData = DataManager.Instance.GetData<EnemyData>(monster.rcode);
                if (UIManager.IsOpened<UIMain>())
                    UIManager.Get<UIMain>().SetMonsterInfo(monster.name, monster.Hp);
            }
        }
        else
        {
            if (UIManager.IsOpened<UIMain>())
                UIManager.Get<UIMain>().SetMonsterInfo("", 0);
        }
    }
    /*
    //Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnClickMouse();
        }
        foreach (var key in actions.Keys)
        {
            if (Input.GetKeyDown((KeyCode)key))
            {
                actions[key]?.Invoke();
            }
        }
    }*/

    public void Subscribe(KeyCode key, UnityAction keyDown, UnityAction keyUp = null)
    {
        this.keyDown[(int)key] += keyDown;
        if(keyUp != null)
            this.keyUp[(int)key] += keyUp;
    }

    public void Unsubscribe(KeyCode key, UnityAction act)
    {
        keyDown[(int)key] -= act;
    }
    #endregion
}
