using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode
{
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public AINode parent;

	public AINode(bool _walkable, Vector3 _worldPos, int _gridX = 0, int _gridY = 0)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}
