using Scraft.BlockSpace;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class ShotShellRS : RunScript
    {
        public GameObject shell;
        public GameObject fireEffect;
        public AudioSource fireAudioSource;
        public List<Transform> FirePoints;
        public float power;
        public int readyingTime;
        public float shotSpeedRate;
        public float rotateCorrectionFactor = 0.5f;

        protected int turretCoreBlockId;
        protected int fireIcon;

        SelectorRS selectorRS;
        RotationRS rotationRS;

        IPoint turretCoor;

        protected float shotSpeed;

        float S, v02, radian, g;
        Vector3 startPosition, targetPosition;
        Quaternion convertQ;

        static public float assistAimPitchAngle;

        static bool isOpenAssistAim;
        Vector3 startEulrAngle;

        static public float receivePredictorData;
        static public float receivePitchAngle;
        static public float receiveRotateAngle;

        void Start()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }           

            PoolerItemSelector.instance.OnSelected += OnSelected;
            PoolerItemSelector.instance.OnCancelButtonClick += OnCancelButtonClick;
            PoolerItemSelector.instance.OnCancelByHitOther += OnCancelButtonClick;
            PoolerItemSelector.instance.OnCustom2ButtonClick += OnCustom2ButtonClick;
            PoolerUI.instance.OnFireButtonClick += OnFireButtonClick;

            fireAudioSource = GetComponent<AudioSource>();

            selectorRS = GetComponent<SelectorRS>();
            rotationRS = GetComponent<RotationRS>();

            initTurret();

            getTurretCoreCoor();
            TurretCore turretCore = BlocksEngine.instance.getBlock(turretCoor) as TurretCore;
            if (turretCore != null && turretCore.getId() == turretCoreBlockId)
            {
                turretCore.setReadyingTime(readyingTime);
                turretCore.setFireShellCount(FirePoints.Count);
            }

            shotSpeed = selectorRS.getBoundSize() * shotSpeedRate;
            shotSpeed = Mathf.Clamp(shotSpeed, 1, 30);
            v02 = shotSpeed * shotSpeed;
            g = 1;
            assistAimPitchAngle = 0;
            receivePredictorData = 0;
            receivePitchAngle = 0;
            receiveRotateAngle = 0;
            isOpenAssistAim = false;

            startEulrAngle = transform.localEulerAngles;
        }

        protected virtual void initTurret()
        {
            turretCoreBlockId =  BlocksManager.instance.turretCore.getId();
            fireIcon = 1;
        }

        void getTurretCoreCoor()
        {
            turretCoor = get2DMapCoor();
            TurretCore turretCore = BlocksEngine.instance.getBlock(turretCoor) as TurretCore;
            if(turretCore == null || turretCore.getId() != turretCoreBlockId)
            {
                if (getTurrectCoreMethod(Dir.left, out turretCoor)) return;
                if (getTurrectCoreMethod(Dir.up, out turretCoor)) return;
                if (getTurrectCoreMethod(Dir.right, out turretCoor)) return;
                if (getTurrectCoreMethod(Dir.down, out turretCoor)) return;
            }
        }

        bool getTurrectCoreMethod(int dir, out IPoint coor)
        {
            IPoint turretCoor = get2DMapCoor();
            turretCoor = turretCoor.getDirPoint(dir);
            TurretCore turretCore = BlocksEngine.instance.getBlock(turretCoor) as TurretCore;
            if(turretCore != null)
            {
                coor = turretCoor;
                return true;
            }
            coor = IPoint.zero;
            return false;
        }

        void OnSelected()
        {
            if (selectorRS.isMainSelecting)
            {
                Pooler.instance.changeFireWeapon(fireIcon);
                ShellTargetPlane.show(true);
                PoolerItemSelector.instance.setCustom2ButtonText(ILang.get(isOpenAssistAim ? "Manual" : "Automatic"));
            }
        }

        void OnCancelButtonClick()
        {
            if (selectorRS.isMainSelecting)
            {
                Pooler.instance.changeFireWeapon(0);
                ShellTargetPlane.show(false);
            }
        }

        void OnCustom2ButtonClick()
        {
            if (selectorRS.isSelecting)
            {
                isOpenAssistAim = !isOpenAssistAim;
                PoolerItemSelector.instance.setCustom2ButtonText(ILang.get(isOpenAssistAim ? "Manual" : "Automatic"));
            }
        }

        void OnFireButtonClick()
        {
            if (selectorRS.isSelecting)
            {
                float water = Buoyancy.getWaterHeight(transform.position);    
                if (Pooler.instance.fireWeapon >= 1 && transform.position.y > water)
                {
                    TurretCore turretCore = BlocksEngine.instance.getBlock(turretCoor) as TurretCore;
                    if (turretCore != null && turretCore.getId() == turretCoreBlockId && turretCore.fire())
                    {
                        placementParameters(turretCore);
                        fire();
                        fireAudioSource.Play();
                    }
                }
            }
        }

        protected virtual void placementParameters(TurretCore turretCore)
        {

        }

        private void Update()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }
            
            float water = Buoyancy.getWaterHeight(transform.position);       
            if (selectorRS.isMainSelecting)
            {                       
                if (transform.position.y > water)
                {
                    ShellTargetPlane.show(true);
                    calculateTarget();
                }
                else
                {
                    ShellTargetPlane.show(false);
                }

                calculateAimData();
            }

            if (isOpenAssistAim && transform.position.y > water)
            {
                AimAssist();
            }

            
        }

        void calculateAimData()
        {
            float distance = AISubMono.nearestAiDistance;
            if (distance > 1000)
            {
                assistAimPitchAngle = 0;
                return;
            }
            float temp = distance * g / v02;
            if (temp >= 1)
            {
                assistAimPitchAngle = 0;
                return;
            }
            float angle = IUtils.radian2angle(Mathf.Asin(temp)) * 0.5f;
            assistAimPitchAngle = angle;           
        }

        void AimAssist()
        {
            if (selectorRS.isMainSelecting)
            {
                TurretCore turretCore = BlocksEngine.instance.getBlock(turretCoor) as TurretCore;
                if (turretCore != null && turretCore.getId() == turretCoreBlockId)
                {
                    receivePredictorData = (int)turretCore.getPredictorData();
                    receivePitchAngle = receivePredictorData % 100;
                    receiveRotateAngle = receivePredictorData / 100;
                }

            }

            if (selectorRS.isSelecting)
            {
                sendAimAngleToRotationRs();
            }         

        }

        void sendAimAngleToRotationRs()
        {
            float pitchAngle = -(IUtils.angleRoundIn180(receivePitchAngle) - 0.01f);
            float rotateAngle = IUtils.angleRoundIn180(receiveRotateAngle + rotateCorrectionFactor - (startEulrAngle.y - 270));

            if (receivePitchAngle != 0)
            {
                rotationRS.setHeadAngle(IUtils.angleRoundIn180(pitchAngle));
            }

            if (receiveRotateAngle != 0)
            {
                rotationRS.setBaseAngle(IUtils.angleRoundIn180(rotateAngle));
            }
        }

        public static float getAssistAimData()
        {
            return assistAimPitchAngle;
        }

        void calculateTarget()
        {
            startPosition = FirePoints[0].position;
            Vector3 direction = FirePoints[0].forward;
            Vector3 plantDirection = new Vector3(direction.x, 0, direction.z);
            convertQ = Quaternion.LookRotation(plantDirection);
            radian = IUtils.angle2radian(Vector3.Angle(direction, plantDirection));

            S = v02 * Mathf.Sin(2 * radian) / g;
            targetPosition = startPosition + convertQ * new Vector3(0, 0, S);
            ShellTargetPlane.setPosition(targetPosition);
        }

        void fire()
        {
            Vector3 effectPosition = Vector3.zero;
            Vector3 effectDir = Vector3.zero;
            foreach (Transform trans in FirePoints)
            {
                instantiateShellMethod(trans);
                if (effectPosition.Equals(Vector3.zero))
                {
                    effectPosition = trans.position;
                    effectDir = trans.forward;
                }
            }

            showFireEffect(effectPosition, effectDir);
        }

        protected virtual void instantiateShellMethod(Transform trans)
        {
            GameObject nShell = Instantiate(shell);
            nShell.GetComponent<Shell>().initialized(trans, shotSpeed, power);
        }

        void showFireEffect(Vector3 position, Vector3 dir)
        {
            if (fireEffect != null)
            {
                GameObject ego = Instantiate(fireEffect) as GameObject;
                ego.transform.position = position;
                ego.GetComponent<Rigidbody>().velocity = Vector3.up + dir * (1 + 0.2f * MainSubmarine.speed) + MainSubmarine.rigidbody.velocity;
                Destroy(ego, 5);
            }
        }

        public void clear()
        {
            SubCamera.instance.onFollowTransformDestory(transform);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            PoolerItemSelector.instance.OnSelected -= OnSelected;
            PoolerItemSelector.instance.OnCancelButtonClick -= OnCancelButtonClick;
            PoolerItemSelector.instance.OnCancelByHitOther -= OnCancelButtonClick;
            PoolerItemSelector.instance.OnCustom2ButtonClick -= OnCustom2ButtonClick;
            PoolerUI.instance.OnFireButtonClick -= OnFireButtonClick;
        }
    }
}