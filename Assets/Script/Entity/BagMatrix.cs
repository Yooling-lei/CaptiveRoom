using System;
using UnityEngine;

namespace Script.Entity
{
    public class BagMatrix<T>
    {
        private T[,] _data;
        private readonly int _rowCount;
        private readonly int _columnCount;

        // 表示接下来设置的元素的行数(减少判断次数)
        public int CurrentRow { get; private set; } = 0;

        public BagMatrix(int rows, int columns)
        {
            _rowCount = rows;
            _columnCount = columns;
            _data = new T[rows, columns];
        }

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
                    return (CurrentRow, i);
                }

                CurrentRow++;
            }
        }

        // 一个遍历方法,接收一个回调函数,若回调函数返回true则返回该元素
        // 这里可以后面优化为Map,再说
        public T FindElement(Func<T, bool> predicate)
        {
            for (var i = 0; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    if (predicate(_data[i, j]))
                    {
                        return _data[i, j];
                    }
                }
            }

            return default;
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