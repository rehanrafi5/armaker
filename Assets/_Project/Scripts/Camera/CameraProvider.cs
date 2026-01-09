// using UnityEngine;
// public class CameraProvider : MonoBehaviour
// {
//     public static Camera Main { get; private set; }
//
//     private void Awake()
//     {
//         if (Main == null)
//         {
//             Main = Camera.main;
//             DontDestroyOnLoad(Main.gameObject);
//         }
//     }
// }