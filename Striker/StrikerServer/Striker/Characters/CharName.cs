using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Striker.Characters
{
    public static class CharName
    {

        private static Random random = new Random();
        private static List<String> names;
        private static List<String> usedNames = new List<string>();

        public static void BuildCache(string fileContents)
        {
            names = fileContents.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static void BuildCache()
        {
            BuildCache(UnityEngine.Resources.Load("names").ToString());
        }

        public static string GetName()
        {
            if (names == null)
            {
                BuildCache();
            }

            String name = null;

            do
            {
                name = names[random.Next(0, names.Count)];
            } while (usedNames.Contains(name));

            usedNames.Add(name);

            return name;
        }

    }
}
