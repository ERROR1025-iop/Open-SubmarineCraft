using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using Scraft;
namespace Scraft.DpartSpace
{
    public class DpartsManager
    {

        public static DpartsManager instance;
        
        public const int MAX_DPARTS_ID = 100;
        int idStack;
        bool isRegisterEnd;
        Dpart[] dpartsArr;

        static public DpartsManager get()
        {
            return instance == null ? new DpartsManager() : instance;
        }

        public DpartsManager()
        {
            instance = this;
            dpartsArr = new Dpart[MAX_DPARTS_ID];
            idStack = 0;
            isRegisterEnd = false;          
            registerDParts();
            isRegisterEnd = true;            

            //createLangText();
        }

        void registerDParts()
        {
            registerDPart("cube", "geometry");
            registerDPart("ship_front_1_1", "ship");
            registerDPart("ship_front_1_2", "ship");
            registerDPart("ship_front_2_1", "ship");
            registerDPart("ship_front_2_2", "ship");
            registerDPart("ship_front_2_3", "ship");
            registerDPart("ship_front_3_1", "ship");
            registerDPart("ship_front_3_2", "ship");
            registerDPart("ship_front_3_3", "ship");
            registerDPart("ship_body_1", "ship");
            registerDPart("ship_body_2", "ship");
            registerDPart("ship_body_3", "ship");
            registerDPart("ship_stern_1_1", "ship");
            registerDPart("ship_stern_2_1", "ship");
            registerDPart("ship_stern_2_2", "ship");
            registerDPart("ship_stern_2_3", "ship");

            registerDPart("cube_chamfer_1", "geometry");
            registerDPart("cube_chamfer_2", "geometry");
            registerDPart("cube_chamfer_3", "geometry");
            registerDPart("cube_chamfer_4", "geometry");
            registerDPart("cube_chamfer_5", "geometry");
            registerDPart("cube_chamfer_6", "geometry");
            registerDPart("cube_chamfer_7", "geometry");
            registerDPart("cube_chamfer_8", "geometry");
            registerDPart("cube_chamfer_9", "geometry");
            registerDPart("cylinder_1_6", "geometry");
            registerDPart("cylinder_2_12", "geometry");
            registerDPart("cylinder_3_16", "geometry");

            registerDPart("window_1", "ship");
            registerDPart("window_2", "ship");
            registerDPart("window_3", "ship");
            registerDPart("window_4", "ship");
            registerDPart("window_4_floor", "ship");
            registerDPart("window_4_head", "ship");

            registerDPart("turret_1", "function");
            registerDPart("ship_decorate_1", "ship");
            registerDPart("ship_decorate_2", "ship");
            registerDPart("propeller_1", "transmission");
            registerDPart("rudder_1", "transmission");

            registerDPart("cube_chamfer_10", "geometry");
            registerDPart("sphere_half", "geometry");
            registerDPart("sphere_14_18", "geometry");
            registerDPart("cylinder_half_18", "geometry");
            registerDPart("cube_sub_bridge_1", "submarine");
            registerDPart("cube_sub_bridge_2", "submarine");
            registerDPart("cube_sub_bridge_3", "submarine");
            registerDPart("cube_sub_bridge_4", "submarine");
            registerDPart("cube_sub_bridge_5", "submarine");
            registerDPart("cube_sub_bridge_5_rail_1", "submarine");
            registerDPart("cube_sub_bridge_5_rail_2", "submarine");
            registerDPart("cube_sub_bridge_6", "submarine");
            registerDPart("sphere_sub_front_1", "submarine");
            registerDPart("sphere_sub_front_2", "submarine");
            registerDPart("cube_sub_antenna_1", "submarine");
            registerDPart("propeller_2", "transmission");
            registerDPart("sub_rudder_1", "transmission");
            registerDPart("sub_rudder_2", "transmission");
            registerDPart("sub_rudder_3", "transmission");
            registerDPart("sub_rudder_4", "transmission");

            registerDPart("light_spot_1", "light");
            registerDPart("light_spot_rotate_1", "light");
            registerDPart("light_point_1", "light");

            registerDPart("sub_periscope_1", "submarine");
            registerDPart("telescope_1", "ship");
            registerDPart("console_desk_1", "ship");
            registerDPart("cockpit_1", "submarine");
            registerDPart("cockpit_2", "submarine");
            registerDPart("turret_2", "function");
            registerDPart("turret_3", "function");

            registerDPart("sub_deck_1_1", "submarine");
            registerDPart("sub_deck_1_2", "submarine");
            registerDPart("sub_deck_1_3", "submarine");
            registerDPart("sub_deck_2_1", "submarine");
            registerDPart("sub_deck_2_2", "submarine");
            registerDPart("sub_deck_3_1", "submarine");
            registerDPart("sub_deck_3_2", "submarine");
            registerDPart("sub_deck_1_3_down", "submarine");
            registerDPart("sub_deck_2_2_down", "submarine");
            registerDPart("sub_deck_3_2_down", "submarine");
            registerDPart("light_spot_decorate_1", "light");
            registerDPart("clinder_cut_1", "geometry");
            
            registerDPart("drill_1", "function");
            registerDPart("turret_4", "function");
            registerDPart("cube_ring_1", "geometry");
            registerDPart("cylinder_ring_1", "geometry");
            registerDPart("drill_2", "function");
            registerDPart("drill_3", "function");
            registerDPart("depth_charge_thrower_1", "function");
            
            registerDPart("rotating_platform_1", "function");
            registerDPart("claw_1", "function");
            registerDPart("telescopic_rod_1", "function");
            registerDPart("magnetic_connector", "function");
            registerDPart("cardan_joint_1", "function");
            registerDPart("cardan_joint_2", "function");

        

        }

        void createLangText()
        {
            if (!GameSetting.isAndroid && !GameSetting.isSteam)
            {
                Debug.Log("[Warning]Create thumbnail image is open!");
                JsonWriter writer = new JsonWriter();
                writer.WriteObjectStart();

                for (int i = 0; i < idStack; i++)
                {
                    string name = getDPartById(i).getName();
                    string langName = name.Replace("ship", "船");
                    langName = langName.Replace("front", "头");
                    langName = langName.Replace("body", "体");
                    langName = langName.Replace("stern", "尾");
                    langName = langName.Replace("chamfer", "切角");
                    langName = langName.Replace("cube", "方块");
                    langName = langName.Replace("cylinder", "圆柱");
                    langName = langName.Replace("window", "窗");
                    langName = langName.Replace("floor", "地板");
                    langName = langName.Replace("head", "顶");
                    langName = langName.Replace("decorate", "装饰");
                    langName = langName.Replace("propeller", "螺旋桨");
                    langName = langName.Replace("rudder", "舵");
                    langName = langName.Replace("sphere", "球");
                    langName = langName.Replace("half", "半");
                    langName = langName.Replace("sub", "潜艇");
                    langName = langName.Replace("bridge", "舰桥");
                    langName = langName.Replace("antenna", "天线");
                    langName = langName.Replace("light", "照明");
                    langName = langName.Replace("spot", "探照灯");
                    langName = langName.Replace("rotate", "可旋转");
                    langName = langName.Replace("point", "灯具");
                    IUtils.keyValue2Writer(writer, name, langName);
                }

                writer.WriteObjectEnd();
                string savePath = Application.dataPath + "/Resources/Lang/zh_CHS/" + "dpart.txt";
                IUtils.write2txt(savePath, writer.ToString());
            }
        }     

        public void registerDPart(string name, string attributeCardName, ModInfo modInfo = null)
        {
            Dpart dpart = new Dpart(getUnuserId(), null);
            dpart.initDpart(name, attributeCardName, modInfo);
            registerDPart(dpart);
        }

        void registerDPart(Dpart dpart)
        {
            int id = dpart.getId();
            dpartsArr[id] = dpart;
        }

        int getUnuserId()
        {
            return idStack++;
        }

        public Dpart getDPartById(int id)
        {
            return dpartsArr[id];
        }

        public Dpart getDPartByName(string name)
        {
            if (isRegisterEnd == false)
            {
                return null;
            }
            if (name == "null")
            {
                return null;
            }

            for (int i = 0; i < MAX_DPARTS_ID; i++)
            {
                Dpart block = dpartsArr[i];
                if (block != null)
                {
                    if (block.getName().Equals(name))
                    {
                        return block;
                    }
                }
            }
            Debug.Log("[Warning]can't find dpart:" + name);

            return null;
        }

        public int getBlockCount()
        {
            return idStack;
        }
    }
}
