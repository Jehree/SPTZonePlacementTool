using Comfort.Common;
using EFT;
using EFT.CameraControl;
using EFT.HealthSystem;
using EFT.InputSystem;
using EFT.Interactive;
using EFT.UI;
using HarmonyLib;
using RootMotion.Demos;
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
using ZonePlacementTool.Helpers;

namespace ZonePlacementTool
{
    public class InteractableComponent : InteractableObject
    {

        public List<ActionsTypesClass> Actions = new List<ActionsTypesClass>();
        public GameObject Parent { get; private set; }

        public void Init()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Interactive");
            Parent = this.gameObject.transform.parent.gameObject;

            Actions.AddRange(
                new List<ActionsTypesClass>()
                {
                    new ActionsTypesClass
                    {
                        Name = "Select / Unselect",
                        Action = () =>
                        {
                            if (Plugin.TargetInteractableComponent == null)
                            {
                                Plugin.SelectObject(this.gameObject);
                            }
                            else if (Plugin.TargetInteractableComponent == this)
                            {
                                Plugin.UnselectObject();
                            }
                            else
                            {
                                Plugin.UnselectObject(mute:true);
                                Plugin.SelectObject(this.gameObject);
                            }
                        }
                    },
                    new ActionsTypesClass
                    {
                        Name = "Move To Player Feet",
                        Action = ResetTranslation
                    },
                    new ActionsTypesClass
                    {
                        Name = "Reset Scale",
                        Action = ResetScale
                    },
                    new ActionsTypesClass
                    {
                        Name = "Reset Rotation",
                        Action = ResetRotation
                    },
                    new ActionsTypesClass
                    {
                        Name = "Face Camera Direction",
                        Action = MatchPlayerYRotation
                    },
                    new ActionsTypesClass
                    {
                        Name = "Delete",
                        Action = Delete
                    }
                }
            );
        }

        public void ResetRotation()
        {
            Parent.transform.rotation = Quaternion.identity;
            this.gameObject.transform.rotation = Quaternion.identity;
            Plugin.MapData.Save();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOff);
        }

        public void MatchPlayerYRotation()
        {
            Vector3 cameraEulerAngles = Plugin.Player.CameraPosition.rotation.eulerAngles;
            Vector3 newObjectEulerAngles = new Vector3(0, cameraEulerAngles.y, 0);
            Parent.transform.rotation = Quaternion.Euler(newObjectEulerAngles);
            Plugin.MapData.Save();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOff);
        }

        public void ResetTranslation()
        {
            Parent.transform.position = Plugin.Player.Transform.position;
            Plugin.MapData.Save();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOff);
        }

        public void ResetScale()
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
            Plugin.MapData.Save();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOff);
        }

        public void Delete()
        {
            Plugin.UnselectObject(mute:true);
            MapDataUtils.RemoveObjectData(GetName());
            Plugin.AllInteractableComponents.Remove(this);
            Plugin.MapData.Save();
            Settings.SelectedObjectName.Value = "";

            this.gameObject.transform.position = new Vector3(0, 0, -9999);
            Destroy(this.gameObject, 3);
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);
        }
         
        public void TranslateMe(string axis, float amount)
        {
            Vector3 translation = new Vector3(0, 0, 0);

            switch (axis)
            {
                case "x": translation = new Vector3(amount, 0, 0); break;
                case "y": translation = new Vector3(0, amount, 0); break;
                case "z": translation = new Vector3(0, 0, amount); break;
            }

            Parent.transform.Translate(translation, Space.Self);
        }

        public void ScaleMe(string axis, float amount)
        {
            Vector3 scaleAmount = new Vector3(0, 0, 0);

            switch (axis)
            {
                case "x": scaleAmount = new Vector3(amount, 0, 0); break;
                case "y": scaleAmount = new Vector3(0, amount, 0); break;
                case "z": scaleAmount = new Vector3(0, 0, amount); break;
            }

            this.gameObject.transform.localScale = this.gameObject.transform.localScale + scaleAmount;
        }

        public void RotateMe(string axis, float amount)
        {
            Vector3 rotation = new Vector3(0, 0, 0);

            switch (axis)
            {
                case "x": rotation = new Vector3(0, amount, 0); break;
                case "y": rotation = new Vector3(0, 0, amount); break;
                case "z": rotation = new Vector3(amount, 0, 0); break;
            }

            if (axis == "x")
            {
                Parent.transform.localRotation = Parent.transform.localRotation * Quaternion.Euler(rotation);
            }
            else if (!Settings.LockXAndZRotation.Value)
            {
                this.gameObject.transform.localRotation = this.gameObject.transform.localRotation * Quaternion.Euler(rotation);
            }
            
        }

        public void SetColor(Color color)
        {
            this.gameObject.GetComponent<Renderer>().material.color = color;
        }

        public void SetName(string name)
        {
            this.gameObject.name = name;
            Parent.name = name + "_parent";
        }

        public string GetName()
        {
            return this.gameObject.name;
        }

        public Vector3 GetPosition()
        {
            return Parent.transform.position;
        }

        public Vector3 GetScale()
        {
            return this.gameObject.transform.localScale;
        }

        public Quaternion GetRotation()
        {
            return this.gameObject.transform.rotation;
        }

        public static void Spawn()
        {
            if (Settings.SelectedObjectName.Value == "")
            {
                ConsoleScreen.LogError($"{Plugin.MOD_NAME}: You must give your new zone object a name!");
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ErrorMessage);
                return;
            }

            if (MapDataUtils.ObjectDataExists(Settings.SelectedObjectName.Value))
            {
                ConsoleScreen.LogError($"{Plugin.MOD_NAME}: An object by the name of {Settings.SelectedObjectName.Value} already exists!");
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ErrorMessage);
                return;
            }

            GameObject parent = new GameObject();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<Renderer>().enabled = true;
            cube.transform.parent = parent.transform;
            cube.AddComponent<InteractableComponent>();
            InteractableComponent interactableComponent = cube.GetComponent<InteractableComponent>();
            interactableComponent.Init();
            interactableComponent.ResetTranslation();
            interactableComponent.MatchPlayerYRotation();
            interactableComponent.SetName(Settings.SelectedObjectName.Value);

            Plugin.UnselectObject();
            Plugin.SelectObject(cube, mute:true);
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.InsuranceInsured);

            ObjectData objectData = MapDataUtils.CreateObjectData(interactableComponent);
            MapDataUtils.AddObjectData(objectData);
            Plugin.AllInteractableComponents.Add(interactableComponent);
        }

        public static void ForceSpawn(ObjectData data)
        {
            GameObject parent = new GameObject();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = parent.transform;

            parent.name = data.Name + "_parent";
            parent.transform.position = data.Position;
            parent.transform.rotation = data.ParentRotation;

            cube.transform.rotation = data.ChildRotation;
            cube.transform.localScale = data.Scale;

            cube.AddComponent<InteractableComponent>();
            InteractableComponent interactableComponent = cube.GetComponent<InteractableComponent>();
            interactableComponent.Init();
            interactableComponent.SetName(data.Name);

            Renderer renderer = interactableComponent.GetComponent<Renderer>();
            renderer.enabled = true;
            renderer.material.color = Color.magenta;

            Plugin.AllInteractableComponents.Add(interactableComponent);
        }

        public static Material GetTransparentMaterial(Color color, float transparency)
        {
            Color transparentColor = new Color(color.r, color.g, color.b, transparency);

            // Create a new material with the Standard Shader
            Material transparentMaterial = new Material(Shader.Find("Standard"));

            // Set the material to use transparency
            transparentMaterial.SetFloat("_Mode", 3); // 3 is the mode for Transparent
            transparentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            transparentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            transparentMaterial.SetInt("_ZWrite", 0);
            transparentMaterial.DisableKeyword("_ALPHATEST_ON");
            transparentMaterial.EnableKeyword("_ALPHABLEND_ON");
            transparentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            transparentMaterial.renderQueue = 3000; // Ensure it's rendered after opaque objects
            transparentMaterial.SetFloat("_Glossiness", 0f);


            // Set the color with alpha for transparency
            transparentMaterial.color = transparentColor;

            return transparentMaterial;
        }

        public static Color GetTransparentColor(Color color, float transparency)
        {
            return new Color(color.r, color.g, color.b, transparency);
        }
    }
}
