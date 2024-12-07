using System.Collections;
using UnityEngine;

public class Nodes 
{
    public GameObject Node { get; private set; }
    private SimpleLinkedList<Nodes> neighbors;

    public Nodes(GameObject nodesRoute)
    {
        Node = nodesRoute;
        neighbors = new SimpleLinkedList<Nodes>();
    }
    public void AddNode(Nodes nodeNeighbor)
    {
        if (!neighbors.SearchValue(nodeNeighbor))
        {
            neighbors.InsertNodeAtEnd(nodeNeighbor);
        }
    }
    public SimpleLinkedList<Nodes> GetNodes()
    {
        return neighbors;
    }
}
