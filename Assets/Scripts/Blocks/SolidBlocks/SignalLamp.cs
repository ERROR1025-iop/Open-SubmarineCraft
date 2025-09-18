using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SignalLamp : Wifi
    {

        float voltage;

        public SignalLamp(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("signalLamp", "signal");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(1f, 0f, 0f);
            density = 3.1f;
            transmissivity = 2.85f;
            voltage = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SignalLamp block = new SignalLamp(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 2 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            voltage = value;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            checkStateRule(blocksEngine);
            voltage = 0;
        }

        protected virtual void checkStateRule(BlocksEngine blocksEngine)
        {
            if (voltage > LogicGate.StandardVoltage)
            {
                setSpriteRect(1);
            }
            else
            {
                setSpriteRect(0);
            }
        }

        public override int isWeSystem()
        {
            return 1;
        }

    }
}
