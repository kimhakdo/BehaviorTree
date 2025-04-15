using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using Ironcow.Network;


[System.Serializable]
public class Profile
{
    public string name;
}

[System.Serializable]
[API("/profile", eRequestType.GET)]
public class RequestProfile : Request<RequestProfile>
{
    
    public RequestProfile(params object[] param) : base(param)
    {

    }
}