using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    public Transform LevelParent;

    public GameObject FloorObject;
    public GameObject WallObject;
    public GameObject LavaObject;

    void Start()
    {
    }

    public void LoadLevel(string name)
    {
        string levelString = Resources.Load("Levels/" + name).ToString();
        Level level = new Level();

        string[] lines = levelString.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        level.Height = lines.Length;

        foreach (var line in lines)
        {
            string[] tiles = line.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            level.Width = tiles.Length;

            foreach (var tile in tiles.Select(t => t.Trim()))
            {
                Element e = CreateGameObject(level, tile[0]);

                for (int i = 1; i < tile.Length; i++)
                {
                    switch (tile[i])
                    {
                        case 'I': e.isItemSpawner = true; break;
                        case 'S': e.isSpawnLocation = true; break;
                    }
                }

                level.map.Add(e);
            }

        }

    }

    private Element CreateGameObject(Level level, char p)
    {
        switch (p)
        {
            case 'F': return CreateFloorObject(level);
            case 'W': return CreateWallObject(level);
            case 'P': return CreateLavaObject(level);
        }

        return null;
    }

    private Element CreateLavaObject(Level level)
    {
        int x = level.map.Count % level.Width;
        int y = level.map.Count / level.Width;

        x *= (int)FloorObject.renderer.bounds.size.x;
        y *= (int)FloorObject.renderer.bounds.size.y;

        GameObject go = (GameObject)GameObject.Instantiate(LavaObject, new Vector3(x, FloorObject.renderer.bounds.size.y * (level.Height - 1) - y, -1), Quaternion.Euler(Vector3.zero));
        go.transform.parent = LevelParent;

        return go.GetComponent<Lava>();
    }

    private Element CreateWallObject(Level level)
    {
        int x = level.map.Count % level.Width;
        int y = level.map.Count / level.Width;

        x *= (int)FloorObject.renderer.bounds.size.x;
        y *= (int)FloorObject.renderer.bounds.size.y;

        GameObject go = (GameObject)GameObject.Instantiate(WallObject, new Vector3(x, FloorObject.renderer.bounds.size.y * (level.Height - 1) - y, 0), Quaternion.Euler(Vector3.zero));
        go.transform.parent = LevelParent;

        return go.GetComponent<Wall>();
    }

    private Element CreateFloorObject(Level level)
    {
        int x = level.map.Count % level.Width;
        int y = level.map.Count / level.Width;

        x *= (int)FloorObject.renderer.bounds.size.x;
        y *= (int)FloorObject.renderer.bounds.size.y;

        GameObject go = (GameObject)GameObject.Instantiate(FloorObject, new Vector3(x, FloorObject.renderer.bounds.size.y * (level.Height - 1) - y, 0), Quaternion.Euler(Vector3.zero));
        go.transform.parent = LevelParent;

        return go.GetComponent<Floor>();
    }

}
