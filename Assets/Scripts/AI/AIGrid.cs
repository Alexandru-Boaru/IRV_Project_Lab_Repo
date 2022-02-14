using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIGrid : MonoBehaviour
{
	public Transform player;
	public LayerMask unwalkableMask;
	public LayerMask enemyMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;

	public int squareChunksX = 1;
	public int squareChunksY = 1;
	public Chunk[,] chunks;
	public List<Chunk> chunksToRefresh;
	

	AINode[,] nodes;
	float nodeDiameter;
	int gridSizeX, gridSizeY;
	public List<AINode> path;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		chunksToRefresh = new List<Chunk>();
		CreateGrid();
	}

    private void FixedUpdate()
    {
		RefreshChunks();
    }

    private void Update()
    {
    }

    private void LateUpdate()
    {
		foreach(Chunk chunk in chunks)
        {
			chunk.refreshed = false;
        }
	}

	void RefreshChunks()
    {
		AINode node = NodeFromWorldPoint(player.position);
		AddToChunksToRefresh(node.chunk);

		List<Chunk> neighbours = node.chunk.GetChunkNeighbours();

		foreach (Chunk neighbour in neighbours)
		{
			AddToChunksToRefresh(neighbour);
		}

		foreach (Chunk chunk in chunksToRefresh)
		{
			chunk.RefreshNodes();
		}
		chunksToRefresh.Clear();
	}

	public void AddToChunksToRefresh(Chunk chunk)
    {
		if (!chunksToRefresh.Contains(chunk)) chunksToRefresh.Add(chunk);
	}

	// Initialize grid
	void CreateGrid()
	{
		int chunksX = (int)(squareChunksX / nodeRadius);
		int chunksY = (int)(squareChunksY / nodeRadius);
		chunks = new Chunk[chunksX, chunksY];
		int chunkSize = gridSizeX / squareChunksX;
		for (int x = 0; x < chunksX; x++)
		{
			for (int y = 0; y < chunksY; y++)
			{
				chunks[x, y] = new Chunk(chunkSize, x, y, this);
			}
		}

		nodes = new AINode[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

				nodes[x, y] = new AINode(AINode.Status.WALKABLE, worldPoint, x, y);
				UpdateNodeStatus(nodes[x, y], worldPoint);

                try{
					chunks[x / chunkSize, y / chunkSize].AddNode(x, y, ref nodes[x, y]);
                }
                catch
                {
					Debug.Log(x + "  " + y);
                }
			}
		}
	}

	public void UpdateNodeStatus(AINode node, Vector3 worldPoint)
    {
		if (!Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask) && !Physics.CheckSphere(worldPoint, nodeRadius, enemyMask))
		{
			node.status = AINode.Status.WALKABLE;
			node.enemyName = "";
			node.strenght = 2;
		}
		else if (!Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
		{
            if (node.status != AINode.Status.ENEMY)
            {
			node.status = AINode.Status.ENEMY;
			node.enemyName = Physics.OverlapSphere(worldPoint, nodeRadius, enemyMask)[0].name;
				node.strenght = 1;
			}

		}
		else
		{
			node.status = AINode.Status.OBSTACLE;
			node.enemyName = Physics.OverlapSphere(worldPoint, nodeRadius, unwalkableMask)[0].name;
			node.strenght = 0;
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

	public Chunk ChunkFromWorldPoint(Vector3 worldPosition)
	{
		float x = worldPosition.x;
		float y = worldPosition.z;

		foreach (Chunk chunk in chunks)
        {
			if(chunk.hasPoint(x, y))
            {
				return chunk;
            }
        }
		return null;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (nodes != null)
        {
            foreach (AINode n in nodes)
            {
                switch (n.status)
                {
					case AINode.Status.WALKABLE:
						Gizmos.color = Color.white;
						break;
					case AINode.Status.OBSTACLE:
						Gizmos.color = Color.blue;
						break;
					case AINode.Status.ENEMY:
						Gizmos.color = Color.red;
						break;
					default:
					break;
                }

                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}

public class Chunk{
	AINode[,] nodes;
	int size;
	int gridX,gridY;
	int minMarkerX, minMarkerY;
	int maxMarkerX, maxMarkerY;
	AIGrid grid;
	public bool refreshed;

	public Chunk(int _size, int _gridX, int _gridY, AIGrid _grid)
    {
		size = _size;

		gridX = _gridX;
		gridY = _gridY;

		minMarkerX = gridX * size;
		minMarkerY = gridY * size;
		maxMarkerX = minMarkerX + size;
		maxMarkerY = minMarkerY + size;

		grid = _grid;

		nodes = new AINode[_size, _size];
    }
	public void AddNode(int x, int y, ref AINode a)
    {
		nodes[x - minMarkerX, y - minMarkerY] = a;
		a.chunk = this;
    }

	public bool hasPoint(float x, float y)
    {
		return (x >= minMarkerX && x < maxMarkerX && y >= minMarkerY && y < maxMarkerY);
    }

	public void RefreshNodes()
    {
		if (!refreshed)
		{
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					Vector3 worldPoint = nodes[x, y].worldPosition;
					grid.UpdateNodeStatus(nodes[x, y], worldPoint);
				}
			}
			refreshed = true;
		}
	}

	public List<Chunk> GetChunkNeighbours()
    {
		List<Chunk> neighbours = new List<Chunk>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = this.gridX + x;
				int checkY = this.gridY + y;

				if (checkX >= 0 && checkX < grid.squareChunksX && checkY >= 0 && checkY < grid.squareChunksY)
				{
					neighbours.Add(grid.chunks[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}
}