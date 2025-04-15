using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using System.Linq;
using System.Threading.Tasks;

#if USE_ADDRESSABLE
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets;
#endif

internal class AddressableUtils : Editor
{
    const string assetBundlePath = "Assets/AddressableDatas";

#if USE_ADDRESSABLE
    [MenuItem("Ironcow/Addressable/Addressable Build")]
    internal static void BuildAddressable()
    {
        Mapping();
        AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion = Application.version;
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);//
        var uploadList = result.FileRegistry.GetFilePaths().ToList();
        uploadList.RemoveAll(obj => !obj.Contains("ServerData"));
        var list = "";
        uploadList.ForEach(obj => list += obj + "\n");
        var path = Application.dataPath + "/lastBuildData.txt";
        File.WriteAllText(path, list);
    }

    [MenuItem("Ironcow/Addressable/Addressable Build And Upload")]
    internal static void BuildAndUploadAddressable()
    {
        BuildAddressable();
        UploadLastBuild();
    }

    [MenuItem("Ironcow/Addressable/Addressable Upload")]
    public async static void UploadLastBuild()
    {
        var path = Application.dataPath + "/lastBuildData.txt";
        var data = File.ReadAllText(path);
        var uploadList = data.Split('\n').ToList();
        var type = GetBuildTargetToString();
        await DeleteFileinFolder(type);
        await CreateFolder(type);

        for (int i = 0; i < uploadList.Count; i++)
        {
            if (uploadList[i].Length > 0)
                await Upload(type, uploadList[i].Replace('\\', '/'));
        }
    }

    [InitializeOnEnterPlayMode]
    [MenuItem("Ironcow/Addressable/Mapping")]
    internal static void Mapping()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        Catalog.instance.Clear();
        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (!entry.AssetPath.Contains("Assets") || entry.AssetPath.Contains("addressableMap")) continue;
                var type = group.Name;
                var dir = Application.dataPath + entry.AssetPath.Replace("Assets", "");
                SetMapping(dir, type, entry.address);
                EditorUtility.SetDirty(Catalog.instance);
            }
        }
    }

    static void SetMapping(string dir, string type, string address = "")
    {
        var files = Directory.GetFiles(dir).ToList();
        files.RemoveAll(obj => obj.Contains(".meta"));
        Catalog mapData = Catalog.instance;
        var dirs = Directory.GetDirectories(dir).ToList();
        foreach (var d in dirs)
        {
            if (d.Contains("Atlas")) continue;
            SetMapping(d, type, address + "/" + d.Split('\\').Last());
        }
        foreach (var file in files)
        {
            var path = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            var data = new CatalogData();
            data.type = type;
            data.key = path.FileName();
            data.address = address == "" ? path : address + "/" + path.Split('/').Last() ;
            mapData.Add(data);
        }
    }

    public async static Task DeleteFileinFolder(string type)
    {
        var ftpUrl = new Uri($"ftp://wocjf84.synology.me/web/{Application.productName}");
        var request = WebRequest.Create(ftpUrl + Application.version + "/" + type) as FtpWebRequest;
        request.Credentials = new NetworkCredential("ftp_uploader", "ftpUploader1");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            var response = res.GetResponseStream();
            var reader = new StreamReader(response);
            var list = reader.ReadToEnd().Split('\n');
            reader.Close();
            response.Close();
            res.Dispose();
            for (int i = 0; i < list.Length; i++)
            {
                var request2 = WebRequest.Create(ftpUrl + type + "/" + list[i]) as FtpWebRequest;
                request2.Credentials = new NetworkCredential("ftp_uploader", "ftpUploader1");
                request2.Method = WebRequestMethods.Ftp.DeleteFile;
                request2.KeepAlive = false;
                request2.UseBinary = true;
                FtpWebResponse res2 = (FtpWebResponse)await request2.GetResponseAsync();
                res2.Dispose();
            }
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        Debug.Log("DeleteFolder ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task DeleteFolder(string type)
    {
        await DeleteFileinFolder(type);
        var ftpUrl = new Uri($"ftp://wocjf84.synology.me/web/{Application.productName}");
        var request = WebRequest.Create(ftpUrl + Application.version + "/" + type) as FtpWebRequest;

        request.Credentials = new NetworkCredential("ftp_uploader", "ftpUploader1");
        request.Method = WebRequestMethods.Ftp.RemoveDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        Debug.Log("DeleteFolder ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task CreateFolder(string type)
    {
        var ftpUrl = new Uri($"ftp://wocjf84.synology.me/web/{Application.productName}");
        var request = WebRequest.Create(ftpUrl + Application.version + "/" + type + "/") as FtpWebRequest;

        request.Credentials = new NetworkCredential("ftp_uploader", "ftpUploader1");
        request.Method = WebRequestMethods.Ftp.MakeDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        Debug.Log("CreateFolders ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task Upload(string type, string path)
    {
        var target = File.ReadAllBytes(path);
        var ftpUrl = new Uri($"ftp://wocjf84.synology.me/web/{Application.productName}/");
        var request = WebRequest.Create(ftpUrl + Application.version + "/" + type + "/" + path.Split('/').Last()) as FtpWebRequest;

        request.Credentials = new NetworkCredential("ftp_uploader", "ftpUploader1");
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.KeepAlive = false;
        request.UseBinary = true;
        request.ContentLength = target.Length;

        using (var ftpStream = request.GetRequestStream())
        {
            await ftpStream.WriteAsync(target, 0, target.Length);
            ftpStream.Close();
        }
        try
        {
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        Debug.Log("CreateFolders ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public static string GetBuildTargetToString()
    {
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? "Android" : EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? "IOS" : "StandaloneWindows";
    }
#endif
}