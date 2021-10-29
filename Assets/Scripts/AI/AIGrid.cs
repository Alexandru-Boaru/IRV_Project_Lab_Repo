using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIGrid : MonoBehaviour
{

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	AINode[,] nodes;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid();
	}

	// Initialize grid
	void CreateGrid()
	{
		nodes = new AINode[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				nodes[x, y] = new AINode(walkable, worldPoint, x, y);
			}
		}
	}

	public List<AINode> GetNeighbours(AINode node)
	{
		List<AINode> neighbours = new List<AINode>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(nodes[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}


	public AINode NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x / gridWorldSize.x + 0.5f) ;
		float percentY = (worldPosition.z / gridWorldSize.y + 0.5f);
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.FloorToInt(Mathf.Min(gridSizeX * percentX, gridSizeX - 1));
		int y = Mathf.FloorToInt(Mathf.Min(gridSizeY * percentY, gridSizeY - 1));
		return nodes[x, y];
	}

	public List<AINode> path;
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

		if (nodes != null)
		{
			foreach (AINode n in nodes)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				if (path != null)
					if (path.Contains(n))
						Gizmos.color = Color.black;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}
}