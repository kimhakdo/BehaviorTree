using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;
#if USE_CLOUD_CODE
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
#endif

namespace Ironcow.Network
{
    public interface INetworkHandler
    {
        public void Init(string baseUrl, int port);
        public Task<Response<T>> SendRequest<T>(string api, string param, eRequestType type);
        public Task<Response<T>> SendRequest<T>(string api, WWWForm form, eRequestType type);
        public Task<Texture2D> SendRequestTexture<T>(string url, eRequestType type);
        public Task<AudioClip> SendRequestAudio<T>(string url, eRequestType type, AudioType audioType);
    }

    public class NetworkManager : Singleton<NetworkManager>
    {
        public bool isInit = false;

        private INetworkHandler handler;

#if USE_CLOUD_CODE
        IroncowModuleBindings module;
#endif
        #region URL
        [SerializeField] string API_BASE_URL = "http://218.38.65.83";
        [SerializeField] string API_BASE_PORT = "";
        [SerializeField] string DOWNLOAD_BASE_URL = "http://ironcow.synology.me/resources/";
#if NEXON_API
        [SerializeField] string NEXON_API_KEY;
#endif
        public string GetApiBaseUrl => string.Format("{0}{1}/", API_BASE_URL, string.IsNullOrEmpty(API_BASE_PORT) ? "" : string.Format(":{0}", API_BASE_PORT));

        #endregion

        protected override void Awake()
        {
            base.Awake();
            Init<NetworkAsyncHandler>(API_BASE_URL, 0);
        }

        public void Init<T>(string ip, int port) where T : INetworkHandler
        {
            handler = Activator.CreateInstance<T>();
            handler.Init(ip, port);
            InitUGS();
            isInit = true;
        }

        public async void InitUGS()
        {
#if USE_CLOUD_CODE
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            module = new IroncowModuleBindings(CloudCodeService.Instance);
            try
            {
                // Call the function within the module and provide the parameters we defined in there
                var result = await module.Signin("test");
                //var result = await module.SayHello("www");
                Debug.Log(result);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
                var result = module.Signup("test");
                
            }
#endif
        }

        public async Task<Response<T>> Request<T>(string api, string param, eRequestType type)
        {
            return await handler.SendRequest<T>(api, param, type); 
        }

        public async Task<Response<T>> Request<T>(string api, WWWForm param, eRequestType type)
        {
            return await handler.SendRequest<T>(api, param, type);
        }
    }
}