using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft{    
    public class CraftShipIcon : MonoBehaviour
    {
        public Sprite[] sprites;

        Crafts crafts;
        int index;
        CraftInfo craftInfo;       
        SpriteRenderer spriteRenderer;

        void Start()
        {
            Vector3 coor = new Vector3(craftInfo.position.x, craftInfo.position.z, -5);
            transform.position = coor / 10;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void initialized(Crafts crafts, int index, CraftInfo craftInfo)
        {
            this.crafts = crafts;
            this.index = index;
            this.craftInfo = craftInfo;                        
        }

        public void onShipIconClick()
        {
            crafts.onShipIconClick(index);
        }

        public void setActivity(bool b)
        {
            spriteRenderer.sprite = sprites[b ? 1 : 0];
        }
    }
}
