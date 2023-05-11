using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float speed;

    private bool _isGoingToEnd = true;
    private float _progress = 0;

    // Update is called once per frame
    void Update()
    {
        _progress += _isGoingToEnd ? Time.deltaTime * speed : -Time.deltaTime * speed;
        _progress = Mathf.Clamp01(_progress);
        
        
    }
}
