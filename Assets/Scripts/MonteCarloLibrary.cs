using System;
using System.Collections.Generic;

namespace MonteCarloLibrary{
    public static class MonteCarloGenerator{
        private static Random random = new Random();

        public static double[] MonteCarlo(int quantity){
            List<double> generated_nums = new List<double>();
            Random random = new Random();
            int seed = (int)(random.NextDouble() * 10000);
            Console.WriteLine("Seed: " + seed);
            int xi = seed;
            while (quantity > 0){
                int x2i = xi * xi;
                int extension = x2i.ToString().Length;
                quantity--;
                int extracted = extractNums(x2i, extension);
                generated_nums.Add((double)extracted / 10000);
                xi = extracted;
            }

            return generated_nums.ToArray();
        }

        private static int extractNums(int num, int extension)
        {
            int segmento = 0;
            string num_str = num.ToString();
            int inicio = (extension - 4) / 2;
            int fin = inicio + 4;

            if (num_str.Length >= 4)
            {
                segmento = int.Parse(num_str.Substring(inicio, 4));
            }

            return segmento;
        }
    }
}
