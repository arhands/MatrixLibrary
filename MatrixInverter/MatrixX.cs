using IntXLib;
using MatrixInverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    class MatrixX
    {

        //variables
        /// <summary>
        /// [Column,Row]
        /// </summary>
        IntX[,] Elements { get; set; }
        //derived variables
        public IntX this[int i, int j]
        {
            get
            {
                return Elements[i, j];
            }
            set
            {
                Elements[i, j] = value;
            }
        }
        public int Width => Elements.GetLength(0);
        public int Height => Elements.GetLength(1);
        //constructors(including public ones)
        public MatrixX(int width, int height)
        {
            Elements = new IntX[width, height];
        }
        public static MatrixX CombineColumns(List<MatrixX> spaces)
        {
            int width = 0;
            foreach (var space in spaces)
                width += space.Width;
            MatrixX matrix = new MatrixX(width, spaces[0].Height);
            for (int i = 0; i < width; i++)
                foreach (var space in spaces)
                    for (int j = 0; j < space.Width; j++)
                    {
                        for (int k = 0; k < space.Height; k++)
                            matrix[i, k] = space[j, k];
                        i++;
                    }
            return matrix;
        }
        //public static MatrixX CombineColumns(Vector[] columns)
        //{
        //    MatrixX matrix = new MatrixX(columns.Length, columns[0].Dimensions);
        //    for (int i = 0; i < matrix.Width; i++)
        //        for (int j = 0; j < matrix.Height; j++)
        //            matrix[i, j] = columns[i][j];
        //    return matrix;
        //}
        //public methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Inverse matrix if invertable, else returns NULL</returns>
        public MatrixX GetAdjointMatrix()
        {
            MatrixX adjointMatrix = GetMinorsMatrixX();

            for (int i = 0; i < adjointMatrix.Width; i++)
                for (int j = 0; j < adjointMatrix.Height; j++)
                    if ((i + j) % 2 == 1)
                        adjointMatrix[i, j] = -adjointMatrix[i, j];
            adjointMatrix = adjointMatrix.Transpose();
            return adjointMatrix;
        }
        public IntX Determinant(bool preserve = true)
        {
            if (preserve)
                return Copy().Determinant(false);
            if (Width != Height)
                throw new ArgumentOutOfRangeException("Is not a square matrix, determinant not defined");
            /*
            if (Width == 2 && Height == 2)
                return Elements[0, 0] * Elements[1, 1] - Elements[0, 1] * Elements[1, 0];
            if (Width == 1 && Height == 1)
                return Elements[0, 0];

            IntX solution = 0;
            MatrixX minorMatrixX = new MatrixX(Width - 1, Height - 1);
            for (int i = 0; i < Width; i++)
            {
                GetMinor(i, 0, ref minorMatrixX);
                if (i % 2 == 0)
                    solution += minorMatrixX.Determinant() * Elements[i, 0];
                else
                    solution -= minorMatrixX.Determinant() * Elements[i, 0];
            }
            */
            IntX determinantDivisor = 1, deterimant = 1;
            for(int i = 0; i < Width - 1; i++)//column & row
            {
                //find canidate row
                int m = i;
                for (; m < Height; m++)
                    if (Elements[i, m] != 0)
                        break;
                if (m == Height)
                    return 0;
                //swap canidate with row i
                if (m != i)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        IntX a = Elements[j, i], b = Elements[j, m];
                        Elements[j, i] = b;
                        Elements[m, i] = a;
                    }
                }
                if (Elements[i, i] == 0)
                    throw new ArgumentOutOfRangeException();
                deterimant *= Elements[i, i];
                //multiply and add to all other rows after i
                for (int j = i + 1; j < Height; j++)//row
                {
                    // Finding scalar multiple
                    IntX canidateMult = Elements[i,j];
                    for(int k = i; k < Width; k++)//column
                    {
                        // Column is k
                        // Canidate row is i
                        // Current row is j
                        // Scalar multiplier for current row is Elements[i,i]
                        // Scalar multiplier for subtraction canidate row is canidateMult (initially Elements[i,j])

                        Elements[k, j] = Elements[k, j] * Elements[i, i]
                            - Elements[k, i] * canidateMult;
                    }
                }
                // Find total divisor.
                determinantDivisor *= IntX.Pow(Elements[i, i],(uint)(Height - i - 1));
            }
            return IntX.Divide(deterimant*Elements[Width - 1, Height - 1],determinantDivisor,DivideMode.Classic);
        }
        public MatrixX Transpose()
        {
            MatrixX m = new MatrixX(Width, Height);
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    m[i, j] = this[j, i];
            return m;
        }
        //private methods
        MatrixX GetMinor(int i, int j)
        {
            MatrixX m = new MatrixX(Width - 1, Height - 1);
            GetMinor(i, j, ref m);
            return m;
        }
        void GetMinor(int i, int j, ref MatrixX destination)
        {
            int k = 0;
            for (; k < i; k++)
            {
                int h = 0;
                for (; h < j; h++)
                    destination[k, h] = Elements[k, h];
                h++;
                for (; h < Height; h++)
                    destination[k, h - 1] = Elements[k, h];
            }
            k++;
            for (; k < Width; k++)
            {
                int h = 0;
                for (; h < j; h++)
                    destination[k - 1, h] = Elements[k, h];
                h++;
                for (; h < Height; h++)
                    destination[k - 1, h - 1] = Elements[k, h];
            }
        }
        MatrixX GetMinorsMatrixX()
        {
            DateTime t0 = DateTime.Now;
            MatrixX minorsMatrixX = new MatrixX(Width, Height);
            MatrixX minorMatrixX = new MatrixX(Width - 1, Height - 1);
            int numerator = Width * Height;
            byte size = (byte)numerator.ToString().Length;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (j + i != 0)
                    {
                        DateTime t = DateTime.Now;
                        TimeSpan dt = t - t0;
                        double ratio = numerator / (i * Width + j + 0d);
                        Console.WriteLine("Get Minors Matrix: {0}/{1} = {2}% Complete. ETA: {3}", (i * Width + j).ToString().PadLeft(size), numerator, Math.Round(100/ratio,4).ToString().PadRight(8)
                            , t0.Add(TimeSpan.FromMilliseconds(dt.TotalMilliseconds * ratio)));
                    }
                    GetMinor(i, j, ref minorMatrixX);
                    minorsMatrixX[i, j] = minorMatrixX.Determinant();
                }
            }
            Console.WriteLine("Ending time: {0}", DateTime.Now);
            return minorsMatrixX;
        }
        public override string ToString()
        {
            string[] rows = ToStringArray();
            string str = "";
            for (int i = 0; i < Height - 1; i++)
                str += rows[i] + '\n';
            str += rows[Height - 1];
            return str;
        }
        string PadLeft(string str, char c, int size)
        {
            while (str.Length < size)
                str = c + str;
            return str;
        }
        public string[] ToStringArray()
        {
            string[,] elements = new string[Width, Height];
            int maxSize = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    elements[i, j] = Elements[i, j].ToString();
                    if (maxSize < elements[i, j].Length)
                        maxSize = elements[i, j].Length;
                }
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    elements[i, j] = PadLeft(elements[i, j], ' ', maxSize);


            string[] rows = new string[Height];

            for (int i = 0; i < Height; i++)
            {
                rows[i] += "[ ";
                for (int j = 0; j < Width; j++)
                    rows[i] += elements[j, i] + " ";
                rows[i] += "]";
            }
            return rows;
        }
        //public void Round(int decimalPlaces)
        //{
        //    for (int i = 0; i < Width; i++)
        //        for (int j = 0; j < Height; j++)
        //            Elements[i, j] = Elements[i, j].Round(decimalPlaces);
        //}
        ///// <summary>
        ///// Throws exception if not a square matrix.
        ///// </summary>
        ///// <returns></returns>
        //public Polynomial FindCharacteristicPolynomial()
        //{
        //    if (Width != Height)
        //        throw new Exception("Must be square matrix.");
        //    //Method can be found at: https://en.wikipedia.org/wiki/Faddeev%E2%80%93LeVerrier_algorithm
        //    FractionX[] c = new FractionX[Width + 1];
        //    c[0] = (FractionX)1;
        //    var I = DiagonalMatrixX.IdentityMatrixX(Width);
        //    var M = (MatrixX)I;
        //    for (int i = 1; i < c.Length; i++)
        //    {
        //        //Console.WriteLine();
        //        //Console.WriteLine(M);
        //        M = this * M;
        //        c[i] = -1.0 / i * M.Trace();

        //        //Console.WriteLine(M);
        //        //Console.WriteLine(c[i]);

        //        if (i + 1 < c.Length)
        //            M += c[i] * I;
        //    }

        //    Polynomial poly = new Polynomial(c.Length);
        //    for (int i = 0; i < c.Length; i++)
        //        poly[i] = c[c.Length - i - 1];
        //    return poly;
        //}
        FractionX Trace()
        {
            FractionX trace = (FractionX)0;
            for (int i = 0; i < Width; i++)
                trace += this[i, i];
            return trace;
        }
        //MatrixX RowReducedEchelonForm()
        //{
        //    var copy = Copy();
        //    int row = 0;
        //    for (int column = 0; column < Width; column++)
        //    {
        //        //finding canidate row
        //        var r = row;
        //        for (; r < Height; r++)
        //            if (copy[r, column] != 0)
        //                break;
        //        //moving canidate row to proper row location
        //        //and dividing out magnitude
        //        if (r == Height)
        //            continue;
        //        var mlt = 1 / copy[r, column];
        //        for (int j = column; j < Width; j++)
        //        {
        //            var swap = copy[r, j];
        //            copy[r, j] = copy[row, j];
        //            copy[row, j] = swap * mlt;
        //        }
        //        //Normalizing Column
        //        for (int i = 0; i < Height; i++)
        //        {
        //            var m = copy[i, column];
        //            for (int j = column; j < Width; j++)
        //            {
        //                copy[j, i] -= m * copy[row, j];
        //            }
        //        }

        //        row++;
        //        if (row == Height)
        //            break;
        //    }
        //    return copy;
        //}
        //FractionX[] FindEigenValues()
        //{
        //    var poly = FindCharacteristicPolynomial();
        //    return poly.FindAllRoots();
        //}
        //MatrixX GetEigenSpace(FractionX eigenValue, int dimensions)
        //{
        //    MatrixX m = (this - eigenValue * DiagonalMatrixX.IdentityMatrixX(Width));

        //    m = m.RowReducedEchelonForm();

        //    MatrixX eigenSpace = new MatrixX(dimensions, Height);
        //    int basis = 0;
        //    for (int i = 0; i + basis < Width; i++)
        //    {
        //        if (m[i + basis, i] != 1)
        //        {
        //            int j = 0;
        //            for (; j < i + basis; j++)
        //                eigenSpace[i, j] = -m[i, j];
        //            eigenSpace[i, j++] = (FractionX)1;
        //            for (; j < Height; j++)
        //                eigenSpace[i, j] = -m[i, j];

        //            basis++;
        //        }
        //    }
        //    return eigenSpace;
        //}
        //Vector[] GetColumns()
        //{
        //    Vector[] columns = new Vector[Width];
        //    for (int i = 0; i < Width; i++)
        //    {
        //        FractionX[] elements = new FractionX[Height];
        //        for (int j = 0; j < Height; j++)
        //            elements[j] = this[i, j];
        //        columns[i] = new Vector(elements);
        //    }
        //    return columns;
        //}
        //MatrixX GramSchmit()
        //{
        //    Vector[] vectors = GetColumns();
        //    Vector[] normalized = new Vector[vectors.Length - 1];
        //    for (int i = 0; i < vectors.Length; i++)
        //    {
        //        for (int j = 0; j < i; j++)
        //            vectors[i] -= normalized[j] * vectors[i] * normalized[j];
        //        if (i < normalized.Length)
        //            normalized[i] = vectors[i].Normalize();
        //    }
        //    return CombineColumns(vectors);
        //}
        //public void Diagonalize(out MatrixX basis, out DiagonalMatrixX diagonal)
        //{
        //    var eigenValues = FindEigenValues();
        //    var l = eigenValues.ToList();
        //    l.Sort((FractionX a, FractionX b) => { return a.Real < b.Real ? 1 : 0; });
        //    eigenValues = l.ToArray();
        //    int m = 0;
        //    List<MatrixX> spaces = new List<MatrixX>();
        //    for (int i = 0; i < eigenValues.Length; i += m)
        //    {
        //        m = 1;//multiplicity
        //        while (i + m < eigenValues.Length && eigenValues[i] == eigenValues[i + m])
        //            m++;
        //        //finding eigen space
        //        var space = GetEigenSpace(eigenValues[i], m);
        //        //using Gram Schmit to orthogonalize stuff.
        //        space.GramSchmit();
        //        spaces.Add(space);
        //    }
        //    basis = CombineColumns(spaces);
        //    diagonal = new DiagonalMatrixX(eigenValues);
        //}
        MatrixX Copy()
        {
            MatrixX matrix = new MatrixX(Width, Height);
            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                    matrix[i, j] = this[i, j];
            return matrix;
        }
        //public MatrixX Pow(FractionX exp) => Pow(this, exp);
        //public static MatrixX Pow(MatrixX a, FractionX exp)
        //{
        //    a.Diagonalize(out MatrixX s, out DiagonalMatrixX b);
        //    var s1 = s.Invert();
        //    return s * DiagonalMatrixX.Pow(b, exp) * s1;
        //}
        //Operators
        public static MatrixX operator *(MatrixX a, MatrixX b)
        {
            if (a.Width != b.Height)
                return null;

            MatrixX product = new MatrixX(a.Height, b.Width);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                {
                    product[i, j] = 0;
                    for (int h = 0; h < a.Width; h++)
                        product[i, j] += a[h, i] * b[j, h];
                }
            return product;
        }
        public static MatrixX operator *(IntX a, MatrixX b)
        {
            MatrixX product = new MatrixX(b.Width, b.Height);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                    product[i, j] = a * b[i, j];
            return product;
        }
        public static MatrixX operator *(MatrixX a, IntX b) => b * a;
        //public static MatrixX operator /(MatrixX a, IntX b) => 1 / b * a;
    }
}
