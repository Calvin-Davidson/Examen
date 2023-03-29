using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ObjectMover : MonoBehaviour
{
    [SerializeField] private bool localSpace = true;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private float lerpTime;

    private Vector3 currentOffset;

    void Awake()
    {
        currentOffset = this.transform.position;
    }

    void MoveOn()
    {

    }

    void MoveOff()
    {

    }
}
