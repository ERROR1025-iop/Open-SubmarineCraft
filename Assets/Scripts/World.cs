using Scraft.AchievementSpace;
using Scraft.BlockSpace;
using Scraft.DpartSpace;
using UnityEngine;


namespace Scraft
{
    public class World : MonoBehaviour
    {


        public static IPoint mapSize = new IPoint(200, 200);
        public static Vector3 ouSizeVector = new Vector3(9999, 9999, 0);

        public static string mapName = "unname";
        public static string dpartName = "unname";
        public static string nextSceneName;

        public int gameMode = 0;
        public static int GameMode = 0;
        public static int GameMode_Builder = 0;
        public static int GameMode_Freedom = 1;
        public static int GameMode_Assembler = 2;        


        /// <summary>
        /// 0:2D+3D视图；
        /// 1:2D视图；
        /// 2:3D视图；
        /// 3:3D+2D视图;
        /// </summary>
        public static int activeCamera = 0;

        public static bool stopUpdata;

        public static GameObject dpartParentObject;

        public static World instance;
        public BlocksManager blocksManager;
        public BlocksEngine blocksEngine;
        public ACManager achManager;
        public DpartsManager dpartsManager;        
        public DpartMaterialsManager dpartColorManager;



        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            instance = this;

            GamePath.init("7B94042C464FA171B0C41BC6B2C04911");
            ISecretLoad.init(); 

            achManager = new ACManager();
            blocksManager = BlocksManager.get();
            dpartsManager = DpartsManager.get();
            dpartColorManager = new DpartMaterialsManager();
            blocksEngine = new BlocksEngine(GameObject.Find("2D Builder Map"), mapSize);
            dpartParentObject = GameObject.Find("3D DParts Map");

            GameMode = gameMode;          
            
        }

        void Update()
        {

        }

        public static void changeMode(int mode)
        {
            if (GameMode == GameMode_Freedom)
            {
                Pooler.changeMode(mode);

            }
            else if (GameMode == GameMode_Builder)
            {
                Builder.changeMode(mode);
            }
            else
            {

            }

        }

        void OnApplicationQuit()
        {
            Pooler.isRunThread = false;
            Debug.Log("Stop Thread");
        }
    }
}
