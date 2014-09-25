using Striker.Levels.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Levels
{
    public static class LevelLoader
    {

        public static Level LoadLevel(string levelString)
        {
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
                    e.Start();

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

            level.BuildSpawnList();

            return level;
        }

        private static Element CreateGameObject(Level level, char p)
        {
            switch (p)
            {
                case 'F': return CreateFloorObject(level);
                case 'W': return CreateWallObject(level);
                case 'P': return CreateLavaObject(level);
            }

            return null;
        }

        private static Element CreateLavaObject(Level level)
        {
            return new Lava();
        }

        private static Element CreateWallObject(Level level)
        {
            return new Wall();
        }

        private static Element CreateFloorObject(Level level)
        {
            return new Floor();
        }

    }
}
