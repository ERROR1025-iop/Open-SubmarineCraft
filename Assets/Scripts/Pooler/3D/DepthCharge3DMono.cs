using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class DepthCharge3DMono : MonoBehaviour
    {

        public GameObject explosion1;
        public GameObject explosion2;
        public GameObject hitWaterExp;
        public float explosionRange = 20;
        public static GameObject depthChargeObject;

        float v0, v02, radian, g, v1, v2, x, y, t, th, S, speedAngle;
        Vector3 startPosition, startEulrAngle, startDirection, targetPosition;
        Quaternion convertQ;

        float depthChargeMaxLifeTime;
        float lifeTime;
        bool m_isBoom;
        bool m_isActivity;
        bool m_isEnemy;
        bool isAttack;
        bool isHitWater;
        float activityDeep;
        Vector3 startVelocity;

        new Rigidbody rigidbody;
        BoxCollider boxCollider;

        bool isPhysicalRun;

        static Color iconColor = IUtils.HexToColor("61FFF6");

        void Start()
        {
            m_isBoom = false;
            m_isActivity = false;
            isAttack = false;
            isHitWater = false;
            depthChargeMaxLifeTime = 100;
            lifeTime = 0;
            g = 1;

            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = startVelocity;          

            if (!m_isEnemy)
            {
                SubCamera.lastWeaponTransform = transform;
            }
        }

        public static DepthCharge3DMono fire(Vector3 position, Vector3 startVelocity, float deep, bool isEnemy)
        {
            if (depthChargeObject == null)
            {
                depthChargeObject = Resources.Load("Prefabs/Pooler/depth charge", typeof(GameObject)) as GameObject;
            }
            DepthCharge3DMono depthCharge = Instantiate(depthChargeObject).GetComponent<DepthCharge3DMono>();
            depthCharge.initDepthCharge(position, startVelocity, deep, isEnemy);
            return depthCharge;
        }

        public void initDepthCharge(Vector3 position, Vector3 startVelocity, float deep, bool isEnemy)
        {
            transform.position = position;
            this.startVelocity = startVelocity;
            activityDeep = deep;
            m_isEnemy = isEnemy;
            isPhysicalRun = true;


            Icon3D icon3D = GetComponent<Icon3D>();
            icon3D.setIconColor(m_isEnemy ? iconColor : Color.green);
            icon3D.setIconTag(m_isEnemy ? 3 : 1);

            Invoke("activityCollider", 10.0f);
        }

        public void initDepthChargeByComputationalSimulation(Transform startPoint, float startSpeed, float deep, bool isEnemy)
        {
            startPosition = startPoint.position;
            transform.position = startPosition;
            startEulrAngle = startPoint.eulerAngles;
            transform.eulerAngles = startEulrAngle;
            v0 = startSpeed;
            v02 = v0 * v0;
            t = 0;
            Vector3 direction = startPoint.forward;
            Vector3 plantDirection = new Vector3(direction.x, 0, direction.z);
            convertQ = Quaternion.LookRotation(plantDirection);
            radian = IUtils.angle2radian(Vector3.Angle(direction, plantDirection));

            isPhysicalRun = false;
            m_isEnemy = isEnemy;
            activityDeep = deep;
        }

        void activityCollider()
        {
            m_isActivity = true;
            boxCollider.enabled = true;
        }

        void FixedUpdate()
        {
            if (World.stopUpdata)
            {
                return;
            }

            if (transform.position.y < Buoyancy.waterHeight)
            {
                rigidbody.AddForce(new Vector3(0, -0.1f, 0));
                isPhysicalRun = true;
                hitWater();
            }
            else if(isPhysicalRun)
            {
                rigidbody.AddForce(new Vector3(0, -1, 0));
            }

            if (!isPhysicalRun)
            {
                calculateTrajectory();
            }

            if (isNeedRelease())
            {
                clear();
            }

            detectDeep();
        }

        void calculateTrajectory()
        {
            t += 0.04f;
            v1 = v0 * Mathf.Cos(radian);
            v2 = v0 * Mathf.Sin(radian) - (g * t);
            x = v1 * t;
            y = v0 * t * Mathf.Sin(radian) - (g * t * t * 0.5f);
            transform.position = startPosition + convertQ * new Vector3(0, y, x);

            speedAngle = IUtils.radian2angle(Mathf.Atan(v2 / v1));
            transform.localEulerAngles = new Vector3(-speedAngle, startEulrAngle.y, startEulrAngle.z);
        }

        void detectDeep()
        {
            if (transform.position.y * 10 < -activityDeep)
            {
                explosion();

                if (m_isEnemy)
                {
                    hitSelfShip();
                }
                else
                {
                    hitAiShip();
                }               
            }
        }

        void hitSelfShip()
        {
            Vector3 shipPosition = MainSubmarine.transform.position;
            float distance = Vector3.Distance(transform.position, shipPosition);
            if (distance < explosionRange)
            {
                MainSubmarine.destroySelfShip((int)(1000 + (explosionRange - distance) * 70), transform.position.y > shipPosition.y ? 1 : 2);

                if (MainSubmarine.rigidbody != null)
                {
                    MainSubmarine.rigidbody.AddExplosionForce(5000 * (explosionRange - distance), transform.position, 5);
                }

                isAttack = true;
            }           
        }

        void hitAiShip()
        {           
            for (int i= 0;i< AISubMono.aiList.Count; i++)
            {
                AISubMono aISub = AISubMono.aiList[i];
                Vector3 shipPosition = aISub.transform.position;
                float distance = Vector3.Distance(transform.position, shipPosition);
                if (distance < explosionRange)
                {
                    if (aISub.isShip)
                    {
                        aISub.onBehit(20);
                    }
                    else
                    {
                        aISub.onBehit((int)(40 + Random.value * 100));
                    }
                  
                  
                    if (aISub.rigidbody != null)
                    {
                        aISub.rigidbody.AddExplosionForce(5000 * (explosionRange - distance), transform.position, 5);
                    }                    
                }
            }           
        }

        public void explosion()
        {
            GameObject ego = Instantiate(isAttack ? explosion1 : explosion2) as GameObject;          
            ego.transform.position = transform.position;
            Destroy(ego, 5);
            m_isBoom = true;
        }

        void hitWater()
        {
            if(hitWaterExp != null && !isHitWater)
            {
                GameObject ego = Instantiate(hitWaterExp) as GameObject;
                ego.transform.position = transform.position;
                Destroy(ego, 5f);                
                isHitWater = true;
            }
        }

        public bool isNeedRelease()
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > depthChargeMaxLifeTime)
            {
                return true;
            }
            return m_isBoom;
        }

        public void clear()
        {
            SubCamera.instance.onFollowTransformDestory(transform);
            Destroy(gameObject);
        }
    }
}
