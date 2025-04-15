using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using Ironcow.Network;


[System.Serializable]
public class Post
{
    public string text;
}

[System.Serializable]
[API("/posts", eRequestType.GET)]
public class RequestPost : Request<RequestPost>
{
    public int id;

    public RequestPost(params object[] param) : base(param)
    {
    }
}