using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

public class CanvasSample : CanvasBase<CanvasSample>
{
    private async void Start()
    {
        await SceneManager.LoadSceneAsync("DontDestroy", LoadSceneMode.Additive);
        if (parents.Count > 0)
            UIManager.SetParents(parents);
        await UniTask.WaitUntil(() => ResourceManager.Instance.isInit);
        await UniTask.WaitUntil(() => DataManager.Instance.isInit);
        GameManager.Instance.Init();
        UIManager.Show<UIMain>();
        //characterCam.targetTexture = UIManager.Show<UIMain>().tex;
    }
}
