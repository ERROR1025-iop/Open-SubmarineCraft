using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class StationsManager
    {
        static public StationsManager instance;

        public List<ComponentInfo> componentInfos;

        public int componentCount;

        static public StationsManager getInstance()
        {
            if(instance == null)
            {
                instance = new StationsManager();               
            }
            return instance;
        }

        public StationsManager()
        {
            instance = this;
            componentInfos = new List<ComponentInfo>();
            componentCount = 0;
            registerStations();
        }

        void registerStations()
        {
            BlocksManager bm = BlocksManager.instance;

            registerStation("wharf 01", new int[3] { 0, 0, 0}, new Block[] { bm.stone, bm.asphalt }, new int[] { 80, 10 });
            registerStation("warehouse 01", new int[3] { 300, 30, 0}, new Block[] { bm.stone, bm.wood }, new int[] { 80, 30 });
            registerStation("planting house 01", new int[3] { 50, 0, 0 }, new Block[] { bm.stone, bm.soil }, new int[] { 30, 60 });
            registerStation("warehouse 02", new int[3] { 1500, 100, 0 }, new Block[] { bm.stone, bm.steel }, new int[] { 200, 60 });
            registerStation("warehouse 03", new int[3] { 2300, 150, 30000 }, new Block[] { bm.stone, bm.steel, bm.battery }, new int[] { 250, 90, 21 });
            registerStation("warehouse 04", new int[3] { 3000, 300, 60000 }, new Block[] { bm.stone, bm.steel, bm.battery }, new int[] { 350, 170, 42 });
            registerStation("warehouse 05", new int[3] { 2500, 300, 60000 }, new Block[] { bm.stone, bm.steel, bm.battery }, new int[] { 350, 130, 42 });
            registerStation("batterhouse 01", new int[3] { 0, 0, 300000 }, new Block[] { bm.stone, bm.steel, bm.battery }, new int[] { 200, 100, 210 });
            registerStation("road 01", new int[3] { 0, 0, 0 }, new Block[] { bm.stone, bm.asphalt }, new int[] { 80, 10 });
            registerStation("oil tank 01", new int[3] { 0, 1000, 0 }, new Block[] { bm.steel }, new int[] { 70 });
            registerStation("oil tank 02", new int[3] { 0, 2000, 0 }, new Block[] { bm.steel }, new int[] { 150});
            registerStation("solar power 01", new int[3] { 0, 0, 33600 }, new Block[] { bm.steel, bm.circuitBoard, bm.battery, bm.semiconductor }, new int[] { 30, 24, 24, 60 });
            registerStation("warehouse 06", new int[3] { 100, 100, 0 }, new Block[] { bm.stone, bm.soil, bm.wood }, new int[] { 100, 30, 100 });
            registerStation("batterhouse 02", new int[3] { 0, 0, 7000 }, new Block[] { bm.stone, bm.wood, bm.battery }, new int[] { 200, 100, 5 });
        }

        void registerStation(string name, int[] canStore, Block[] blocks, int[] counts)
        {
            ComponentInfo componentInfo = new ComponentInfo(componentCount, name, canStore, blocks, counts);
            componentInfos.Add(componentInfo);
            componentCount++;
        }
    }
}
