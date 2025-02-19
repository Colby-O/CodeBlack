using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack.Helpers
{
    public class NameGenerator
    {
        public static string GenerateName()
        {
            string[] firstNames = ((TextAsset)Resources.Load("Data/first-names")).text.Split("\n");
            string[] lastNames = ((TextAsset)Resources.Load("Data/last-names")).text.Split("\n");

            return firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
        }

        public static List<string> GenerateNames(int n)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < n; i++)
            {
                names.Add(GenerateName());
            }

            return names;
        }
    }
}
