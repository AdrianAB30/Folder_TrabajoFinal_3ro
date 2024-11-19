using System.Collections;
using UnityEngine;

public class Stack<T>
{
    private class Node
    {
        public T Value { get; }
        public Node Next { get; set; }
        public Node Previous { get; set; }

        public Node(T value)
        {
            Value = value;
            Next = null;
            Previous = null;
        }
    }
    private Node Head { get; set; }
    private Node Top { get; set; }
    public int Count { get; private set; }

    public void Push(T value)
    {
        Node newNode = new Node(value);

        if (Head == null)
        {
            Head = newNode;
            Top = newNode;
        }
        else
        {
            newNode.Previous = Top;
            Top.Next = newNode;
            Top = newNode;
        }
        Count++;
    }
    public T Pop()
    {
        if (Head == null)
        {
            throw new System.InvalidOperationException("La pila está vacía.");
        }

        T topValue = Top.Value;

        if (Head == Top)
        {
            Head = null;
            Top = null;
        }
        else
        {
            Node previousNode = Top.Previous;
            previousNode.Next = null;
            Top = previousNode;
        }

        Count--;
        return topValue;
    }
    public T Peek()
    {
        if (Head == null)
        {
            throw new System.InvalidOperationException("La pila está vacía.");
        }
        return Top.Value;
    }
    public bool IsEmpty()
    {
        return Count == 0;
    }
    public bool Contains(T value)
    {
        Node currentNode = Top;

        dynamic dynamicValue = value;

        while (currentNode != null)
        {
            if ((dynamic)currentNode.Value == dynamicValue)
            {
                return true;
            }
            currentNode = currentNode.Previous;
        }
        return false;
    }
}



