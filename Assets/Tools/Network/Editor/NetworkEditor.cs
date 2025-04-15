using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ironcow.Network
{
	public class NetworkEditor : Editor
    {
        public static void CreateManagerInstance()
        {
            if (GameObject.Find("NetworkManager")) return;
            var obj = new GameObject("NetworkManager");
            obj.AddComponent<NetworkManager>();
        }
    }
}