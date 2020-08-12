using System.Collections.Generic;
using UnityEngine;

public class RoadChain
{ 
    public static Vector3 CurvatureRadiusMin=new Vector3(3,3,3);
    public List<Transform> Nodes;

    private float length;
    public float Length => length;

    public float CalcuLength()
    {
        if (Nodes == null || Nodes.Count < 2)
            return 0;
        var res = 0f;
        for (int i = 0; i < Nodes.Count - 1; i++)
        {
            res += CalcuSegment(Nodes[i], Nodes[i + 1]).Length;
        }
        return res;
    }

    public RoadSegment CalcuSegment(Transform head, Transform tail)
    {
        
        return null;
    }

}
