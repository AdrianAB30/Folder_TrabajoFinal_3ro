using System.Collections;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public SimpleLinkedList<Nodes> nodes = new SimpleLinkedList<Nodes>();

    // O(1)
    public void AddNode(GameObject nodeObject)
    {
        if (!CurrentNodes(nodeObject))
        {
            nodes.InsertNodeAtEnd(new Nodes(nodeObject));
        }
    }
    // O(n)
    public void AddDirectedConnection(GameObject fromNode, GameObject toNode)
    {
        Nodes graphFromNode = SearchNextNode(fromNode);
        Nodes graphToNode = SearchNextNode(toNode);

        if (graphFromNode != null && graphToNode != null)
        {
            graphFromNode.AddNode(graphToNode); 
        }
    }
    // O(n)
    public SimpleLinkedList<GameObject> GetNeighbors(GameObject nodeObject)
    {
        Nodes node = SearchNextNode(nodeObject);
        if (node != null)
        {
            SimpleLinkedList<GameObject> neighbors = new SimpleLinkedList<GameObject>();
            SimpleLinkedList<Nodes> neighborNodes = node.GetNodes();
            for (int i = 0; i < neighborNodes.Length; i++)
            {
                neighbors.InsertNodeAtEnd(neighborNodes.GetNodeAtPosition(i).Node);
            }
            return neighbors;
        }
        return null;
    }
    // O(n)
    public bool CurrentNodes(GameObject nodeObject)
    {
        return SearchNextNode(nodeObject) != null;
    }
    // O(n)
    private Nodes SearchNextNode(GameObject nodeObject)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes.GetNodeAtPosition(i).Node == nodeObject)
            {
                return nodes.GetNodeAtPosition(i);
            }
        }
        return null;
    }
}
