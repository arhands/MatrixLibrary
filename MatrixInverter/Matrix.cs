using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    class Matrix
    {
        
        //variables
        Complex[,] Elements { get; set; }
        //derived variables
        public Complex this[int i, int j]
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
        public Matrix(int width, int height)
        {
            Elements = new Complex[width, height];
        }
        public static Matrix CombineColumns(List<Matrix> spaces)
        {
            int width = 0;
            foreach (var space in spaces)
                width += space.Width;
            Matrix matrix = new Matrix(width, spaces[0].Height);
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
        public static Matrix CombineColumns(Vector[] columns)
        {
            Matrix matrix = new Matrix(columns.Length, columns[0].Dimensions);
            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                    matrix[i, j] = columns[i][j];
            return matrix;
        }
        //public methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Inverse matrix if invertable, else returns NULL</returns>
        public Matrix Invert()
        {
            Complex determinant = Determinant();
            if (determinant.IsZero)
                return null;
            if(Width == 1)
            {
                Matrix m = new Matrix(1, 1);
                m[0, 0] = 1 / determinant;
                return m;
            }
            Matrix inverse = GetMinorsMatrix();
            
            for (int i = 0; i < inverse.Width; i++)
                for (int j = 0; j < inverse.Height; j++)
                    if ((i + j) % 2 == 1)
                        inverse[i, j] = -inverse[i, j];
            inverse = inverse.Transpose();
            inverse /= determinant;
            return inverse;
        }
        public Complex Determinant()
        {
            if (Width != Height)
                return (Complex)double.NaN;
            if (Width == 2 && Height == 2)
                return Elements[0, 0] * Elements[1, 1] - Elements[0, 1] * Elements[1, 0];
            if (Width == 1 && Height == 1)
                return Elements[0, 0];

            Complex solution = (Complex)0;
            Matrix minorMatrix = new Matrix(Width - 1, Height - 1);
            for (int i = 0; i < Width; i++)
            {
                GetMinor(i, 0, ref minorMatrix);
                if(i % 2 == 0)
                    solution += minorMatrix.Determinant() * Elements[i, 0];
                else
                    solution -= minorMatrix.Determinant() * Elements[i, 0];
            }
            return solution;
        }
        public Matrix Transpose()
        {
            Matrix m = new Matrix(Width, Height);
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    m[i, j] = this[j, i];
            return m;
        }
        //private methods
        Matrix GetMinor(int i, int j)
        {
            Matrix m = new Matrix(Width - 1, Height - 1);
            GetMinor(i, j,ref m);
            return m;
        }
        void GetMinor(int i, int j, ref Matrix destination)
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
        Matrix GetMinorsMatrix()
        {
            Matrix minorsMatrix = new Matrix(Width, Height);
            Matrix minorMatrix = new Matrix(Width - 1, Height - 1);
            for(int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    GetMinor(i, j, ref minorMatrix);
                    minorsMatrix[i, j] = minorMatrix.Determinant();
                }
            }
            return minorsMatrix;
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
            while(str.Length < size)
                str = c + str;
            return str;
        }
        public string[] ToStringArray()
        {
            string[,] elements = new string[Width,Height];
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

            for(int i = 0; i < Height; i++)
            {
                rows[i] += "[ ";
                for (int j = 0; j < Width; j++)
                    rows[i] += elements[j, i] + " ";
                rows[i] += "]";
            }
            return rows;
        }
        public void Round(int decimalPlaces)
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    Elements[i, j] = Elements[i, j].Round(decimalPlaces);
        }
        /// <summary>
        /// Throws exception if not a square matrix.
        /// </summary>
        /// <returns></returns>
        public Polynomial FindCharacteristicPolynomial()
        {
            if (Width != Height)
                throw new Exception("Must be square matrix.");
            //Method can be found at: https://en.wikipedia.org/wiki/Faddeev%E2%80%93LeVerrier_algorithm
            Complex[] c = new Complex[Width + 1];
            c[0] = (Complex)1;
            var I = DiagonalMatrix.IdentityMatrix(Width);
            var M = (Matrix)I;
            for(int i = 1; i < c.Length; i++)
            {
                //Console.WriteLine();
                //Console.WriteLine(M);
                M = this * M;
                c[i] = -1.0 / i * M.Trace();

                //Console.WriteLine(M);
                //Console.WriteLine(c[i]);

                if (i + 1 < c.Length)
                    M += c[i]*I;
            }

            Polynomial poly = new Polynomial(c.Length);
            for (int i = 0; i < c.Length; i++)
                poly[i] = c[c.Length - i - 1];
            return poly;
        }
        Complex Trace()
        {
            Complex trace = (Complex)0;
            for (int i = 0; i < Width; i++)
                trace += this[i, i];
            return trace;
        }
        Matrix RowReducedEchelonForm()
        {
            var copy = Copy();
            int row = 0;
            for(int column = 0; column < Width; column++)
            {
                //finding canidate row
                var r = row;
                for (; r < Height; r++)
                    if (copy[r, column] != 0)
                        break;
                //moving canidate row to proper row location
                //and dividing out magnitude
                if (r == Height)
                    continue;
                var mlt = 1/copy[r, column];
                for(int j = column; j < Width; j++)
                {
                    var swap = copy[r, j];
                    copy[r, j] = copy[row,j];
                    copy[row, j] = swap * mlt;
                }
                //Normalizing Column
                for (int i = 0; i < Height; i++)
                {
                    var m = copy[i, column];
                    for (int j = column; j < Width; j++)
                    {
                        copy[j, i] -= m * copy[row, j];
                    }
                }

                row++;
                if (row == Height)
                    break;
            }
            return copy;
        }
        Complex[] FindEigenValues()
        {
            var poly = FindCharacteristicPolynomial();
            return poly.FindAllRoots();
        }
        Matrix GetEigenSpace(Complex eigenValue, int dimensions)
        {
            Matrix m = (this - eigenValue * DiagonalMatrix.IdentityMatrix(Width));

            m = m.RowReducedEchelonForm();

            Matrix eigenSpace = new Matrix(dimensions, Height);
            int basis = 0;
            for(int i = 0; i + basis < Width; i++)
            {
                if (m[i + basis, i] != 1)
                {
                    int j = 0;
                    for (; j < i + basis; j++)
                        eigenSpace[i, j] = -m[i, j];
                    eigenSpace[i, j++] = (Complex)1;
                    for (; j < Height; j++)
                        eigenSpace[i, j] = -m[i, j];

                    basis++;
                }
            }
            return eigenSpace;
        }
        Vector[] GetColumns()
        {
            Vector[] columns = new Vector[Width];
            for(int i = 0; i < Width; i++)
            {
                Complex[] elements = new Complex[Height];
                for (int j = 0; j < Height; j++)
                    elements[j] = this[i, j];
                columns[i] = new Vector(elements);
            }
            return columns;
        }
        Matrix GramSchmit()
        {
            Vector[] vectors = GetColumns();
            Vector[] normalized = new Vector[vectors.Length - 1];
            for(int i = 0; i < vectors.Length; i++)
            {
                for (int j = 0; j < i; j++)
                    vectors[i] -= normalized[j] * vectors[i] * normalized[j];
                if (i < normalized.Length)
                    normalized[i] = vectors[i].Normalize();
            }
            return CombineColumns(vectors);
        }
        public void Diagonalize(out Matrix basis, out DiagonalMatrix diagonal)
        {
            var eigenValues = FindEigenValues();
            var l = eigenValues.ToList();
            l.Sort((Complex a, Complex b) => { return a.Real < b.Real ? 1 : 0; });
            eigenValues = l.ToArray();
            int m = 0;
            List<Matrix> spaces = new List<Matrix>();
            for(int i = 0; i < eigenValues.Length; i += m)
            {
                m = 1;//multiplicity
                while (i + m < eigenValues.Length && eigenValues[i] == eigenValues[i + m])
                    m++;
                //finding eigen space
                var space = GetEigenSpace(eigenValues[i],m);
                //using Gram Schmit to orthogonalize stuff.
                space.GramSchmit();
                spaces.Add(space);
            }
            basis = CombineColumns(spaces);
            diagonal = new DiagonalMatrix(eigenValues);
        }
        Matrix Copy()
        {
            Matrix matrix = new Matrix(Width, Height);
            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                    matrix[i, j] = this[i, j];
            return matrix;
        }
        public Matrix Pow(Complex exp) => Pow(this, exp);
        public static Matrix Pow(Matrix a, Complex exp)
        {
            a.Diagonalize(out Matrix s, out DiagonalMatrix b);
            var s1 = s.Invert();
            return s * DiagonalMatrix.Pow(b, exp) * s1;
        }
        //Operators
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Width != b.Height)
                return null;

            Matrix product = new Matrix(a.Height, b.Width);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                {
                    product[i, j] = (Complex)0;
                    for (int h = 0; h < a.Width; h++)
                        product[i, j] += a[h, j] * b[i, h];
                }
            return product;
        }
        public static Matrix operator *(double a, Matrix b)
        {
            Matrix product = new Matrix(b.Width, b.Height);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                    product[i, j] = a * b[i, j];
            return product;
        }
        public static Matrix operator *(Complex a, Matrix b)
        {
            Matrix product = new Matrix(b.Width, b.Height);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                    product[i, j] = a * b[i, j];
            return product;
        }
        public static Matrix operator *(Matrix a, double b) => b * a;
        public static Matrix operator *(Matrix a, Complex b) => b * a;
        public static Matrix operator /(Matrix a, double b) => 1 / b * a;
        public static Matrix operator /(Matrix a, Complex b) => 1 / b * a;
    }
}
