using Striker.Levels.Elements;
using Striker.States.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Levels
{
    public class Level
    {

        public List<Element> map;
        public int Width;
        public int Height;
        public List<SpawnPoint> availableSpawns;

        public Level()
        {
            map = new List<Element>();
        }

        public void BuildSpawnList()
        {
            availableSpawns = map.Where(e => e.isSpawnLocation).Select(e =>
            {
                int x = map.IndexOf(e) % Width;
                int y = map.IndexOf(e) / Width;
                return new SpawnPoint() { x = x, y = y };
            }).ToList().Shuffle();
        }

        public Element GetMapTile(float x, float y)
        {
            int indexX = (int)((x + 1f) / 2f);
            int indexY = (int)(Height - ((y + 1f)  / 2f));

            if (indexX < 0 || indexX > Width - 1)
            {
                return null;
            }

            if (indexY < 0 || indexY > Height - 1)
            {
                return null;
            }

            int index = indexY * Width + indexX;
            return map[index];
        }

        public Element GetMapTileNormalized(int x, int y)
        {
            int index = y * Width + x;
            return map[index];
        }

    }
}
