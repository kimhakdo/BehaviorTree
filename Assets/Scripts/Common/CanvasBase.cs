using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CanvasBase<T> : Singleton<T> where T : CanvasBase<T>
{
    [SerializeField] protected List<Transform> parents;
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UIManager.IsInstance);
        if (parents.Count > 0)
            UIManager.SetParents(parents);
    }
}