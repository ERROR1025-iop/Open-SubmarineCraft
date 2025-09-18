using Scraft.BlockSpace;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class MainSubmarine : MonoBehaviour
    {

        public static MainSubmarine instance;

        World world;
        BlocksEngine blocksEngine;
        BlocksManager blocksManager;
        Pooler pooler;
        PoolerUI poolerUI;
        Radar radar;
        Gyro gyro;

        new static public Rigidbody rigidbody;
        new static public Transform transform;
        Camera camera3D;
        Camera camera2D;
        Transform BuilderMapTrans;

        //推力方块合集
        static public List<Block> forwardForceBlocks;
        static public List<Block> sideForceBlocks;

        //坐标
        Vector3 coor;
        static public Vector2 direction;
        static public float deep;
        static public int forward;
        static public float pitch;
        static public float inversion;
        static public float roll;
        //数值
        static public float speed;
        static public float verticalSpeed;
        static public float tonnage;
        static public float baseResistance;
        //力
        static public float forwardForce;
        static public float rudderTorque;
        static public float sideForce;
        static public float pitchTorque;

        //最大值
        static public float maxDeep;
        static public float maxSpeed;


        //属性
        static public int lightLevel;
        static public Color lightColor;
        static public Bounds bounds;
        static public Vector3 transfromCenter;
        static public Vector3 weightCenter;
        static public bool isRunSurface;

        //声音
        AudioSource audioSourceBgm;
        AudioSource audioSourceFireTorpedo;
        static public AudioClip[] bgms;

        static public float fireAngle;

        void Awake()
        {
            instance = this;

            forwardForceBlocks = new List<Block>();
            sideForceBlocks = new List<Block>();

            rigidbody = GetComponent<Rigidbody>();
            transform = GetComponent<Transform>();

            baseResistance = 0;
        }

        void Start()
        {

            this.world = World.instance;
            this.pooler = Pooler.instance;
            blocksEngine = world.blocksEngine;
            blocksManager = world.blocksManager;
            poolerUI = pooler.poolerUI;
            radar = poolerUI.radar;
            gyro = poolerUI.gyro;

            //坐标
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            coor = new Vector3(0, 0, 0);

            speed = 0;
            sideForce = 0;
            tonnage = 0;
            lightLevel = 0;
            forward = 1;
            deep = 0;

            audioSourceBgm = Camera.main.gameObject.GetComponent<AudioSource>();
            audioSourceFireTorpedo = GetComponent<AudioSource>();
            bgms = new AudioClip[4];
            bgms[0] = Resources.Load("Sounds/sonar", typeof(AudioClip)) as AudioClip;
            bgms[1] = Resources.Load("Sounds/deep1", typeof(AudioClip)) as AudioClip;
            bgms[2] = Resources.Load("Sounds/deep2", typeof(AudioClip)) as AudioClip;
            bgms[3] = Resources.Load("Sounds/deep3", typeof(AudioClip)) as AudioClip;

            camera3D = Camera.main;
            camera2D = GameObject.Find("2D Camera").GetComponent<Camera>();

            rigidbody.velocity = Vector3.zero;
        }

        void FixedUpdate()
        {

            //更新力
            setForce();//更新推进力
            setSideForce();//更新侧向推力
                           //更新显示
            speedUpdate();//更新前进速度
            directionUpdate();//更新转向速度
            balanceRoll();//平衡翻滚
            updataDeep();//更新垂直速度   
            updateResistance();//更新阻力系数
        }

        public void onDpartMapLoadFinish(GameObject dpartsMap)
        {
            bounds = IUtils.GetBounds(dpartsMap);
            transfromCenter = IUtils.centerOfGameObjects(dpartsMap);
            weightCenter = IUtils.weigthCenterOfGameObjects(dpartsMap, Pooler.massOffsetY);            
            rigidbody.centerOfMass = weightCenter;
        }

        /// <summary>
        ///更新推进力
        /// </summary>
        public void setForce()
        {
            float totalForce = 0;
            int count = forwardForceBlocks.Count;
            for (int i = 0; i < count; i++)
            {

                Block block = forwardForceBlocks[i];
                if (block != null && block.getForce() != 0)
                {
                    float pushForce = block.getForce();
                    if (pushForce < 0)
                    {
                        pushForce *= 0.7f;
                    }
                    totalForce += pushForce;
                }
            }
            forwardForce = totalForce;
        }

        /// <summary>
        ///更新侧向推力
        /// </summary>
        public void setSideForce()
        {
            float totalSideForce = 0;
            int count = sideForceBlocks.Count;
            for (int i = 0; i < count; i++)
            {

                Block block = sideForceBlocks[i];
                if (block != null && block.getSideForce() != 0)
                {
                    float pushSideForce = block.getSideForce();
                    totalSideForce += pushSideForce;
                }
            }
            sideForce = totalSideForce;
        }

        /// <summary>
        ///更新前进速度
        /// </summary>
        public void speedUpdate()
        {

            float mass = Pooler.mass * 0.03f;
            if (mass == 0)
            {
                forwardForce = 0;
            }
            if(mass < 5)
            {
                mass = 5;
            }

            speed = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude;            
            forward = speed < 0.1f ? 1 : Vector3.Dot(rigidbody.velocity, -transform.right) > 0 ? 1 : -1;
            verticalSpeed = rigidbody.velocity.y;
            rigidbody.mass = mass;
            setCoor(IUtils.coor3DConvert2D(transform.position));
            radar.setSpeed(speed);
            radar.setLocation(IUtils.vector3DConvert2DPlant(transform.position));

            if (speed > maxSpeed)
            {
                maxSpeed = speed;
            }
        }

        /// <summary>
        ///更新转向速度
        /// </summary>
        public void directionUpdate()
        {
            setAngle(transform.eulerAngles.y);
            bool view2DRotate = World.activeCamera > 1 || poolerUI.view2DRotateButton.value;
            camera2D.transform.localEulerAngles = new Vector3(0, 0, view2DRotate ? -transform.localEulerAngles.z : 0);
            pitch = Vector3.Dot(transform.right, Vector3.down);
            inversion = Vector3.Dot(transform.up, Vector3.up);

            rigidbody.AddRelativeTorque(Vector3.up * sideForce * rudderTorque);
        }

        /// <summary>
        ///平衡翻滚
        /// </summary>
        void balanceRoll()
        {
            if (!isRunSurface)
            {
                Vector3 normal = Vector3.Cross(transform.right, Vector3.up);
                roll = Vector3.Angle(normal, transform.forward);

                if (!isRunSurface)
                {
                    Quaternion q = Quaternion.FromToRotation(transform.forward, normal);
                    Quaternion tq = Quaternion.Lerp(transform.localRotation, q * transform.localRotation, Time.deltaTime * 5f * (1 - Mathf.Abs(pitch)));
                    transform.localRotation = tq;
                }
            }           
        }

        /// <summary>
        ///更新垂直速度（显示）及背景音乐
        /// </summary>
        public void updataDeep()
        {
            poolerUI.deepGauge.setDeep(deep);
            poolerUI.deepGauge.setVerticalSpeed(verticalSpeed);

            if (deep > maxDeep)
            {
                maxDeep = deep;
            }

            playDeepMusic();

            isRunSurface = transform.TransformPoint(Buoyancy.floatCenter).y > Buoyancy.waterHeight - 0.2f;

            float rg = Mathf.Clamp(-transform.position.y, 0, 300) * 0.0033f;
            lightColor = Color.HSVToRGB(0.5944f, rg, 1);
        }

        void playDeepMusic()
        {
            if (GameSetting.isMusicOpen)
            {
                if (deep > 50 && deep < 1000)
                {
                    if (!GameSetting.isCreateAi)
                    {
                        if (audioSourceBgm.isPlaying == false)
                        {
                            audioSourceBgm.clip = bgms[1];
                            audioSourceBgm.Play();
                        }
                    }                    
                }
                else if (deep > 1000 && deep < 2000)
                {
                    if (audioSourceBgm.isPlaying == false)
                    {
                        audioSourceBgm.clip = bgms[2];
                        audioSourceBgm.Play();
                    }

                    if (((int)(Random.value * 10000) % 100) == 11)
                    {
                        pooler.playSound(bgms[0]);
                    }
                }
                else if (deep > 2000)
                {
                    if (audioSourceBgm.isPlaying == false)
                    {
                        audioSourceBgm.clip = bgms[3];
                        audioSourceBgm.Play();
                    }
                }
            }
        }

        /// <summary>
        ///更新阻力系数
        /// </summary>
        void updateResistance()
        {
            baseResistance = Pooler.underwaterCount * Pooler.underwaterCount * 0.03f;
            float tmp = 2 + baseResistance + speed * speed * 0.005f;
            rigidbody.drag = tmp;
            rigidbody.angularDrag = tmp;

        }      

        /// <summary>
        ///设置潜艇的虚拟坐标
        /// </summary>
        void setCoor(Vector3 coor)
        {
            deep = -coor.y;
            this.coor = coor;
        }

        /// <summary>
        ///更新潜艇的坐标
        /// </summary>
        public void setLocation(Vector2 loc)
        {
            setCoor(new Vector3(loc.x, coor.y, loc.y));
            poolerUI.radar.setLocation(loc);
            transform.localPosition = IUtils.coor2DConvert3D(coor);//3D位置更新
        }

        /// <summary>
        ///获取潜艇的坐标
        /// </summary>
        public Vector2 getLocation()
        {
            return new Vector2(coor.x, coor.z);
        }

        /// <summary>
        ///获取潜艇的航向角度
        /// </summary>
        public static float getAngle()
        {
            return transform.eulerAngles.y;
        }

        /// <summary>
        ///设置潜艇的航向角度
        /// </summary>
        public void setAngle(float angle, bool isChangeTransform = false)
        {
            angle = (angle < 0) ? (angle + 360) : (angle > 360 ? angle - 360 : angle);
            direction = new Vector2(Mathf.Sin(IUtils.angle2radian(angle)), Mathf.Cos(IUtils.angle2radian(angle)));
            gyro.setAngle(angle);
            radar.setAngle(angle);
            if (isChangeTransform)
            {
                transform.localEulerAngles = new Vector3(0, angle, 0);
            }
        }

        /// <summary>
        ///设置潜艇的深度（正值）
        /// </summary>
        public void setDeep(float deep)
        {
            if (deep < 0)
            {
                deep = 0;
            }
            setCoor(new Vector3(coor.x, -deep, coor.z));
        }

        public void randomLocation()
        {
            float locx = (Random.value * 2 - 1) * 6000;
            float locy = (Random.value * 2 - 1) * 6000;
            float angle = Random.value * 360;
            setAngle(angle);
            setCoor(new Vector3(locx, 0, locy));
        }

        public static void addTorpedp(float targetAngle)
        {
            float toAngle = targetAngle == 0 ? fireAngle : IUtils.angleRoundIn180(targetAngle + getAngle());
            Torpedo3DMono.fire(transform.localPosition, speed * 2.5f + 10, getAngle(), toAngle, false);
            instance.audioSourceFireTorpedo.Play();
        }

        /// <summary>
        ///自身受到损坏
        ///fireType：0：任意，1：上方，2：下方
        /// </summary>
        static public bool destroySelfShip(float damageRatio, int fireType = 0)
        {
            bool result = destroy2DShip(damageRatio, fireType);
            if (result)
            {
                PoolerUI.instance.showBehitLabel();
            }
            return result;
        }

        static private bool destroy2DShip(float damageRatio, int fireType)
        {
            IRect rect = Pooler.instance.getShipRect();
            int with = rect.with;
            int lowest = rect.getMinY();
            int highest = rect.getMaxY();
            int reSelectCount = 0;

            reSelect:
            int x = (int)(rect.getMinX() + 3 + (Random.value * (with - 6)));
            int y = lowest;
            Block attackBlock = null;
            if (fireType == 2 || (fireType != 1 && ((int)(Random.value * 100)) % 2 == 0))
            {
                for (int k = lowest; k < highest; k++)
                {
                    Block block = BlocksEngine.instance.getBlock(x, k);
                    if (block != null && !block.isBorder() && block.equalPState(PState.solid))
                    {
                        y = k;
                        attackBlock = block;
                        break;
                    }
                }
            }
            else
            {
                for (int k = highest; k > lowest; k--)
                {
                    Block block = BlocksEngine.instance.getBlock(x, k);
                    if (block != null && !block.isBorder() && block.equalPState(PState.solid))
                    {
                        y = k;
                        attackBlock = block;
                        break;
                    }
                }
            }

            if (attackBlock != null && !attackBlock.equalBlock(BlocksManager.instance.propeller))
            {
                attackBlock.addTemperature(100 * damageRatio);
                attackBlock.addPress(14 * damageRatio);
                return true;
            }
            else
            {
                if (reSelectCount < with * 0.5f)
                {
                    reSelectCount++;
                    goto reSelect;
                }
            }
            return false;
        }
    }
}
