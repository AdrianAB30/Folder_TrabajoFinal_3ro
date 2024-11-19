using System.Collections;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public SimpleLinkedList<Nodes> nodes = new SimpleLinkedList<Nodes>();

    public void AddNode(GameObject nodeObject)
    {
        if (!CurrentNodes(nodeObject))
        {
            nodes.InsertNodeAtEnd(new Nodes(nodeObject));
        }
    }
    public void AddDirectedConnection(GameObject fromNode, GameObject toNode)
    {
        Nodes graphFromNode = SearchNextNode(fromNode);
        Nodes graphToNode = SearchNextNode(toNode);

        if (graphFromNode != null && graphToNode != null)
        {
            graphFromNode.AddNode(graphToNode); 
        }
    }
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
    public bool CurrentNodes(GameObject nodeObject)
    {
        return SearchNextNode(nodeObject) != null;
    }
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
