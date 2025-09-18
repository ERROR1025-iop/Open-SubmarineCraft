using Scraft.BlockSpace;
using UnityEngine;

namespace Scraft
{
    public class RadarArrClear : MonoBehaviour
    {

        Radar radar;

        void Start()
        {
            radar = Radar.instance;
        }

        void Update()
        {
            radar.showArr(Dir.up, false);
            radar.showArr(Dir.down, false);
            radar.showArr(Dir.left, false);
            radar.showArr(Dir.right, false);           
        }
    }
}