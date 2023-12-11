using System.Collections.Generic;
using UnityEngine;

public class GroundNode : MonoBehaviour
{
    private List<GameBody> GameBodiesInNode = new List<GameBody>();
    private uint NodeIndex;

    public bool BlockedNode;
    public bool IsBridgeNode;
    public List<uint> TeamsOnNode = new List<uint>();
    public MapGraph.Direction Quad;
    public GroundNode Left;
    public GroundNode Top;
    public GroundNode TopRight;
    public GroundNode Right;
    public GroundNode TopLeft;
    public GroundNode BottomRight;
    public GroundNode Bottom;
    public GroundNode BottomLeft;

    public void Initialize(uint index, float xOffset, float yOffset)
    {
        NodeIndex = index;
    }

    public bool IsAPathNode()
    {
        return !BlockedNode || IsBridgeNode;
    }

    public uint GetNodeIndex()
    {
        return NodeIndex;
    }

    public bool IsOccupied()
    {
        return GameBodiesInNode.Count != 0;
    }

    public List<GameBody> GetGameBodiesInNode()
    {
        return GameBodiesInNode;
    }

    public GameBody GetOpponent(uint playerTeam)
    {
        for (int i = 0; i < GameBodiesInNode.Count; i++)
        {
            if (GameBodiesInNode[i].GetMyPlayer().GetMyTeam() != playerTeam)
            {
                return GameBodiesInNode[i];
            }
        }
        Debug.LogError("Opponent lost");
        return null;
    }

    public bool HasOpponent(uint playerTeam)
    {
        return TeamsOnNode.Count > 0 && !TeamsOnNode.Contains(playerTeam);
    }

    public void EnterNode(GameBody gameBody)
    {
        GameBodiesInNode.Add(gameBody);
        uint team = gameBody.GetMyPlayer().GetMyTeam();
        {
            TeamsOnNode.Add(team);
        }
    }

    public void LeaveNode(GameBody gameBody)
    {
        GameBodiesInNode.Remove(gameBody);
        TeamsOnNode.Remove(gameBody.GetMyPlayer().GetMyTeam());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameBody collidedBody = collision.GetComponent<GameBody>();

        if (collidedBody)
        {
            collidedBody.SetGroundNode(this);
            EnterNode(collidedBody);
        }
    }
}
