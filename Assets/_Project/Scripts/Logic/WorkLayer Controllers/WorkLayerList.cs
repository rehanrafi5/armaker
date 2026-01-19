// using System.Collections.Generic;
// using UnityEngine;
//
// namespace ARMarker
// {
//     public class WorkLayerList : MonoBehaviour
//     {
//         [SerializeField] private WorkLayerListItemUI itemPrefab;
//         [SerializeField] private Transform contentParent;
//
//         private readonly Dictionary<WorkLayer, WorkLayerListItemUI> items = new();
//
//         private void OnEnable()
//         {
//             // Subscribe
//             WorkSpaceSingleton.Instance.RegisterOnNewLayerAdded(OnLayerAdded);
//             WorkSpaceSingleton.Instance.RegisterOnLayerRemoved(OnLayerRemoved);
//             
//             // Rebuild UI every time the panel becomes active
//             RebuildList();
//         }
//
//         private void OnDisable()
//         {
//             WorkSpaceSingleton.Instance.RegisterOnNewLayerAdded(OnLayerAdded, true);
//             WorkSpaceSingleton.Instance.RegisterOnLayerRemoved(OnLayerRemoved, true);
//         }
//
//         private void RebuildList()
//         {
//             // Clear everything
//             foreach (Transform c in contentParent)
//                 Destroy(c.gameObject);
//
//             items.Clear();
//
//             // Rebuild from current layers
//             foreach (var layer in WorkSpaceSingleton.Instance.GetLayers())
//                 CreateItem(layer);
//         }
//
//         private void OnLayerAdded(WorkLayer layer)
//         {
//             if (!gameObject.activeInHierarchy) return;
//             CreateItem(layer);
//         }
//
//         private void OnLayerRemoved(WorkLayer layer)
//         {
//             if (!gameObject.activeInHierarchy) return;
//             if (layer == null) return;
//
//             if (items.TryGetValue(layer, out var item))
//             {
//                 Destroy(item.gameObject);
//                 items.Remove(layer);
//             }
//         }
//
//         private void CreateItem(WorkLayer layer)
//         {
//             if (layer == null || items.ContainsKey(layer)) return;
//
//             var item = Instantiate(itemPrefab, contentParent);
//             item.SetLayer(layer);
//             items[layer] = item;
//         }
//     }
// }
