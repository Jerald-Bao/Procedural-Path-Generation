using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BezierCurve
{
    Vector3[] points;
    public float tangent;
    public float EstimatedCurvature;

    public BezierCurve(Transform head, Transform tail,Transform local,float tangent)
    {
        this.tangent = tangent;
        points=new Vector3[4];
        points[0] =  head.localPosition;
        points[1] =  head.localRotation * Vector3.forward *tangent ;
        points[1] += head.localPosition; 
        points[2] =  tail.localRotation * -Vector3.forward * tangent;
        points[2] += tail.localPosition; 
        points[3] = tail.localPosition;
        EstimatedCurvature = 1 - Vector3.Dot(head.forward , tail.forward);    // TO-DO 
    }
    public Vector3 GetPoint(float t)
    {
        Vector3 a = Vector3.Lerp( points[0], points[1], t );
        Vector3 b = Vector3.Lerp( points[1], points[2], t );
        Vector3 c = Vector3.Lerp( points[2], points[3], t );
        Vector3 d = Vector3.Lerp( a, b, t );
        Vector3 e = Vector3.Lerp( b, c, t );
        return Vector3.Lerp( d, e, t );
    }
    public Vector3 GetTangent(float t)
    {
        Vector3 a = Vector3.Lerp( points[0], points[1], t );
        Vector3 b = Vector3.Lerp( points[1], points[2], t );
        Vector3 c = Vector3.Lerp( points[2], points[3], t );
        Vector3 d = Vector3.Lerp( a, b, t );
        Vector3 e = Vector3.Lerp( b, c, t );
        return ( e - d ).normalized;
    }
}
