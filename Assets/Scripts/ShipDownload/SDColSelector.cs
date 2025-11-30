using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scraft
{
    public class SDColSelector : MonoBehaviour
    {
        public List<SDColSelectorCell> cells;
        public string value;

        //添加一个回调函数，用于通知其他脚本cell点击了，使用using UnityEngine.Events
        public UnityEvent<string> onCellChanged;
        void Start()
        {
            foreach (var cell in cells)
            {
                cell.sdColSelector = this;
            }
            onCellClick(cells[0]);
        }

        public void onCellClick(SDColSelectorCell cell)
        {
            value = cell.value;
            foreach (var cell1 in cells)
            {
                if (cell1 != cell)
                {
                    cell1.titleText.color = Color.gray;
                }
                else
                {
                    cell1.titleText.color = Color.green;
                }
            }
            if (onCellChanged != null)
            {
                onCellChanged.Invoke(value);
            }
        }
    } 
}
