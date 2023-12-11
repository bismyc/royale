using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour, IGameData
{

    [SerializeField]
    private GameData Data;
    [SerializeField]
    private GameObject PlayerPrefab;
    [SerializeField]
    private GameObject MapNodePrefab;

    [SerializeField]
    private Player player1;
    [SerializeField]
    private Player player2;

    private MapGraph MapNodesGraph;
    (uint rows, uint columns) GroundNodeMatrixDimension = (30, 18);

    private enum CardDrawingStrategy {
        CHOOSE_EARLIEST_AVAILABLE_CARD = 0,
        CHOOSE_BEST_ACTIVE_CARD = 1
    }
   

    void Start()
    {
    
        MapNodesGraph = new MapGraph(GroundNodeMatrixDimension);

        GroundNode[] groundNodes = GameObject.Find("Map").GetComponentsInChildren<GroundNode>();
        GroundNode[,] groundNodesMatrix = new GroundNode[GroundNodeMatrixDimension.rows, GroundNodeMatrixDimension.columns];

        uint totalNodes = GroundNodeMatrixDimension.rows * GroundNodeMatrixDimension.columns;
        uint blockedNodesInEachTerritory = GroundNodeMatrixDimension.columns;
        uint totalNodesInEachTerritory = (totalNodes / 2) - blockedNodesInEachTerritory;
        (GroundNode start, GroundNode end) player1Territory = (groundNodes[totalNodes - totalNodesInEachTerritory], groundNodes[totalNodes - 1]); //(groundNodesMatrix[(int)GroundNodeMatrixDimension.rows / 2 + 1, 0], groundNodesMatrix[(int)GroundNodeMatrixDimension.rows - 1, (int)GroundNodeMatrixDimension.columns - 1]);
        (GroundNode start, GroundNode end) player2Territory = (groundNodes[0], groundNodes[totalNodesInEachTerritory - 1]); // (groundNodes[0], groundNodesMatrix[(int)GroundNodeMatrixDimension.rows / 2 - 2, (int)GroundNodeMatrixDimension.columns - 1]);

        List<uint> spawnableNodesForPlayer1 = new List<uint>();
        List<uint> spawnableNodesForPlayer2 = new List<uint>();
        (uint row, uint col) spawnableStartNodeForPlayer2 = (7, 0);
        (uint row, uint col) spawnableEndNodeForPlayer2 = (13, 17);
        (uint row, uint col) spawnableStartNodeForPlayer1 = (16, 0);
        (uint row, uint col) spawnableEndNodeForPlayer1 = (22, 17);

        float xOffset = GroundNodeMatrixDimension.columns * 0.53f/ 2;
        float yOffset = GroundNodeMatrixDimension.rows * 0.56f / 2;

        for (uint row = 0; row < GroundNodeMatrixDimension.rows; row++)
        {
            for (uint column = 0; column < GroundNodeMatrixDimension.columns; column++)
            {
                uint nodeIndex = row * GroundNodeMatrixDimension.columns + column;

                if (row >= spawnableStartNodeForPlayer1.row && column >= spawnableStartNodeForPlayer1.col && row <= spawnableEndNodeForPlayer1.row && column <= spawnableEndNodeForPlayer1.col)
                {
                    if(!groundNodes[nodeIndex].BlockedNode)
                    {
                        spawnableNodesForPlayer1.Add(nodeIndex);
                    }
                }
                else if (row >= spawnableStartNodeForPlayer2.row && column >= spawnableStartNodeForPlayer2.col && row <= spawnableEndNodeForPlayer2.row && column <= spawnableEndNodeForPlayer2.col)
                {
                    if (!groundNodes[nodeIndex].BlockedNode)
                    {
                        spawnableNodesForPlayer2.Add(nodeIndex);
                    }
                }
                groundNodesMatrix[row, column] = groundNodes[nodeIndex];
                groundNodesMatrix[row, column].name = nodeIndex + "_[" + row + "," + column + "]_" + "MapNode";
                groundNodesMatrix[row, column].Initialize(nodeIndex, column, row);

            }
        }


        // order of edges is left, topLeft, top, topRight, right, bottomRight, bottom, bottomLeft

        for (uint row = 0; row < GroundNodeMatrixDimension.rows; row++)
        {
            for (uint column = 0; column < GroundNodeMatrixDimension.columns; column++)
            {

                uint nodeIndex = row * GroundNodeMatrixDimension.columns + column;

                var edges =
                (GetLeftNode(row, column, groundNodesMatrix), GetTopLeftNode(row, column, groundNodesMatrix),
                GetTopNode(row, column, groundNodesMatrix), GetTopRightNode(row, column, groundNodesMatrix),
                GetRightNode(row, column, groundNodesMatrix), GetBottomRighttNode(row, column, groundNodesMatrix),
                GetBottomNode(row, column, groundNodesMatrix), GetBottomLeftNode(row, column, groundNodesMatrix));

                MapNodesGraph.AddEdges(groundNodesMatrix[row, column], edges);
            }
        }

        player1.Initialize(1, spawnableNodesForPlayer1, MapNodesGraph, player1Territory, totalNodes);
        player2.Initialize(2, spawnableNodesForPlayer2, MapNodesGraph, player2Territory, totalNodes);
        player1.SetOpponentPlayer(player2);
        player2.SetOpponentPlayer(player1);
    }

    GroundNode GetLeftNode (uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (column == 0)
        {
            return null;
        }
        return groundNodesMatrix[row, column - 1];
    }

    GroundNode GetTopLeftNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (column == 0 || row == 0)
        {
            return null;
        }
        return groundNodesMatrix[row - 1, column - 1];
    }

    GroundNode GetTopNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (row == 0)
        {
            return null;
        }
        return groundNodesMatrix[row - 1, column];
    }

    GroundNode GetTopRightNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (column == GroundNodeMatrixDimension.columns - 1 || row == 0)
        {
            return null;
        }
        return groundNodesMatrix[row - 1, column + 1];
    }

    GroundNode GetRightNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (column == GroundNodeMatrixDimension.columns - 1)
        {
            return null;
        }
        return groundNodesMatrix[row, column + 1];
    }

    GroundNode GetBottomRighttNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (column == GroundNodeMatrixDimension.columns - 1 || row == GroundNodeMatrixDimension.rows - 1)
        {
            return null;
        }
        return groundNodesMatrix[row + 1, column + 1];
    }

    GroundNode GetBottomNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (row == GroundNodeMatrixDimension.rows - 1)
        {
            return null;
        }
        return groundNodesMatrix[row + 1, column];
    }

    GroundNode GetBottomLeftNode(uint row, uint column, GroundNode[,] groundNodesMatrix)
    {
        if (column == 0 || row == GroundNodeMatrixDimension.rows - 1)
        {
            return null;
        }
        return groundNodesMatrix[row + 1, column - 1];
    }


    void Update()
    {

    }

    GameData IGameData.GetGameData()
    {
        return Data;
    }
}
