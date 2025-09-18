using Scraft.BlockSpace;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class DepthChargeThrowerRS : ShotShellRS
    {

        float activityDeep;

        protected override void initTurret()
        {
            turretCoreBlockId = BlocksManager.instance.depthChargeThrowerCore.getId();
            fireIcon = 2;
        }

        protected override void placementParameters(TurretCore turretCore)
        {
            activityDeep = turretCore.getCurrentSettingValue();
        }

        protected override void instantiateShellMethod(Transform trans)
        {
            GameObject nShell = Instantiate(shell);
            nShell.GetComponent<DepthCharge3DMono>().initDepthChargeByComputationalSimulation(trans, shotSpeed, activityDeep, false);
        }

    }
}