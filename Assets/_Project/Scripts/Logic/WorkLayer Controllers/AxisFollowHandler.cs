using System;
using UnityEngine;

public class AxisFollowHandler : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private bool isArrows;

    void Start()
    {
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = target.position;
        if (isArrows)
        {
            transform.rotation = Quaternion.identity;   
        }
    }
}
