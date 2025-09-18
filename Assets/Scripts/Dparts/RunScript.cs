using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;

namespace Scraft.DpartSpace {

    public class RunScript : MonoBehaviour
    {
        

        public IPoint get2DMapCoor()
        {
            return GetComponent<DpartParent>().getDpart().get2DMapCoor();
        }
    }    
}
