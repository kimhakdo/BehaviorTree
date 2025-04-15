using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> parents;
    private List<UIBase> uiList = new List<UIBase>();

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance.uiList.Clear();
    }

    public static T Show<T>(params object[] param) where T : UIBase
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        if (ui == null)
        {
            var prefab = ResourceManager.Instance.LoadAsset<T>(typeof(T).ToString(), ResourceType.UI);
            ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
            ui.name = ui.name.Replace("(Clone)", "");
            Instance.uiList.Add(ui);
        }
        if (ui.uiPosition == eUIPosition.UI)
        {
            Instance.uiList.ForEach(obj =>
            {
                if (obj.uiPosition == eUIPosition.UI) obj.gameObject.SetActive(false);
            });
        }
        ui.gameObject.SetActive(true);
        ui.Opened(param);
        return (T)ui;
    }

    public static void Hide<T>(params object[] param) where T : UIBase
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        if (ui != null)
        {
            Instance.uiList.Remove(ui);
            if (ui.uiPosition == eUIPosition.UI)
            {
                var prevUI = Instance.uiList.FindLast(obj => obj.uiPosition == eUIPosition.UI);
                prevUI.gameObject.SetActive(true);
            }
            Destroy(ui.gameObject);
        }
    }

    public static T Get<T>() where T : UIBase
    {
        return (T)Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
    }

    public static bool IsOpened<T>() where T : UIBase
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        return ui != null && ui.gameObject.activeInHierarchy;
    }
}