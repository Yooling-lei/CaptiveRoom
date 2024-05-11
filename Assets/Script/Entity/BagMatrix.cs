﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Script.Entity
{
    public class BagMatrix<T>
    {
        /// <summary>
        /// 目前背包定一个简单的功能:
        /// 只有拾取和使用,没有排序等功能
        /// </summary>
        private T[,] _data;

        private readonly int _rowCount;
        private readonly int _columnCount;

        // 表示接下来设置的元素的行数(减少判断次数)
        public int CurrentRow { get; private set; } = 0;
        public (int x, int y) LastPosition { get; set; } = (0, 0);

        // FIXME: 可重构为一维数组

        /*
         * 如果是1维数组,
         * 位置信息需要再存一个数组就行
         * 可以在添加时根据长度计算一下要添加物品的行列即可,
         * 方便插入和删除等,都有封装好的
         *
         */

        public BagMatrix(int rows, int columns)
        {
            _rowCount = rows;
            _columnCount = columns;
            _data = new T[rows, columns];
        }


        // 添加一个元素到背包中,返回元素的位置
        public (int row, int col) PushElement(T value)
        {
            while (true)
            {
                if (CurrentRow >= _rowCount)
                {
                    Debug.Log("背包已满");
                    return (-1, -1);
                }

                // 遍历currentRow
                for (var i = 0; i < _columnCount; i++)
                {
                    if (_data[CurrentRow, i] != null) continue;
                    _data[CurrentRow, i] = value;
                    LastPosition = (CurrentRow, i);
                    return (CurrentRow, i);
                }

                CurrentRow++;
            }
        }

        // TODO: 测试这个函数
        // 移除一个元素
        public void RemoveElement(int row, int column)
        {
            for (var i = 0; i < _rowCount; i++)
            {
                if (i < row) continue;
                for (var j = 0; j < _columnCount; j++)
                {
                    if (i == row && j < column) continue;
                    var nextItem = NextItem(i, j);
                    _data[i, j] = nextItem;
                    if (nextItem != null) continue;
                    CurrentRow = i;
                    return;
                }
            }
        }

        private T NextItem(int row, int column)
        {
            if (column + 1 < _columnCount)
            {
                return _data[row, column + 1];
            }

            if (row + 1 < _rowCount)
            {
                return _data[row + 1, 0];
            }

            return default;
        }


        // 一个遍历方法,接收一个回调函数,若回调函数返回true则返回该元素
        // 这里可以后面优化为Map,再说
        public (T data, int x, int y) FindElement(Func<T, bool> predicate)
        {
            T data = default;
            var i = 0;
            var y = 0;
            bool hasResult = false;
            TraverseElement((item, row, col) =>
            {
                if (!predicate(item)) return false;
                data = item;
                i = row;
                y = col;
                hasResult = true;
                return true;
            });

            if (hasResult) return (data, i, y);
            return default;
        }

        // 遍历所有元素方法
        public void TraverseElement(Func<T, int, int, bool> method)
        {
            for (var i = 0; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    var shouldBreak = method(_data[i, j], i, j);
                    if (!shouldBreak) continue;
                    i = _rowCount;
                    j = _columnCount;
                }
            }
        }


        // 添加访问器、修改器和其他方法
        public T GetElement(int row, int column)
        {
            return _data[row, column];
        }

        public void SetElement(int row, int column, T value)
        {
            _data[row, column] = value;
        }
    }
}