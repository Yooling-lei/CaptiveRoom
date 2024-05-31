using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Script.Entity;
using Script.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 临时用于测试的控制器
/// </summary>
public class TestManager : Singleton<TestManager>
{
    public List<GameObject> testInBagItem;
    public GameObject testRawImage;
    public Camera testCamera;
    

    private void Update()
    {

    }


    void Start()
    {

        DebugLogConsole.AddCommand("save", "Save", SaveManager.SaveData);
        // StartCoroutine(TestSave());
        // StartCoroutine(TestGet());
    }

    private IEnumerator TestSave()
    {
        yield return new WaitForSeconds(1);
        SaveManager.SaveData();
    }

    private IEnumerator TestGet()
    {
        yield return new WaitForSeconds(3);
        SaveManager.SaveData();
    }


    private void TestUseItem(string testName)
    {
        BagManager.Instance.UseItemInBag(testName);
    }
}