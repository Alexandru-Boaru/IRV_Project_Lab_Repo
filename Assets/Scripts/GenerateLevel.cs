using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public int difficulty = 1;
    public int upperDifficultyBound = 30;
    public GameObject[] pieces; // 0 is the default connecting starting piece, the last is the deadend
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

    private (int, int)[] directionMovements = {(0, -5), (-5, 0), (0, 5), (5, 0)}; // how to advance rendering
    public enum MapInfo
    {
        BLOCKED,
        FREE,
        ENEMY,
        PLAYER
    }

    public Dictionary<(int, int), MapInfo> map;

    // Start is called before the first frame update
    void Start()
    {
        difficulty = Mathf.Max(1, Mathf.Min(difficulty, upperDifficultyBound));
        visited = new Dictionary<(int, int), bool>();
        map = new Dictionary<(int, int), MapInfo>();
        RecursiveGeneration(0, 0, -1);
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
        // middle is always free
        for (int i = x - 1; i <= x + 1; ++i) {
            for (int j = z - 1; j <= z + 1; ++j) {
                map.Add((i, j), MapInfo.FREE);
            }
        }

        // corners are always blocked
        map.Add((x - 2, z - 2), MapInfo.BLOCKED);
        map.Add((x - 2, z + 2), MapInfo.BLOCKED);
        map.Add((x + 2, z - 2), MapInfo.BLOCKED);
        map.Add((x + 2, z + 2), MapInfo.BLOCKED);

        // directional posibilities
        map.Add((x - 2, z), connectivity[1] == '1' ? MapInfo.FREE : MapInfo.BLOCKED);
        map.Add((x + 2, z), connectivity[3] == '1' ? MapInfo.FREE : MapInfo.BLOCKED);
        map.Add((x, z - 2), connectivity[0] == '1' ? MapInfo.FREE : MapInfo.BLOCKED);
        map.Add((x, z + 2), connectivity[2] == '1' ? MapInfo.FREE : MapInfo.BLOCKED);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool checkLoadCompletion() {
        return this.loaded;
    }
}
