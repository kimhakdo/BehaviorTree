using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Ironcow.Network
{
    public class NetworkAsyncHandler : INetworkHandler
    {
        public string baseUrl;
        public int basePort;

        public void Init(string baseUrl, int port)
        {
            this.baseUrl = baseUrl;
            this.basePort = port;
        }

        public async Task<Response<T>> SendRequest<T>(string api, string param, eRequestType type)
        {
            string api_url = baseUrl + (basePort == 0 ? "" : ":" + basePort);
            string url = api_url + api + param;
            Response<T> retData = null;
            UnityWebRequest response = UnityWebRequest.Get(url);
            if (type == eRequestType.DELETE)
            {
                response = UnityWebRequest.Delete(url);
                response.downloadHandler = new DownloadHandlerBuffer();
            }

            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯
            await response.SendWebRequest();
            try
            {
                Debug.Log("Request API : " + api + " Request Type : " + type + "\nResponse : " + response.downloadHandler.text);
                retData = JsonUtility.FromJson<Response<T>>(response.downloadHandler.text);
                if (retData.error == null)
                {

                }
                else
                {
                    Debug.LogError(retData.error);
                }
            }
            catch (Exception e)
            {
                Debug.Log(response.error);
            }
            finally
            {
                response.Dispose();
            }
            return retData;
        }

        public async Task<Response<T>> SendRequest<T>(string api, WWWForm form, eRequestType type)
        {
            string api_url = baseUrl + (basePort == 0 ? "" : ":" + basePort);
            string url = api_url + api;
            Response<T> retData = null;

            UnityWebRequest response = UnityWebRequest.Post(url, form);
            if (type == eRequestType.DELETE)
            {
                response = UnityWebRequest.Delete(url);
                response.downloadHandler = new DownloadHandlerBuffer();
            }

            response.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");


            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯

            await response.SendWebRequest();

            try
            {
                Debug.Log("Request API : " + api + " Request Type : " + type + "\nResponse : " + response.downloadHandler.text);
                retData = JsonUtility.FromJson<Response<T>>(response.downloadHandler.text);
                if (retData.error == null)
                {

                }
                else
                {
                    Debug.LogError(retData.error);
                }
            }
            catch (Exception e)
            {
                Debug.Log(response.error);
            }
            finally
            {
                response.Dispose();
            }
            return retData;
        }

        public async Task<Texture2D> SendRequestTexture<T>(string url, eRequestType type)
        {
            Texture2D retTexture = null;

            UnityWebRequest response = UnityWebRequestTexture.GetTexture(url);
            if (type == eRequestType.DELETE)
            {
                response = UnityWebRequest.Delete(url);
                response.downloadHandler = new DownloadHandlerBuffer();
            }

            response.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");


            //비동기는 보통 백그라운드 처리라 로딩창 안띄우는 게 맞는듯
            await response.SendWebRequest();

            try
            {
                retTexture = ((DownloadHandlerTexture)response.downloadHandler).texture;
                if (retTexture == null)
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
            }
            return retTexture;
        }

        public async Task<AudioClip> SendRequestAudio<T>(string url, eRequestType type, AudioType audioType)
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
            await response.SendWebRequest();
            try
            {
                retAudio = ((DownloadHandlerAudioClip)response.downloadHandler).audioClip;
                if (retAudio == null)
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
            }
            return retAudio;
        }
    }
}