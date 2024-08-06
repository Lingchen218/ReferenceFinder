﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Unity.Burst.Intrinsics;

public class ReferenceFinderData
{
    //缓存路径
    private const string CACHE_PATH = "xdfdf";
    private const string Csss = "Library/ReferenceFinderCache";
    private const string CACHE_VERSION = "V1";
    //资源引用信息字典
    public Dictionary<string, AssetDescription> assetDict = new Dictionary<string, AssetDescription>();

    [DllImport("Unity_FlatBuffers_Dll")]
    private static extern int Add(int x, int y, string b, List<string> bbbb);
    [DllImport("Unity_FlatBuffers_Dll")]
    private static extern int Max(int x, int y);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern int GenerateItems(IntPtr itemCount, IntPtr itemsFound);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern int GenerateItems1(int arrayLength, string[] stringArray);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern void readgunserializedGuid(string file, int arrayLength, IntPtr stringArray);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern void readgunserializedGuidSize(string file, IntPtr arrayLength);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern void readgunserializedDependencyHash(string file, int arrayLength, IntPtr stringArray);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern void readgunserializedDependencyHashSize(string file, IntPtr arrayLength);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern void readgunserializedDenpendencies(string file, int arrayLength, int itemCount2, IntPtr[] stringArray, IntPtr callbackfun, IntPtr delegatefun);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern void readgunserializedDenpendenciesSize(string file, IntPtr arrayLength);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern int readgunserializedDenpendenciesIntArraySize(string file, int GuidSizes, int[] arrayLength);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern int CreateFlatBuffersFileTest(string file，bool isCreateFile);

    // 定义一个返回值的委托类型
    public delegate int MyDelegate(int value);

    //收集资源引用信息并更新缓存
    public void CollectDependenciesInfo()
    {
        try
        {          
            ReadFromCache();
            var allAssets = AssetDatabase.GetAllAssetPaths();
            int totalCount = allAssets.Length;
            for (int i = 0; i < allAssets.Length; i++)
            {
                //每遍历100个Asset，更新一下进度条，同时对进度条的取消操作进行处理
                if ((i % 100 == 0) && EditorUtility.DisplayCancelableProgressBar("Refresh", string.Format("Collecting {0} assets", i), (float)i / totalCount))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
                if (File.Exists(allAssets[i]))
                    ImportAsset(allAssets[i]);
                if (i % 2000 == 0)
                    GC.Collect();
            }      
            //将信息写入缓存
            EditorUtility.DisplayCancelableProgressBar("Refresh", "Write to cache", 1f);
            WriteToChache();
            //生成引用数据
            EditorUtility.DisplayCancelableProgressBar("Refresh", "Generating asset reference info", 1f);
            UpdateReferenceInfo();
            EditorUtility.ClearProgressBar();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            EditorUtility.ClearProgressBar();
        }
    }

    //通过依赖信息更新引用信息
    private void UpdateReferenceInfo()
    {
        foreach(var asset in assetDict)
        {
            foreach(var assetGuid in asset.Value.dependencies)
            {
                if(assetDict.ContainsKey(assetGuid) && !assetDict[assetGuid].references.Contains(asset.Key))
                {
                    assetDict[assetGuid].references.Add(asset.Key);
                }
            }
        }
    }

    //生成并加入引用信息
    private void ImportAsset(string path)
    {
        if (!path.StartsWith("Assets/"))
            return;

        //通过path获取guid进行储存
        string guid = AssetDatabase.AssetPathToGUID(path);
        //获取该资源的最后修改时间，用于之后的修改判断
        Hash128 assetDependencyHash = AssetDatabase.GetAssetDependencyHash(path);
        //如果assetDict没包含该guid或包含了修改时间不一样则需要更新
        if (!assetDict.ContainsKey(guid) || assetDict[guid].assetDependencyHash != assetDependencyHash.ToString())
        {
            //将每个资源的直接依赖资源转化为guid进行储存
            var guids = AssetDatabase.GetDependencies(path, false).
                Select(p => AssetDatabase.AssetPathToGUID(p)).
                ToList();

            //生成asset依赖信息，被引用需要在所有的asset依赖信息生成完后才能生成
            AssetDescription ad = new AssetDescription();
            ad.name = Path.GetFileNameWithoutExtension(path);
            ad.path = path;
            ad.assetDependencyHash = assetDependencyHash.ToString();
            ad.dependencies = guids;

            if (assetDict.ContainsKey(guid))
                assetDict[guid] = ad;
            else
                assetDict.Add(guid, ad);
        }
    }

    //读取缓存信息
    public unsafe bool ReadFromCache()
    {
        assetDict.Clear();
        if (!File.Exists(CACHE_PATH))
        {
            return false;
        }

        var serializedGuid = new List<string>();
        var serializedDependencyHash = new List<string>();
        var serializedDenpendencies = new List<int[]>();
        //反序列化数据
        //FileStream fs = File.OpenRead(CACHE_PATH);
        try
        {
            //BinaryFormatter bf = new BinaryFormatter();
           // string cacheVersion = (string) bf.Deserialize(fs);
            //if (cacheVersion != CACHE_VERSION)
           // {
                //return false;
           // }

            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache", 0);
            //serializedGuid = (List<string>) bf.Deserialize(fs);
            //serializedDependencyHash = (List<string>) bf.Deserialize(fs);
            //serializedDenpendencies = (List<int[]>) bf.Deserialize(fs);


            int cstsint = CreateFlatBuffersFileTest(CACHE_PATH,false);
            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache 0.1 ", 0.1f);
            if (cstsint == 1)
            {
                Debug.Log("序列化文件已经存在");
            }
            else if (cstsint == 0)
            {
                Debug.Log("序列化文件不存在");
            }
            int bs;
            IntPtr dd = (IntPtr)(&bs);
            readgunserializedGuidSize(CACHE_PATH, dd);
            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache 0.3", 0.3f);
            // 创建字符串数组
            string[] stringArray = new string[bs];
             // 分配非托管内存
             IntPtr unmanagedArray = Marshal.AllocHGlobal(IntPtr.Size * stringArray.Length);

            // 调用C++函数
            readgunserializedGuid(CACHE_PATH, bs, unmanagedArray);
            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache 0.4 ", 0.4f);

            // 从非托管内存中读取字符串
            for (int i = 0; i < stringArray.Length; i++)
            {
                IntPtr strPtr = Marshal.ReadIntPtr(unmanagedArray, i * IntPtr.Size);
                stringArray[i] = Marshal.PtrToStringAnsi(strPtr);
                
            }
            List<string> bbb = new List<string>();
            serializedGuid = stringArray.ToList();
            // 释放非托管内存
            Marshal.FreeHGlobal(unmanagedArray);
            int DependencyHashSize = 0;
            IntPtr DependencyHashSizeptr = (IntPtr)(&DependencyHashSize);
            readgunserializedDependencyHashSize(CACHE_PATH, DependencyHashSizeptr);
            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache .0.5", 0.5f);
            // 创建字符串数组
            string[] DependencyHasheArray = new string[DependencyHashSize];
            // 分配非托管内存
            IntPtr DependencyHasheArrayPtr = Marshal.AllocHGlobal(IntPtr.Size * stringArray.Length);
            readgunserializedDependencyHash(CACHE_PATH, DependencyHashSize, DependencyHasheArrayPtr);
            // 从非托管内存中读取字符串
            for (int i = 0; i < DependencyHasheArray.Length; i++)
            {
                IntPtr strPtr = Marshal.ReadIntPtr(DependencyHasheArrayPtr, i * IntPtr.Size);
                DependencyHasheArray[i] = Marshal.PtrToStringAnsi(strPtr);

            }
            serializedDependencyHash = DependencyHasheArray.ToList();
            // 释放非托管内存
            Marshal.FreeHGlobal(DependencyHasheArrayPtr);




            int aba = 0;
            IntPtr abaptr = (IntPtr)(&aba);
            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache 0.6", 0.6f);
            readgunserializedDenpendenciesSize(CACHE_PATH, abaptr);
            
            int[] bbbc = new int[aba];
            int resvalue = readgunserializedDenpendenciesIntArraySize(CACHE_PATH, aba, bbbc);


            List<int[]> list = new List<int[]>();

            for (int i = 0; i < aba; i++)
            {
                list.Add(new int[bbbc[i]]);
                //list.Add(new int[0]);
            }
            // 转换为IntPtr  
            IntPtr[] pointers = new IntPtr[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                pointers[i] = Marshal.AllocHGlobal(list[i].Length * sizeof(int));
                Marshal.Copy(list[i], 0, pointers[i], list[i].Length);
            }

            
            MyDelegate functest = new MyDelegate(MyFunction);
            IntPtr functestptr = Marshal.GetFunctionPointerForDelegate(functest);
            EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache", 0.7f);
            readgunserializedDenpendencies(CACHE_PATH, aba, 1, pointers, functestptr, functestptr);


            Marshal.Copy(pointers, 0, (IntPtr)((IntPtr)Marshal.AllocHGlobal(pointers.Length * sizeof(IntPtr))), pointers.Length);
            // 读取修改后的数据  
            for (int i = 0; i < list.Count; i++)
            {
                int[] modifiedArray = new int[list[i].Length];
                Marshal.Copy(pointers[i], modifiedArray, 0, list[i].Length);
                list[i] = modifiedArray;

                // 清理内存  
                Marshal.FreeHGlobal(pointers[i]);
            }
            serializedDenpendencies = list;
            // 输出修改后的数组  
            //foreach (int[] arr in list)
            //{
                // Debug.Log($"[{String.Join(", ", arr)}]");
            //}

            EditorUtility.ClearProgressBar();
        }
        catch
        {
            //兼容旧版本序列化格式
            return false;
        }
        finally
        {
            //fs.Close();
        }

        for (int i = 0; i < serializedGuid.Count; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(serializedGuid[i]);
            if (!string.IsNullOrEmpty(path))
            {
                var ad = new AssetDescription();
                ad.name = Path.GetFileNameWithoutExtension(path);
                ad.path = path;
                ad.assetDependencyHash = serializedDependencyHash[i];
                assetDict.Add(serializedGuid[i], ad);
            }
        }

        for(int i = 0; i < serializedGuid.Count; ++i)
        {
            string guid = serializedGuid[i];
            if (assetDict.ContainsKey(guid))
            {
                var guids = serializedDenpendencies[i].
                    Select(index => serializedGuid[index]).
                    Where(g => assetDict.ContainsKey(g)).
                    ToList();
                // 这里太慢了？
                //Debug.Log($"[{String.Join(", ", guids)}]");
                
                assetDict[guid].dependencies = guids;
            }
        }
        //  后面也很慢
        UpdateReferenceInfo();
        return true;
    }

    //写入缓存
    private void WriteToChache()
    {
        if (File.Exists(CACHE_PATH))
            File.Delete(CACHE_PATH);

        var serializedGuid = new List<string>();
        var serializedDependencyHash = new List<string>();
        var serializedDenpendencies = new List<int[]>();
        //辅助映射字典
        var guidIndex = new Dictionary<string, int>();
        //序列化
        using (FileStream fs = File.OpenWrite(CACHE_PATH))
        {
            foreach (var pair in assetDict)
            {
                guidIndex.Add(pair.Key, guidIndex.Count);
                serializedGuid.Add(pair.Key);
                serializedDependencyHash.Add(pair.Value.assetDependencyHash);
            }

            foreach(var guid in serializedGuid)
            {
                //使用 Where 子句过滤目录
                int[] indexes = assetDict[guid].dependencies.
                    Where(s => guidIndex.ContainsKey(s)).
                    Select(s => guidIndex[s]).ToArray();
                serializedDenpendencies.Add(indexes);
            }

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, CACHE_VERSION);
            bf.Serialize(fs, serializedGuid);
            bf.Serialize(fs, serializedDependencyHash);
            bf.Serialize(fs, serializedDenpendencies);
        }
    }

    public static int MyFunction(int value)
    {
        return value * 2;
    }

    //更新引用信息状态
    public void UpdateAssetState(string guid)
    {
        AssetDescription ad;
        if (assetDict.TryGetValue(guid,out ad) && ad.state != AssetState.NODATA)
        {            
            if (File.Exists(ad.path))
            {
                //修改时间与记录的不同为修改过的资源
                if (ad.assetDependencyHash != AssetDatabase.GetAssetDependencyHash(ad.path).ToString())
                {
                    ad.state = AssetState.CHANGED;
                }
                else
                {
                    //默认为普通资源
                    ad.state = AssetState.NORMAL;
                }
            }
            //不存在为丢失
            else
            {
                ad.state = AssetState.MISSING;
            }
        }
        
        //字典中没有该数据
        else if(!assetDict.TryGetValue(guid, out ad))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ad = new AssetDescription();
            ad.name = Path.GetFileNameWithoutExtension(path);
            ad.path = path;
            ad.state = AssetState.NODATA;
            assetDict.Add(guid, ad);
        }
    }

    //根据引用信息状态获取状态描述
    public static string GetInfoByState(AssetState state)
    {
        if(state == AssetState.CHANGED)
        {
            return "<color=#F0672AFF>Changed</color>";
        }
        else if (state == AssetState.MISSING)
        {
            return "<color=#FF0000FF>Missing</color>";
        }
        else if(state == AssetState.NODATA)
        {
            return "<color=#FFE300FF>No Data</color>";
        }
        return "Normal";
    }

    public class AssetDescription
    {
        public string name = "";
        public string path = "";
        public string assetDependencyHash;
        public List<string> dependencies = new List<string>();
        public List<string> references = new List<string>();
        public AssetState state = AssetState.NORMAL;
    }

    public enum AssetState
    {
        NORMAL,
        CHANGED,
        MISSING,
        NODATA,        
    }
}
