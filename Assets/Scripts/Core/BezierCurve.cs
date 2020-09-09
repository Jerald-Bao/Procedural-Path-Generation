using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BezierCurve
{
    Vector3[] points;
    public float tangent;
    public float EstimatedCurvature;
    public float ConstraintCurvature;
    public Transform local;
    public BezierCurve(Transform head, Transform tail,Transform local,float tangent,float constraintCurvature = 5f)
    {
        this.tangent = tangent;
        points=new Vector3[4];
        points[0] =  local.InverseTransformPoint(head.position); 
        points[1] =  local.InverseTransformDirection(head.forward) *tangent ;
        points[1] += local.InverseTransformPoint(head.position); 
        points[2] =  -local.InverseTransformDirection(tail.forward) * tangent;
        points[2] += local.InverseTransformPoint(tail.position); 
        points[3] = local.InverseTransformPoint(tail.position);
        EstimatedCurvature = 1 - Vector3.Dot(head.forward , tail.forward);    // TO-DO 
        ConstraintCurvature = constraintCurvature;
        this.local = local;
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

    public void DrawGizmo()
    {
        Gizmos.color=Color.red;
        Gizmos.matrix = local.localToWorldMatrix;
        Gizmos.DrawSphere(points[0],0.2f);
        Gizmos.DrawSphere(points[1],0.2f);
        Gizmos.DrawSphere(points[2],0.2f);
        Gizmos.DrawSphere(points[3],0.2f);


    }
    
}
