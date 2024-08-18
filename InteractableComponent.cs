using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using EFT.InputSystem;
using EFT.Interactive;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ObjectPlacementTool
{
    internal class InteractableComponent : InteractableObject
    {

        public List<ActionsTypesClass> Actions = new List<ActionsTypesClass>();

        public void Init()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Interactive");

            Actions.AddRange(
                new List<ActionsTypesClass>()
                {
                    new ActionsTypesClass
                    {
                        Name = "Select / Unselect",
                        Action = () =>
                        {
                            if (Plugin.TargetObject == null)
                            {
                                Plugin.SelectObject(this.gameObject);
                            }
                            else if (Plugin.TargetObject == this.gameObject)
                            {
                                Plugin.UnselectObject();
                            }
                            else
                            {
                                Plugin.UnselectObject();
                                Plugin.SelectObject(this.gameObject);
                            }
                        }
                    },
                    new ActionsTypesClass
                    {
                        Name = "Rename (press escape to free cursor)",
                        Action = () =>
                        {
                            ShowNotesUI();
                            FillNotesText(this.gameObject.name);
                        }
                    },
                    new ActionsTypesClass
                    {
                        Name = "Delete",
                        Action = () =>
                        {
                            Plugin.UnselectObject();
                            this.gameObject.transform.position = new Vector3(0, 0, -9999);
                            Destroy(this.gameObject, 2);
                        }
                    }
                }
            );
        }

        public void ShowNotesUI()
        {
            Plugin.LockInput = true;
            Plugin.InventoryUI.ShowGameObject(true);
            Plugin.TasksUI.gameObject.SetActive(true);
            Plugin.NoteUIPanel.Show(UpdateName, EndRename);
        }

        public void FillNotesText(string text)
        {
            var tmpField = AccessTools.Field(typeof(NoteWindow), "_note");
            TMP_InputField tmp = tmpField.GetValue(Plugin.NoteUIPanel) as TMP_InputField;
            tmp.text = text;
            tmpField.SetValue(Plugin.NoteUIPanel, tmp);
        }

        public void UpdateName(string newName)
        {
            this.gameObject.name = newName;
            EndRename();
        }

        public void EndRename()
        {
            Plugin.NoteUIPanel.Hide();
            Plugin.TasksUI.gameObject.SetActive(false);
            Plugin.InventoryUI.gameObject.SetActive(false);
            Plugin.LockInput = false;
        }
    }
}
