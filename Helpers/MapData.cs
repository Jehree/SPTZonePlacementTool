using System;
using System.CodeDom;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using System.Configuration.Assemblies;

namespace ZonePlacementTool.Helpers
{
    public class ObjectData
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("Position")]
        public UnityEngine.Vector3 Position;
        [JsonProperty("Rotation")]
        public UnityEngine.Quaternion Rotation;
        [JsonProperty("Scale")]
        public UnityEngine.Vector3 Scale;
        [JsonProperty("ParentRotation")]
        public Quaternion ParentRotation;
        [JsonProperty("ChildRotation")]
        public Quaternion ChildRotation;
    }

    public class MapData
    {
        [JsonProperty("MapID")]
        public string MapID;
        [JsonProperty("Objects")]
        public List<ObjectData> Objects;

        public MapData(string mapId)
        {
            MapID = mapId;
            Objects = new List<ObjectData>();
        }

        public static MapData GetDataFromJson(string json)
        {
            return JsonConvert.DeserializeObject<MapData>(json);
        }

        public static string CreateJsonFromMapData(MapData mapData)
        {
            return JsonConvert.SerializeObject(mapData, Formatting.Indented);
        }

        public static string GetPathByMapID(string mapId)
        {
            string locationsPath = Path.Combine(Path.GetDirectoryName(Plugin.AssemblyPath), "Locations");
            string filePath = Path.Combine(locationsPath, $"{mapId}.json");
            return filePath;
        }

        public void Save()
        {
            string json = CreateJsonFromMapData(this);
            string filePath = GetPathByMapID(this.MapID);
            File.WriteAllText(filePath, json);
        }
    }

    public class MapDataUtils : MonoBehaviour
    {

        public static bool ObjectDataExists(string name)
        {
            foreach (ObjectData obj in Plugin.MapData.Objects)
            {
                if (obj.Name == name) return true;
            }
            return false;
        }

        public static ObjectData GetObjectData(string name)
        {
            foreach (ObjectData obj in Plugin.MapData.Objects)
            {
                if (obj.Name == name) return obj;
            }
            return null;
        }

        public static ObjectData CreateObjectData(InteractableComponent component)
        {
            var objectData = new ObjectData();
            objectData.Position = component.GetPosition();
            objectData.Rotation = component.GetRotation();
            objectData.Scale = component.GetScale();
            objectData.Name = component.GetName();
            objectData.ParentRotation = component.Parent.transform.rotation;
            objectData.ChildRotation = component.gameObject.transform.rotation;
            return objectData;
        }

        public static void AddObjectData(ObjectData obj)
        {
            Plugin.MapData.Objects.Add(obj);
        }

        public static void UpdateObjectData(string oldName, InteractableComponent component)
        {

            ObjectData oldData = GetObjectData(oldName);
            RemoveObjectData(oldName);
            AddObjectData(CreateObjectData(component));
        }

        public static void RemoveObjectData(string name)
        {
            ObjectData data = GetObjectData(name);
            if (data != null)
            {
                Plugin.MapData.Objects.Remove(data);
            }
        }
    }

}
