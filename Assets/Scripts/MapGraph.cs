using System.Collections.Generic;
using UnityEngine;

public class MapGraph : IMapGraph
{
    private List<GroundNode>[] AdjacencyList;
    (uint rows, uint columns) GraphMatrixDimension;

    public enum Direction
    {
        LEFT = 0,
        TOP_LEFT = 1,
        TOP = 2,
        TOP_RIGHT = 3,
        RIGHT = 4,
        BOTTOM_RIGHT = 5,
        BOTTOM = 6,
        BOTTOM_LEFT = 7
    }

    public MapGraph((uint rows, uint columns) graphMatrixDimension)
    {
        GraphMatrixDimension = graphMatrixDimension;
        uint totalNodes = GraphMatrixDimension.rows * GraphMatrixDimension.columns;
        AdjacencyList = new List<GroundNode>[totalNodes];
        for (uint i = 0; i < totalNodes; i++)
        {
            AdjacencyList[i] = new List<GroundNode>();
        }
    }

    public void AddEdges(GroundNode groundNode, (GroundNode left, GroundNode topLeft, GroundNode top, GroundNode topRight, GroundNode right, GroundNode bottomRight, GroundNode bottom, GroundNode bottomLeft) edges)
    {
        switch (GetQuad(groundNode))
        {
            case Direction.TOP_LEFT:
                groundNode.Quad = Direction.TOP_LEFT;
                AddEdge(groundNode, edges.top, Direction.TOP);
                AddEdge(groundNode, edges.topLeft, Direction.TOP_LEFT);
                AddEdge(groundNode, edges.left, Direction.LEFT);
                AddEdge(groundNode, edges.bottomLeft, Direction.BOTTOM_LEFT);
                AddEdge(groundNode, edges.bottom, Direction.BOTTOM);
                AddEdge(groundNode, edges.bottomRight, Direction.BOTTOM_RIGHT);
                AddEdge(groundNode, edges.right, Direction.RIGHT);
                AddEdge(groundNode, edges.topRight, Direction.TOP_RIGHT);
                break;
            case Direction.TOP_RIGHT:
                groundNode.Quad = Direction.TOP_RIGHT;
                AddEdge(groundNode, edges.top, Direction.TOP);
                AddEdge(groundNode, edges.topRight, Direction.TOP_RIGHT);
                AddEdge(groundNode, edges.right, Direction.RIGHT);
                AddEdge(groundNode, edges.bottomRight, Direction.BOTTOM_RIGHT);
                AddEdge(groundNode, edges.bottom, Direction.BOTTOM);
                AddEdge(groundNode, edges.bottomLeft, Direction.BOTTOM_LEFT);
                AddEdge(groundNode, edges.left, Direction.LEFT);
                AddEdge(groundNode, edges.topLeft, Direction.TOP_LEFT);
                break;
            case Direction.BOTTOM_RIGHT:
                groundNode.Quad = Direction.BOTTOM_RIGHT;
                AddEdge(groundNode, edges.bottom, Direction.BOTTOM);
                AddEdge(groundNode, edges.bottomRight, Direction.BOTTOM_RIGHT);
                AddEdge(groundNode, edges.right, Direction.RIGHT);
                AddEdge(groundNode, edges.topRight, Direction.TOP_RIGHT);
                AddEdge(groundNode, edges.top, Direction.TOP);
                AddEdge(groundNode, edges.topLeft, Direction.TOP_LEFT);
                AddEdge(groundNode, edges.left, Direction.LEFT);
                AddEdge(groundNode, edges.bottomLeft, Direction.BOTTOM_LEFT);
                break;
            case Direction.BOTTOM_LEFT:
                groundNode.Quad = Direction.BOTTOM_LEFT;
                AddEdge(groundNode, edges.bottom, Direction.BOTTOM);
                AddEdge(groundNode, edges.bottomLeft, Direction.BOTTOM_LEFT);
                AddEdge(groundNode, edges.left, Direction.LEFT);
                AddEdge(groundNode, edges.topLeft, Direction.TOP_LEFT);
                AddEdge(groundNode, edges.top, Direction.TOP);
                AddEdge(groundNode, edges.topRight, Direction.TOP_RIGHT);
                AddEdge(groundNode, edges.right, Direction.RIGHT);
                AddEdge(groundNode, edges.bottomRight, Direction.BOTTOM_RIGHT);
                break;
        } 
    }

    private void AddEdge(GroundNode parentNode, GroundNode childNode, Direction direction)
    {
        if (childNode != null && childNode.IsAPathNode())
        {
            AdjacencyList[parentNode.GetNodeIndex()].Add(childNode);
            switch(direction)
            {
                case Direction.LEFT:
                    parentNode.Left = childNode;
                    break;
                case Direction.TOP_LEFT:
                    parentNode.TopLeft = childNode;
                    break;
                case Direction.TOP:
                    parentNode.Top = childNode;
                    break;
                case Direction.TOP_RIGHT:
                    parentNode.TopRight = childNode;
                    break;
                case Direction.RIGHT:
                    parentNode.Right = childNode;
                    break;
                case Direction.BOTTOM_RIGHT:
                    parentNode.BottomRight = childNode;
                    break;
                case Direction.BOTTOM:
                    parentNode.Bottom = childNode;
                    break;
                case Direction.BOTTOM_LEFT:
                    parentNode.BottomLeft = childNode;
                    break;
            }
        }
    }

    public Direction GetQuad(GroundNode groundNode)
    {
        uint nodeIndex = groundNode.GetNodeIndex();
        uint rowIndex = nodeIndex / GraphMatrixDimension.columns;
        uint columnIndex = nodeIndex % GraphMatrixDimension.columns;
        uint quadWidth = GraphMatrixDimension.columns / 2;
        uint quadHeight = GraphMatrixDimension.rows / 2; ;

        if      (rowIndex < quadHeight && columnIndex < quadWidth) { return Direction.TOP_LEFT; }
        else if (rowIndex < quadHeight && columnIndex >= quadWidth) { return Direction.TOP_RIGHT; }
        else if (rowIndex >= quadHeight && columnIndex < quadWidth) { return Direction.BOTTOM_LEFT; }
        else if (rowIndex >= quadHeight && columnIndex >= quadWidth) { return Direction.BOTTOM_RIGHT; }
        else { return Direction.TOP_LEFT; }
    }

    private uint GetUp(uint nodeIndex)
    {
        (uint r, uint c) rc = GetRowColumnFromIndex(nodeIndex);
        rc.r -= 1;
        return GetIndexFromRowColumn(rc);
    }

    private uint GetLeft(uint nodeIndex)
    {
        (uint r, uint c) rc = GetRowColumnFromIndex(nodeIndex);
        rc.c -= 1;
        return GetIndexFromRowColumn(rc);
    }

    private uint GetRight(uint nodeIndex)
    {
        (uint r, uint c) rc = GetRowColumnFromIndex(nodeIndex);
        rc.c += 1;
        return GetIndexFromRowColumn(rc);
    }

    private uint GetDown(uint nodeIndex)
    {
        (uint r, uint c) rc = GetRowColumnFromIndex(nodeIndex);
        rc.r += 1;
        return GetIndexFromRowColumn(rc);
    }

    private uint GetIndexFromRowColumn((uint r, uint c) rc)
    {
        return rc.r * GraphMatrixDimension.columns + rc.c ;
    }

    private (uint r, uint c) GetRowColumnFromIndex(uint value)
    {
        return (value / GraphMatrixDimension.columns, value % GraphMatrixDimension.columns);
    }

    private bool IsValidIndex(uint index)
    {
        return index >= 0 && index < GraphMatrixDimension.rows * GraphMatrixDimension.columns;
    }

    //private double GridManhattanDistance(GroundNode from, GroundNode to)
    //{
    //    //(uint r, uint c) fromRC = GetRowColumnFromIndex(fromIndex);
    //    //(uint r, uint c) ToRC = GetRowColumnFromIndex(toIndex);
    //    float xDist = System.Math.Abs(to.transform.position.x - from.transform.position.x);
    //    float yDist = System.Math.Abs(to.transform.position.y - from.transform.position.y);

    //    float distanceMax = Mathf.Max(xDist, yDist);
    //    float distanceMin = Mathf.Min(xDist, yDist);
    //    return (distanceMax - distanceMin) + distanceMin * 1.414f;
    //   // return System.Math.Sqrt(xDist * xDist + yDist * yDist);
    //}

    public double GridDiagonalDistance(uint fromIndex, uint toIndex)
    {
        (uint r, uint c) fromRC = GetRowColumnFromIndex(fromIndex);
        (uint r, uint c) toRC = GetRowColumnFromIndex(toIndex);
        float xDist = Mathf.Abs((float)toRC.c - (float)fromRC.c);
        float yDist = Mathf.Abs((float)toRC.r - (float)fromRC.r);

        float distanceMax = Mathf.Max(xDist, yDist);
        float distanceMin = Mathf.Min(xDist, yDist);
        return (distanceMax - distanceMin) + distanceMin * 1.414f;
    }

    private double DiagonalDistance(uint fromIndex, uint toIndex)
    {
        //quad wise no need
        (uint r, uint c) fromRC = GetRowColumnFromIndex(fromIndex);
        (uint r, uint c) toRC = GetRowColumnFromIndex(toIndex);
        float xDist = Mathf.Abs((float)toRC.c - (float)fromRC.c);
        float yDist = Mathf.Abs((float)toRC.r - (float)fromRC.r);

        float distanceMax = Mathf.Max(xDist, yDist);
        float distanceMin = Mathf.Min(xDist, yDist);
        return (distanceMax - distanceMin) + distanceMin * 1.414f;
    }

    private Direction GetDirection(uint from, uint to)
    {
        int fromIndex = (int)from;
        int toIndex = (int)to;
        if ((toIndex - fromIndex) == 1) { return Direction.RIGHT; }
        else if ((toIndex - fromIndex) == -1) { return Direction.LEFT; }
        else if ((toIndex - fromIndex) == -GraphMatrixDimension.columns) { return Direction.TOP; }
        else if ((toIndex - fromIndex) == GraphMatrixDimension.columns) { return Direction.BOTTOM; }
        else if ((toIndex - fromIndex) == -GraphMatrixDimension.columns + 1) { return Direction.TOP_RIGHT; }
        else if ((toIndex - fromIndex) == -GraphMatrixDimension.columns - 1) { return Direction.TOP_LEFT; }
        else if ((toIndex - fromIndex) == GraphMatrixDimension.columns - 1) { return Direction.BOTTOM_LEFT; }
        else if ((toIndex - fromIndex) == GraphMatrixDimension.columns + 1) { return Direction.BOTTOM_RIGHT; }
        return Direction.TOP;
    }

    public List<GroundNode> AstarPath(GroundNode startNode, GroundNode goalNode)
    {
        Heap openNodes = new Heap();
        List<GroundNode> closedList = new List<GroundNode>();
        Dictionary<GroundNode, GroundNode> parentNodes = new Dictionary<GroundNode, GroundNode>();

        Node start = new Node(startNode, 0, 0);
        openNodes.Add(start);
        
        while (openNodes.GetSize() > 0)
        {
            Node currentNode = openNodes.Poll();
            uint currentNodeIndex = currentNode.groundNode.GetNodeIndex();
            closedList.Add(currentNode.groundNode);

            GroundNode currentGroundNode = currentNode.groundNode;
            if (currentNode.groundNode == goalNode) {
                List<GroundNode> path = new List<GroundNode>();
                while(parentNodes.Count > 0)
                {
                    path.Add(parentNodes[currentGroundNode]);
                    currentGroundNode = parentNodes[currentGroundNode];

                    if (currentGroundNode == startNode)
                    {
                        break;
                    }
                }
                path.Reverse();
                
                return path;
            }
            foreach (GroundNode groundNode in AdjacencyList[currentNodeIndex])
            {
                uint currentEdgeIndex = groundNode.GetNodeIndex();
                if(closedList.Contains(groundNode)) { continue; }


                Direction edgeDirection = GetDirection(currentNodeIndex, currentEdgeIndex);
                float cost = 1;
                if(edgeDirection == Direction.TOP_LEFT || edgeDirection == Direction.TOP_RIGHT || edgeDirection == Direction.BOTTOM_LEFT || edgeDirection == Direction.BOTTOM_RIGHT)
                {
                    cost = 1.414f;
                }
                float g = currentNode.G + cost;
                double h = GridDiagonalDistance(currentEdgeIndex, goalNode.GetNodeIndex());

                (uint r, uint c) RC = GetRowColumnFromIndex(currentEdgeIndex);
                if(RC.c != 3 && RC.c != 14) { h += 0.5f; }
                if (goalNode.GetNodeIndex() == currentEdgeIndex) { h = 0; }

                double f = g + h;

                Node currentEdgeNode = new Node(groundNode, g, f);
                if (f < openNodes.FindValue(groundNode))
                {
                    openNodes.Add(currentEdgeNode); 
                    parentNodes[groundNode] = currentGroundNode;
                }
            }
        }
        return new List<GroundNode>();
    }

    public GroundNode DFS(GroundNode startNode, uint nodeIndex, bool[] visited)
    {
        if (startNode != null && !visited[startNode.GetNodeIndex()])
        {
            if (startNode.GetNodeIndex() == nodeIndex)
            {
                return startNode;
            }
            visited[startNode.GetNodeIndex()] = true;

            List<GroundNode> edgeNodes = AdjacencyList[startNode.GetNodeIndex()];
            for (int i = 0; i < edgeNodes.Count; i++)
            {
                GroundNode found = DFS(edgeNodes[i], nodeIndex, visited);
                if (found != null)
                {
                    return found;
                }
            }

        }
        return null;
    }


    public GroundNode DepthFirstSearchPartGraph(GroundNode startNode, GroundNode lastNode, uint nodeIndex, bool[] visited)
    {
        if(lastNode == null) { lastNode = startNode; }
        if (startNode != null && startNode.GetNodeIndex() <= lastNode.GetNodeIndex() && !visited[startNode.GetNodeIndex()])
        {
            if (startNode.GetNodeIndex() == nodeIndex)
            {
                return startNode;
            }
            visited[startNode.GetNodeIndex()] = true;

            List<GroundNode> edgeNodes = AdjacencyList[startNode.GetNodeIndex()];
            for (int i = 0; i < edgeNodes.Count; i++)
            {
                GroundNode found = DepthFirstSearchPartGraph(edgeNodes[i], lastNode, nodeIndex, visited);
                if(found != null)
                {
                    return found;
                }  
            }

        }
        return null;
    }

    public GroundNode BreadthFirstSearch(GroundNode startNode, GroundNode goalNode = null, uint playerTeam = 0, uint radius = 0)
    {
        if (startNode == null) { return null; }
        bool searchOpponent = playerTeam > 0 && goalNode == null;
        bool searchGoalNode = goalNode != null;
        if (!searchOpponent && !searchGoalNode) { return null; }

        bool searchWithinRadius = radius > 0;

        List<uint> visited = new List<uint>();
        Queue<GroundNode> queue = new Queue<GroundNode>();

        queue.Enqueue(startNode);
        if (searchWithinRadius) { queue.Enqueue(null); }

        visited.Add(startNode.GetNodeIndex());
        int depth = 1;

        while (queue.Count != 0)
        {
            GroundNode currentMapNode = queue.Peek();
            if((searchGoalNode && currentMapNode == goalNode)
            || (searchOpponent && currentMapNode != null && currentMapNode.HasOpponent(playerTeam)))
            {
                return currentMapNode;
            }

            // track depth from startNode
            if (searchWithinRadius && currentMapNode == null)
            {
                queue.Dequeue();
                depth++;
                queue.Enqueue(null);

                if (queue.Peek() == null) { break; }
                else { continue; }
            }

            if (searchWithinRadius && depth > radius)
            {
                return null;
            }

            uint edgeIndex = currentMapNode.GetNodeIndex();
            queue.Dequeue();
            foreach (GroundNode groundNode in AdjacencyList[edgeIndex])
            {
                if (!visited.Contains(groundNode.GetNodeIndex()))
                {
                    visited.Add(groundNode.GetNodeIndex());
                    queue.Enqueue(groundNode);
                }
            }
        }

        return null;
    }

    public GameBody BreadthFirstSearchForOpponent(GroundNode startNode, uint radius, uint team, bool lookForTower = false)
    {
        bool[] visited = new bool[GraphMatrixDimension.rows * GraphMatrixDimension.columns];
        Queue<GroundNode> queue = new Queue<GroundNode>();
        queue.Enqueue(startNode);
        queue.Enqueue(null);

        int depth = 1;

        while (queue.Count != 0)
        {
            GroundNode currentMapNode = queue.Peek();

            // track depth from startNode
            if(currentMapNode == null)
            {
                queue.Dequeue();
                depth++;
                queue.Enqueue(null);

                if(queue.Peek() == null) { break; }
                else { continue; }
            }


            uint edgeIndex = currentMapNode.GetNodeIndex();
            queue.Dequeue();
            if (!visited[edgeIndex] && (depth <= radius || lookForTower))
            {
                List<GameBody> gameBodies = currentMapNode.GetGameBodiesInNode();
                for (int i = 0; i < gameBodies.Count; i++)
                {
                    if (gameBodies[i].GetMyPlayer().GetMyTeam() != team)
                    {
                        return gameBodies[i];
                    } 
                }

                visited[edgeIndex] = true;
                foreach (GroundNode groundNode in AdjacencyList[edgeIndex])
                {
                    queue.Enqueue(groundNode);
                }  
            }
        }

        return null;
    }


}
