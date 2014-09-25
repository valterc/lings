using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker
{
    public static class Extensions
    {

        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random random = new Random();
            int[] rValues = new int[list.Count];

            for (int i = 0; i < rValues.Length; i++)
            {
                rValues[i] = random.Next();
            }

            List<T> newList = list.OrderBy(e => rValues[list.IndexOf(e)]).ToList();
            return newList;
        }

    }
}
