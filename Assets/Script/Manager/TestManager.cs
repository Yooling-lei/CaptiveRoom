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
    public GameObject testInBagItem;
    public GameObject testRawImage;
    public Camera testCamera;

    public void testCalculate(Vector2 mousePoint)
    {
        // 获取 rawImage的宽高
        var width = testRawImage.GetComponent<RectTransform>().rect.width;
        var height = testRawImage.GetComponent<RectTransform>().rect.height;
        Debug.Log("width: " + width + " height: " + height);

        // 范围: 中心点位 + - 宽高的一半
        var pointPosition = testRawImage.transform.position;
        var xRange = new Vector2(pointPosition.x - width / 2, pointPosition.x + width / 2);
        var yRange = new Vector2(pointPosition.y - height / 2, pointPosition.y + height / 2);
        Debug.Log("Range:" + xRange + " " + yRange);

        // 判断是否在范围内
        if (mousePoint.x > xRange.x && mousePoint.x < xRange.y && mousePoint.y > yRange.x && mousePoint.y < yRange.y)
        {
            Debug.Log("In Range");
        }
        else
        {
            Debug.Log("Out Range");
        }

        // 1.panel的中心点位
        // 2.panel中心点位 => 宽高 => 左上角点位
        // 3.根据左上角点位计算每个格子的位置

        // slotImage: 250 * 250
        // 偏移量: 间距: 200 = 250%2 + 75(margin)
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            var mousePos = Mouse.current.position.ReadValue();
            // testCalculate(mousePos);
            Vector3 worldPos = testCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            // Debug.Log("worldPos: " + worldPos);
        }
    }


    void Start()
    {
        // DebugLogConsole.AddCommand("AddItem", "Add Item In To Bag", TestAddToBag);
        // DebugLogConsole.AddCommand<string>("RemoveItem", "Remove an item from the backpack.", TestRemoveElement);
        TestAddToBag();
        DebugLogConsole.AddCommand<string>("UseItem", "Use an item from the backpack.", TestUseItem);
    }

    private int testIndex = 0;

    private void TestAddToBag()
    {
        TestAddToBag2("test1");
        TestAddToBag2("test2");
        TestAddToBag2("test3");
        TestAddToBag2("test4");
        TestAddToBag2("test5");
        TestAddToBag2("test5");
        TestAddToBag2("test6");
        TestAddToBag2("test6");
        TestAddToBag2("test6");
        TestAddToBag2("test6");
    }

    private void TestUseItem(string testName)
    {
        BagManager.Instance.OnItemUse(testName);
    }

    private void TestAddToBag2(string testName)
    {
        var matrix = BagManager.Instance.GameBagMatrix;
        var (found, _, _) = BagManager._FindElement(testName, matrix);
        if (found != null)
        {
            found.Count++;
            found.RefreshCountText();
        }
        else
        {
            BagManager.Instance.AddIntoBagMatrix(testName, testInBagItem, matrix);
        }
    }

    private void TestRemoveElement(string testName)
    {
        var matrix = BagManager.Instance.GameBagMatrix;
        BagManager.Instance.RemoveItemFromPackage(testName, matrix);
    }
}