using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AINode
{

	public Status status = Status.OBSTACLE;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public string enemyName = "";
	public int gCost; // make array
	public int hCost; // make array
	public AINode parent; // make array
	public Chunk chunk;
	public int strenght = 1; // make array

	public enum Status
	{
		WALKABLE,
		OBSTACLE,
		ENEMY
	}

	public AINode(Status _status, Vector3 _worldPos, int _gridX, int _gridY)
	{
		status = _status;
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