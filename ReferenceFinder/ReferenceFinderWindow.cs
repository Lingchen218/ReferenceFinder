﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System;
using static UnityEngine.Rendering.DebugUI;
using static UnityEditor.Progress;
using System.Globalization;
using System.Linq;

public class ReferenceFinderWindow : EditorWindow
{
    //依赖模式的key
    const string isDependPrefKey = "ReferenceFinderData_IsDepend";
    //是否需要更新信息状态的key
    const string needUpdateStatePrefKey = "ReferenceFinderData_needUpdateState";

    private static ReferenceFinderData data = new ReferenceFinderData();
    private static bool initializedData = false;
    
    private bool isDepend = false;
    private bool needUpdateState = true;

    private bool needUpdateAssetTree = false;
    private bool initializedGUIStyle = false;
    //工具栏按钮样式
    private GUIStyle toolbarButtonGUIStyle;
    //工具栏样式
    private GUIStyle toolbarGUIStyle;
    //选中资源列表
    private List<string> selectedAssetGuid = new List<string>();    

    private AssetTreeView m_AssetTreeView;

    [SerializeField]
    private TreeViewState m_TreeViewState;

    [DllImport("Unity_FlatBuffers_Dll")]
    private static extern int Add(int x, int y, string b, List<string> bbbb);
    [DllImport("Unity_FlatBuffers_Dll")]
    private static extern int Max(int x, int y);

    [DllImport("Unity_FlatBuffers_Dll")]
    static  extern bool GenerateItems(   IntPtr itemCount, IntPtr itemsFound);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool GenerateItems1(int arrayLength,string[] stringArray );

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool readgunserializedGuid(string file, int arrayLength, IntPtr stringArray);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool readgunserializedGuidSize(string file, IntPtr arrayLength);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool readgunserializedDependencyHash(string file, int arrayLength, IntPtr stringArray);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool readgunserializedDependencyHashSize(string file, IntPtr arrayLength);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool readgunserializedDenpendencies(string file, int arrayLength, int itemCount2, IntPtr[] stringArray, IntPtr callbackfun, IntPtr delegatefun);

    [DllImport("Unity_FlatBuffers_Dll")]
    static extern bool readgunserializedDenpendenciesSize(string file, IntPtr arrayLength);

    //查找资源引用信息
    [MenuItem("Assets/Find References In Project %#&f", false, 25)]
    static unsafe void test()
    {

        int bs;
        IntPtr dd = (IntPtr)(&bs);

        // 创建字符串数组
        string[] stringArray = new string[560000];

        // 分配非托管内存
        IntPtr unmanagedArray = Marshal.AllocHGlobal(IntPtr.Size * stringArray.Length);

        // 调用C++函数
        readgunserializedGuid("xdfdf", 10, unmanagedArray);
        // FillStringArray(unmanagedArray, stringArray.Length);

        // 从非托管内存中读取字符串
        for (int i = 0; i < stringArray.Length; i++)
        {
            IntPtr strPtr = Marshal.ReadIntPtr(unmanagedArray, i * IntPtr.Size);
            stringArray[i] = Marshal.PtrToStringAnsi(strPtr);
            
        }

        // 释放非托管内存
        Marshal.FreeHGlobal(unmanagedArray);

        List<string> bbb = new List<string>();
        bbb = stringArray.ToList();
        Debug.Log("xxxxxx zhuanhyuangchengg" + bbb.Count.ToString());
        int ii = 0;
        foreach (string b in bbb)
        {
            Debug.Log("xxxxxx zhuanhyuangchengg" + b);
            ii++;
            if (ii == 300)
            {
                break;
            }
        }
        




    }
    static void FindRef()
    {
       
            //Add(1, 2);
         Debug.Log("xxxxxx"); 
        
        List<string> liststr = new List<string>();
        liststr.Add("bbbbb");
        liststr.ToArray();

        // Debug.Log("xxxxxx Start  " + Add(1, 2, "xxxxx", liststr));
        Debug.Log("xxxxxx Start  " + Max(1, 2));
        InitDataIfNeeded();
        OpenWindow();
        ReferenceFinderWindow window = GetWindow<ReferenceFinderWindow>();
        window.UpdateSelectedAssets();
    }
    
    //打开窗口
    [MenuItem("Window/Reference Finder", false, 1000)]
    static void OpenWindow()
    {
        ReferenceFinderWindow window = GetWindow<ReferenceFinderWindow>();
        window.wantsMouseMove = false;
        window.titleContent = new GUIContent("Ref Finder");
        window.Show();
        window.Focus();        
    }

    //初始化数据
    static void InitDataIfNeeded()
    {
        if (!initializedData)
        {
            //初始化数据
            if(!data.ReadFromCache())
            {
                data.CollectDependenciesInfo();
            }
            initializedData = true;
        }
    }

    //初始化GUIStyle
    void InitGUIStyleIfNeeded()
    {
        if (!initializedGUIStyle)
        {
            toolbarButtonGUIStyle = new GUIStyle("ToolbarButton");
            toolbarGUIStyle = new GUIStyle("Toolbar");
            initializedGUIStyle = true;
        }
    }
    
    //更新选中资源列表
    private void UpdateSelectedAssets()
    {
        selectedAssetGuid.Clear();
        foreach(var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            //如果是文件夹
            if (Directory.Exists(path))
            {
                string[] folder = new string[] { path };
                //将文件夹下所有资源作为选择资源
                string[] guids = AssetDatabase.FindAssets(null, folder);
                foreach(var guid in guids)
                {
                    if (!selectedAssetGuid.Contains(guid) &&
                        !Directory.Exists(AssetDatabase.GUIDToAssetPath(guid)))
                    {
                        selectedAssetGuid.Add(guid);
                    }                        
                }
            }
            //如果是文件资源
            else
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                selectedAssetGuid.Add(guid);
            }
        }
        needUpdateAssetTree = true;
    }

    //通过选中资源列表更新TreeView
    private void UpdateAssetTree()
    {
        if (needUpdateAssetTree && selectedAssetGuid.Count != 0)
        {
            var root = SelectedAssetGuidToRootItem(selectedAssetGuid);
            if(m_AssetTreeView == null)
            {
                //初始化TreeView
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();
                var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width);
                var multiColumnHeader = new MultiColumnHeader(headerState);
                m_AssetTreeView = new AssetTreeView(m_TreeViewState, multiColumnHeader);
            }
            m_AssetTreeView.assetRoot = root;
            m_AssetTreeView.CollapseAll();
            m_AssetTreeView.Reload();
            needUpdateAssetTree = false;
        }
    }

    private void OnEnable()
    {
        isDepend = PlayerPrefs.GetInt(isDependPrefKey, 0) == 1;
        needUpdateState = PlayerPrefs.GetInt(needUpdateStatePrefKey, 1) == 1;
    }

    private void OnGUI()
    {
        InitGUIStyleIfNeeded();
        DrawOptionBar();
        UpdateAssetTree();
        if (m_AssetTreeView != null)
        {
            //绘制Treeview
            m_AssetTreeView.OnGUI(new Rect(0, toolbarGUIStyle.fixedHeight, position.width, position.height - toolbarGUIStyle.fixedHeight));
        }        
    }
    
    //绘制上条
    public void DrawOptionBar()
    {
        EditorGUILayout.BeginHorizontal(toolbarGUIStyle);
        //刷新数据
        if (GUILayout.Button("Refresh Data", toolbarButtonGUIStyle))
        {
            data.CollectDependenciesInfo();
            needUpdateAssetTree = true;
            EditorGUIUtility.ExitGUI();
        }
        //修改模式
        bool PreIsDepend = isDepend;
        isDepend = GUILayout.Toggle(isDepend, isDepend ? "Model(Depend)" : "Model(Reference)", toolbarButtonGUIStyle,GUILayout.Width(100));
        if(PreIsDepend != isDepend){
            OnModelSelect();
        }
        //是否需要更新状态
        bool PreNeedUpdateState = needUpdateState;
        needUpdateState = GUILayout.Toggle(needUpdateState, "Need Update State", toolbarButtonGUIStyle);
        if (PreNeedUpdateState != needUpdateState)
        {
            PlayerPrefs.SetInt(needUpdateStatePrefKey, needUpdateState ? 1 : 0);
        }
        GUILayout.FlexibleSpace();

        //扩展
        if (GUILayout.Button("Expand", toolbarButtonGUIStyle))
        {
            if (m_AssetTreeView != null) m_AssetTreeView.ExpandAll();
        }
        //折叠
        if (GUILayout.Button("Collapse", toolbarButtonGUIStyle))
        {
            if (m_AssetTreeView != null) m_AssetTreeView.CollapseAll();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void OnModelSelect()
    {
        needUpdateAssetTree = true;
        PlayerPrefs.SetInt(isDependPrefKey, isDepend ? 1 : 0);
    }


    //生成root相关
    private HashSet<string> updatedAssetSet = new HashSet<string>();
    //通过选择资源列表生成TreeView的根节点
    private  AssetViewItem SelectedAssetGuidToRootItem(List<string> selectedAssetGuid)
    {
        updatedAssetSet.Clear();
        int elementCount = 0;
        var root = new AssetViewItem { id = elementCount, depth = -1, displayName = "Root", data = null };
        int depth = 0;
        var stack = new Stack<string>();
        foreach (var childGuid in selectedAssetGuid)
        {
            var child = CreateTree(childGuid, ref elementCount, depth, stack);
            if (child != null)
                root.AddChild(child);
        }
        updatedAssetSet.Clear();
        return root;
    }
    //通过每个节点的数据生成子节点
    private  AssetViewItem CreateTree(string guid, ref int elementCount, int _depth, Stack<string> stack)
    {
        if (stack.Contains(guid))
            return null;

        stack.Push(guid);
        if (needUpdateState && !updatedAssetSet.Contains(guid))
        {
            data.UpdateAssetState(guid);
            updatedAssetSet.Add(guid);
        }        
        ++elementCount;
        var referenceData = data.assetDict[guid];
        var root = new AssetViewItem { id = elementCount, displayName = referenceData.name, data = referenceData, depth = _depth };
        var childGuids = isDepend ? referenceData.dependencies : referenceData.references;
        foreach (var childGuid in childGuids)
        {
            var child = CreateTree(childGuid, ref elementCount, _depth + 1, stack);
            if (child != null)
                root.AddChild(child);
        }

        stack.Pop();
        return root;
    }
}
