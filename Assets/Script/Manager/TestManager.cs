using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Script.Entity;
using Script.Manager;
using UnityEngine;

/// <summary>
/// 临时用于测试的控制器
/// </summary>
public class TestManager : Singleton<TestManager>
{
    public GameObject testInBagItem;

    void Start()
    {
        DebugLogConsole.AddCommand("AddItem", "Add Item In To Bag", TestAddToBag);
        DebugLogConsole.AddCommand("RemoveItem", "Remove an item from the backpack.", TestRemoveElement);
    }

    private int testIndex = 0;
    private readonly BagMatrix<ItemInPackage> _bagMatrix = new(4, 3);

    private void TestAddToBag()
    {
        TestAddToBag2();
        TestAddToBag2();
        TestAddToBag2();
        TestAddToBag2();
    }
    private void TestAddToBag2()
    {
        // 随机生成一个string
        var random = new System.Random();
        var str = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 5)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        GameManager.Instance.AddIntoBag(str + testIndex, testInBagItem, _bagMatrix);
        testIndex++;
    }

    private void TestRemoveElement()
    { 
        _bagMatrix.RemoveElement(0, 1);
        _bagMatrix.TraverseElement((x, row, col) =>
        {
            if (x == null) return true;
            Debug.Log($" {x.ItemName} : ({row},{col})");
            return false;
        });
    }
}