using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public float mouseDragSpeed;
    public void LateUpdate()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        var deltaX = Input.GetAxisRaw("Mouse X");
        var deltaY = Input.GetAxisRaw("Mouse Y");
        
        var sa = Vector3.SignedAngle(Vector3.up, transform.forward, transform.right);
        if (!Mathf.Approximately(deltaX, 0) || !Mathf.Approximately(deltaY, 0))
        {
            transform.Rotate(Vector3.up, deltaX * mouseDragSpeed * Time.deltaTime, Space.World);

            var r = deltaY * -1 * mouseDragSpeed * Time.deltaTime;
            var a = sa + r;
            transform.Rotate(transform.right, r, Space.World);
        }
        
        var deltaPosX = Input.GetAxisRaw("Horizontal");
        var deltaPosZ = Input.GetAxisRaw("Vertical");
        transform.Translate(deltaPosX,0,deltaPosZ);
    }
}
