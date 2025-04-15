using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public static class ExtensionMethods
{
    public static List<T> ToList<T>(this T[] obj)
    {
        return new List<T>(obj);
    }

    public static Vector3 ToVector3(this string str)
    {
        if (str[0] == '(' && str.Last() == ')')
        {
            var pos = str.Substring(1, str.Length - 2).Split(',');
            return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
        return Vector3.zero;
    }

    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
    {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new TaskCompletionSource<UnityWebRequest.Result>();
        reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

        if (reqOp.isDone)
            tsc.TrySetResult(reqOp.webRequest.result);

        return tsc.Task.GetAwaiter();
    }

    public static string FileName(this string path)
    {
        return path.Split('/').Last().Split('.')[0];
    }
}
