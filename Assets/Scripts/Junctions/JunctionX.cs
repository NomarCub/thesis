using System.Collections.Generic;

public abstract class JunctionX : Junction
{
    protected Node NodeA;
    protected Node NodeB;
    protected Node NodeC;
    protected Node NodeD;
    protected Node NodeE;
    protected Node NodeF;
    protected Node NodeG;
    protected Node NodeH;
    protected List<(Node, Node)> rightTurns;
    protected List<(Node, Node)> leftTurns;
    protected List<(Node, Node)> straights;
    protected List<((Node, Node), (Node, Node))> straightsCrossing;
    protected List<((Node, Node), (Node, Node))> leftTurnsCrossing;

    protected virtual void Start()
    {
        var nodes = gameObject.GetComponentsInChildren<Node>();

        NodeA = nodes[0];
        NodeB = nodes[1];
        NodeC = nodes[2];
        NodeD = nodes[3];
        NodeE = nodes[4];
        NodeF = nodes[5];
        NodeG = nodes[6];
        NodeH = nodes[7];

        rightTurns = new List<(Node, Node)> {
            (NodeA, NodeB),
            (NodeC, NodeD),
            (NodeE, NodeF),
            (NodeG, NodeH)
        };

        leftTurns = new List<(Node, Node)> {
            (NodeA, NodeF),
            (NodeC, NodeH),
            (NodeE, NodeB),
            (NodeG, NodeD)
        };

        straights = new List<(Node, Node)> {
            (NodeA, NodeD),
            (NodeC, NodeF),
            (NodeE, NodeH),
            (NodeG, NodeB)
        };

        // TODO: Node pairs would be enough
        straightsCrossing = new List<((Node, Node), (Node, Node))> {
            ((NodeA, NodeD), (NodeC, NodeF)),
            ((NodeA, NodeD), (NodeG, NodeB)),
            ((NodeE, NodeH), (NodeC, NodeF)),
            ((NodeE, NodeH), (NodeG, NodeB)),
        };

        // TODO: Node pairs would be enough
        leftTurnsCrossing = new List<((Node, Node), (Node, Node))> {
            ((NodeA, NodeF),(NodeC, NodeH)),
            ((NodeA, NodeF),(NodeG, NodeD)),
            ((NodeE, NodeB),(NodeC, NodeH)),
            ((NodeE, NodeB),(NodeG, NodeD))
        };
    }

    protected bool Intersect((Node from, Node to) path, (Node from, Node to) otherPath)
    {
        if (path.from == otherPath.from)
            return false;
        if (path.to == otherPath.to)
            return true;
        if (rightTurns.Contains(path) || rightTurns.Contains(otherPath))
        {
            return false;
        }
        // no right turns left
        if (straights.Contains(path) && straights.Contains(otherPath))
        {
            if (straightsCrossing.Contains((path, otherPath)) || straightsCrossing.Contains((otherPath, path)))
                return true;
            else return false;
        }
        // two left turns or a left turn and a straight left
        if (!(leftTurns.Contains(path) && leftTurns.Contains(otherPath)))
        {
            return true;
        }
        // only left turns left
        if (leftTurnsCrossing.Contains((path, otherPath)) || leftTurnsCrossing.Contains((otherPath, path)))
            return true;
        else return false;
    }

}