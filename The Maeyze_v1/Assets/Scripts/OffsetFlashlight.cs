using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetFlashlight : MonoBehaviour
{
    [SerializeField] private Vector3 localOffset = new Vector3(0.78f, -0.52f, 0.58f);
    [SerializeField] private Vector3 localEuler = Vector3.zero;
    [SerializeField] private float speed = 10f;   // mas mababa = mas malaki ang delay

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.position = cam.TransformPoint(localOffset);

        Quaternion target = cam.rotation * Quaternion.Euler(localEuler);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
    }
}