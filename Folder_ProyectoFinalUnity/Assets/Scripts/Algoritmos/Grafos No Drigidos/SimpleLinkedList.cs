using System;
using System.Collections;
using UnityEngine;

public class SimpleLinkedList<T>
{
    private class Node
    {
        public T Value { get; set; }
        public Node Next { get; set; }
        public Node(T value)
        {
            this.Value = value;
            Next = null;
        }
    }
    private Node Head { get; set; }
    public int Length { get; set; }

    public void InsertNodeAtStart(T value)
    {
        if (Head == null)
        {
            Node newNode = new Node(value);
            Head = newNode;
            Length = Length + 1;
        }
        else
        {
            Node newNode = new Node(value);
            newNode.Next = Head;
            Head = newNode;
            Length = Length + 1;
        }
    }
    public void InsertNodeAtEnd(T value)
    {
        if (Head == null)
        {
            InsertNodeAtStart(value);
        }
        else
        {
            Node last = SearchLastNode();
            while (last.Next != null)
            {
                last = last.Next;
            }
            Node newNode = new Node(value);
            last.Next = newNode;
            Length = Length + 1;
        }
    }
    public void InsertNodeAtPosition(T value, int position)
    {
        if (position == 0)
        {
            InsertNodeAtStart(value);
        }

        else if (position == Length)
        {
            InsertNodeAtEnd(value);
        }

        else if (position > Length)
        {
            Console.WriteLine("No existe esa posicion.");
        }

        else
        {
            Node newNode = new Node(value);
            Node previusNode = Head;
            Node positionNode;
            int iterator = 0;

            while (iterator < position - 1)
            {
                previusNode = previusNode.Next;
                iterator = iterator + 1;
            }
            positionNode = previusNode.Next;
            previusNode.Next = null;
            previusNode.Next = newNode;
            newNode.Next = positionNode;
            Length = Length + 1;
        }
    }
    public void ModifyAtStart(T newValue)
    {
        if (Head == null)
        {
            throw new NullReferenceException("No hay elementos en la Lista");
        }
        else
        {
            Head.Value = newValue;
        }
    }

    public void ModifyAtEnd(T newValue)
    {
        if (Head == null)
        {
            throw new NullReferenceException("No hay elementos en la Lista");
        }
        else
        {
            Node lastNode = SearchLastNode();
            lastNode.Value = newValue;
        }
    }

    public void ModifyAtPosition(T newValue, int position)
    {
        if (position == 0)
        {
            ModifyAtStart(newValue);
        }
        else if (position == Length)
        {
            ModifyAtEnd(newValue);
        }
        else if (position > Length)
        {
            Console.WriteLine("No existe esa posicion.");
        }
        else
        {
            Node nodePosition = Head;
            int iterator = 0;
            while (iterator < position)
            {
                nodePosition = nodePosition.Next;
                iterator++;
            }
            nodePosition.Value = newValue;
        }
    }

    public T GetNodeAtStart()
    {
        if (Head == null)
        {
            throw new Exception("No hay elementos en la Lista");
        }
        else
        {
            return Head.Value;
        }
    }

    public T GetNodeAtEnd()
    {
        if (Head == null)
        {
            return GetNodeAtStart();
        }
        else
        {
            Node classNode = SearchLastNode();
            return classNode.Value;
        }
    }

    public T GetNodeAtPosition(int position)
    {
        if (position == 0)
        {
            return GetNodeAtStart();
        }
        else if (position == Length)
        {
            return GetNodeAtEnd();
        }
        else if (position > Length)
        {
            throw new Exception("No existe esa posicion.");
        }
        else
        {
            Node nodePosition = Head;
            int iterator = 0;
            while (iterator < position)
            {
                nodePosition = nodePosition.Next;
                iterator++;
            }
            return nodePosition.Value;
        }
    }

    public void DeleteAtStart()
    {
        if (Head == null)
        {
            throw new Exception("No hay elementos en la Lista");
        }
        else
        {
            Node newHead = Head;
            Head = newHead.Next;
            newHead.Next = null;
            Length = Length - 1;
        }
    }

    public void DeleteAtEnd()
    {
        if (Head == null)
        {
            DeleteAtStart();
        }
        else
        {
            Node previusLastNode = Head;
            while (previusLastNode.Next.Next != null)
            {
                previusLastNode = previusLastNode.Next;
            }
            Node lastNode = previusLastNode.Next;
            previusLastNode.Next = null;
            lastNode = null;
            Length = Length - 1;
        }
    }

    public void DeleteNodeAtPosition(int position)
    {
        if (position == 0)
        {
            DeleteAtStart();
        }
        else if (position == Length)
        {
            DeleteAtEnd();
        }
        else if (position >= Length)
        {
            throw new Exception("No existe esa posicion.");
        }
        else
        {
            Node previous = Head;
            int iterator = 0;
            while (iterator < position - 1)
            {
                previous = previous.Next;
                iterator++;
            }
            Node nodePosition = previous.Next;
            Node next = nodePosition.Next;
            previous.Next = null;
            nodePosition = null;
            previous.Next = next;
            nodePosition = null;
            Length = Length - 1;
        }
    }
    public bool SearchValue(T value)
    {
        Node current = Head;

        while (current != null)
        {
            if (current.Value.Equals(value)) 
            {
                return true;
            }
            current = current.Next;
        }
        return false;
    }
    private Node SearchLastNode()
    {
        if (Head == null)
        {
            throw new InvalidOperationException("La lista está vacía.");
        }
        Node lastNode = Head;
        while (lastNode.Next != null)
        {
            lastNode = lastNode.Next;
        }
        return lastNode;
    }
} 
      


