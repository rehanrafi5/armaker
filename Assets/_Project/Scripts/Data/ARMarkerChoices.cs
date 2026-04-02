using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace ARMarker
{

    [CreateAssetMenu(
        menuName = ConstantStrings.MENU_ROOT + "Create AR Marker Choice List")]
    public class ARMarkerChoices : ScriptableObject
    {
        [SerializeField] private XRReferenceImageLibrary TheLibrary;
        public List<ARImageUIData> MarkerData = new List<ARImageUIData>();
        
        [SerializeField]
        private List<Sprite> choices = new();

        public List<Sprite> Choices => choices;

        [ContextMenu("Auto Fill From Library")]
        public void AutoFill()
        {
            MarkerData.Clear();

            string[] searchFolders = new string[]
            {
                "Assets/_Project/Textures/_Markers"
            };

            for (int i = 0; i < TheLibrary.count; i++)
            {
                var refImage = TheLibrary[i];
                string imageName = refImage.name;

                // Search ONLY inside given folder
                string[] guids = AssetDatabase.FindAssets(
                    $"\"{imageName}\" t:Sprite",
                    searchFolders
                );

                if (guids.Length == 0)
                {
                    Debug.LogWarning($"Sprite not found for: {imageName}");
                    continue;
                }

                if (guids.Length > 1)
                {
                    Debug.LogWarning($"Multiple sprites found for: {imageName}, using first.");
                }

                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                MarkerData.Add(new ARImageUIData
                {
                    imageName = imageName,
                    previewSprite = sprite
                });
            }

            Debug.Log("Auto Fill Complete");
        }
    }
    [Serializable]
    public class ARImageUIData
    {
        public string imageName;
        public Sprite previewSprite;
    }
}