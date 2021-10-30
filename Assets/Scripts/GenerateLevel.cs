using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public int difficulty = 1;
    public int upperDifficultyBound = 30;
    public int piecesSize = 30;
    public int requiredCollectables; // number of collectibles needed to collect in order to progress
    public int maxCollectables; // maximum number of spawned collectibles
    public GameObject[] pieces; // 0 is the default connecting starting piece, the last is the deadend

    public GameObject collectable;
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

    private bool loaded = false; // true when the procedural generation is over

    private (int, int)[] directionMovements; // how to advance rendering
    public enum MapInfo
    {
        BLOCKED,
        FREE,
        ENEMY,
        PLAYER,
        COLLECTABLE,
    }

    public Dictionary<(int, int), MapInfo> map;

    // Start is called before the first frame update
    void Start()
    {
        difficulty = Mathf.Max(1, Mathf.Min(difficulty, upperDifficultyBound));
        visited = new Dictionary<(int, int), bool>();
        map = new Dictionary<(int, int), MapInfo>();
        directionMovements = new (int, int)[]{(0, -piecesSize), (-piecesSize, 0), (0, piecesSize), (piecesSize, 0)};
        RecursiveGeneration(0, 0, -1);
        GenerateCollectibles();
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
            Instantiate(pieces[0], Vector3.zero, Quaternion.identity);
            nextDirections = "1111";
            AddInfoToMap(0, 0, freePaths[0]);
            map[(0, 0)] = MapInfo.PLAYER;
        } else {
            // generate 2nd+ piece

            int chosenPieceIndex = pieces.Length - 1;
            currentDirection = (currentDirection + 2) % 4; // we must rotate the down free spot to match the current facing direction

            // in this case, generate a deadend
            if (depth > difficulty * 2 && (depth > difficulty * 4 || Random.Range(0, 4) == 0)) {
                Instantiate(pieces[chosenPieceIndex], new Vector3(x, 0, z), Quaternion.Euler(0, currentDirection * 90f, 0));
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
                Instantiate(pieces[chosenPieceIndex], new Vector3(x, 0, z), Quaternion.Euler(0, rotationDir * 90f, 0));
            }

            AddInfoToMap(x, z, nextDirections);
        }

        // generate neighbouring pieces
        for (int i = 0; i < 4; ++i) {
            if (nextDirections[i] == '1')
                RecursiveGeneration(x + this.directionMovements[i].Item1, z + this.directionMovements[i].Item2, i, depth + 1);
        }
    }

    private void GenerateCollectibles() {
        var locations = new List<(int, int)>(map.Keys);
        for (int i = 0; i < maxCollectables; ++i) {
            int posX, posZ;
            (posX, posZ) = locations[Random.Range(0, map.Keys.Count)];
            if (map[(posX, posZ)] == MapInfo.FREE) {
                Instantiate(collectable, new Vector3(posX, 3.0f, posZ), Quaternion.identity);
                map[(posX, posZ)] = MapInfo.COLLECTABLE;
            } else {
                --i;
            }
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool checkLoadCompletion() {
        return this.loaded;
    }
}
