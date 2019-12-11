using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float RotationSpeed = 5;
    public float MovementSpeed = 1;
    public float ScaleSpeed = 1;

    private Vector3 _center = Vector3.zero;
    private Vector3 _destScale = Vector3.one;

    private Controls _controls = null;

    // Update is called once per frame
    void Update()
    {
        _controls = FindObjectOfType<Controls>();
        transform.Rotate(Vector3.up, Time.deltaTime * RotationSpeed, Space.World);
        transform.position = Vector3.Lerp(transform.position, _center, Time.deltaTime * _controls.GetSpeed() * MovementSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, _destScale, Time.deltaTime * _controls.GetSpeed() * ScaleSpeed);
    }

    public void SetCenter(Vector3 center)
    {
        _center = center;
    }

    public void SetScale(int size)
    {
        float scale = Mathf.Max(size / 2.5f, 1);
        _destScale = Vector3.one * scale;
    }
}
