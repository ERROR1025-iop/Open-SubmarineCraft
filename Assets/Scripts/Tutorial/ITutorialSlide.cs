using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scraft
{
    public class ITutorialSlide : MonoBehaviour
    {

        public bool horizontal;       
        public float distance;
        public float speed;

        RectTransform rectTransform;
        Vector2 start;
        Vector2 target;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            start = rectTransform.anchoredPosition;
            target = horizontal ? new Vector2(distance, 0) : new Vector2(0, distance);
            target += rectTransform.anchoredPosition;
        }


        void Update()
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, target, speed * Time.deltaTime);

            if (rectTransform.anchoredPosition == target)
            {
                rectTransform.anchoredPosition = start;
            }
        }
    }
}
