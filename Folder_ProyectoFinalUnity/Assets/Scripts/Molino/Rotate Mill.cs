using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMill : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public GameObject mill;
    private Vector3 fixedRotation;

    void Start()
    {
        fixedRotation = new Vector3(mill.transform.localEulerAngles.x, 90, -90);
    }

    void Update()
    {
        fixedRotation.x = fixedRotation.x + rotationSpeed * Time.deltaTime;
        mill.transform.localEulerAngles = fixedRotation;
    }
}
