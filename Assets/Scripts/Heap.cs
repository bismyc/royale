using System.Collections;
using System.Collections.Generic;

public class Heap
{
    private uint Size = 0;
    private static uint Capacity = 2;
    private Node[] items = new Node[Capacity];
    private Dictionary<GroundNode, uint> NodeMap = new Dictionary<GroundNode, uint>();

    public uint GetSize()
    {
        return Size;
    }

    uint GetLeftChild(uint index)
    {
        return index * 2 + 1;
    }

    uint GetRightChild(uint index)
    {
        return index * 2 + 2;
    }

    uint GetParent(uint index)
    {
        return (index - 1) / 2;
    }

    void Swap(uint index1, uint index2)
    {
        Node temp = items[index1];
        items[index1] = items[index2];
        NodeMap[items[index1].groundNode] = index1;
        items[index2] = temp;
        NodeMap[items[index2].groundNode] = index2;
    }

    void EnsureCapacity()
    {
        if (Capacity < Size)
        {
            Capacity *= 2;
            Node[] newArray = new Node[Capacity];
            for (uint i = 0; i < Size - 1; i++)
            {
                newArray[i] = items[i];
            }
            items = newArray;
        }
    }

    bool IsValidIndex(uint index)
    {
        return index >= 0 && index < Size;
    }

    public double FindValue(GroundNode node)
    {
        if (NodeMap.ContainsKey(node)) {
            uint index = NodeMap[node];
            return items[index].F;
        }
        return double.MaxValue;
    }

    void HeapifyUp(uint index)
    {
        uint currentIndex = index;
        uint parentIndex = GetParent(currentIndex);

        while (IsValidIndex(parentIndex) && items[currentIndex].F < items[parentIndex].F)
        {
            Swap(currentIndex, parentIndex);
            currentIndex = parentIndex;
            parentIndex = GetParent(currentIndex);
        }

    }


    uint GetSmallestChildIndex(uint index)
    {
        uint leftChildIndex = GetLeftChild(index);
        bool isLeftChildValid = IsValidIndex(leftChildIndex);
        uint rightChildIndex = GetRightChild(index);
        bool isRightChildValid = IsValidIndex(rightChildIndex);
        if (isLeftChildValid || isRightChildValid)
        {
            return (isLeftChildValid ? items[leftChildIndex].F : uint.MaxValue) < (isRightChildValid ? items[rightChildIndex].F : uint.MaxValue) ? leftChildIndex : rightChildIndex;
        }
        return Size + 1; //invalid index
    }

    void HeapifyDown(uint startIndex)
    {
        uint currentIndex = startIndex;
        uint smallestChildIndex = GetSmallestChildIndex(currentIndex);

        while (IsValidIndex(smallestChildIndex) && items[currentIndex].F > items[smallestChildIndex].F)
        {
            Swap(currentIndex, smallestChildIndex);
            currentIndex = smallestChildIndex;
            smallestChildIndex = GetSmallestChildIndex(currentIndex);
        }
    }

    public void Add(Node node)
    {
        if(NodeMap.ContainsKey(node.groundNode))
        {
            Modify(node);
        }
        else
        {
            ++Size;
            EnsureCapacity();
            items[Size - 1] = node;
            NodeMap[items[Size - 1].groundNode] = Size - 1;
            HeapifyUp(Size - 1);
        } 
    }

    private void Modify(Node n)
    {
        uint index = NodeMap[n.groundNode];
        items[index].F = n.F;
        items[index].G = n.G;
        if (items[index].F < n.F)
        {
            HeapifyUp(index);
        }
        else if (items[index].F > n.F)
        {
            HeapifyDown(index);
        }
    }

    public Node Poll()
    {
        if (Size == 0) { throw new System.Exception(); }
        Node min = items[0];
        NodeMap.Remove(items[0].groundNode);
        items[0] = null;
        if (Size > 1)
        {
            items[0] = items[Size - 1];
            NodeMap[items[0].groundNode] = 0;
        }
        --Size;
        HeapifyDown(0);
        return min;
    }

    public Node Peek()
    {
        if (Size == 0) { throw new System.Exception(); }
        return items[0];
    }

    public void Delete(GroundNode node)
    {
        uint index = NodeMap[node];
        items[index] = items[Size - 1];
        NodeMap[items[index].groundNode] = index;
        NodeMap.Remove(node);
        items[Size - 1] = null;
        Size--;
        HeapifyDown(index);
    }
}


public class Node
{
    public Node(GroundNode index, float g, double f)
    {
        groundNode = index; G = g; F = f;
    }
    public float G;
    public double F;
    public GroundNode groundNode;

    public static bool operator <(Node b, Node c)
    {
        return b.F < c.F;
    }

    public static bool operator >(Node b, Node c)
    {
        return b.F > c.F;
    }
}
