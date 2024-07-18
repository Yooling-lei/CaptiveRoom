using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Script.Controller.Interactable;
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
    public List<PickupItemController> testInBagItem;
    public GameObject testRawImage;
    public Camera testCamera;
    

    void Start()
    {
        DebugLogConsole.AddCommand("save", "Save", SaveManager.SaveData);
        DebugLogConsole.AddCommand("load", "Save", SaveManager.LoadData);
        DebugLogConsole.AddCommand("add", "AddItemToBag", AddTestItemToBag);
    }

    private void AddTestItemToBag()
    {
        BagManager.Instance.SetBagSelectMode(EBagSelectMode.Multiple);
        // BagManager.Instance._b
        // 1. 破碎杯子
        // 2. 胶水
        foreach (var item in testInBagItem)
        {
            BagManager.Instance.AddItemToPackage(item.itemName, item);
        }
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