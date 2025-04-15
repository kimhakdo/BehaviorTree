using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using Ironcow.Network;


[System.Serializable]
public class Test
{
    public string test;
}

[System.Serializable]
[API("/test", eRequestType.GET)]
public class RequestTest : Request<RequestTest>
{
    
    public RequestTest(params object[] param) : base(param)
    {

    }
}