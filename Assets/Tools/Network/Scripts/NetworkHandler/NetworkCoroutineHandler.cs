using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Ironcow.Network
{
    public class NetworkCoroutineHandler : INetworkHandler
    {
        public interface NetworkState
        {
            public bool isRunning { get; set; }
        }

        public class NetworkRunning<T> : NetworkState
        {
            public bool isRunning { get; set; }
            public Response<T> data;
        }

        public class NetworkTexture : NetworkState
        {
            public bool isRunning { get; set; }
            public Texture2D data;
        }

        public class NetworkAudioClip : NetworkState
        {
            public bool isRunning { get; set; }
            public AudioClip data;
        }

        public string baseUrl;
        public int basePort;

        public void Init(string baseUrl, int port)
        {
            this.baseUrl = baseUrl;
            this.basePort = port;
        }

        public async Task<Response<T>> SendRequest<T>(string api, string param, eRequestType type)
        {
            var runningData = new NetworkRunning<T>();
            runningData.isRunning = true;
            NetworkManager.Instance.StartCoroutine(SendCoroutine<T>(api, param, type, runningData));

            while (runningData.isRunning)
            {
                await Task.Yield();
            }

            var ret = runningData.data;
            return ret;
        }

        public IEnumerator SendCoroutine<T>(string api, string param, eRequestType type, NetworkRunning<T> runningData)
        {
            string api_url = baseUrl + (basePort == 0 ? "" : ":" + basePort);
            string url = api_url + api + param;

            UnityWebRequest response = UnityWebRequest.Get(url);
            if (type == eRequestType.DELETE)
            {
                response = UnityWebRequest.Delete(url);
                response.downloadHandler = new DownloadHandlerBuffer();
            }

            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯

            yield return response.SendWebRequest();
            try
            {
                Debug.Log("Request API : " + api + " Request Type : " + type + "\nResponse : " + response.downloadHandler.text);
                var data = JsonUtility.FromJson<Response<T>>(response.downloadHandler.text);
                if (data.error == null)
                {
                    runningData.data = data;
                }
                else
                {
                    Debug.LogError(data.error);
                }
            }
            catch (Exception e)
            {
                Debug.Log(response.error);
            }
            finally
            {
                response.Dispose();
                runningData.isRunning = false;
            }
        }

        public async Task<Response<T>> SendRequest<T>(string api, WWWForm form, eRequestType type)
        {
            var runningData = new NetworkRunning<T>();
            runningData.isRunning = true;
            NetworkManager.Instance.StartCoroutine(SendCoroutine<T>(api, form, type, runningData));

            while (runningData.isRunning)
            {
                await Task.Yield();
            }

            var ret = runningData.data;
            return ret;
        }

        public IEnumerator SendCoroutine<T>(string api, WWWForm form, eRequestType type, NetworkRunning<T> runningData)
        {
            string api_url = baseUrl + (basePort == 0 ? "" : ":" + basePort);
            string url = api_url + api;

            UnityWebRequest response = UnityWebRequest.Post(url, form);
            if (type == eRequestType.DELETE)
            {
                response = UnityWebRequest.Delete(url);
                response.downloadHandler = new DownloadHandlerBuffer();
            }

            response.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");


            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯
            yield return response.SendWebRequest();
            try
            {
                Debug.Log("Request API : " + api + " Request Type : " + type + "\nResponse : " + response.downloadHandler.text);
                var data = JsonUtility.FromJson<Response<T>>(response.downloadHandler.text);
                if (data.error == null)
                {
                    runningData.data = data;
                }
                else
                {
                    Debug.LogError(data.error);
                }
            }
            catch (Exception e)
            {
                Debug.Log(response.error);
            }
            finally
            {
                response.Dispose();
                runningData.isRunning = false;
            }
        }

        public async Task<Texture2D> SendRequestTexture<T>(string url, eRequestType type)
        {
            var runningData = new NetworkTexture();
            runningData.isRunning = true;
            NetworkManager.Instance.StartCoroutine(SendCoroutineTexture(url, type, runningData));

            while (runningData.isRunning)
            {
                await Task.Yield();
            }

            var ret = runningData.data;
            return ret;
        }

        public IEnumerator SendCoroutineTexture(string url, eRequestType type, NetworkTexture runningData)
        {
            Texture2D retTexture = null;

            UnityWebRequest response = UnityWebRequestTexture.GetTexture(url);

            response.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯
            yield return response.SendWebRequest();
            try
            {
                retTexture = ((DownloadHandlerTexture)response.downloadHandler).texture;
                if (retTexture != null)
                {
                    runningData.data = retTexture;
                }
                else
                {
                    Debug.LogError(response.error);
                }
            }
            catch (Exception e)
            {
                Debug.Log(response.error);
            }
            finally
            {
                response.Dispose();
                runningData.isRunning = false;
            }
        }

        public async Task<AudioClip> SendRequestAudio<T>(string url, eRequestType type, AudioType audioType)
        {
            var runningData = new NetworkAudioClip();
            runningData.isRunning = true;
            NetworkManager.Instance.StartCoroutine(SendCoroutineAudio(url, type, runningData, audioType));

            while (runningData.isRunning)
            {
                await Task.Yield();
            }

            var ret = runningData.data;
            return ret;
        }

        public IEnumerator SendCoroutineAudio(string url, eRequestType type, NetworkAudioClip runningData, AudioType audioType)
        {
            AudioClip retAudio = null;

            UnityWebRequest response = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            if (type == eRequestType.DELETE)
            {
                response = UnityWebRequest.Delete(url);
                response.downloadHandler = new DownloadHandlerBuffer();
            }

            response.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");


            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯
            yield return response.SendWebRequest();
            try
            {
                retAudio = ((DownloadHandlerAudioClip)response.downloadHandler).audioClip;
                if (retAudio != null)
                {
                    runningData.data = retAudio;
                }
                else
                {
                    Debug.LogError(response.error);
                }
            }
            catch (Exception e)
            {
                Debug.Log(response.error);
            }
            finally
            {
                response.Dispose();
                runningData.isRunning = false;
            }
        }
    }
}