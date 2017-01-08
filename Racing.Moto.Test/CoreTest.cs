using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Racing.Moto.Test
{
    [TestClass]
    public class CoreTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();  //Logger对象代表与当前类相关联的日志消息的来源  

        [TestMethod]
        public void HungarianAlgorithmTest()
        {
            const int SIZE = 10;

            var matrix = generateMatrix(SIZE);

            printMatrix(matrix);

            var algorithm = new Racing.Moto.Core.GraphAlgorithms.HungarianAlgorithm(matrix);

            var result = algorithm.Run();

            printArray(result);

            Console.ReadKey();
        }

        #region HungarianAlgorithmTest
        static int[,] generateMatrix(int size)
        {
            var matrix = new int[size, size];

            var rnd = new Random();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = rnd.Next() % size;

            return matrix;
        }

        static void printMatrix(int[,] matrix)
        {
            Console.WriteLine("Matrix:");
            var size = matrix.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                    Console.Write("{0,5:0}", matrix[i, j]);
                Console.WriteLine();
            }
        }

        static void printArray(int[] array)
        {
            Console.WriteLine("Array:");
            var size = array.Length;
            for (int i = 0; i < size; i++)
                Console.Write("{0,5:0}", array[i]);
            Console.WriteLine();
        }
        #endregion
    }
}
