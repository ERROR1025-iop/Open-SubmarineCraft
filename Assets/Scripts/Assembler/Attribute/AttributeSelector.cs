using System.Collections.Generic;
using Battlehub.RTHandles;
using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{

    public class AttributeSelector : MonoBehaviour
    {
        public static AttributeSelector instance;

        public static ISwitchImageTextButton multButton;

        Sprite[] btnSprites;

        void Start()
        {
            instance = this;

            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onDeleteButtonClick);
            transform.GetChild(2).GetComponent<Button>().onClick.AddListener(onMultSelectButtonClick);
            transform.GetChild(3).GetComponent<Button>().onClick.AddListener(onCancelSelectButtonClick);
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(onCancelMirrorButtonClick);
            transform.GetChild(5).GetComponent<Button>().onClick.AddListener(onGroupButtonClick);
            transform.GetChild(6).GetComponent<Button>().onClick.AddListener(onUngroupButtonClick);
            transform.GetChild(7).GetComponent<Button>().onClick.AddListener(onParentButtonClick);
            transform.GetChild(8).GetComponent<Button>().onClick.AddListener(onUnParentButtonClick);

            btnSprites = Resources.LoadAll<Sprite>("assembler/select-contor");
            multButton = transform.GetChild(2).GetComponent<ISwitchImageTextButton>();

            RuntimeSelectionComponent.Multiselect = false;
        }


        public void onDeleteButtonClick()
        {
            IRT.deleteGameObjects(IRT.Selection.gameObjects);
            cancelSelect(false);
        }

        void onMultSelectButtonClick()
        {
            RuntimeSelectionComponent.Multiselect = !RuntimeSelectionComponent.Multiselect;
            updateMultButtonImage();
        }

        public void cancelMultSelect()
        {
            RuntimeSelectionComponent.Multiselect = false;
            updateMultButtonImage();
        }

        public void updateMultButtonImage()
        {
            multButton.change(btnSprites[RuntimeSelectionComponent.Multiselect ? 6 : 3], multButton.showString);
        }

        public void onCancelSelectButtonClick()
        {
            cancelSelect();
        }

        public void cancelSelect(bool undo = true)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject selectedObj = selection[i];
                if (selectedObj != null && selectedObj.activeSelf)
                {
                    Dpart dpart = selectedObj.GetComponent<DpartParent>().getDpart();
                    dpart.isSelecting = false;
                }
            }

            IRT.Undo.Enabled = undo;
            IRT.Selection.activeObject = null;
            IRT.Undo.Enabled = true;
            Assembler.instance.dpartAttribute.show(false);
            cancelMultSelect();
            AttributeAlign.instance.show(false);
        }

        void onCancelMirrorButtonClick()
        {
            AssemblerInput.changeSelectDpart(go =>
            {
                go.GetComponent<DpartParent>().getDpart().cancelMirrorDpart();
            });
        }

        void onGroupButtonClick()
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            GroupDpart groupDpart = new GroupDpart(-1, World.dpartParentObject);
            groupDpart.initGroupDpart("new_group");

            Vector3 centerPoint = IUtils.centerOfGameObjects(selection);
            groupDpart.getTransform().localPosition = centerPoint;

            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject selectedObj = selection[i];
                if (selectedObj != null && selectedObj.activeSelf)
                {
                    Dpart dpart = selectedObj.GetComponent<DpartParent>().getDpart();
                    Assembler.dpartsEngine.dpartArr[dpart.getIdentifyId()] = null;
                    groupDpart.addGroupChildrens(dpart);
                    dpart.getTransform().SetParent(groupDpart.getTransform());
                    dpart.setGroupDpart(groupDpart);
                    dpart.cancelMirrorDpart();
                }
            }

            Assembler.dpartsEngine.addDpartArr(groupDpart);
            IRT.Selection.objects = null;
            IRT.Selection.activeObject = groupDpart.getGameObject();
            groupDpart.isSelecting = true;
            DpartAttribute.instance.show(true);
        }

        void onUngroupButtonClick()
        {
            if (IRT.Selection.activeGameObject != null)
            {
                GroupDpart groupDpart = IRT.Selection.activeGameObject.GetComponent<DpartParent>().getDpart() as GroupDpart;
                if (groupDpart != null)
                {
                    int count = groupDpart.groupChildrens.Count;
                    GameObject[] gos = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        Dpart dpart = groupDpart.groupChildrens[i];
                        dpart.getTransform().SetParent(World.dpartParentObject.transform, true);
                        dpart.setGroupDpart(null);
                        Assembler.dpartsEngine.addDpartArr(dpart);
                        gos[i] = dpart.getGameObject();
                    }

                    Assembler.dpartsEngine.dpartArr[groupDpart.getIdentifyId()] = null;
                    groupDpart.setOutline(false);
                    groupDpart.groupChildrens.Clear();
                    groupDpart.cancelMirrorDpart();
                    groupDpart.clear();

                    IRT.Selection.objects = gos;
                }
            }
        }

        void onParentButtonClick()
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null || selection.Length < 2)
            {
                return;
            }

            Dpart parent = selection[0].GetComponent<DpartParent>().getDpart();
            List<int> linkedUid = new List<int>();
            for (int i = 1; i < selection.Length; ++i)
            {
                GameObject selectedObj = selection[i];
                if (selectedObj != null && selectedObj.activeSelf)
                {
                    Dpart dpart = selectedObj.GetComponent<DpartParent>().getDpart();
                    linkedUid.Add(dpart.uid);
                    if (dpart.linkedUid.Contains(parent.uid)){
                        dpart.linkedUid.Remove(parent.uid);
                    }
                }
            }            
            
            parent.linkedUid = linkedUid;
            parent.drawArc = false;
            parent.DrawLinkedLine();
        }

        void onUnParentButtonClick()
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null || selection.Length <= 0)
            {
                return;
            }

            Dpart parent = selection[0].GetComponent<DpartParent>().getDpart();
            if(selection.Length < 2)
            {
                parent.linkedUid.Clear();
            }
            else
            {                
                for (int i = 1; i < selection.Length; ++i)
                {
                    GameObject selectedObj = selection[i];
                    if (selectedObj != null && selectedObj.activeSelf)
                    {
                        Dpart dpart = selectedObj.GetComponent<DpartParent>().getDpart();                    
                        if (parent.linkedUid.Contains(dpart.uid)){
                            parent.linkedUid.Remove(dpart.uid);
                        }
                    }
                }
            }            
            parent.drawArc = false;
            parent.DrawLinkedLine();
        }
    }
}
