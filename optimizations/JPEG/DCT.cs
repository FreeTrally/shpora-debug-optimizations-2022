using System;
using System.Threading.Tasks;
using JPEG.Utilities;

namespace JPEG
{
    public class DCT
    {
        private static double inverseSqrt = 1 / Math.Sqrt(2);

        public static double[,] DCT2D(double[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);

            var beta = Beta(height, width);
            var piHeight = Math.PI / (2 * height);
            var piWeight = Math.PI / (2 * width);
            var coeffs = new double[width, height];

            Parallel.For(0, width, u =>
                Parallel.For(0, height, v =>
                {
                    var sum = 0.0;

                    for (var x = 0; x < width; x++)
                    for (var y = 0; y < height; y++)
                        sum += BasisFunction(input[x, y], u, v, x, y, piHeight, piWeight);

                    coeffs[u, v] = sum * beta * Alpha(u) * Alpha(v);
                }));

            //MathEx.LoopByTwoVariables(
            //    0, width,
            //    0, height,
            //    (u, v) =>
            //    {
            //        var sum = MathEx
            //            .SumByTwoVariables(
            //                0, width,
            //                0, height,
            //                (x, y) => BasisFunction(input[x, y], u, v, x, y, height, width));

            //        coeffs[u, v] = sum * Beta(height, width) * Alpha(u) * Alpha(v);
            //    });

            return coeffs;
        }

        public static void IDCT2D(double[,] coeffs, double[,] output)
        {
            var height = coeffs.GetLength(0);
            var width = coeffs.GetLength(1);
            
            var beta = Beta(height, width);
            var piHeight = Math.PI / (2 * height);
            var piWeight = Math.PI / (2 * width);

            Parallel.For(0, width, x =>
                Parallel.For(0, height, y =>
                {
                    var sum = 0.0;
                    for (var u = 0; u < width; u++)
                    for (var v = 0; v < height; v++)
                        sum += BasisFunction(coeffs[u, v], u, v, x, y, piHeight, piWeight) * Alpha(u) * Alpha(v);

                    output[x, y] = sum * beta;
                }));

            //for (var x = 0; x < coeffs.GetLength(1); x++)
            //{
            //    for (var y = 0; y < coeffs.GetLength(0); y++)
            //    {
            //        var sum = MathEx
            //            .SumByTwoVariables(
            //                0, coeffs.GetLength(1),
            //                0, coeffs.GetLength(0),
            //                (u, v) => BasisFunction(coeffs[u, v], u, v, x, y, coeffs.GetLength(0), coeffs.GetLength(1)) * Alpha(u) * Alpha(v));

            //        output[x, y] = sum * Beta(coeffs.GetLength(0), coeffs.GetLength(1));
            //    }
            //}
        }

        public static double BasisFunction(double a, double u, double v, 
            double x, double y, double piHeight, double piWidth)
        {
            var b = Math.Cos((2d * x + 1d) * u * piWidth);
            var c = Math.Cos((2d * y + 1d) * v * piHeight);

            return a * b * c;
        }

        private static double Alpha(int u)
        {
            if (u == 0)
                return inverseSqrt;
            return 1;
        }

        private static double Beta(int height, int width)
        {
            return 1d / width + 1d / height;
        }
    }
}