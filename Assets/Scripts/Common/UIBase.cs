using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class UIBase : MonoBehaviour
{
    public eUIPosition uiPosition;
    public abstract void Opened(params object[] param);
    public abstract void HideDirect();
}