using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPathfinding : MonoBehaviour
{

	public Transform target;
	public List<AINode> path;
	public Vector3 nextStop;
	public AIGrid grid;
	Vector3 scale;

	void Awake()
	{
		//	grid = GetComponent<AIGrid>();
		nextStop = transform.position;
		scale = transform.lossyScale;
	}


	public void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		AINode startNode = grid.NodeFromWorldPoint(startPos);
		AINode targetNode = grid.NodeFromWorldPoint(targetPos);

		//startNode.chunk.RefreshNodes();

		List<AINode> openSet = new List<AINode>();
		HashSet<AINode> closedSet = new HashSet<AINode>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			AINode node = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				RetracePath(startNode, targetNode);
				return;
			}

			foreach (AINode neighbour in grid.GetNeighbours(node))
			{
				if ((neighbour.status.Equals(AINode.Status.OBSTACLE) && neighbour != targetNode) || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) * node.strenght;
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	void RetracePath(AINode startNode, AINode endNode)
	{
		List<AINode> currentPath = new List<AINode>();
		AINode currentNode = endNode;

		while (currentNode != startNode)
		{
			currentPath.Add(currentNode);
			currentNode = currentNode.parent;
		}
		currentPath.Reverse();

		path = currentPath;
		nextStop = path.Count > 0? path[0].worldPosition : nextStop;
	}

	int GetDistance(AINode nodeA, AINode nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		// calculate diagonal distance and what's left as a straight line
		// 14 units when moving on the diagonal
		// 10 units when moving on x or y
		return 14 * Mathf.Min(dstX, dstY) + 10 * Mathf.Abs(dstX - dstY);
	}
}