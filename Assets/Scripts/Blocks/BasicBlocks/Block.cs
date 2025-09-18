using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;

namespace Scraft.BlockSpace
{
    public class Block
    {

        protected int blockId;
        protected string blockName;
        protected int price;        
        protected string attributeCardName;
        protected string texturePath;
        protected bool isMod;
        bool isUnlock;        

        protected IPoint mapSize;
        GameObject parentObject;
        IPoint coor;
        protected Color thumbnailColor;
        protected Color normalColor;
        protected bool m_isNeedDelete;

        protected SpriteRenderer spriteRenderer;
        protected Sprite sprite;
        protected Sprite[] sprites;
        protected GameObject blockObject;

        protected int pState;
        protected bool m_isFluid;
        protected bool m_isLargeBlock;

        protected float density;
        protected float temperature;
        protected float transmissivity;
        protected float heatCapacity;
        protected float heatQuantity;
        protected float press;
        protected float calorific;
        protected int quality;
        protected float max_storeAir;
        protected float storeAir;
        protected ParticleBlock particleBlockLayer;
        protected float penetrationRate;         

        /// <summary>
        /// 是否可以存储在仓库?0:不可存储;1:存于固体;2:存于液体
        /// </summary>
        protected int canStoreInTag;

        public Block(int id, GameObject parentObject, GameObject blockObject)
        {
            blockId = id;
            this.parentObject = parentObject;
            this.blockObject = blockObject;

            m_isNeedDelete = false;
            m_isFluid = false;
            isUnlock = false;
            isMod = getModName() != null;

            canStoreInTag = 0;            

            thumbnailColor = new Color(1, 1, 0);
            normalColor = Color.white;
            price = 1;
            density = 1.0f;
            temperature = 25;
            transmissivity = 3.2f;
            heatCapacity = 450f;
            press = 100;
            calorific = 0;
            quality = 2;
            penetrationRate = 0.95f;
            max_storeAir = 0;
            storeAir = 0;

        }

        protected void initBlock(string blockName, string attributeCardName, bool isNeedAddBackGround = false)
        {
            this.blockName = blockName;
            this.attributeCardName = attributeCardName;
            createBlockObject();
        }

        protected virtual void createBlockObject()
        {
            texturePath = getSelfTexturePath();

            if (parentObject == null)
            {
                loadSprite();
            }
            else
            {
                initSprite();
            }



            if (parentObject != null)
            {
                if (blockObject == null)
                {
                    blockObject = Object.Instantiate(Resources.Load("Prefabs/block")) as GameObject;
                    blockObject.transform.SetParent(parentObject.transform);
                    spriteRenderer = blockObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                }
                else
                {
                    blockObject.transform.SetParent(parentObject.transform);
                    spriteRenderer = blockObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                }
                spriteRenderer.color = Pooler.HeatMap_Mode > 0 ? Color.blue : Color.white;
            }
        }

        void loadSprite()
        {
            if (isMod)
            {
                Texture2D texture = IUtils.loadTexture2DFromSD(texturePath);
                int w = texture.width / 16;
                int h = texture.height / 16;
                sprites = new Sprite[w * h];
                int index = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        sprites[index] = Sprite.Create(texture, new Rect(new Vector2(x * 16, (h - y - 1) * 16), new Vector2(16, 16)), new Vector2(0.5f, 0.5f));
                        index++;
                    }
                }
                sprite = sprites[0];
            }
            else
            {
                sprite = Resources.Load(texturePath, typeof(Sprite)) as Sprite;
                sprites = Resources.LoadAll<Sprite>(texturePath);
            }
        }

        public void initSprite()
        {
            Block blockStatic = BlocksManager.instance.getBlockById(blockId);
            if (blockStatic != null)
            {
                sprites = blockStatic.getSprites();
                sprite = blockStatic.getSprite();
            }
        }

        protected virtual string getModName()
        {
            return null;
        }

        protected virtual string getSelfTexturePath()
        {
            return isMod ? string.Format("{0}{1}/res/{2}.png", GamePath.modFolder, getModName(), blockName) : "blocks/" + blockName;

        }

        public virtual void setBuilderModeHightLight(bool isLight)
        {
            if (isLight)
            {
                //spriteRenderer.color = new Color(0, 1f, 1f);
                spriteRenderer.color = normalColor;
            }
            else
            {
                spriteRenderer.color = normalColor;
            }
        }

        public int getId()
        {
            return blockId;
        }

        public string getName()
        {
            return blockName;
        }

        public string getLangName()
        {
            
            return ILang.get(blockName, isMod ? getModName() + ILang.getSelectedLangName() : "main");
        }

        public int getPrice()
        {
            return price;
        }

        public bool getIsUnlock()
        {
            return BlocksManager.instance.getIsUnlock(this);
        }

        public virtual bool isRootUnlock()
        {
            return false;
        }

        public virtual Color getThumbnailColor(JsonData blockData)
        {
            return thumbnailColor;
        }

        public int getQuality()
        {
            return quality;
        }

        public virtual int getTotalPrice()
        {
            return price;
        }

        public string getTexturePath()
        {
            return texturePath;
        }

        public Sprite getSprite()
        {
            return sprite;
        }

        public Sprite[] getSprites()
        {
            return sprites;
        }

        protected virtual void setSpriteRect(int index)
        {
            if (m_isNeedDelete == false)
            {
                spriteRenderer.sprite = sprites[index];
            }
        }

        public void openHeatMapMode(bool open)
        {
            spriteRenderer.sharedMaterial = Pooler.shareMaterials[open ? 1 : 0];
            spriteRenderer.color = open ? Color.blue : normalColor;
        }

        public bool isLargerBlock()
        {
            return m_isLargeBlock;
        }

        /// <summary>
        /// 获取方块的GameObject
        /// </summary>
        public GameObject getBlockObject()
        {
            return blockObject;
        }

        /// <summary>
        /// 依附于哪个分类
        /// </summary>
        public string getAttributeCardName()
        {
            return attributeCardName;
        }

        /// <summary>
        /// （由引擎调用）当BlocksManager注册时调用
        /// </summary>
        public virtual void onRegister()
        {

        }

        /// <summary>
        /// （由引擎调用）克隆（实例化）方块.1
        /// </summary>
        public virtual Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            return new Block(blockId, parentObject, blockObject);
        }

        /// <summary>
        /// 克隆（实例化）方块.2
        /// </summary>
        public virtual Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject, IPoint mapSize)
        {
            Block block = clone(parentObject, blocksManager, blockObject);
            block.setMapSize(mapSize);
            return block;
        }

        /// <summary>
        /// 克隆（实例化）方块.3
        /// </summary>
        public virtual Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject, IPoint coor, IPoint mapSize)
        {
            Block block = clone(parentObject, blocksManager, blockObject, mapSize);
            block.setPosition(coor);
            return block;
        }

        /// <summary>
        /// （标准）（由引擎调用）设置方块坐标（会移动方块）
        /// </summary>
        public virtual void setPosition(IPoint coor)
        {
            setCoor(coor);
            blockObject.transform.localPosition = coor.mapIPoint2WordVector(mapSize);
        }

        /// <summary>
        /// 设置方块是否可见
        /// </summary>
        public virtual void setBlockVisible(bool visible)
        {
            blockObject.SetActive(visible);
        }

        public IPoint getCoor()
        {
            return coor;
        }

        /// <summary>
        /// （由引擎调用）设置方块坐标（不会移动方块）
        /// </summary>
        public void setCoor(IPoint coor)
        {
            this.coor = coor;
        }

        /// <summary>
        /// （由引擎调用）设置引擎地图大小
        /// </summary>
        public void setMapSize(IPoint mapSize)
        {
            this.mapSize = mapSize;
        }

        /// <summary>
        /// 合成表里显示的贴图Index
        /// </summary>
        public virtual int getSyntIconSpriteIndex()
        {
            return 0;
        }

        /// <summary>
        /// 合成表里显示的贴图
        /// </summary>
        public virtual Sprite getSyntIconSprite()
        {
            if(getSyntIconSpriteIndex() >= sprites.Length)
            {
                Debug.Log(getLangName());
            }
            return sprites[getSyntIconSpriteIndex()];
        }

        /// <summary>
        /// 是否是流体
        /// </summary>
        public bool isFluid()
        {
            return m_isFluid;
        }

        /// <summary>
        /// 是否是液体或汽液混合体
        /// </summary>
        public bool isLiquidOrMushy()
        {
            return equalPState(PState.liquild) || equalPState(PState.mushy);
        }

        public virtual float getDensity()
        {
            return density;
        }

        public void setDensity(float d)
        {
            if (d <= 0)
            {
                Debug.Log("Warning: density <= 0");
                return;
            }
            density = d;
        }

        public void setTemperature(float t)
        {
            temperature = t;
            heatQuantity = temperature2HeatQuantity(temperature);
        }

        public void setHeatQuantity(float q)
        {
            heatQuantity = q;
            if ((getHeatNoMassCapacity()) < 1)
            {
                Debug.Log(getName() + ":" + getHeatNoMassCapacity());
            }
            temperature = heatQuantity2Temperature(heatQuantity);
        }

        public float heatQuantity2Temperature(float heatQuantity)
        {
            return heatQuantity / getHeatNoMassCapacity();
        }

        public float temperature2HeatQuantity(float temperature)
        {
            return getHeatNoMassCapacity() * temperature;
        }

        public float getTemperature()
        {
            return temperature;
        }

        public float getHeatQuantity()
        {
            return heatQuantity;
        }

        public void setPress(float p)
        {
            press = p;
        }

        public virtual float getPress()
        {
            return press;
        }

        public virtual float getTiPress()
        {
            return press;
        }

        public virtual float getOusidePress()
        {
            return press;
        }

        public void setTransmissivity(float t)
        {
            transmissivity = t;
        }

        public float getTransmissivity()
        {
            return transmissivity;
        }

        [System.Obsolete("use addHeatQuantity")]
        public void addTemperature(float add_t)
        {
            setTemperature(temperature + add_t);
        }

        [System.Obsolete("use decHeatQuantity")]
        public void decTemperature(float dec_t)
        {
            setTemperature(temperature - dec_t);
        }

        public void addHeatQuantity(float add_q)
        {
            if (add_q < 0)
            {
                Debug.Log("add_q < 0(" + blockName + "):" + add_q);
                return;
            }
            setHeatQuantity(heatQuantity + add_q);
        }

        public void decHeatQuantity(float dec_q)
        {
            if (dec_q < 0)
            {
                Debug.Log("dec_q < 0(" + blockName + "):" + dec_q);
                return;
            }
            setHeatQuantity(heatQuantity - dec_q);
        }

        public float getHeatCapacity()
        {
            return heatCapacity;
        }

        public float getHeatNoMassCapacity()
        {
            return density * heatCapacity;
        }

        public void addPress(float add_p)
        {
            press += add_p;
        }

        public void decPress(float dec_p)
        {
            press -= dec_p;
        }

        public float getCalorific()
        {
            return calorific;
        }

        public virtual float getBurningPoint()
        {
            return 0;
        }


        public virtual float getMeltingPoint()
        {
            return 0;
        }

        public virtual float getSolidMaxPress()
        {
            return 0;
        }


        public virtual float getFreezingPoint()
        {
            return 0;
        }

        public virtual float getBoilingPoint()
        {
            return 0;
        }

        public void setCalorific(float c)
        {
            calorific = c;
        }

        public void setHeatCapacity(float HC)
        {
            heatCapacity = HC;
        }

        public void addCalorific(float add_c)
        {
            calorific += add_c;
        }

        public void decCalorific(float dec_c)
        {
            calorific -= dec_c;
        }

        public bool equalBlock(Block block)
        {
            return blockId == block.getId();
        }

        public bool equalPState(int pState)
        {
            return this.pState == pState;
        }

        public bool equalPState(Block pState)
        {
            return this.pState == pState.getPState();
        }

        public int getPState()
        {
            return this.pState;
        }

        public virtual float getSignalId()
        {
            return 0;
        }

        public virtual float getMass()
        {
            return density;
        }

        public float getCanStoreAir()
        {
            return max_storeAir;
        }

        public float getStoreAir()
        {
            return storeAir;
        }

        public float getPenetrationRate()
        {
            return penetrationRate;
        }

        public float chargeAir(float charge)
        {
            if (storeAir < max_storeAir)
            {
                if (storeAir + charge <= max_storeAir)
                {
                    storeAir += charge;
                    return 0;
                }
                else
                {
                    float overflow = storeAir + charge - max_storeAir;
                    storeAir = max_storeAir;
                    return overflow;
                }
            }
            else
            {
                return charge;
            }
        }

        public float requireAir(float require)
        {
            if (storeAir > 0)
            {
                if (storeAir - require > 0)
                {
                    storeAir -= require;
                    return require;
                }
                else
                {
                    float surplus = storeAir;
                    storeAir = 0;
                    return surplus;
                }
            }
            else
            {
                return 0;
            }
        }

        public void inheritAir(float storeAir)
        {          
            if(storeAir  < max_storeAir)
            {
                this.storeAir = storeAir;
            }
            else
            {
                this.storeAir = max_storeAir;
            }
            
        }

        public void inheritAir(Block block)
        {
            max_storeAir = block.getCanStoreAir();
            storeAir = block.getStoreAir();
        }

        public virtual float getForce()
        {
            return 0;
        }

        public virtual float getSideForce()
        {
            return 0;
        }

        public int getOutsideTag()
        {
            return Pooler.outsideArea[coor.x, coor.y];
        }

        public int getCargoAreaTag()
        {
            return Pooler.cargoArea[coor.x, coor.y];
        }

        public bool isAir()
        {
            return blockId == 0;
        }

        public bool isBorder()
        {
            return blockId == 1;
        }

        public bool isWater()
        {
            return blockId == 5;
        }

        public bool isWaterGas()
        {
            return blockId == 6 || blockId == 7 || blockId == 120 || blockId == 121;
        }

        public bool isFire()
        {
            return blockId == 14;
        }

        public virtual int getCompressChildrenCount()
        {
            return 0;
        }

        /// <summary>
        /// 主线程更新
        /// </summary>
        public virtual void update(BlocksEngine blocksEngine)
        {
            heatMapRule();
            if(particleBlockLayer != null)
            {
                particleBlockLayer.insideUpdate(blocksEngine, this);
            }
        }

        public virtual void heatMapRule()
        {
            bool isChangeMaterial = false;
            if (Pooler.HeatMap_Mode > 0 && getOutsideTag() != 3)
            {
                switch (Pooler.HeatMap_Mode)
                {
                    case 1:
                        spriteRenderer.color = Pooler.instance.getHeatColor(temperature);
                        isChangeMaterial = true;
                        break;
                    case 2:
                        spriteRenderer.color = Pooler.instance.getHeatColor(getTiPress());
                        isChangeMaterial = true;
                        break;
                    case 3:
                        if (equalPState(PState.particle) || isContainParticleBlock())
                        {
                            spriteRenderer.sharedMaterial = Pooler.shareMaterials[1];
                            spriteRenderer.color = Color.blue;
                        }
                        else
                        {
                            spriteRenderer.sharedMaterial = Pooler.shareMaterials[0];
                            spriteRenderer.color = Color.grey;
                        }
                        isChangeMaterial = false;
                        break;
                }
                
            }
            spriteRenderer.sharedMaterial = Pooler.shareMaterials[isChangeMaterial ? 1 : 0];
        }

        /// <summary>
        /// 线程更新
        /// </summary>
        public virtual void threadUpdate(BlocksEngine blocksEngine)
        {
            temperatureRule();
        }

        /// <summary>
        /// 帧更新
        /// </summary>
        public virtual bool frameUpdate()
        {
            return false;
        }


        /// <summary>
        /// 温度传递规律（热传冷，传递系数由热方块决定）
        /// </summary>
        public virtual void temperatureRule()
        {
            float minTemperature;
            Block neighbor = getNeighborMinTemperatureBlock(out minTemperature);
            if (neighbor != null && minTemperature < temperature)
            {
                float dt = (temperature - neighbor.getTemperature()) * (transmissivity * 0.04f);
                float avgT = (temperature + neighbor.getTemperature()) * 0.5f;
                float giveQ = temperature2HeatQuantity(dt);
                float preT = preheating(this, -giveQ);
                float preNT = preheating(neighbor, giveQ);

                float pt = temperature;
                float npt = neighbor.getTemperature();

                float NMinGiveQ = giveQ;
                if (preT < avgT || preNT > avgT)
                {
                    float NGiveQ = temperature2HeatQuantity(temperature - avgT);
                    float NNReceiverQ = neighbor.temperature2HeatQuantity(avgT - neighbor.getTemperature());
                    NMinGiveQ = Mathf.Min(Mathf.Abs(NGiveQ), Mathf.Abs(NNReceiverQ));
                }
                decHeatQuantity(NMinGiveQ);
                neighbor.addHeatQuantity(NMinGiveQ);

                if (neighbor.getTemperature() > avgT)
                {
                    neighbor.setTemperature(avgT);
                }

                if (temperature < avgT)
                {
                    setTemperature(avgT);
                }
            }
        }

        protected float preheating(Block block, float addQ)
        {
            return block.heatQuantity2Temperature(block.getHeatQuantity() + addQ);
        }

        private Block getNeighborMinTemperatureBlock(out float minTemperature)
        {
            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            minTemperature = Mathf.Min(new float[4]{up_block.getTemperature(),
                                right_block.getTemperature(),
                                down_block.getTemperature(),
                                left_block.getTemperature()});

            if (minTemperature == up_block.getTemperature())
            {
                return up_block;
            }
            else if (minTemperature == right_block.getTemperature())
            {
                return right_block;
            }
            else if (minTemperature == down_block.getTemperature())
            {
                return down_block;
            }
            else if (minTemperature == left_block.getTemperature())
            {
                return left_block;
            }
            return null;
        }

        /// <summary>
        /// 获取相邻方块
        /// </summary>
        public Block getNeighborBlock(int dir)
        {
            return BlocksEngine.instance.getBlock(coor.getDirPoint(dir));
        }

        /// <summary>
        /// （标准）移动方块
        /// </summary>
        public void moveTo(IPoint toCoor)
        {
            if (!m_isNeedDelete)
            {                
                Block block2 = BlocksEngine.instance.getBlock(toCoor);

                block2.setPosition(this.getCoor());
                BlocksEngine.instance.setBlock(block2.getCoor(), block2);

                this.setPosition(toCoor);
                BlocksEngine.instance.setBlock(this.getCoor(), this);
            }
        }

        public void moveToByDir(int dir)
        {
            moveTo(coor.getDirPoint(dir));
        }

        /// <summary>
        ///粒子方块进入
        /// </summary>
        public void particleMoveIn(ParticleBlock block, bool isPlaceIn)
        {
            particleBlockLayer = block;
            block.onInsideOtherBlock(this);
            if (!isPlaceIn)
            {
                BlocksEngine.instance.removeBlock(block.getCoor(), false, block.getPress());
            }   
        }

        public void particleMoveOut(Block outBlock)
        {
            particleBlockLayer.onOutOtherBlock();          
            particleBlockLayer.setTemperature(outBlock.getTemperature());
            particleBlockLayer.setPress(outBlock.getPress());
            BlocksEngine.instance.placeBlock(outBlock.getCoor(), particleBlockLayer);
            particleBlockLayer = null;
        }

        public ParticleBlock particlePushOut()
        {
            particleBlockLayer.onOutOtherBlock();
            ParticleBlock particleBlock = particleBlockLayer;
            particleBlockLayer = null;
            return particleBlock;
        }

        public bool isContainParticleBlock()
        {
            return particleBlockLayer != null;
        }

        /// <summary>
        /// 世界模式下被摧毁时调用
        /// </summary>
        public virtual void onWorldModeDestroy()
        {
            PoolerUI.instance.updateMaxAir();
        }

        /// <summary>
        /// 世界模式下的保存信息流
        /// </summary>
        public virtual JsonWriter onWorldModeSave(JsonWriter writer)
        {
            IUtils.keyValue2Writer(writer, "id", blockId);
            IUtils.keyValue2Writer(writer, "x", coor.x);
            IUtils.keyValue2Writer(writer, "y", coor.y);
            IUtils.keyValue2Writer(writer, "sa", storeAir);
            return writer;
        }

        /// <summary>
        /// 世界模式下的读取信息流
        /// </summary>
        public virtual void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            setCoor(coor);
            storeAir = IUtils.getJsonValue2Float(blockData, "sa", 0);
            //setTemperature(IUtils.getJsonValue2Float(blockData, "t"));
            //press = IUtils.getJsonValue2Float(blockData, "p");
        }

        /// <summary>
        /// 获取合成表
        /// </summary>
        public virtual SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            return null;
        }

        /// <summary>
        /// （由引擎调用）当接收到机械能时调用
        /// </summary>
        public virtual void onReciverMe(float me, int putterDir, Block putter)
        {

        }

        /// <summary>
        /// （由引擎调用）当接收到弱电时调用
        /// </summary>
        public virtual void onReciverWe(float voltage, int putterDir, Block putter)
        {

        }

        /// <summary>
        /// （由引擎调用）获取一个鱼雷
        /// </summary>
        public virtual bool giveOneTorpedp(Block taker)
        {
            return false;
        }

        /// <summary>
        /// （由引擎调用）获取一个枚炮弹
        /// </summary>
        public virtual bool giveOneShell(BlocksEngine blocksEngine, Block taker)
        {
            return false;
        }

        /// <summary>
        /// （由引擎调用）获取一个枚炮弹
        /// </summary>
        public virtual bool giveOneDepthCharge(BlocksEngine blocksEngine, Block taker)
        {
            return false;
        }

        /// <summary>
        /// 是否是弱电系统，用于连接电缆
        /// </summary>
        public virtual int isWeSystem()
        {
            return 0;
        }

        /// <summary>
        /// 获取远程弱电能
        /// </summary>
        public virtual float getRomaoteMe()
        {
            return 0;
        }

        /// <summary>
        /// 是否可以输入机械能
        /// </summary>
        public virtual bool isCanReceiveMe()
        {
            return false;
        }

    

        /// <summary>
        /// 是否可发送无线信号
        /// </summary>
        public virtual bool isCanSendWifi()
        {
            return false;
        }

        /// <summary>
        /// 是否可以绑定按钮
        /// </summary>
        public virtual bool isCanBind()
        {
            return false;
        }

        /// <summary>
        /// 是否可以设置整定值，-1为不可以，0-3默认位数
        /// </summary>
        public virtual int isCanSettingValue()
        {
            return -1;
        }

        /// <summary>
        /// 最大储存电量
        /// </summary>
        public virtual float getMaxStoreElectric()
        {
            return 0;
        }

        /// <summary>
        /// 是否可以存储在仓库?0:不可存储;1:存于固体;2:存于液体
        /// </summary>
        public int isCanStoreInWarehouse()
        {
            return canStoreInTag;
        }

        /// <summary>
        /// 是否是货舱方块
        /// </summary>
        public virtual bool isCargohold()
        {
            return false;
        }

        /// <summary>
        /// 生涯模式中是否可以无限制使用该方块
        /// </summary>
        virtual public bool isCareerModeStoreUnlimit()
        {
            return false;
        }

        /// <summary>
        /// 方块基本信息
        /// </summary>
        public virtual string getBasicInformation()
        {
            string langName = isMod ? blockName + ".info" : blockName;
            string information = ILang.get(langName, isMod ? getModName() + ILang.getSelectedLangName() : "information");
            if (information.Equals(langName))
            {
                return string.Format("{0},{1}", getLangName(), blockBasicInformation());
            }
            else
            {
                return string.Format("{0},{1},{2}", getLangName(), information, blockBasicInformation());
            }
        }

        string blockBasicInformation()
        {
            return string.Format("{0}:{1},{2}:{3}", ILang.get("transmissivity", "menu"), transmissivity, ILang.get("Specific heat capacity", "menu"), heatCapacity);
        }


        /// <summary>
        /// 当被粒子碰撞的时候
        /// </summary>
        public virtual void onParticleCollide(ParticleBlock particleBlock)
        {

        }

        /// <summary>
        /// 建造模式下的点击
        /// </summary>
        public virtual void onBuilderModeClick()
        {

        }

        /// <summary>
        /// 世界模式下的点击
        /// </summary>
        public virtual void onWorldModeClick()
        {

        }

        /// <summary>
        /// 在航行模式下加载完成
        /// </summary>
        public virtual void onPoolerModeInitFinish()
        {
            setTemperature(25);           
        }       

        /// <summary>
        /// 建造模式被建造
        /// </summary>
        public virtual void onBuilderModeCreated()
        {
            BlocksManager.instance.addConsumeCargoCount(this, 1);
            CardManager.instance.updatecargoCount();
        }

        /// <summary>
        /// 建造模式被删除
        /// </summary>
        public virtual void onBuilderModeDeleted()
        {
            BlocksManager.instance.addConsumeCargoCount(this, -1);
            CardManager.instance.updatecargoCount();
            CardManager.instance.setDrawerActivited(CardManager.instance.getDrawerByBlockId(blockId), false);
        }

        /// <summary>
        /// 旋转按钮点击
        /// </summary>
        public virtual void onRotateButtonClick()
        {

        }

        /// <summary>
        /// 开火按钮点击点击
        /// </summary>
        public virtual bool onFireButtonClicked()
        {
            return false;
        }

        /// <summary>
        /// 是否可旋转
        /// </summary>
        public virtual bool isCanRotate()
        {
            return false;
        }

        /// <summary>
        /// 是否被销毁
        /// </summary>
        public bool isNeedDelete()
        {
            return m_isNeedDelete;
        }

        /// <summary>
        /// 乘客是否可以穿过
        /// </summary>
        public virtual bool isCollider()
        {
            return false;
        }

        public void clearParticleBlockLayer()
        {
            if (particleBlockLayer != null)
            {
                particleBlockLayer.clear(true);
                particleBlockLayer = null;
            }
        }

        /// <summary>
        /// （由引擎调用）销毁方块
        /// </summary>
        public virtual void clear(bool destroy)
        {
            m_isNeedDelete = true;
            setSpriteRect(0);
            if (destroy)
            {
                Object.Destroy(blockObject);
            }
            blockObject = null;
            clearParticleBlockLayer();
        }
    }

    public class SyntInfo
    {
        public int[,] syntData;        
        public Block produeBlockStatic;
        public int produeNumber;

        public SyntInfo(Block produeBlockStatic, int[,] syntData, int produeNumber)
        {
            this.produeBlockStatic = produeBlockStatic;
            this.syntData = syntData;            
            this.produeNumber = produeNumber;
        }
    }
}