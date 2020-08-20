using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoadChainGenerator : MonoBehaviour
{
    public float maxLength=10f;
    public Material matValid;
    public Material matInvalid;
    public float maxSlope = 40f;
    public float maxCurvature = 2f;
    public float minLength = 2f;
    public float maxRayCastDistance = 100f;
    private NodeType nodeType=NodeType.None;
    private State state=State.Idle;
    private RoadSegment blueprint;
    private RoadNode currentNode;
    private Transform undockedNode;
    public GameObject RoadSegmentPrefab;
    private static Vector3 UnattachedNodeRotation;
    public void Update()
    {
        RoadNode node;
        bool isForward;
        switch (state)
        {
            case State.Idle:
                if (TryGetAnyNode(out node, out isForward))
                {
                    currentNode = node;
                    OnPointToAvaliableNode(node, isForward);
                }
                break;
            case State.Pointing:
                if (TryGetAnyNode(out node, out isForward))
                {
                    bool isSameNode = node == currentNode;
                    bool isSameDirection = nodeType == GetNodeTypeByColliderDirection(isForward);
                    nodeType = GetNodeTypeByColliderDirection(isForward);
                    
                    if (!(isSameNode && isSameDirection))
                        OnPointToAvaliableNode(node, isForward);
                    if (Input.GetMouseButtonDown(0))
                    {
                        state = State.AttachingOneSide;
                        if (isForward)
                        {
                            node.segmentForward = blueprint;
                        }
                        else
                        {
                            node.segmentBackward = blueprint;
                        }
                        node.UpdateColliders();
                    }
                }
                else
                {
                    blueprint.DestroySelf();
                    state = State.Idle;
                }
                break;
            case State.AttachingOneSide:
                if (TryGetAnyNode(out node, out isForward))
                {
                    bool isValidDirection = nodeType != GetNodeTypeByColliderDirection(isForward);
                    if (isValidDirection)
                    {
                        blueprint.SetNode(node,!isForward);
                        state = State.AttachingBothSide;
                    }
                }
                else
                {
                    UpdateUndockedNodeRotation();
                    SetUndockedNodeTransform();
                    blueprint.SetNode(undockedNode,nodeType!=NodeType.Tail);
                }
                break;
        }
    }

    public void UpdateUndockedNodeRotation()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        //if (Input.GetKey(KeyCode.LeftControl))
        {
            UnattachedNodeRotation =
                new Vector3(UnattachedNodeRotation.x, UnattachedNodeRotation.y + scroll, UnattachedNodeRotation.z);
        }
    }

    public bool TryGetAnyNode(out RoadNode node,out bool isForward)
    {
        isForward = true;
        node = null;
        var ray=GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        var hit=Physics.Raycast(ray,out var hitInfo, maxRayCastDistance, 1 << LayerMask.NameToLayer("RoadNode"));
        if (hit)
        {
            node = hitInfo.transform.GetComponent<RoadNode>();
            node.GetDirectionFromCollider(hitInfo.collider, out isForward);
        }
        return hit;    
    }
    public void SetUndockedNodeTransform()
    {
        var ray=GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        var hit=Physics.Raycast(ray,out var hitInfo, maxRayCastDistance,  ~(1 << LayerMask.NameToLayer("RoadNode")));
        
        if (undockedNode == null)
            undockedNode = new GameObject("Node(Temp)").transform;
        var transform = undockedNode.transform;
        if (hit)
        {
            transform.position = hitInfo.point;
            transform.rotation = Quaternion.Euler(UnattachedNodeRotation);
        }
        else
        {
            transform.position = Camera.main.transform.TransformPoint( new Vector3(0,-4,8));
            transform.rotation = Quaternion.Euler(UnattachedNodeRotation);
        }
    }
    
    public bool TryGetAnyMesh(out RoadNode node,out bool isForward)
    {
        isForward = true;
        node = null;
        var ray=GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        var hit=Physics.Raycast(ray,out var hitInfo, maxRayCastDistance, 1 << LayerMask.NameToLayer("RoadNode"));
        if (hit)
        {
            node = hitInfo.transform.GetComponent<RoadNode>();
            node.GetDirectionFromCollider(hitInfo.collider, out isForward);
        }
        return hit;    
    }

    public void GenerateShortestRoadBlueprint(RoadNode node,bool isHead)
    {
        blueprint = Instantiate(RoadSegmentPrefab,Vector3.zero,Quaternion.identity).GetComponent<RoadSegment>();
        if (isHead)
            blueprint.head = node.transform;
        else
            blueprint.tail = node.transform;
        blueprint.GenerateShortSegment(minLength);
        blueprint.GenerateMesh();
        blueprint.SetBlueprint();
    }

    public void OnPointToAvaliableNode(RoadNode node,bool isForward)
    {
        Destroy(blueprint);
        state = State.Pointing;
        nodeType = GetNodeTypeByColliderDirection(isForward);
        GenerateShortestRoadBlueprint(node, isForward);
    }
    public void GenerateRoad()
    {
    }

    public void UpdateRoadBlueprint()
    {
        
    }

    public NodeType GetNodeTypeByColliderDirection(bool isForward)
    {
        return (isForward ? NodeType.Tail : NodeType.Head);
    }
    
    public enum NodeType
    {
        None,Head,Tail
    }
    public enum State
    {
        Idle,Pointing,AttachingOneSide,AttachingBothSide
    }
}
