using Oculus.Platform;
using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGraphManager : MonoBehaviour
{
    public static WorldGraphManager _instance;
    public float edgeDistanceMax = 3.0f;
    public float edgeDistanceMin = 2.12f;
    Graph graph;

    public int fromIndex;
    public int toIndex;

    List<Node> path;

    public bool showPath = false;
    private void Awake()
    {
        _instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetPath();
            showPath = true;
        }
    }
    private void Start()
    {
        // InitializeGraph();
    }

    public void InitializeGraph()
    {
        graph = new Graph();
        //Transform room = WorldConfiguration._instance.bigRubikRooms[0];
        Transform[] rooms = WorldConfiguration._instance.bigRubikRooms;
        //rooms.childCout = 6
        foreach (Transform room in rooms)
        {
            Transform[] walls = new Transform[6];
            Transform[] bricks = new Transform[54];

            //rooms.childCount * walls[0].childCount=6*9=54
            int index = 0;
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i] = room.GetChild(i);
                Transform curWall = walls[i];
                for (int j = 0; j < curWall.childCount; j++)
                {
                    bricks[index] = curWall.GetChild(j);
                    index++;
                }

            }

            for (int i = 0; i < bricks.Length; i++)
            {
                Transform brick = bricks[i];
                graph.AddNode(brick.position, brick);
            }
        }

        List<Node> allNodes = graph.Nodes;
        foreach (Node from in allNodes)
        {
            foreach (Node to in allNodes)
            {
                float distance = Vector3.Distance(from.worldPosition, to.worldPosition);
                //if (distance <= edgeDistanceMax && distance > edgeDistanceMin && from != to)
                //{
                //    graph.AddEdge(from, to);
                //}

                if (distance <= edgeDistanceMax)
                {
                    if (BrickInSameRoom(from.brickTransform, to.brickTransform))
                    {
                        graph.AddEdge(from, to);
                    }
                    else if (IsBothPassages(from.brickTransform, to.brickTransform) && distance < 1f)
                    {
                        graph.AddEdge(from, to);
                    }
                    //Debug.Log(GenerateLevel.passages.Contains(from.transform));
                    //if (GenerateLevel.passages.Contains(from.transform) && GenerateLevel.passages.Contains(to.transform))
                    //{
                    //    Debug.Log(true);
                    //    graph.AddEdge(from, to);
                    //}
                }
            }
        }
        AddNodeToBrick();
    }

    private bool IsBothPassages(Transform brickOne, Transform brickTwo)
    {
        if (GenerateLevel.bigRubikPassages.Contains(brickOne.transform) && GenerateLevel.bigRubikPassages.Contains(brickTwo.transform))
            return true;
        return false;
    }

    private void OnDrawGizmos()
    {
        if (graph == null)
        {
            return;
        }
        List<Edge> allEdges = graph.Edges;
        foreach (Edge e in allEdges)
        {
            Debug.DrawLine(e.from.worldPosition, e.to.worldPosition, Color.white);
        }

        List<Node> allNodes = graph.Nodes;
        foreach (Node n in allNodes)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(n.worldPosition, 0.3f);
        }


        if (showPath)
        {
            if (fromIndex < allNodes.Count && toIndex < allNodes.Count)
            {

                if (path.Count > 1)
                {
                    for (int i = 0; i < path.Count-1; i++)
                    {
                        Debug.DrawLine(path[i].brickTransform.position, path[i+1].brickTransform.position, Color.red);
                    }
                }
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(allNodes[fromIndex].brickTransform.position, 0.3f);
                Gizmos.DrawSphere(allNodes[toIndex].brickTransform.position, 0.3f);
                //showPath = false;
            }
        }
    }

    public void GetPath()
    {
        List<Node> allNodes = graph.Nodes;
        path = graph.GetPath(allNodes[fromIndex], allNodes[toIndex]);
    }


    public bool BrickInSameRoom(Transform brickOne, Transform brickTwo)
    {
        if (brickOne.parent.parent == brickTwo.parent.parent)
            return true;
        return false;
    }

    public void AddNodeToBrick()
    {
        List<Node> allNodes = graph.Nodes;
        Transform[] rooms = WorldConfiguration._instance.bigRubikRooms;
        foreach (Transform room in rooms)
        {
            foreach (Transform wall in room)
            {
                foreach (Transform brick in wall)
                {
                    foreach (Node node in allNodes)
                    {
                        if (brick == node.brickTransform)
                        {
                            Brick b = brick.gameObject.AddComponent(typeof(Brick)) as Brick;
                            b.index = node.index;
                        }
                    }
                }
            }
        }
    }

}
