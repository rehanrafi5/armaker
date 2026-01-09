namespace ARMarker
{

    using UnityEngine;

    /// <summary>
    /// The parent of any class intended to function as a singleton and
    /// supposed to be persistent (stays even if Scene has changed). Handles
    /// Singleton declaration/initialization + duplicates eradication.
    /// </summary>
    /// <typeparam name="T">The child class to be made into a 
    /// persistent singleton at runtime.</typeparam>
    public abstract class BaseSingleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {

        #region Singleton Instance

        private static readonly object s_lock = new object();
        private static T s_instance;
        public static T Instance
        {
            get
            {
                lock (s_lock)
                {
                    if (s_instance == null)
                    {
                        s_instance = FindObjectOfType<T>();
                    }

                    return s_instance;
                }
            }
        }

        #endregion

        [SerializeField]
        private bool destroyDupeObject = false;

        [SerializeField]
        private bool persistOnSceneChange = true;

        protected virtual void Awake()
        {
            InitSingleton();
        }

        #region Class Implementation

        private void InitSingleton()
        {
            if (Instance.GetInstanceID() == GetInstanceID())
            {
                if (persistOnSceneChange)
                {
                    DontDestroyOnLoad(gameObject);
                    Debug.Log($"{GetType().Name} Singleton created! " +
                    $"Persisting through scene change.", gameObject);
                }
                else 
                {
                    Debug.Log($"{GetType().Name} Singleton created! " +
                        $"But not persisting through scene change", 
                        gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}.{GetType().Name}.Awake(): " +
                    $"Cannot have >1 Instances of this class. " +
                    $"destroyDupeObject? {destroyDupeObject}");

                if (destroyDupeObject)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(this);
                }
            }
        }

        #endregion

    }

}