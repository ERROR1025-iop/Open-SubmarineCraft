using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft{ public class IScrollView : MonoBehaviour
    {

        public int perRowCellCount = 1;

        Transform contentTransform;
        RectTransform contentRectTransform;
        float perCellHeight = 32;

        public void addCell(Transform cell)
        {
            if (contentTransform == null)
            {
                contentTransform = transform.GetChild(0).GetChild(0);
                contentRectTransform = contentTransform.GetComponent<RectTransform>();
            }
            cell.SetParent(contentTransform);
            int count = contentTransform.childCount;
            cell.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, count * perCellHeight / perRowCellCount);
        }

        public void setPerCellHeight(float h)
        {
            perCellHeight = h;
        }

        public void removeCell(int index)
        {
            Destroy(contentTransform.GetChild(index).gameObject);
        }

        public void removeAllCell()
        {
            int count = contentTransform.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(contentTransform.GetChild(i).gameObject);
            }
        }

    }
}
