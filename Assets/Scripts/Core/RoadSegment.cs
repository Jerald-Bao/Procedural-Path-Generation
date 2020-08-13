

using System;
using System.Collections.Generic;
using UnityEngine;
//We can simply generate all Segments as a Bezier Curve with a equivalent length for all the tangent of nodes.
//Cubic Bezier Curve Equation Reference: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
[ExecuteInEditMode]
public class RoadSegment: MonoBehaviour
{
    private static readonly float CurveElaborationCoefficient = 0.3f;
    private static Mesh2D _mesh2D;
    public static Mesh2D Mesh2D{
        get
        {
            if (_mesh2D == null || !_mesh2D)
            {
                _mesh2D=Resources.Load<Mesh2D>("Road Shape");
            }
            return _mesh2D;
        }
    }
    private static Material _roadMaterial;
    public static Material RoadMaterial{
        get
        {
            if (_roadMaterial == null || !_roadMaterial)
            {
                //_roadMaterial=Resources.Load<Material>("Road Material");
                _roadMaterial=Resources.Load<Material>("");
            }
            return _roadMaterial;
        }
    }
    private Vector3 HeadPos;
    private Vector3 TailPos;
    [SerializeField]
    private Transform head;
    [SerializeField]
    private Transform tail;
    [SerializeField]
    private float TextureTileScale=10f;
    [SerializeField]
    private float MeshTileScale=2f;
    private BezierCurve curve;
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    [SerializeField] [HideInInspector]
    private int ownerID;
    private Mesh mesh
    {
        get
        {
            if (_meshFilter==null)
                _meshFilter = GetComponent<MeshFilter>();
            
            bool isOwner = ownerID == gameObject.GetInstanceID();
            bool filterHasMesh = _meshFilter.sharedMesh != null;
            if (!isOwner || !filterHasMesh)
            {
                ownerID = gameObject.GetInstanceID(); // Mark self as owner of this mesh
                _mesh = _meshFilter.sharedMesh = new Mesh();
                _mesh.name = "Mesh [" + ownerID + "]";
                _mesh.hideFlags = HideFlags.HideAndDontSave; 
                _mesh.MarkDynamic();
            }else if( isOwner && filterHasMesh && _mesh== null ) {
                _mesh =_meshFilter.sharedMesh;
            }

            return _mesh;
        }
    }
    public float Length => (HeadPos - TailPos).magnitude;
    public float OffsetV;
    public void SetRoadSegment()
    {
        
        curve = new BezierCurve(head, tail, transform,(head.position - tail.position).magnitude /2 /*Hard - Code*/);
        HeadPos = head.position;
        TailPos = tail.position;
        GenerateMesh();
    }
    

    public void OnValidate()
    {
        SetRoadSegment();
    }

    public void Update()
    {
        if (head.hasChanged || tail.hasChanged)
            OnValidate();
    }


    public void GenerateMesh()
    {
        
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        // meshRenderer.sharedMaterial = new Material(RoadMaterial);
        
        //The Main part of Mesh Generation
        {
            mesh.Clear();
            int division = Mathf.CeilToInt(Length * (1 + curve.EstimatedCurvature * CurveElaborationCoefficient) * MeshTileScale);
            Vector3[] vertices = new Vector3[division * Mesh2D.VertexCount];
            int[] indices = new int[(division-1) * Mesh2D.VertexCount * 3 ];
            Vector3[] tangents = new Vector3[division * Mesh2D.VertexCount];
            Vector3[] normals = new Vector3[division * Mesh2D.VertexCount];
            Vector2[] uvs0 = new Vector2[division * Mesh2D.VertexCount];
            Vector2[] uvs1 = new Vector2[division * Mesh2D.VertexCount];
            float deltaT = 1f / (division-1);
            float t;
            var UVoffsetV = OffsetV;
            for (int i = 0; i < division; i++)
            {
                t = i * deltaT;
                var rootIndex = i * Mesh2D.VertexCount;
                //ensure there would be smooth and no gap  between segments
                if (i == division - 1)
                    t = 1;
                var pointRef = curve.GetPoint(t);
                
                // Generate vertices & normals
                for (int innerIndex = 0; innerIndex < Mesh2D.VertexCount; innerIndex++)
                {
                    Matrix4x4 matrix=Matrix4x4.Rotate(Quaternion.LookRotation(curve.GetTangent(t)));
                    var mesh2DVertex = Mesh2D.vertices[innerIndex];
                    var offset = (matrix * new Vector3(mesh2DVertex.point.x, mesh2DVertex.point.y, 0));
                    vertices[rootIndex + innerIndex] = pointRef + new Vector3(offset.x,offset.y,offset.z);
                    normals[rootIndex + innerIndex] = matrix * mesh2DVertex.normal;
                }
                
                // Generate UVs
                
                if (i != 0)
                    // We need to keep that middle lines are the same size for each tile
                {
                    //Find the mid point of the road
                    var currentMidPoint = (vertices[rootIndex] + vertices[rootIndex + Mesh2D.VertexCount - 1]) / 2;
                    var lastMidPoint = (vertices[rootIndex - Mesh2D.VertexCount] + vertices[rootIndex - 1]) / 2;
                    
                    UVoffsetV += Vector3.Distance(currentMidPoint, lastMidPoint) /
                                 TextureTileScale;
                }
                
                for (int innerIndex = 0; innerIndex < Mesh2D.VertexCount; innerIndex++)
                {
                    uvs0[rootIndex + innerIndex] = new Vector2(Mesh2D.vertices[innerIndex].u, UVoffsetV);
                }
                
                // Generate Triangles for Mesh
                var indexOffsetTri = i * Mesh2D.VertexCount * 3;
                if (i<division-1)
                    for (int innerIndex = 0; innerIndex < Mesh2D.VertexCount; innerIndex += 2)
                    {
                        // Generate a quad each loop
                        var indexTri = indexOffsetTri + innerIndex * 3;
                        int lineindexA = Mesh2D.lineIndices[innerIndex];
                        int lineindexB = Mesh2D.lineIndices[innerIndex + 1];
                        indices[indexTri] = rootIndex + lineindexB;
                        indices[indexTri + 1] = rootIndex+lineindexA;
                        indices[indexTri + 2] = rootIndex + lineindexB + Mesh2D.LineCount;
                        indices[indexTri + 3] = rootIndex + lineindexA + Mesh2D.LineCount;
                        indices[indexTri + 4] = rootIndex + lineindexB + Mesh2D.LineCount;
                        indices[indexTri + 5] = rootIndex + lineindexA;
                    }
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices,0);
            mesh.SetNormals(normals);
            mesh.SetUVs(0,uvs0);
            mesh.SetUVs(1,uvs1);
        }
    }
    public void OnDrawGizmosSelected()
    {
        var vertices = new List<Vector3>();
        mesh.GetVertices(vertices);
        foreach (var vertex in vertices)
        {
            Gizmos.DrawSphere(transform.position + vertex,0.1f);
        }
    }
}