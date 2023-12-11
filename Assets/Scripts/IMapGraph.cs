
public interface IMapGraph
{
    public GameBody BreadthFirstSearchForOpponent(GroundNode startNode, uint team, uint radius, bool lookForTower = false);
    public GroundNode DepthFirstSearchPartGraph(GroundNode startNode, GroundNode lastNode, uint nodeIndex, bool[] visited);
    public MapGraph.Direction GetQuad(GroundNode groundNode);
    public System.Collections.Generic.List<GroundNode> AstarPath(GroundNode startNode, GroundNode goalNode);
    public GroundNode BreadthFirstSearch(GroundNode startNode, GroundNode goalNode = null, uint playerTeam = 0, uint radius = 0);
    public double GridDiagonalDistance(uint fromIndex, uint toIndex);
}
