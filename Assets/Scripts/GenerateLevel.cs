using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenerateLevel : MonoBehaviour
{
    public int difficulty = 1;
    public int upperDifficultyBound = 30;
    public int piecesSize = 30;
    private int requiredCollectables; // number of collectibles needed to collect in order to progress
    private int maxCollectables; // maximum number of spawned collectibles
    public GameObject[] pieces; // 0 is the default connecting starting piece, the last is the deadend
    public GameObject mazeParent; // object at the top of the maze hierarchy
    public GameObject collectableParent; // object at the top of the collectables hierarchy
    public GameObject collectable;
    public GameObject floor;
    public float minimumDistanceBetweenCollectibles = 10.0f;
    public string[] freePaths;
    /*
        Strings are related to the connectivity of the pieces
        example:
            freePaths[3] = abcd <=> pieces[3] has the connectivity abcd, where:
            a -> 1 if path is free downwards, 0 otherwise
            b -> 1 if path is free leftwards, 0 otherwise
            c -> 1 if path is free upwards, 0 otherwise
            d -> 1 if path is free rightwards, 0 otherwise
    */

    private Dictionary<(int, int), bool> visited;
    private List<GameObject> mazeRooms;

    private bool loaded = false; // true when the procedural generation is over

    private (int, int)[] directionMovements; // how to advance rendering

    /* TODO Later -- Check if this piece of code will be needed later
    public enum MapInfo
    {
        BLOCKED,
        FREE,
        ENEMY,
        PLAYER,
        COLLECTABLE,
    }

    public Dictionary<(int, int), MapInfo> map;
    */
    public Dictionary<(float, float), string> map;

    public EnemyUnits enemyUnits;

    private float floorMinX = 0, floorMinZ = 0, floorMaxX = 0, floorMaxZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        difficulty = Mathf.Max(1, Mathf.Min(difficulty, upperDifficultyBound));
        visited = new Dictionary<(int, int), bool>();
        mazeRooms = new List<GameObject>();
        // TODO Later -- Check if this piece of code will be needed later map = new Dictionary<(int, int), MapInfo>();
        map = new Dictionary<(float, float), string>();
        directionMovements = new (int, int)[]{
            (0, -piecesSize * (int) mazeParent.transform.localScale.z),
            (-piecesSize * (int) mazeParent.transform.localScale.x, 0),
            (0, piecesSize * (int) mazeParent.transform.localScale.z),
            (piecesSize * (int) mazeParent.transform.localScale.x, 0)
        };
        RecursiveGeneration(0, 0, -1);
        floorMinZ += directionMovements[0].Item2;
        floorMinX += directionMovements[1].Item1;
        floorMaxZ += directionMovements[2].Item2;
        floorMaxX += directionMovements[3].Item1;
        // Instantiate floor
        Debug.Log($"{floorMinX} {floorMaxX} {floorMinZ} {floorMaxZ}");
        Debug.Log($"{piecesSize} {mazeParent.transform.localScale.x} {mazeParent.transform.localScale.z}");
        floor.transform.position = new Vector3(
            (floorMaxX + floorMinX) / 2,
            (floor.transform.position.y),
            (floorMaxZ + floorMinZ) / 2
        );
        floor.transform.localScale = new Vector3((floorMaxX - floorMinX) / piecesSize, 1, (floorMaxZ - floorMinZ) / piecesSize);
        floor.GetComponent<NavMeshSurface>().BuildNavMesh();
        GenerateCollectibles();
        GenerateNavMeshes();
        GenerateEnemies();
        loaded = true;
    }

    private void RecursiveGeneration(int x, int z, int currentDirection, int depth = 0) {
        // check if that component was already generated
        if (visited.ContainsKey((x, z))) {
            return;
        }

        // mark current location as generated
        visited.Add((x, z), true);
        string nextDirections = "0000";
        // first piece
        if (x == 0 && z == 0) {
            mazeRooms.Add(Instantiate(pieces[0], Vector3.zero, Quaternion.identity, mazeParent.transform));
            nextDirections = "1111";
            AddInfoToMap(0, 0, freePaths[0]);
            // TODO Later -- Check if this piece of code will be needed later map[(0, 0)] = MapInfo.PLAYER;
        } else {
            // generate 2nd+ piece

            int chosenPieceIndex = pieces.Length - 1;
            currentDirection = (currentDirection + 2) % 4; // we must rotate the down free spot to match the current facing direction

            // in this case, generate a deadend
            if (depth > Mathf.Sqrt(difficulty) * 2 && (depth > Mathf.Sqrt(difficulty) * 4 || Random.Range(0, 4) == 0)) {
                mazeRooms.Add(Instantiate(pieces[chosenPieceIndex], new Vector3(x, 0, z), Quaternion.Euler(0, currentDirection * 90f, 0), mazeParent.transform));
                nextDirections = Rotate(freePaths[chosenPieceIndex], currentDirection);
            } else {
                // generate a continuation piece
                int rotationDir;
                while (true) {
                    chosenPieceIndex = Random.Range(0, pieces.Length - 2);
                    rotationDir = Random.Range(0, 3);
                    nextDirections = Rotate(freePaths[chosenPieceIndex], rotationDir);
                    if (nextDirections[currentDirection] == '1')
                        break;
                }
                mazeRooms.Add(Instantiate(pieces[chosenPieceIndex], new Vector3(x, 0, z), Quaternion.Euler(0, rotationDir * 90f, 0), mazeParent.transform));
            }
            floorMinX = Mathf.Min(floorMinX, x);
            floorMinZ = Mathf.Min(floorMinZ, z);
            floorMaxX = Mathf.Max(floorMaxX, x);
            floorMaxZ = Mathf.Max(floorMaxZ, z);
            AddInfoToMap(x, z, nextDirections);
        }

        // generate neighbouring pieces
        for (int i = 0; i < 4; ++i) {
            if (nextDirections[i] == '1')
                RecursiveGeneration(x + this.directionMovements[i].Item1, z + this.directionMovements[i].Item2, i, depth + 1);
        }
    }

    private void GenerateCollectibles() {
        maxCollectables = difficulty * 2;
        requiredCollectables = (int) Mathf.Sqrt(maxCollectables);
        Dictionary<(float, float), bool> placedCollectibles = new Dictionary<(float, float), bool>();
        List<(float, float)> spawnedCollectibles = new List<(float, float)>();
        var locations = new List<(float, float)>(map.Keys);
        int minMaxBounds = (this.piecesSize - 1) / 2; // room maximum bounds
        int guaranteedFreeBounds = (this.piecesSize - 2 * (this.piecesSize - 1) / 6) / 2;
        int guaranteedNoClipWallsBounds = (int) (guaranteedFreeBounds - collectableParent.transform.localScale.y);
        float roomXCoord, roomZCoord, spawnX, spawnZ;
        for (int i = 0; i < maxCollectables; ++i) {
            // get a random room
            (roomXCoord, roomZCoord) = locations[Random.Range(0, map.Keys.Count)];
            // get a random coord in this room which doesn't clip into a wall
            spawnX = Random.Range(-guaranteedNoClipWallsBounds * 1.0f + roomXCoord, guaranteedNoClipWallsBounds + roomXCoord);
            spawnZ = Random.Range(-guaranteedNoClipWallsBounds * 1.0f + roomZCoord, guaranteedNoClipWallsBounds + roomZCoord);
            // check if the collectible is too close to another collectible
            bool distanceRespectedFlag = true;
            foreach ((float, float) col in spawnedCollectibles) {
                if (Mathf.Sqrt(
                    (col.Item1 - spawnX) * (col.Item1 - spawnX) +
                    (col.Item2 - spawnZ) * (col.Item2 - spawnZ)
                ) < minimumDistanceBetweenCollectibles) {
                    distanceRespectedFlag = false;
                    break;
                }
            }
            if (!distanceRespectedFlag) {
                i--;
                continue;
            }

            GameObject collObj = Instantiate(collectable, new Vector3(spawnX, 0, spawnZ), Quaternion.identity, collectableParent.transform);
            /* make the collectible responsive to scaling */
            collObj.transform.localPosition = new Vector3(
                    collObj.transform.localPosition.x,
                    0.5f + 2.5f / collectableParent.transform.localScale.y,
                    collObj.transform.localPosition.z
            );
            spawnedCollectibles.Add((spawnX, spawnZ));
        }
    }

    public Vector3 GetStartingPosition()
    {
        return mazeParent.transform.position + new Vector3(0,10,0);
    }

    public void GenerateNavMeshes()
    {
        foreach(GameObject go in mazeRooms)
        {
            NavMeshSurface nms = go.GetComponent<NavMeshSurface>();
            if(nms != null)
            {
                nms.BuildNavMesh();
            }
        }
    }

    public void GenerateEnemies()
    {
        foreach (GameObject go in mazeRooms)
        {
            GameObject er = enemyUnits.GetRandomEnemy();
            Instantiate(er, go.transform, false);
        }
    }

    private string Rotate(string connectivity, int dir) {
        switch (dir) {
            case 0:
                return connectivity;
            case 1:
                return connectivity[3] + connectivity.Substring(0, 3);
            case 2:
                return connectivity.Substring(2, 2) + connectivity.Substring(0, 2);
            case 3:
                return connectivity.Substring(1, 3) + connectivity[0];
            default:
                return null;
        }
    }

    private void AddInfoToMap(int x, int z, string connectivity) {
        map.Add((x, z), connectivity);
        /* TODO Later -- Check if this piece of code will be needed later
        int minMaxBounds = (this.piecesSize - 1) / 2;
        int guaranteedFreeBounds = (this.piecesSize - 2 * (this.piecesSize - 1) / 6) / 2;

        // middle is always free
        for (int i = x - guaranteedFreeBounds; i <= x + guaranteedFreeBounds; ++i) {
            for (int j = z - guaranteedFreeBounds; j <= z + guaranteedFreeBounds; ++j) {
                map.Add((i, j), MapInfo.FREE);
            }
        }

        // corners are always blocked
        for (int i = x - minMaxBounds; i < x - guaranteedFreeBounds; ++i) {
            for (int j = z - minMaxBounds; j < z - guaranteedFreeBounds; ++j) {
                map.Add((i, j), MapInfo.BLOCKED);
            }
        }

        for (int i = x + guaranteedFreeBounds + 1; i <= x + minMaxBounds; ++i) {
            for (int j = z - minMaxBounds; j < z - guaranteedFreeBounds; ++j) {
                map.Add((i, j), MapInfo.BLOCKED);
            }
        }

        for (int i = x - minMaxBounds; i < x - guaranteedFreeBounds; ++i) {
            for (int j = z + guaranteedFreeBounds + 1; j <= z + minMaxBounds; ++j) {
                map.Add((i, j), MapInfo.BLOCKED);
            }
        }

        for (int i = x + guaranteedFreeBounds + 1; i <= x + minMaxBounds; ++i) {
            for (int j = z + guaranteedFreeBounds + 1; j <= z + minMaxBounds; ++j) {
                map.Add((i, j), MapInfo.BLOCKED);
            }
        }

        // directional posibilities
        MapInfo fillValue = connectivity[0] == '1' ? MapInfo.FREE : MapInfo.BLOCKED;
        for (int i = x - guaranteedFreeBounds; i <= x + guaranteedFreeBounds; ++i) {
            for (int j = z + guaranteedFreeBounds + 1; j <= z + minMaxBounds; ++j) {
                map.Add((i, j), fillValue);
            }
        }

        fillValue = connectivity[1] == '1' ? MapInfo.FREE : MapInfo.BLOCKED;
        for (int i = x - minMaxBounds; i < x - guaranteedFreeBounds; ++i) {
            for (int j = z - guaranteedFreeBounds; j <= z + guaranteedFreeBounds; ++j) {
                map.Add((i, j), fillValue);
            }
        }

        fillValue = connectivity[2] == '1' ? MapInfo.FREE : MapInfo.BLOCKED;
        for (int i = x - guaranteedFreeBounds; i <= x + guaranteedFreeBounds; ++i) {
            for (int j = z - minMaxBounds; j < z - guaranteedFreeBounds; ++j) {
                map.Add((i, j), fillValue);
            }
        }

        fillValue = connectivity[3] == '1' ? MapInfo.FREE : MapInfo.BLOCKED;
        for (int i = x + guaranteedFreeBounds + 1; i <= x + minMaxBounds; ++i) {
            for (int j = z - guaranteedFreeBounds; j <= z + guaranteedFreeBounds; ++j) {
                map.Add((i, j), fillValue);
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool checkLoadCompletion() {
        return this.loaded;
    }
}
