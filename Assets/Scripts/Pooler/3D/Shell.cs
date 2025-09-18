using UnityEngine;

namespace Scraft
{
    public class Shell : MonoBehaviour
    {

        float v0, v02, radian, g, v1, v2, x, y, t, th, S, speedAngle;
        Vector3 startPosition, startEulrAngle, startDirection, targetPosition;
        Quaternion convertQ;

        public GameObject hitWaterExp;
        public GameObject hitTargetExp;

        bool m_isBoom;
        bool m_isActivity;
        float power;

        void Start()
        {
            g = 1;

            SubCamera.lastWeaponTransform = transform;

            m_isBoom = false;
            m_isActivity = false;

            Invoke("setActivity", 0.1f);
        }

        void setActivity()
        {
            m_isActivity = true;
        }

        public void initialized(Transform startPoint, float startSpeed, float power)
        {
            this.power = power;
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
        }

        void FixedUpdate()
        {
            calculateTrajectory();

            if (m_isBoom)
            {
                Destroy(gameObject);
            }

            if (!m_isBoom && transform.position.y < 0)
            {
                hitWater();
            }
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

        void OnTriggerStay(Collider other)
        {
            if (m_isActivity && !m_isBoom)
            {
                explosion();

                if (other.tag == "self ship")
                {
                    MainSubmarine.destroySelfShip(100, transform.position.y < -30 ? 0 : 1);
                }
                else if (other.tag == "other ship")
                {
                    other.gameObject.GetComponent<AISubMono>().onBehit((int)(5 + Random.value * power));
                }
            }
        }

        void hitWater()
        {
            GameObject ego = Instantiate(hitWaterExp) as GameObject;
            ego.transform.position = transform.position;
            Destroy(ego, 5f);
            clear();
        }

        void explosion()
        {
            GameObject ego = Instantiate(hitTargetExp) as GameObject;
            ego.transform.position = transform.position;
            Destroy(ego, 5f);
            clear();
        }

        void clear()
        {
            m_isBoom = true;
            SubCamera.instance.onFollowTransformDestory(transform);
        }
    }
}