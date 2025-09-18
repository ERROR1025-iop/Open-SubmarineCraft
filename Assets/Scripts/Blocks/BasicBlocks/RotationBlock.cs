using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace{ public class RotationBlock : SolidBlock
    {

        protected int dir;

        protected static int lastPlaceDir = -10;

        public RotationBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            dir = Dir.up;
        }

        public override void onRotateButtonClick()
        {
            if (dir == Dir.left)
            {
                setDir(Dir.up);
            }
            else
            {
                setDir(dir + 1);
            }
            lastPlaceDir = dir;
        }

        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            if (lastPlaceDir != -10)
            {
                setDir(lastPlaceDir);
            }
        }

        public void initRotationBlock()
        {

        }

        protected virtual void setDir(int dir)
        {
            this.dir = dir;
            getBlockObject().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90 * dir));
        }

        protected Block getRelativeNeighborBlock(int dir)
        {
            return getNeighborBlock(getRelativeDir(dir));
        }

        protected IPoint getRelativeDirPoint(int dir)
        {
            return getCoor().getDirPoint(getRelativeDir(dir));
        }

        public int getRelativeDir(int dir)
        {
            int relativeDir = this.dir + dir;
            if (relativeDir > Dir.left)
            {
                relativeDir -= 4;
            }
            return relativeDir;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "dir", dir);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            setDir(IUtils.getJsonValue2Int(blockData, "dir"));
        }

        public override bool isCanRotate()
        {
            return true;
        }

        public override void clear(bool destroy)
        {
            setDir(Dir.up);
            base.clear(destroy);
        }
    }

}
