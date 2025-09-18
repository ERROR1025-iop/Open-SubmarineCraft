using UnityEngine;
using System.Collections.Generic;

namespace Scraft
{
    public class AISubMono : MonoBehaviour
    {
        public static int AI_Count;
        public static List<AISubMono> aiList;

        public ParticleSystem backBubble;

        new public Rigidbody rigidbody;
        SubStabilization subStabilization;
        Vector3 force;
        float speed;

        int damage;
        bool isSink;
        int fireReady;

        public bool isShip;
        bool isWillGoUp;
        float divingDeep;
        int shipModeIndex;

        float deep;
        float dDeep;
        float dist;
        float dangle;
        Vector3 dvector;
        Vector3 playerPos;
        Vector3 aiPos;

        static public float nearestAiDistance;
        static public Vector3 nearestAiPosition;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            subStabilization = GetComponent<SubStabilization>();

            fireReady = 0;
            damage = 0;
            isSink = false;
            AI_Count++;

            aiList.Add(this);
        }

        public static void createAi(int count)
        {
            for (int k = 0; k < count; k++)
            {
                AISubMono.create();
            }
        }

        public static AISubMono create()
        {
            bool isShip = Random.value < 0.5f;
            string shipModeName = isShip ? "AIShip" : "AISub";
            int maxShipMode = isShip ? 2 : 4;
            int shipModeIndex = (int)(Random.value * (float)maxShipMode);
            string modeAllName = shipModeName + shipModeIndex;
            AISubMono aiSubMono = (Instantiate(Resources.Load("Prefabs/Pooler/Ships/" + modeAllName)) as GameObject).GetComponent<AISubMono>();
            Vector3 position = randomPosition(isShip);
            float aiAngle = Random.value * 360;
            aiSubMono.initAiShip(isShip, shipModeIndex, position, aiAngle);
            return aiSubMono;
        }

        public static Vector3 randomPosition(bool isShip)
        {
            Vector3 position;
            int stack = 0;
            do
            {
                float locx = MainSubmarine.instance.getLocation().x + (Random.value * 2 - 1) * 6000;
                float locy = MainSubmarine.instance.getLocation().y + (Random.value * 2 - 1) * 6000;
                float aiDeep = isShip ? 0 : -Random.value * 600;
                position = new Vector3(locx, aiDeep, locy);
            } while (!isInSea(position) && stack < 50);
            Debug.Log("randomPosition time:" + stack);
            return position;
        }

        public static bool isInSea(Vector3 position)
        {
            foreach (AISpawn aISpawn in AISpawn.aISpawns)
            {
                if (Vector3.Distance(IUtils.coor2DConvert3D(position), aISpawn.transform.position) < aISpawn.radius)
                {
                    return true;
                }
            }
            return false;
        }

        public void initAiShip(bool isShip, int shipModeIndex, Vector3 position, float angle)
        {
            this.isShip = isShip;
            this.shipModeIndex = shipModeIndex;
            isWillGoUp = shipModeIndex < 3;
            if (!isWillGoUp)
            {
                divingDeep = Random.value * 200 + 200;
            }
            transform.localPosition = IUtils.coor2DConvert3D(position);
            setAngle(angle);
        }

        void FixedUpdate()
        {
            if (World.stopUpdata)
            {
                return;
            }

            treeUpdate();//Ai树更新   
            rigidbody.AddForce(force);
            updateNearestPosition();
        }

        public void updateNearestPosition()
        {
            if (!isSink)
            {
                float distance = Vector3.Distance(transform.position, MainSubmarine.transform.position);
                if (distance < nearestAiDistance)
                {
                    nearestAiDistance = distance;
                    nearestAiPosition = transform.position;
                }
            }            
        }


        public void treeUpdate()
        {
            playerPos = new Vector3(MainSubmarine.transform.position.x, 0, MainSubmarine.transform.position.z);
            aiPos = new Vector3(transform.position.x, 0, transform.position.z);
            dist = IUtils.coor3DConvert2D(aiPos - playerPos).magnitude;
            dvector = playerPos - aiPos;
            deep = getDeep();
            dDeep = deep - MainSubmarine.deep;
            dangle = Vector3.Angle(Vector3.forward, dvector);
            if (Vector3.Angle(Vector3.right, dvector) > 90)
            {
                dangle = -dangle;
            }

            if (isSink)
            {
                force = Vector3.zero;
                setVerticalForce(-300);
                rigidbody.AddTorque(new Vector3(100, 0, 0));
                return;
            }

            force = Vector3.zero;
            force = transform.forward * 3000;

            rotationSequence();
            verticalSequence();
            fireSequence();
            depthDepthChargeSequence();
        }

        void rotationSequence()
        {

            if (dist > 1000)
            {
                float rotationSpeed = isShip ? (Mathf.Clamp( MainSubmarine.deep, 1, 120) * 0.005f + 0.1f ): 0.1f;
                rotationAction(rotationSpeed);
            }
        }

        void rotationAction(float speed)
        {
            Vector3 transAngle = transform.localRotation.eulerAngles;
            float slepQ = Mathf.LerpAngle(transAngle.y, dangle, speed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(new Vector3(transAngle.x, slepQ, transAngle.z));
        }

        void verticalSequence()
        {
            if (isShip && deep > 0)
            {
                setVerticalForce(deep * 10);
            }
            else if (deep > 0)
            {
                if (Mathf.Abs(dDeep) > 0.1f)
                {
                    if (dDeep > 0)
                    {
                        if (isWillGoUp)
                        {
                            setVerticalForce(deep < 200 ? (deep + 100) : 300);
                        }
                        else
                        {
                            if (deep > divingDeep)
                            {
                                setVerticalForce(deep < 200 ? (deep + 100) : 300);
                            }
                            else
                            {
                                setVerticalForce(-300);
                            }
                        }
                    }
                    else if (dDeep < 0 && MainSubmarine.deep < 900)
                    {
                        setVerticalForce(-300);
                    }
                }
            }
        }

        void fireSequence()
        {
            if (dist < 3000 && Mathf.Abs(dDeep) < 30 && fireReady > 300)
            {
                float torpedpAngle = dangle;
                torpedpAngle += (Random.value * 2.0f - 1) * 10;
                fireTorpedp(torpedpAngle);
                fireReady = isShip ? (shipModeIndex == 1 ? 160 : 200) : 100;
            }
            else
            {
                fireReady++;
            }
        }

        void fireTorpedp(float angle)
        {
            if (isShip)
            {
                Torpedo3DMono.fire(transform.localPosition + new Vector3(0, 1, 0), 20, angle + 90, angle + 90, true);
            }
            else
            {
                Torpedo3DMono.fire(transform.localPosition, 0, angle + 90, angle + 90, true);
            }

        }

        void depthDepthChargeSequence()
        {
            if (isShip)
            {
                if (dist < 500 && MainSubmarine.deep > 5 && fireReady > 300)
                {
                    fireReady = shipModeIndex == 1 ? 160 : 200;
                    fireDepthCharge();
                }
            }
        }

        void fireDepthCharge()
        {
            Vector3 startVelocity = (dvector.normalized + Vector3.up) * 3.0f;
            DepthCharge3DMono.fire(transform.position, startVelocity, MainSubmarine.deep, true);
        }

        public Vector2 getLocation()
        {
            return IUtils.vector3DConvert2DPlant(transform.position);
        }

        public void setAngle(float angle)
        {
            transform.localEulerAngles = new Vector3(0, angle, 0);
        }

        public float getDeep()
        {
            return -IUtils.coor3DConvert2D(transform.position).y;
        }

        public void setVerticalForce(float verticalForce)
        {
            force = new Vector3(force.x, verticalForce, force.z);
        }

        public void onBehit(int d)
        {
            addDamage(d);
            PoolerUI.instance.showHitLabel();
        }

        public void addDamage(int d)
        {
            if (!isSink)
            {
                damage += (isShip ? (int)(d * 0.5f) : d);
                if (damage > 100)
                {
                    subStabilization.enabled = false;
                    isSink = true;
                    GetComponent<Icon3D>().setIconColor(Color.gray);
                    Destroy(GetComponent<RadarPoint>());
                    AI_Count--;
                    Debug.Log("Sink!!:" + AI_Count);
                    backBubble.enableEmission = false;
                    aiList.Remove(this);
                    if (AI_Count <= 0)
                    {
                        Pooler.instance.entrnNextWave();
                    }
                }
            }
        }

        public void clear()
        {
            GameObject.Destroy(transform.gameObject);
        }
    }
}
