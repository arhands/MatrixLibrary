using IntXLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MatrixInverter
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime t0 = DateTime.Now;
            Console.WriteLine("Start Time: {0}", t0);
            List<string> str = new List<string>();
            File.WriteAllLines("Test.txt",
                PolynomialGeneratorX.GenerateAndBuild("eccentricemery", PolynomialGeneratorX.InputTypes.Primes, TextFormat.PlainText, 'x', "f", 47, true)
                );
            DateTime t1 = DateTime.Now;
            Console.WriteLine("End Time: {0}",t1);
            Console.WriteLine("Total Time: {0}", t1 - t0);
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            return;
            //Matrix m = new Matrix(3, 3);

            //m[0, 0] = (Complex)3;
            //m[1, 0] = (Complex)1;
            //m[2, 0] = (Complex)5;

            //m[0, 1] = (Complex)3;
            //m[1, 1] = (Complex)3;
            //m[2, 1] = (Complex)1;

            //m[0, 2] = (Complex)4;
            //m[1, 2] = (Complex)6;
            //m[2, 2] = (Complex)4;

            Matrix m = new Matrix(3, 3);
            Random ran = new Random(465165313);
            for (int i = 0; i < m.Width; i++)
                for (int j = 0; j < m.Height; j++)
                    m[i, j] = new Complex(ran.Next() / 100.0, ran.Next() / 100.0 * 0);


            Console.WriteLine(m);
            Polynomial poly = m.FindCharacteristicPolynomial();
            Console.WriteLine("f(x) = {0}", poly);
            ////poly[0] = (Complex)(3);
            ////poly[1] = (Complex)(47);
            ////poly[2] = (Complex)(182);
            ////poly[3] = (Complex)(327);
            ////poly[4] = (Complex)(1027);
            foreach (var root in poly.FindAllRoots())
                Console.WriteLine("Root: {0}, Approx: {1}", root,poly.F(root));
            Console.ReadLine();
            Console.WriteLine("SBS^(-1) = ");
            m.Diagonalize(out Matrix S, out DiagonalMatrix B);
            var S1 = S.Invert();
            //S.Round(5);
            //S1.Round(5);
            //B.Round(5);
            string[] M1 = S.ToStringArray(), M2 = ((Matrix)B).ToStringArray(), M3 = S1.ToStringArray();
            for(int i = 0; i < B.Height; i++)
            {
                Console.WriteLine(M1[i] + M2[i] + M3[i]);
            }
            Console.WriteLine("=");
            Console.WriteLine(S * B * S1);
            Console.WriteLine("M^20 = ");
            Console.WriteLine(S * DiagonalMatrix.Pow(B,20) * S1);
            Matrix m20 = (Matrix)DiagonalMatrix.IdentityMatrix(2);
            for (int i = 0; i < 20; i++)
                m20 *= m;
            Console.WriteLine("?=");
            Console.WriteLine(m20);
            Console.WriteLine("(M^(1/3))^3 = ");
            var cubeRt = S * DiagonalMatrix.Pow(B, 1.0/3) * S1;
            Console.WriteLine(cubeRt*cubeRt*cubeRt);
            Console.WriteLine("END");
            Console.ReadLine();
            //Matrix m = GetSquareMatrixFromUser();
            //Console.WriteLine("\nInput Matrix: \n" + m);
            //Matrix inverse = m.Invert();
            //if (inverse == null)
            //    Console.WriteLine("Matrix is not invertable");
            //else
            //{
            //    Console.WriteLine("\nInverse Matrix: \n" + inverse);
            //    Matrix product = inverse * m;
            //    Console.WriteLine("\nProduct: \n" + product);
            //    product.Round(10);
            //    Console.WriteLine("\nProduct (rounded to 10 decimel places): \n" + product);
            //    Console.WriteLine("\nPress Enter to exit");
            //    Console.ReadLine();
            //}

        }
        static Matrix GetSquareMatrixFromUser()
        {
            Console.WriteLine("What is the width of your matrix?");
            Console.WriteLine("NOTE: the height will be the same value since it is a square matrix.");
            int width = GetUserInput("Width: ");
            return GetMatrixFromUser(width, width);
        }
        static Matrix GetMatrixFromUser() => GetMatrixFromUser(GetUserInput("Width: "), GetUserInput("Height: "));
        static Matrix GetMatrixFromUser(int width, int height)
        {
            Matrix matrix = new Matrix(width,height);
            Console.WriteLine("Please enter the values using space delimiters");
            Console.WriteLine("and press enter at the end of each row.");
            for(int i = 0; i < matrix.Height; i++)
            {
                Console.Write("Row {0}: ",i+1);
                var rawValues = Console.ReadLine().Split(' ');
                if(rawValues.Length < matrix.Width)
                {
                    Console.WriteLine("ERROR - insufficient elements on row {0}",i + 1);
                    Console.WriteLine("Please reenter row {0}", i--);
                    continue;
                }
                for (int j = 0; j < matrix.Width; j++)
                {
                    if (double.TryParse(rawValues[j], out double result))
                        matrix[j, i] = (Complex)result;
                    else
                    {
                        Console.WriteLine("ERROR - unable to parse value at {0},{0}", j + 1, i + 1);
                        Console.WriteLine("Please reenter row {0}",i--);
                        break;
                    }
                }
            }
            return matrix;
        }
        static int GetUserInput(string prompt)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result))
                return result;
            Console.WriteLine("\nERROR");
            return GetUserInput(prompt);
        }
    }
}
