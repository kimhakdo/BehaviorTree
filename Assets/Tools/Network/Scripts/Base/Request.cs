using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Ironcow.Network
{
    public static class RequestExtension
    {

        public static int GetColumnCount(this string str)
        {
            return str.Split('{').Length - 1;
        }

    }

    public class Request<V>
    {
        public Request(params object[] param)
        {
            var fields = typeof(V).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int idx = 0;
            foreach (var field in fields)
            {
                if (param.Length <= idx) break;
                if (field.FieldType == typeof(int))
                {
                    field.SetValue(this, (int)param[idx]);
                }
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(this, (string)param[idx]);
                }
                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(this, (bool)param[idx]);
                }
                if (field.FieldType == typeof(short))
                {
                    field.SetValue(this, (short)param[idx]);
                }
                if (field.FieldType == typeof(float))
                {
                    field.SetValue(this, (float)param[idx]);
                }
                if (field.FieldType == typeof(string[]))
                {
                    field.SetValue(this, (string[])param[idx]);
                }
                if (field.FieldType == typeof(Vector3))
                {
                    field.SetValue(this, (Vector3)param[idx]);
                }
                idx++;
            }
        }

        public WWWForm ToWWWForm()
        {
            WWWForm form = new WWWForm();
            var fields = typeof(V).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(int))
                {
                    form.AddField(field.Name, (int)field.GetValue(this));
                }
                if (field.FieldType == typeof(string))
                {
                    var data = field.GetValue(this) == null ? "" : field.GetValue(this);
                    form.AddField(field.Name, data.ToString());
                }
                if (field.FieldType == typeof(bool))
                {
                    var data = field.GetValue(this) == null ? "" : field.GetValue(this);
                    form.AddField(field.Name, data.ToString());
                }
                if (field.FieldType == typeof(short))
                {
                    form.AddField(field.Name, (short)field.GetValue(this));
                }
                if (field.FieldType == typeof(float))
                {
                    var data = field.GetValue(this) == null ? "" : field.GetValue(this);
                    form.AddField(field.Name, data.ToString());
                }
                if (field.FieldType == typeof(string[]))
                {
                    var data = field.GetValue(this) == null ? "" : field.GetValue(this);
                    form.AddField(field.Name, JsonUtility.ToJson((string[])data));
                }
                if (field.FieldType == typeof(Vector3))
                {
                    var data = field.GetValue(this) == null ? "" : field.GetValue(this);
                    form.AddField(field.Name, data.ToString());
                }

            }
            return form;
        }

        public string GetFirstValue()
        {
            var fields = typeof(V).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                return field.GetValue(this).ToString();
            }
            return "";
        }

        public string[] GetValues()
        {
            List<string> retList = new List<string>();
            var fields = typeof(V).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                retList.Add(field.GetValue(this).ToString());
            }
            return retList.ToArray();
        }

        public string ToParam()
        {
#if NEXON_API
            string retParam = "";
            string and = "?";
            var fields = typeof(V).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                retParam += and + field.Name + "=" + field.GetValue(this)?.ToString();
                and = "&";
            }
#else
            string retParam = "";
            string and = "/";
            var fields = typeof(V).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var (API, requestType) = GetAttribute();
            var count = API.GetColumnCount();
            foreach (var field in fields)
            {
                if (count > 0) count--;
                else retParam += and + field.GetValue(this)?.ToString();
            }
#endif
            return retParam;
        }

        public async Task<Response<T>> SendRequest<T>()
        {
            string API = "";
            eRequestType requestType = eRequestType.GET;
            (API, requestType) = GetAttribute();
            if (API.Contains("{0}"))
            {
                API = string.Format(API, GetValues());
            }

            switch (requestType)
            {
                case eRequestType.POST:
                case eRequestType.PUT:
                    {
                        return await NetworkManager.Instance.Request<T>(API, ToWWWForm(), requestType);
                    }
                case eRequestType.GET:
                case eRequestType.DELETE:
                    {
                        return await NetworkManager.Instance.Request<T>(API, ToParam(), requestType);
                    }
            }
            return null;
        }

        public (string, eRequestType) GetAttribute()
        {
            string api;
            eRequestType requestType = eRequestType.GET;

            Type type = typeof(V);
            var attributes = Attribute.GetCustomAttributes(type);
            foreach (var attribute in attributes)
            {
                APIAttribute a = attribute as APIAttribute;
                if (a != null)
                {
                    api = a.API;
                    requestType = a.requestType;
                    return (api, requestType);
                }
            }
            return ("", requestType);
        }
    }

    /// <summary>
    /// API 속성 (api 주소 및 타입 지정에 사용) (API attribute, using for api address and setting request type)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class APIAttribute : Attribute
    {
        public string API = "";
        public eRequestType requestType = eRequestType.POST;

        public APIAttribute(string api, eRequestType requestType = eRequestType.POST)
        {
            API = api;
            this.requestType = requestType;
        }
    }

}

public enum eRequestType
{
    POST = 0,
    GET = 1,
    DELETE = 2,
    PUT = 3,
}