using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour
{
    public RoadSegment segmentForward;
    public RoadSegment segmentBackward;
    public BoxCollider colliderForward; 
    public BoxCollider colliderBackward; 
    public void Init()
    {
        var colliders=GetComponents<BoxCollider>();
        if (colliders.Length < 0)
            gameObject.AddComponent<BoxCollider>();
    }

    public void GetDirectionFromCollider(Collider collider,out bool forward)
    {    
        forward = true;    
        if (collider == colliderForward)
            forward = true;
        else if (collider == colliderBackward)
            forward = false;
        else
        {
            Debug.LogError("The input Collider is not belong to this node");
        }
    }

    public void OnValidate()
    {
        OnStateChanged();
    }

    public void OnStateChanged()
    {
        UpdateColliders();
    }

    public void UpdateColliders()
    {
        colliderForward.enabled = (segmentForward == null || !segmentForward);
        colliderBackward.enabled = (segmentBackward == null || !segmentBackward);
    }

    public void SetColliderEnable(bool enabled,bool forward)
    {
        if (forward)
            colliderForward.enabled = enabled;
        else
            colliderBackward.enabled = enabled;
    }
}
