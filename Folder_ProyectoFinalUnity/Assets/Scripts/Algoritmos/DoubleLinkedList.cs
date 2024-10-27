using System.Collections;
using UnityEngine;

public class DoubleLinkedList<T>
{
    private class Node
    {
        public T Value { get; set; }
        public Node Next { get; set; }
        public Node Previous { get; set; }

        public Node(T value)
        {
            Value = value;
            Next = null;
            Previous = null;
        }
    }

    private Node head;
    private Node current;
    private int length;

    public int Length => length;

    public void AddEnd(T value)
    {
        Node newNode = new Node(value);
        if (head == null)
        {
            head = newNode;
            current = head;
        }
        else
        {
            Node lastNode = head;
            while (lastNode.Next != null)
            {
                lastNode = lastNode.Next;
            }
            lastNode.Next = newNode;
            newNode.Previous = lastNode;
        }
        length++;
    }

    public T GetNext()
    {
        if (current.Next != null)
        {
            current = current.Next;
        }
        else
        {
            current = head;
        }
        return current.Value;
    }

    public T GetPrevious()
    {
        if (current.Previous != null)
        {
            current = current.Previous;
        }
        else
        {
            current = GetLastNode();
        }
        return current.Value;
    }

    private Node GetLastNode()
    {
        Node lastNode = head;
        while (lastNode.Next != null)
        {
            lastNode = lastNode.Next;
        }
        return lastNode;
    }

    public T GetCurrent()
    {
        if (current != null)
        {
            return current.Value; 
        }
        else
        {
            throw new System.Exception("No hay un nodo actual."); 
        }
    }

    public T GetAt(int index)
    {
        if (index < 0 || index >= length)
        {
            throw new System.ArgumentOutOfRangeException("Index is out of range."); 
        }

        Node currentNode = head;
        for (int i = 0; i < index; i++)
        {
            currentNode = currentNode.Next; 
        }
        return currentNode.Value; 
    }
}
