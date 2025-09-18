using Scraft.BlockSpace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class PoolerUI
    {

        public static PoolerUI instance;
        Pooler pooler;
        MainSubmarine subMono;

        Transform followSubTrans;

        Camera Camera3DWorld;
        Camera Camera2DWorld;

        BlocksEngine blocksEngine;
        BlocksManager blocksManager;

        IRect shipRect;

        public delegate void FireButtonClickDelegate();
        public event FireButtonClickDelegate OnFireButtonClick;

        public IPressButton upButton;
        public IPressButton downButton;
        public IChangeImageButton autoButton;
        public IChangeImageButton periscopeButton;
        public IChangeImageButton viewButton;
        public IChangeImageButton heatViewButton;
        public IChangeImageButton viewLockButton;
        public IChangeImageButton view2DRotateButton;
        public IPressButton comButton1;
        public IPressButton comButton2;
        public IChangeImageButton radarButton;
        public IChangeImageButton lightButton;
        public IChangeImageButton directionButton;
        public IChangeImageButton autoPitchButton;
        public BarHorizontal rudder;
        public BarVertical powerBar;
        public PitchBar pitchBar;
        public RectTransform radarRect;
        public RectTransform radarTextRect;
        public Image fireButtonImage;
        public Button fireButton;
        public Sprite[] fireButtonSprites;   

        public Radar radar;
        public DeepGauge deepGauge;
        public Gyro gyro;
        public ElectricMeter electricMeter;
        public AirMeter airMeter;

        public PoolerMenu poolerMenu;
        public PoolerGameOver poolerGameOver;

        List<SolidBlock> bindBlockArr;

        Text timeText;       

        public int attackAiCount;

        int hitingTime;
        int behitingTime;
        int hitLabelShowTime;
        int behitLabelShowTime;
        bool isShowHitLabel;
        bool isShowBehitLabel;
        GameObject behitLabel;
        GameObject hitLabel;
        Text behitLabelText;
        Text hitLabelText;

        float subRotoZ;

        public PoolerUI()
        {
            instance = this;

            pooler = Pooler.instance;
            subMono = MainSubmarine.instance;
            blocksEngine = BlocksEngine.instance;
            blocksManager = BlocksManager.instance;

            bindBlockArr = new List<SolidBlock>();

            upButton = GameObject.Find("Canvas/up").GetComponent<IPressButton>();
            downButton = GameObject.Find("Canvas/down").GetComponent<IPressButton>();
            rudder = GameObject.Find("Canvas/h bar").GetComponent<BarHorizontal>();
            powerBar = GameObject.Find("Canvas/power bar").GetComponent<BarVertical>();
            pitchBar = GameObject.Find("Canvas/pitch bar").GetComponent<PitchBar>();
            autoButton = GameObject.Find("Canvas/auto").GetComponent<IChangeImageButton>();
            periscopeButton = GameObject.Find("Canvas/periscope").GetComponent<IChangeImageButton>();
            viewButton = GameObject.Find("Canvas/view").GetComponent<IChangeImageButton>();
            viewLockButton = GameObject.Find("Canvas/view_lock").GetComponent<IChangeImageButton>();
            timeText = GameObject.Find("Canvas/radar rect/radar text rect/time").GetComponent<Text>();
            lightButton = GameObject.Find("Canvas/searchlight").GetComponent<IChangeImageButton>();
            comButton1 = GameObject.Find("Canvas/comButton1").GetComponent<IPressButton>();
            comButton2 = GameObject.Find("Canvas/comButton2").GetComponent<IPressButton>();
            radarButton = GameObject.Find("Canvas/radarButton").GetComponent<IChangeImageButton>();
            directionButton = GameObject.Find("Canvas/directionButton").GetComponent<IChangeImageButton>();
            autoPitchButton = GameObject.Find("Canvas/autoPitchButton").GetComponent<IChangeImageButton>();
            heatViewButton = GameObject.Find("Canvas/heat_view").GetComponent<IChangeImageButton>();
            view2DRotateButton = GameObject.Find("Canvas/view2d_rotate").GetComponent<IChangeImageButton>();
            fireButton = GameObject.Find("Canvas/fire").GetComponent<Button>();
            fireButton.onClick.AddListener(onfireButtonClick);
            fireButtonImage = fireButton.transform.GetComponent<Image>();
            fireButtonSprites = Resources.LoadAll<Sprite>("Pooler/fire");



            radarRect = GameObject.Find("Canvas/radar rect").GetComponent<RectTransform>();
            radarTextRect = GameObject.Find("Canvas/radar rect/radar text rect").GetComponent<RectTransform>();



            Camera3DWorld = Camera.main;
            Camera2DWorld = GameObject.Find("2D Camera").GetComponent<Camera>();
            followSubTrans = GameObject.Find("3D FollowSub").transform;

            upButton.setValueChangeListener(onUpButtonValueChanged);
            downButton.setValueChangeListener(onDownButtonValueChanged);
            rudder.addListener(onRudderPush);
            powerBar.addListener(onPowerBarPush);
            pitchBar.addListener(onPitchBarPush);
            autoButton.addListener(onAutoButtonClick);
            autoPitchButton.addListener(onAutoPitchButtonClick);
            periscopeButton.addListener(onPeriscopeButtonClick);
            viewButton.addListener(onViewButtonClick);
            lightButton.addListener(onlightButtonClick);
            comButton1.setValueChangeListener(onComButton1ValueChanged);
            comButton2.setValueChangeListener(onComButton2ValueChanged);
            directionButton.addListener(onDirectionButtonClick);
            radarButton.addListener(onRadarButtonClick);
            viewLockButton.addListener(onViewLockButtonClick);
            heatViewButton.addListener(onHeatViewButtonClick);
            view2DRotateButton.addListener(onView2DRotateButtonClick);

            radar = new Radar();
            deepGauge = new DeepGauge();
            gyro = new Gyro();
            electricMeter = new ElectricMeter();
            airMeter = new AirMeter();

            GameObject.Find("Canvas/radar rect/radar text rect/menu btn").GetComponent<Button>().onClick.AddListener(onMenuButtonClick);
            GameObject.Find("Canvas/radar rect/radar text rect/wharf").GetComponent<Button>().onClick.AddListener(onWharfButtonClick); ;
            


            poolerMenu = new PoolerMenu(pooler);
            poolerGameOver = new PoolerGameOver(pooler);            

            initHitText();

            World.activeCamera = 0;
        }

        private void initHitText()
        {
            attackAiCount = 0;

            hitLabel = GameObject.Find("Canvas/hitLabel");
            behitLabel = GameObject.Find("Canvas/behitLabel");
            hitLabelText = hitLabel.GetComponent<Text>();
            behitLabelText = behitLabel.GetComponent<Text>();
            hitLabel.SetActive(false);
            behitLabel.SetActive(false);
            hitLabelText.text = ILang.get("hit", "menu");
            behitLabelText.text = ILang.get("behit", "menu");

            hitLabelShowTime = 0;
            behitLabelShowTime = 0;
            hitingTime = 0;
            behitingTime = 0;
            isShowHitLabel = false;
            isShowBehitLabel = false;
        }

        public void set3DCamera(Camera camera)
        {
            Camera3DWorld = camera;
        }

        public void init()
        {
            shipRect = pooler.getShipRect();

            for (int x = shipRect.x; x < shipRect.getMaxX(); x++)
                for (int y = shipRect.y; y < shipRect.getMaxY(); y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null)
                    {
                        SolidBlock soildblock = block as SolidBlock;
                        if (soildblock != null && soildblock.isCanBind())
                        {
                            int bindId = soildblock.getCurrentBindId();
                            if (bindId != 0)
                            {
                                bindBlockArr.Add(soildblock);
                            }
                        }
                    }
                }

            updateMaxElectric();
            updateMaxAir();
        }

        public void updateMaxElectric()
        {
            float totalMaxElectric = 0;
            float totalElectric = 0;
            int count = Battery.battersArr.Count;
            Block block;
            for (int i = 0; i < count; i++)
            {
                block = Battery.battersArr[i];
                if (block != null && block.getMaxStoreElectric() > 0 && block.isNeedDelete() == false)
                {
                    Battery battery = block as Battery;
                    totalMaxElectric += block.getMaxStoreElectric();
                    totalElectric += battery.getStroreElectric();
                }
            }
            electricMeter.setMaxElectirc(totalMaxElectric);
            electricMeter.setElectric(totalElectric);
        }

        public void updateElectric()
        {
            float totalElectric = 0;
            int count = Battery.battersArr.Count;
            Block block;
            for (int i = 0; i < count; i++)
            {
                block = Battery.battersArr[i];
                if (block != null && block.getMaxStoreElectric() > 0)
                {
                    Battery battery = block as Battery;
                    totalElectric += battery.getStroreElectric();
                }
            }
            electricMeter.setElectric(totalElectric);
        }

        public void updateMaxAir()
        {
            if (shipRect == null)
            {
                return;
            }

            float totalMaxAir = 0;
            float totalAir = 0;
            int count = Battery.battersArr.Count;
            Block block;
            for (int x = shipRect.x; x < shipRect.getMaxX(); x++)
            {
                for (int y = shipRect.y; y < shipRect.getMaxY(); y++)
                {
                    if (Pooler.outsideArea[x, y] == 0)
                    {
                        block = blocksEngine.getBlock(x, y);
                        if (block != null && block.getCanStoreAir() > 0)
                        {
                            totalMaxAir += block.getCanStoreAir();
                            totalAir += block.getStoreAir();
                        }
                    }
                }
            }
            airMeter.setMaxAir(totalMaxAir);
            airMeter.setAir(totalAir);
        }

        public void updateAir()
        {
            float totalAir = 0;          
            Block block;
            for (int x = shipRect.x; x < shipRect.getMaxX(); x++)
            {
                for (int y = shipRect.y; y < shipRect.getMaxY(); y++)
                {
                    if (Pooler.outsideArea[x, y] == 0)
                    {
                        block = blocksEngine.getBlock(x, y);
                        if (block != null && block.getCanStoreAir() > 0)
                        {
                            totalAir += block.getStoreAir();
                        }
                    }
                }
            }
            airMeter.setAir(totalAir);
        }

        public void onMenuButtonClick()
        {
            poolerMenu.show(true);
        }

        public void onWharfButtonClick()
        {
            PoolerStationMenu.instance.show(true);
        }

        public void changeFireImageWeapon(int weapon)
        {
            switch (weapon)
            {
                case 0:
                    fireButtonImage.sprite = fireButtonSprites[0];
                    break;
                case 1:
                    fireButtonImage.sprite = fireButtonSprites[2];
                    break;
                case 2:
                    fireButtonImage.sprite = fireButtonSprites[4];
                    break;
                default:
                    fireButtonImage.sprite = fireButtonSprites[0];
                    break;
            }
        }

        public void onfireButtonClick()
        {
            if (pooler.fireWeapon == 0)
            {
                for (int x = shipRect.x; x < shipRect.getMaxX(); x++)
                    for (int y = shipRect.y; y < shipRect.getMaxY(); y++)
                    {
                        Block block = blocksEngine.getBlock(x, y);
                        if (block != null && block.isNeedDelete() == false)
                        {
                            if (block.onFireButtonClicked())
                            {
                                return;
                            }
                        }

                    }
            }
            else
            {
                if (OnFireButtonClick != null)
                {
                    OnFireButtonClick();
                }
            }
        }

        void onAutoButtonClick()
        {

        }

        void onAutoPitchButtonClick()
        {

        }

        public void onPeriscopeButtonClick()
        {
            if (World.activeCamera == 0)
            {
                changeActivieCamera(3);
            }
            else if (World.activeCamera == 1)
            {
                changeActivieCamera(2);
            }
            else if (World.activeCamera == 2)
            {
                changeActivieCamera(1);
            }
            else if (World.activeCamera == 3)
            {
                changeActivieCamera(0);
            }
        }

        public void onViewButtonClick()
        {
            if (World.activeCamera == 0)
            {
                changeActivieCamera(1);
            }
            else if (World.activeCamera == 1)
            {
                changeActivieCamera(0);
            }
            else if (World.activeCamera == 2)
            {
                changeActivieCamera(3);
            }
            else if (World.activeCamera == 3)
            {
                changeActivieCamera(2);
            }
        }

        public void onlightButtonClick()
        {
            bool isOpen = lightButton.value;
            if (isOpen)
            {
                MainSubmarine.lightLevel = Pooler.searchlightCount;
            }
            else
            {
                MainSubmarine.lightLevel = 0;
            }
        }

        void changeActivieCamera(int camera)
        {
            World.activeCamera = camera;
            switch (camera)
            {
                case 0:
                    {
                        Camera2DWorld.depth = 0;
                        Camera3DWorld.depth = 1;
                        Camera2DWorld.rect = new Rect(0, 0, 1, 1);
                        Camera3DWorld.rect = new Rect(0, 0.7f, 0.3f, 0.3f);
                        break;
                    }
                case 1:
                    {
                        Camera2DWorld.depth = 1;
                        Camera3DWorld.depth = 0;
                        Camera2DWorld.rect = new Rect(0, 0, 1, 1);
                        break;
                    }
                case 2:
                    {
                        Camera2DWorld.depth = 0;
                        Camera3DWorld.depth = 1;
                        Camera3DWorld.rect = new Rect(0, 0, 1, 1);
                        break;
                    }
                case 3:
                    {
                        Camera2DWorld.depth = 1;
                        Camera3DWorld.depth = 0;
                        Camera3DWorld.rect = new Rect(0, 0, 1, 1);
                        Camera2DWorld.rect = new Rect(0, 0.7f, 0.3f, 0.3f);
                        break;
                    }
            }
        }

        void onViewLockButtonClick()
        {
            Camera3DWorld.transform.SetParent(viewLockButton.value ? MainSubmarine.transform : followSubTrans);
        }

        void onHeatViewButtonClick()
        {
            Pooler.HeatMap_Mode++;
            if (Pooler.HeatMap_Mode > 2)
            {
                Pooler.HeatMap_Mode = 0;
            }

            heatViewButton.setValue(Pooler.HeatMap_Mode > 0);
            Console.printLang("heatMap_" + Pooler.HeatMap_Mode);

            for (int x = shipRect.x; x < shipRect.getMaxX(); x++)
                for (int y = shipRect.y; y < shipRect.getMaxY(); y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null)
                    {
                        block.openHeatMapMode(heatViewButton.value);
                    }
                }
        }

        void onView2DRotateButtonClick()
        {

        }

        public void onRadarButtonClick()
        {
            if (radarButton.value)
            {
                radarRect.anchoredPosition = new Vector2(0, 0);
                radarTextRect.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                radarRect.anchoredPosition = new Vector2(168, 0);
                radarTextRect.anchoredPosition = new Vector2(0, 0);
            }
        }

        public void onUpButtonValueChanged()
        {
            int count = bindBlockArr.Count;
            for (int i = 0; i < count; i++)
            {
                SolidBlock soildblock = bindBlockArr[i];
                if (soildblock != null && soildblock.isNeedDelete() == false && soildblock.getCurrentBindId() == 1)
                {
                    soildblock.onPreesButtonClick(upButton.value);
                }
            }
        }

        public void onDownButtonValueChanged()
        {
            int count = bindBlockArr.Count;
            for (int i = 0; i < count; i++)
            {
                SolidBlock soildblock = bindBlockArr[i];
                if (soildblock != null && soildblock.isNeedDelete() == false && soildblock.getCurrentBindId() == 2)
                {
                    soildblock.onPreesButtonClick(downButton.value);
                }
            }
        }

        public void onComButton1ValueChanged()
        {
            int count = bindBlockArr.Count;
            for (int i = 0; i < count; i++)
            {
                SolidBlock soildblock = bindBlockArr[i];
                if (soildblock != null && soildblock.isNeedDelete() == false && soildblock.getCurrentBindId() == 4)
                {
                    soildblock.onPreesButtonClick(comButton1.value);
                }
            }
        }

        public void onComButton2ValueChanged()
        {
            int count = bindBlockArr.Count;
            for (int i = 0; i < count; i++)
            {
                SolidBlock soildblock = bindBlockArr[i];
                if (soildblock != null && soildblock.isNeedDelete() == false && soildblock.getCurrentBindId() == 5)
                {
                    soildblock.onPreesButtonClick(comButton2.value);
                }
            }
        }

        public void onPowerBarPush()
        {
            int count = bindBlockArr.Count;
            for (int i = 0; i < count; i++)
            {
                SolidBlock soildblock = bindBlockArr[i];
                if (soildblock != null && soildblock.isNeedDelete() == false && soildblock.getCurrentBindId() == 3)
                {
                    soildblock.onPowerBarPush(powerBar.getValue());
                }
            }
        }




        public void onDirectionButtonClick()
        {
            int count = bindBlockArr.Count;
            for (int i = 0; i < count; i++)
            {
                SolidBlock soildblock = bindBlockArr[i];
                if (soildblock != null && soildblock.isNeedDelete() == false && soildblock.getCurrentBindId() == 3)
                {
                    soildblock.onDirectionBarPush(directionButton.value ? 1 : -1);
                }
            }
        }

        public void onRudderPush()
        {
            MainSubmarine.rudderTorque = (rudder.getValue() - 0.5f) * 2;
        }

        public void onPitchBarPush()
        {
            MainSubmarine.pitchTorque = (pitchBar.getValue() - 0.5f) * 2;
        }

        public void updata()
        {
            electricMeter.updataPower();
            balanceVSpeed();
            balancePitch();
            updataTime();
            updateHitLabel();
        }

        void updateHitLabel()
        {

            if (isShowHitLabel && hitLabelShowTime > 20)
            {
                isShowHitLabel = false;
                hitLabel.SetActive(false);
                hitingTime = 0;
            }

            if (isShowBehitLabel && behitLabelShowTime > 20)
            {
                isShowBehitLabel = false;
                behitLabel.SetActive(false);
                behitingTime = 0;
            }
            hitLabelShowTime++;
            behitLabelShowTime++;
        }

        void balanceVSpeed()
        {
            if (autoButton.value)
            {
                float vSpeed = MainSubmarine.verticalSpeed;
                if (Mathf.Abs(vSpeed) > 0.4f)
                {
                    if (vSpeed < 0)
                    {
                        upButton.setValue(true);
                        onUpButtonValueChanged();
                        downButton.setValue(false);
                        onDownButtonValueChanged();
                    }
                    else
                    {
                        downButton.setValue(true);
                        onDownButtonValueChanged();
                        upButton.setValue(false);
                        onUpButtonValueChanged();
                    }
                }
                else
                {
                    upButton.setValue(false);
                    onUpButtonValueChanged();
                    downButton.setValue(false);
                    onDownButtonValueChanged();
                    autoButton.setValue(false);
                }
            }
        }

        void balancePitch()
        {
            if (autoPitchButton.value)
            {
                if (!(pitchBar.isClickBar() || PoolerPCInput.isPitchBarClick))
                {
                    float ZSpeed = MainSubmarine.transform.InverseTransformVector(MainSubmarine.rigidbody.angularVelocity).z * MainSubmarine.forward;
                    float dz = (MainSubmarine.transform.localEulerAngles.z - subRotoZ) * MainSubmarine.forward;
                    float maxPitchForce = MainSubmarine.speed;
                    float value = 0;
                    if (Mathf.Abs(maxPitchForce) > 0.5f)
                    {
                        value = ZSpeed * 100 / maxPitchForce;

                    }
                    else
                    {
                        value = ZSpeed;
                    }
                    value += Mathf.Sin(dz);
                    value = -value + 0.5f;
                    value = Mathf.Clamp01(value);
                    //pitchBar.setValue(value);
                    float torque = (value - 0.5f) * 2;
                    torque = Mathf.Lerp(MainSubmarine.pitchTorque, torque, 0.1f);
                    MainSubmarine.pitchTorque = torque;
                }
                else
                {
                    subRotoZ = MainSubmarine.transform.localEulerAngles.z;
                }

            }
            else if (!autoPitchButton.value)
            {
                onPitchBarPush();
                subRotoZ = MainSubmarine.transform.localEulerAngles.z;
            }
        }



        void updataTime()
        {
            timeText.text = System.DateTime.Now.ToString("HH:mm");
        }

        public void showBehitLabel()
        {
            isShowBehitLabel = true;
            behitLabel.SetActive(true);
            behitLabelShowTime = 0;
            behitingTime++;
            if (behitingTime > 1)
            {
                behitLabelText.text = ILang.get("behit", "menu") + "x" + behitingTime;
            }
            else
            {
                behitLabelText.text = ILang.get("behit", "menu");
            }
        }

        public void showHitLabel()
        {
            isShowHitLabel = true;
            hitLabel.SetActive(true);
            hitLabelShowTime = 0;
            hitingTime++;
            if (hitingTime > 1)
            {
                hitLabelText.text = ILang.get("hit", "menu") + "x" + hitingTime;
            }
            else
            {
                hitLabelText.text = ILang.get("hit", "menu");
            }
        }



    }
}