using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;

namespace Scraft.DpartSpace
{
    public class DrillRS : RunScript
    {
        public Transform drillPoint;
        public DrillPointRS drillPointRS;       
        public Transform drillHead;
        public Vector3 axis;

        SelectorRS selectorRS;

        BlocksManager blocksManager;

        bool isOpen;
        float speed;

        DrillCore drillCore;
                
        float timer;

        Area stayArea;
        SmallArea staySmallArea;

        void Start()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            blocksManager = BlocksManager.instance;

            selectorRS = GetComponent<SelectorRS>();

            PoolerItemSelector.instance.OnCustom1ButtonClick += OnCustom1ButtonClick;
            PoolerItemSelector.instance.OnCancelButtonClick += OnCancelButtonClick;

            isOpen = true;

            int distance = 99999;
            IPoint coor2d = get2DMapCoor();
            if (DrillCore.drillCores != null)
            {
                foreach (DrillCore core in DrillCore.drillCores)
                {
                    int t_distance = core.getCoor().getFrameDistance(coor2d);
                    if (core.getCoor().getStraightDistance(coor2d) < distance)
                    {
                        distance = t_distance;
                        drillCore = core;
                    }
                }
            }

            timer = 0;
        }

        void OnCustom1ButtonClick()
        {
            if (selectorRS.isSelecting)
            {
                isOpen = !isOpen;
                PoolerItemSelector.instance.setCustom1ButtonText(ILang.get(isOpen ? "Stop" : "Open"));
            }
        }

        void OnCancelButtonClick()
        {
            
        }

        void Update()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            if (drillCore != null)
            {
                if (drillCore.isNeedDelete())
                {
                    drillCore = null;
                    return;
                }

                if (isOpen)
                {

                    speed = drillCore.getSpeed();
                    drillHead.Rotate(axis, speed);
                   
                    timer -= Time.deltaTime;
                    if (timer <= 0 && speed > 10)
                    {
                        Block block = blocksManager.stone;
                        if (AreaManager.staySmallArea != null)
                        {
                            block = AreaManager.staySmallArea.randomOreBlock();
                        }else if (AreaManager.stayArea != null)
                        {
                            block = AreaManager.stayArea.randomOreBlock();
                        }

                        if (drillPointRS.isTouchTerrain)
                        {
                            Pooler.instance.addCargo(block);
                        }
                        timer = 30 / speed;
                    }
                }
            }
        }   

        private void OnDestroy()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }
            PoolerItemSelector.instance.OnCustom1ButtonClick -= OnCustom1ButtonClick;
            PoolerItemSelector.instance.OnCancelButtonClick -= OnCancelButtonClick;
        }
    }
}
