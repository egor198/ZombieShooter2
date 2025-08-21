using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimMel : MonoBehaviour
{
    [Tooltip("Скорость вращения в градусах в секунду")]
    public float rotationSpeed = 30f;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
