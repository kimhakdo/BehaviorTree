using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;

public class PopupNetworkTest : UIBase
{
    [SerializeField] private Text text;

    public override void HideDirect()
    {
        UIManager.Hide<PopupNetworkTest>();
    }

    public async void OnClickButtonPost(int idx)
    {
        RequestPost req = new RequestPost(idx);
        var res = await req.SendRequest<Post>();
        text.text = res.data.text;
    }

    public async void OnClickButtonProfile()
    {
        //RequestProfile req = new RequestProfile();
        //var res = await req.SendRequest<Profile>();
        //text.text = res.data.name;
        RequestTest req = new RequestTest();
        var res = await req.SendRequest<Test>();
        text.text = res.data.test;
    }

    public override void Opened(params object[] param)
    {
        
    }
}