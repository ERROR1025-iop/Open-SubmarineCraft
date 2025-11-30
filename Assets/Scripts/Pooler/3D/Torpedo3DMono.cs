using UnityEngine;


namespace Scraft
{
    public class Torpedo3DMono : MonoBehaviour
    {

        public GameObject explosion1;
        public GameObject explosion2;
        public static GameObject torpedoObject;

        float torpedoMaxLifeTime;
        float lifeTime;
        bool m_isBoom;
        bool m_isActivity;        

        float targetAngle;
        float targetDeep;
        float startSpeed;
        float startPosY;
        bool isSteering;
        bool m_isEnemy;

        float force;


        new Rigidbody rigidbody;
        CapsuleCollider capsuleCollider;

        void Start()
        {

            m_isBoom = false;
            m_isActivity = false;
            torpedoMaxLifeTime = 30;
            lifeTime = 0;
            force = 0;

            capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.enabled = false;
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = transform.forward * startSpeed;

            if (!m_isEnemy)
            {
                SubCamera.lastWeaponTransform = transform;
            }
        }


        public static Torpedo3DMono fire(Vector3 position, float startSpeed, float angle, float targetAngle, bool isEnemy, float deep)
        {
            if (torpedoObject == null)
            {
                torpedoObject = Resources.Load("Prefabs/Pooler/torpedp3D", typeof(GameObject)) as GameObject;
            }
            Torpedo3DMono torpedp = Instantiate(torpedoObject).GetComponent<Torpedo3DMono>();
            torpedp.initTorpedo(position, startSpeed, angle, targetAngle, isEnemy, deep);
            return torpedp;
        }

        void initTorpedo(Vector3 position, float startSpeed, float angle, float targetAngle, bool isEnemy, float deep)
        {
            this.targetAngle = targetAngle;
            this.targetDeep = deep;
            initTorpedo(position, startSpeed, angle, isEnemy);
            Invoke("activitySteering", 0.5f);
        }

        void initTorpedo(Vector3 position, float startSpeed, float angle, bool isEnemy) 
        {
            transform.position = position;
            startPosY = position.y;
            setAngle(angle);
            this.startSpeed = startSpeed;
            Invoke("activityCollider", 3.0f);
            m_isEnemy = isEnemy;
            Icon3D icon3D = GetComponent<Icon3D>();
            icon3D.setIconColor(m_isEnemy ? Color.red : Color.green);
            icon3D.setIconTag(m_isEnemy ? 3 : 1);
        }

        void activityCollider()
        {
            m_isActivity = true;
            capsuleCollider.enabled = true;
        }

        void activitySteering()
        {
            isSteering = true;
        }

        void FixedUpdate()
        {
            if (World.stopUpdata)
            {
                return;
            }

            force = Mathf.Lerp(force, 50, 0.002f);
            if (transform.position.y < 0.3f)
            {
                rigidbody.AddForce(transform.forward * force);
            }

            if (transform.position.y - startPosY < 0.1f)
            {
                rigidbody.AddForce(Vector3.up * 2);
            }

            if (isSteering)
            {
                steering();
            }

            toTargetDeep();

            if (isNeedRelease())
            {
                clear();
            }
        }

        void steering()
        {
            Vector3 transAngle = transform.localRotation.eulerAngles;
            float slepQ = Mathf.LerpAngle(transAngle.y, targetAngle - 90, Time.deltaTime);
            transform.localRotation = Quaternion.Euler(new Vector3(transAngle.x, slepQ, transAngle.z));
        }

        void toTargetDeep()
        {
            Vector3 pos = transform.position;
            float deepDiff = targetDeep - pos.y;
            if (Mathf.Abs(deepDiff) > 0.1f)
            {
                float slepY = Mathf.Lerp(pos.y, targetDeep, Time.deltaTime * 0.1f);
                transform.position = new Vector3(pos.x, slepY, pos.z);
            }
        }

        void OnTriggerStay(Collider other)
        {

            if ((m_isActivity || (m_isEnemy ^ other.tag.Equals("other ship"))) && !m_isBoom)
            {
                explosion();

                if (other.tag.Equals("self ship"))
                {
                    MainSubmarine.destroySelfShip((int)(1000 + Random.value * 1000), transform.position.y < -30 ? 0 : 2);
                }
                else if (other.tag == "other ship")
                {
                    var ai = other.gameObject.GetComponent<AISubMono>();
                    ai?.onBehit((int)(40 + Random.value * 100));
                }

                Rigidbody rigidbody = other.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(10000, transform.position, 3);
                }
            }
        }

        public void explosion()
        {
            GameObject ego = Instantiate(transform.position.y < -2 ? explosion1 : explosion2) as GameObject;
            ego.transform.position = transform.position;
            Destroy(ego, 5);
            m_isBoom = true;
        }

        public bool isNeedRelease()
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > torpedoMaxLifeTime)
            {
                return true;
            }
            return m_isBoom;
        }


        public void setAngle(float angle)
        {
            transform.localEulerAngles = new Vector3(0, angle - 90, 0);
        }


        public void clear()
        {
            SubCamera.instance.onFollowTransformDestory(transform);
            Destroy(gameObject);
        }
    }
}
