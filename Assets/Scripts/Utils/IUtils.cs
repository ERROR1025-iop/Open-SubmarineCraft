using LitJson;
using Scraft.BlockSpace;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Scraft
{
    public class IUtils
    {
        public static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = false;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        static public string getSyntString(SyntInfo[] syntInfos, string newlineChar)
        {
            if (syntInfos == null)
            {
                return "";
            }

            SyntInfo syntInfo = syntInfos[0];
            int[,] syntData = syntInfo.syntData;
            int makingsCount = syntData.GetLength(0);
            string result = "";
            for (int k = 0; k < makingsCount; k++)
            {
                string blockName = BlocksManager.instance.getBlockById(syntData[k, 0]).getLangName();
                result += string.Format("{1}x{2}{3}", k, blockName, syntData[k, 1], newlineChar);
            }

            //result+= string.Format("\n{0}:{1}", ILang.get("produeNumber"), syntInfo.produeNumber);
            return result;
        }

        static public string serializeIntArray(int[] data)
        {
            string datastr = "start";
            foreach (var d in data)
            {
                datastr += "," + d.ToString();
            }
            return datastr;
        }

        static public int[] unserializeIntArray(string datastr)
        {
            string[] datas = datastr.Split(',');
            int[] data = new int[datas.Length - 1];
            for (int i = 1; i < datas.Length; i++)
            {
                data[i - 1] = int.Parse(datas[i]);
            }
            return data;
        }

        static public void initializedArray<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        static public Vector3 vectorRoundInPlane(Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        static public bool approximatelyEquel(float a, float b, float range)
        {
            return Mathf.Abs(a - b) < range;
        }

        //各分量相乘
        static public Vector3 vector3ComponeMUL(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        // 将大于180度角进行以负数形式输出
        static public float angleRoundIn180(float value)
        {
            value = reviseAngleIn360(value);

            float angle = value - 180;

            if (angle > 0)
            {
                return angle - 180;
            }

            if (value == 0)
            {
                return 0;
            }

            return angle + 180;
        }

        static public bool isPointGUI()
        {
            bool isPoint = GameSetting.isAndroid ? Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : false : EventSystem.current.IsPointerOverGameObject();
            if (isPoint)
            {
                return true;
            }
            return false;
        }

        static public bool comparePointUIObjectTag(string tag)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            if (GameSetting.isAndroid)
            {
                if (Input.touchCount > 1)
                {
                    return false;
                }
                eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }
            else
            {
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count > 0)
            {
                return results[0].gameObject.CompareTag(tag);
            }
            return false;
        }

        static public IPoint getCenterOfBlocks(Block[,] blocks)
        {
            IPoint center = new IPoint(0, 0);
            int count = 0;
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    if (blocks[x, y] != null)
                    {
                        center += blocks[x, y].getCoor();
                        count++;
                    }
                }
            }
            center = new IPoint(center.x / count, center.y / count);
            return center;
        }

        static public float sigmoid(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }

        static public void copyFile(string from, string to, bool isCover)
        {
            if (isCover || !new FileInfo(to).Exists)
            {
                string s1 = readFromTxt(from);
                if (s1 != null)
                {
                    write2txt(to, s1);
                }
            }
        }

        static public void renameFileByCopy(string path, string filename, string newName, bool isCover)
        {
            if (isCover || !new FileInfo(path + newName).Exists)
            {
                string s1 = readFromTxt(path + filename);
                if (s1 != null)
                {
                    write2txt(path + newName, s1);
                }
            }
        }

        static public void changeLayerWithChildrens(GameObject go, int layer)
        {
            Transform[] transforms = go.GetComponentsInChildren<Transform>();
            foreach (Transform tran in transforms)
            {
                tran.gameObject.layer = layer;
            }
        }

        static public void deleteAllChildrens(Transform transform)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Object.DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        static public Bounds GetBounds(GameObject obj)
        {
            Vector3 Min = new Vector3(99999, 99999, 99999);
            Vector3 Max = new Vector3(-99999, -99999, -99999);
            MeshRenderer[] renders = obj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renders.Length; i++)
            {
                if (renders[i].gameObject.name.Equals("Lightbeam"))
                {
                    continue;
                }
                if (renders[i].bounds.min.x < Min.x)
                    Min.x = renders[i].bounds.min.x;
                if (renders[i].bounds.min.y < Min.y)
                    Min.y = renders[i].bounds.min.y;
                if (renders[i].bounds.min.z < Min.z)
                    Min.z = renders[i].bounds.min.z;

                if (renders[i].bounds.max.x > Max.x)
                    Max.x = renders[i].bounds.max.x;
                if (renders[i].bounds.max.y > Max.y)
                    Max.y = renders[i].bounds.max.y;
                if (renders[i].bounds.max.z > Max.z)
                    Max.z = renders[i].bounds.max.z;
            }

            Vector3 center = (Min + Max) / 2;
            Vector3 size = new Vector3(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);
            return new Bounds(center, size);
        }

        static public Bounds GetBounds(GameObject[] objs)
        {
            Vector3 Min = new Vector3(99999, 99999, 99999);
            Vector3 Max = new Vector3(-99999, -99999, -99999);
            for (int k = 0; k < objs.Length; k++)
            {
                if (objs[k] == null)
                {
                    continue;
                }

                MeshRenderer[] renders = objs[k].GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renders.Length; i++)
                {
                    if (renders[i].gameObject.name.Equals("Lightbeam"))
                    {
                        continue;
                    }

                    if (renders[i].bounds.min.x < Min.x)
                        Min.x = renders[i].bounds.min.x;
                    if (renders[i].bounds.min.y < Min.y)
                        Min.y = renders[i].bounds.min.y;
                    if (renders[i].bounds.min.z < Min.z)
                        Min.z = renders[i].bounds.min.z;

                    if (renders[i].bounds.max.x > Max.x)
                        Max.x = renders[i].bounds.max.x;
                    if (renders[i].bounds.max.y > Max.y)
                        Max.y = renders[i].bounds.max.y;
                    if (renders[i].bounds.max.z > Max.z)
                        Max.z = renders[i].bounds.max.z;
                }
            }

            Vector3 center = (Min + Max) / 2;
            Vector3 size = new Vector3(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);
            return new Bounds(center, size);
        }

        static public void centerOnChildrens(GameObject parentGameObject)
        {
            centerOnChildrens(parentGameObject, Vector3.zero);
        }

        static public void centerOnChildrens(GameObject parentGameObject, Vector3 offset)
        {
            Vector3 position = parentGameObject.transform.position;
            parentGameObject.transform.position = Vector3.zero;
            Vector3 centerPoint = centerOfGameObjects(parentGameObject);
            Transform newTransform = new GameObject("GameObject").transform;
            newTransform.localPosition = centerPoint - offset;
            while (parentGameObject.transform.childCount > 0)
            {
                Transform transform = parentGameObject.transform.GetChild(0);
                parentGameObject.transform.GetChild(0).SetParent(newTransform, true);
            }

            newTransform.position = Vector3.zero;
            while (newTransform.childCount > 0)
            {
                Transform transform = newTransform.GetChild(0);
                newTransform.GetChild(0).SetParent(parentGameObject.transform, true);
            }
            parentGameObject.transform.position = position;
            Object.Destroy(newTransform.gameObject);
        }

        static public Vector3 weigthCenterOfGameObjects(GameObject parentGameObject, float offset)
        {
            Vector3 center;
            weigthCenterOfGameObjects(parentGameObject, out center, offset);
            return center;
        }

        static public bool weigthCenterOfGameObjects(GameObject parentGameObject, out Vector3 centerPoint, float offset)
        {
            centerPoint = Vector3.zero;
            Vector3 position = Vector3.zero;
            float weight;
            float totalWeight = 0;
            if (parentGameObject != null)
            {
                Transform[] childrens = parentGameObject.GetComponentsInChildren<Transform>();
                int count = childrens.Length;
                for (int i = 0; i < count; i++)
                {
                    if (childrens[i].gameObject.Equals(parentGameObject))
                    {
                        continue;
                    }
                    position = childrens[i].position;
                    MeshRenderer meshRenderer = childrens[i].GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        if (meshRenderer.gameObject.name.Equals("Lightbeam"))
                        {
                            continue;
                        }
                        weight = meshRenderer.bounds.size.sqrMagnitude;
                        centerPoint += meshRenderer.bounds.center * weight;
                        totalWeight += weight;
                    }
                }
                centerPoint = centerPoint / totalWeight;
                centerPoint += new Vector3(0, offset, 0);
                centerPoint = parentGameObject.transform.TransformPoint(centerPoint);
                return totalWeight > 0;
            }
            return false;
        }

        static public Vector3 centerOfGameObjects(GameObject parentGameObject)
        {
            Vector3 centerPoint = Vector3.zero;
            Vector3 position = Vector3.zero;
            int count = 0;
            if (parentGameObject != null && parentGameObject.transform.childCount > 0)
            {
                Transform[] childrens = parentGameObject.GetComponentsInChildren<Transform>();

                for (int i = 0; i < childrens.Length; i++)
                {
                    if (childrens[i].gameObject.activeSelf && !childrens[i].gameObject.Equals(parentGameObject))
                    {
                        position = childrens[i].position;
                        centerPoint.x += position.x;
                        centerPoint.y += position.y;
                        centerPoint.z += position.z;
                        count++;
                    }
                }
                centerPoint = centerPoint / count;
                centerPoint = parentGameObject.transform.TransformPoint(centerPoint);
            }
            return centerPoint;
        }

        static public Vector3 centerOfGameObjects(GameObject[] gameObjects)
        {
            Vector3 centerPoint = Vector3.zero;
            Vector3 position = Vector3.zero;
            if (gameObjects != null && gameObjects.Length > 0)
            {
                int count = 0;
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    if (gameObjects[i] != null)
                    {
                        position = gameObjects[i].transform.localPosition;
                        centerPoint.x += position.x;
                        centerPoint.y += position.y;
                        centerPoint.z += position.z;
                        count++;
                    }
                }
                if (count > 0)
                {
                    centerPoint = centerPoint / count;
                }
            }
            return centerPoint;
        }

        static public void detectionRedundantFiles(string listPath, string listSuffix, string detectedPath)
        {
            if (Directory.Exists(listPath) && Directory.Exists(detectedPath))
            {
                DirectoryInfo listDirection = new DirectoryInfo(listPath);
                FileInfo[] listFolders = listDirection.GetFiles(listSuffix, SearchOption.TopDirectoryOnly);

                DirectoryInfo detectedDirection = new DirectoryInfo(detectedPath);
                FileInfo[] detectedFolders = detectedDirection.GetFiles("*", SearchOption.TopDirectoryOnly);

                for (int i = 0; i < detectedFolders.Length; i++)
                {
                    string detectedName = removeFileSuffix(detectedFolders[i].Name);
                    bool needDel = true;
                    for (int j = 0; j < listFolders.Length; j++)
                    {
                        string listName = removeFileSuffix(listFolders[j].Name);
                        if (detectedName.Equals(listName))
                        {
                            needDel = false;
                        }
                    }
                    if (needDel)
                    {
                        detectedFolders[i].Delete();
                    }
                }
            }
        }

        static public string removeFileSuffix(string file)
        {
            return file.Substring(file.LastIndexOf("\\") + 1, (file.LastIndexOf(".") - file.LastIndexOf("\\") - 1));
        }

        static public void saveTexture2D2SD(Texture2D texture2D, string path)
        {
            byte[] png = texture2D.EncodeToPNG();
            File.WriteAllBytes(path, png);
        }

        static public Color HexToColor(string hex)
        {
            try
            {
                Color nowColor;
                ColorUtility.TryParseHtmlString("#" + hex, out nowColor);
                return nowColor;
            }
            catch
            {
                return Color.white;
            }

        }

        static public Texture2D captureScreen(Camera camera, Rect r)
        {
            camera.enabled = true;
            RenderTexture rt = new RenderTexture((int)r.width, (int)r.height, 24);

            camera.targetTexture = rt;
            camera.Render();

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGB24, false);

            screenShot.ReadPixels(r, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.Destroy(rt);
            camera.enabled = false;
            return screenShot;
        }

        static public Texture2D loadTexture2DFromSD(string _url)
        {
            //创建文件读取流
            FileStream _fileStream = new FileStream(_url, FileMode.Open, FileAccess.Read);
            _fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] _bytes = new byte[_fileStream.Length];
            _fileStream.Read(_bytes, 0, (int)_fileStream.Length);
            _fileStream.Close();
            _fileStream.Dispose();
            //创建Texture
            Texture2D _texture2D = new Texture2D(2, 2);
            _texture2D.LoadImage(_bytes);
            return _texture2D;
        }

        static public string vector3Serialize(Vector3 vector3)
        {
            return string.Format("{0}^{1}^{2}", vector3.x, vector3.y, vector3.z);
        }

        static public Vector3 vector3Parse(string data)
        {
            try
            {
                string[] s = data.Split('^');
                return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
            }
            catch
            {
                return Vector3.zero;
            }
        }

        static public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixel((int)((float)j / (float)result.width * source.width), (int)((float)i / (float)result.height * source.height));
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }

        static public void resetImageSize(Image image, Vector2 maxSize)
        {
            Texture2D texture2D = image.sprite.texture;
            float w = texture2D.width;
            float h = texture2D.height;
            if (w > h * 1.5f)
            {
                float p = w / h;
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(maxSize.x, maxSize.x / p);
            }
            else
            {
                float p = h / w;
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(maxSize.y / p, maxSize.y);
            }
        }

        static public Vector2 vector3DConvert2DPlant(Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }

        static public Vector3 vector3DConvert2DPlant(Vector2 pant, Vector3 org)
        {
            return new Vector3(pant.x, org.y, pant.y);
        }

        static public Vector3 coor2DConvert3D(Vector3 a)
        {
            return new Vector3(a.x * 0.1f, a.y * 0.1f, a.z * 0.1f);
        }

        static public Vector3 coor3DConvert2D(Vector3 a)
        {
            return new Vector3(a.x * 10f, a.y * 10f, a.z * 10f);
        }

        static public int getQuadrant(float a)
        {
            while (a < 0 || a > 360)
            {
                if (a < 0)
                {
                    a += 360;
                }
                else if (a > 360)
                {
                    a -= 360;
                }
            }

            if (a > 0 && a <= 90)
            {
                return 1;
            }
            else if (a > 90 && a <= 180)
            {
                return 2;
            }
            else if (a > 180 && a <= 270)
            {
                return 3;
            }
            else if (a > 270 && a <= 360)
            {
                return 4;
            }
            return 1;
        }

        static public float reviseAngle180(float a)
        {
            a = reviseAngleIn360(a);
            if (a > 180)
            {
                a = 360 - a;
            }
            else if (a < -180)
            {
                a = 360 + a;
            }
            return a;
        }

        static public float reviseAngleIn360(float d)
        {
            if (d < 0f)
            {
                while (true)
                {
                    d += 360.0f;
                    if (d >= 0.0) break;
                }
            }
            else if (d >= 360.0f)
            {
                while (true)
                {
                    d -= 360.0f;
                    if (d < 360.0f) break;
                }
            }
            return d;
        }

        /// <summary>
        /// Determine the signed angle between two vectors, with normal 'n'
        /// as the rotation axis.
        /// </summary>
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public static float AngleSigned(Vector2 from, Vector2 to)
        {
            float angle;
            Vector3 cross = Vector3.Cross(from, to);
            angle = Vector2.Angle(from, to);
            return cross.z > 0 ? -angle : angle;
        }

        static public float angle2radian(float a)
        {
            return a * 0.017453f;
        }

        static public float radian2angle(float r)
        {
            return r * 57.29577f;
        }

        static public Vector3 reviseMousePos(Vector3 pos)
        {
            float dx = 800f / Screen.width;
            pos.x = pos.x * dx;
            pos.y = pos.y * dx;
            return pos;
        }

        static public string convertTo32(int num)
        {
            int toBase = 32;
            var str = "0123456789abcdefghijklmnopqrstuvwxyz";
            var numList = new List<char>();
            do
            {
                var remainder = num % toBase;
                numList.Add(str[remainder]);

                num = num / toBase;

                if (num != 0) continue;

                numList.Reverse();
                return new string(numList.ToArray());
            } while (true);
        }

        static public int getFolderFilesCount(string folderPath, string searchPattern = "*")
        {
            FileInfo[] files = IUtils.getFolderFile(folderPath, searchPattern);
            return files.Length;
        }

        static public void createFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        static public FileInfo[] getFolderFile(string folderPath, string searchPattern)
        {
            DirectoryInfo direction = new DirectoryInfo(folderPath);
            FileInfo[] files = direction.GetFiles(searchPattern, SearchOption.AllDirectories);
            return files;
        }

        static public void write2txt(string path, string data)
        {
            FileInfo fi = new FileInfo(path);
            StreamWriter sw = fi.CreateText();
            sw.Write(data);
            sw.Dispose();
            sw.Close();
        }

        static public string readFromTxt(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                Debug.Log("[Warning]Cannot open Data" + path);
                return null;
            }

            StreamReader sr = File.OpenText(path);
            string loadString = sr.ReadToEnd();
            sr.Dispose();
            sr.Close();
            return loadString;
        }

        static public byte[] readFromDll(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                Debug.Log("[Warning]Cannot open Data" + path);
                return null;
            }
            FileStream sr = File.OpenRead(path);
            byte[] bytsize = new byte[1024 * 1024 * 5];
            while (true)
            {
                //返回实际读取到的字节
                int r = sr.Read(bytsize, 0, bytsize.Length);
                //当字节位0的时候 证明已经读取结束
                if (r == 0)
                {
                    break;
                }
            }
            return bytsize;
        }

        static public JsonWriter keyValue2Writer(JsonWriter writer, string key, Block value)
        {
            return keyValue2Writer(writer, key, value == null ? -1 : value.getId());
        }

        static public JsonWriter keyValue2Writer(JsonWriter writer, string key, Vector3 value)
        {
            writer.WritePropertyName(key);
            writer.Write(vector3Serialize(value));
            return writer;
        }

        static public JsonWriter keyValue2Writer(JsonWriter writer, string key, bool value)
        {
            writer.WritePropertyName(key);
            if (value)
                writer.Write(1);
            else
                writer.Write(0);
            return writer;
        }

        static public JsonWriter keyValue2Writer(JsonWriter writer, string key, int value)
        {
            writer.WritePropertyName(key);
            writer.Write(value);
            return writer;
        }

        static public JsonWriter keyValue2Writer(JsonWriter writer, string key, string value)
        {
            writer.WritePropertyName(key);
            writer.Write(value);
            return writer;
        }

        static public JsonWriter keyValue2Writer(JsonWriter writer, string key, float value)
        {
            writer.WritePropertyName(key);
            writer.Write(value);
            return writer;
        }

        static public Block getJsonValue2Block(JsonData jsonData, string key)
        {
            int id = getJsonValue2Int(jsonData, key);
            if (id > 0)
            {
                return BlocksManager.instance.getBlockById(id);
            }
            return BlocksManager.instance.air;
        }

        static public Block getJsonValue2Block(JsonData jsonData, string key, Block error)
        {
            int id = getJsonValue2Int(jsonData, key);
            if (id > 0)
            {
                return BlocksManager.instance.getBlockById(id);
            }
            return error;
        }

        static public Vector3 getJsonValue2Vector3(JsonData jsonData, string key)
        {
            return vector3Parse(jsonData[key].ToString());
        }

        static public string getJsonValue2String(JsonData jsonData, string key)
        {
            try
            {
                return jsonData[key].ToString();
            }
            catch
            {
                return null;
            }
        }

        static public string getJsonValue2String(JsonData jsonData, string key, string defaul)
        {
            try
            {
                return jsonData[key].ToString();
            }
            catch
            {
                return defaul;
            }
        }

        static public bool getJsonValue2Bool(JsonData jsonData, string key)
        {
            try
            {
                return getJsonValue2String(jsonData, key).Equals("1");
            }
            catch
            {
                return false;
            }
        }

        static public bool getJsonValue2Bool(JsonData jsonData, string key, bool defaul)
        {
            try
            {
                return getJsonValue2String(jsonData, key).Equals("1");
            }
            catch
            {
                return defaul;
            }
        }

        static public int getJsonValue2Int(JsonData jsonData, string key)
        {
            try
            {
                return int.Parse(getJsonValue2String(jsonData, key));
            }
            catch
            {
                Debug.Log("[Warning] Can't Parse(" + getJsonValue2String(jsonData, key) + ")");
                return 0;
            }
        }

        static public int getJsonValue2Int(JsonData jsonData, string key, int errorInt)
        {
            try
            {
                return int.Parse(getJsonValue2String(jsonData, key));
            }
            catch
            {
                return errorInt;
            }
        }

        static public float getJsonValue2Float(JsonData jsonData, string key)
        {
            try
            {
                return float.Parse(getJsonValue2String(jsonData, key));
            }
            catch
            {
                return 0f;
            }
        }

        static public float getJsonValue2Float(JsonData jsonData, string key, float errorFloat)
        {
            try
            {
                return float.Parse(getJsonValue2String(jsonData, key));
            }
            catch
            {
                return errorFloat;
            }
        }
    }
}