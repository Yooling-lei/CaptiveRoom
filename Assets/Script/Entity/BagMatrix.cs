using UnityEngine;

namespace Script.Entity
{
    public class CustomClass
    {
        // 这里定义你自定义类的属性和方法
    }

    public class BagMatrix
    {
        private CustomClass[,] data;
        private readonly int _rowCount;
        private readonly int _columnCount;

        // 表示接下来设置的元素的行数(减少判断次数)
        public int CurrentRow { get; private set; } = 0;

        public BagMatrix(int rows, int columns)
        {
            _rowCount = rows;
            _columnCount = columns;
            data = new CustomClass[rows, columns];
        }

        public (int row, int col) PushElement(CustomClass value)
        {
            while (true)
            {
                if (CurrentRow >= _rowCount)
                {
                    Debug.Log("背包已满");
                    return (-1, -1);
                }

                // 遍历currentRow
                for (var i = 0; i < _columnCount ; i++)
                {
                    if (data[CurrentRow, i] != null) continue;
                    data[CurrentRow, i] = value;
                    return (CurrentRow, i);
                }

                CurrentRow++;
            }
        }

        // 添加访问器、修改器和其他方法
        public CustomClass GetElement(int row, int column)
        {
            return data[row, column];
        }

        public void SetElement(int row, int column, CustomClass value)
        {
            data[row, column] = value;
        }
    }
}