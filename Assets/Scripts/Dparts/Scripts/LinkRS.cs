using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class LinkRS : RunScript
    {       
        // Delay in frames before executing Start logic (default 5)
        private int startDelayFrames = 2;

        void Start()
        {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            for (int i = 0; i < startDelayFrames; ++i)
                yield return null;

            if (World.GameMode == World.GameMode_Freedom && enabled)
            {
                // 从自己开始往父级遍历，直到找到DpartParent组件，再进行下一步
                DpartParent dpartParent = GetComponentInParent<DpartParent>();
                List<int> linkedUid = dpartParent.getDpart().linkedUid;
                if (linkedUid != null && linkedUid.Count > 0)
                {
                    DpartParent[] allParents = MainSubmarine.instance.GetComponentsInChildren<DpartParent>(true);
                    for (int j = 0; j < allParents.Length; ++j)
                    {
                        DpartParent dp = allParents[j];
                        Dpart dpD = dp.getDpart();
                        if (linkedUid.Contains(dpD.uid))
                        {
                            GameObject go = dp.gameObject;
                            SimulateParentFollow spf = go.AddComponent<SimulateParentFollow>();
                            spf.targetParent = transform;
                        }
                    }
                }
            }
        }
    }
}