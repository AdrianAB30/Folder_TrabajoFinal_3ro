using System;
using System.Collections;
using UnityEngine;

public class DoubleCircularLinkedList<T>
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

    private Node Head { get; set; }
    public int Count { get; private set; }

    //O(n)
    public void InsertAtStart(T value)
    {
        Node newNode = new Node(value);
        if (Head == null)
        {
            Head = newNode;
            newNode.Next = Head;
            newNode.Previous = Head;
        }
        else
        {
            Node lastNode = SearchLastNode();
            newNode.Next = Head;
            newNode.Previous = lastNode;
            lastNode.Next = newNode;
            Head.Previous = newNode;
        }
        Head = newNode;
        ++Count;
    }
    //O(n)
    public void InsertAtEnd(T value)
    {
        if (Head == null)
        {
            InsertAtStart(value);
        }
        else
        {
            Node newNode = new Node(value);
            Node lastNode = SearchLastNode();
            newNode.Next = Head;
            newNode.Previous = lastNode;
            lastNode.Next = newNode;
            Head.Previous = newNode;
            ++Count;
        }
    }
    //O(1)
    public T GetAtStart()
    {
        if (Head == null)
        {
            throw new NullReferenceException("La lista está vacía.");
        }
        return Head.Value;
    }
    //O(1)
    public T GetAtEnd()
    {
        if (Head == null)
            throw new NullReferenceException("La lista está vacía.");

        Node lastNode = SearchLastNode();
        return lastNode.Value;
    }
    //O(n)
    public T GetAtPosition(int position)
    {
        if (position < 0 || position >= Count)
        {
            throw new NullReferenceException("Índice fuera de rango.");
        }

        Node currentNode = Head;
        for (int i = 0; i < position; i++)
        {
            currentNode = currentNode.Next;
        }
        return currentNode.Value;
    }
    //O(1)
    public void DeleteAtStart()
    {
        if (Head == null)
        {
            throw new NullReferenceException("La lista está vacía.");
        }

        if (Head.Next == Head)
        {
            Head = null;
            Count = 0;
        }
        else
        {
            Node lastNode = Head.Previous;
            Node newHead = Head.Next;
            newHead.Previous = lastNode;
            lastNode.Next = newHead;
            Head = newHead;
            --Count;
        }
    }
    //O(1)
    public void DeleteAtEnd()
    {
        if (Head == null)
        {
            throw new NullReferenceException("La lista está vacía.");
        }

        if (Count == 1)
        {
            Head = null;
            Count = 0;
        }
        else
        {
            Node lastNode = SearchLastNode();
            Node newLastNode = lastNode.Previous;
            newLastNode.Next = Head;
            Head.Previous = newLastNode;
            --Count;
        }
    }
    //O(n)
    private Node SearchLastNode()
    {
        if (Head == null)
        {
            return null;
        }

        Node currentNode = Head;
        while (currentNode.Next != Head)
        {
            currentNode = currentNode.Next;
        }
        return currentNode;
    }
}
