using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    class DiagonalMatrix
    {
        public DiagonalMatrix(int size)
            :this(new Complex[size])
        {

        }
        public DiagonalMatrix(Complex[] matrix)
        {
            Diagonal = matrix;
        }
        public static DiagonalMatrix IdentityMatrix(int size)
        {
            var matrix = new Complex[size];
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = (Complex)1;
            return new DiagonalMatrix(matrix);
        }
        public Complex this[int i]
        {
            get
            {
                return Diagonal[i];
            }
            set
            {
                Diagonal[i] = value;
            }
        }
        Complex[] Diagonal { get; set; }
        public int Width => Diagonal.Length;
        public int Height => Diagonal.Length;
        public static DiagonalMatrix Pow(DiagonalMatrix matrix,Complex exp)
        {
            if (exp.Imaginary == 0)
                return Pow(matrix, exp.Real);
            DiagonalMatrix newMatrix = new DiagonalMatrix(matrix.Width);
            for (int i = 0; i < newMatrix.Width; i++)
                newMatrix[i] = Complex.Pow(matrix[i], exp);
            return newMatrix;
        }
        public static DiagonalMatrix Pow(DiagonalMatrix matrix, double exp)
        {
            DiagonalMatrix newMatrix = new DiagonalMatrix(matrix.Width);
            for (int i = 0; i < newMatrix.Width; i++)
                newMatrix[i] = Complex.Pow(matrix[i], exp);
            return newMatrix;
        }
        public Complex Determinant()
        {
            Complex d = (Complex)1;
            foreach (var c in Diagonal)
                d *= c;
            return d;
        }
        public DiagonalMatrix Invert()
        {
            DiagonalMatrix newMatrix = new DiagonalMatrix(Width);
            for (int i = 0; i < newMatrix.Width; i++)
                newMatrix[i] = (Complex)1/Diagonal[i];
            return newMatrix;
        }
        //operators
        public static Matrix operator *(DiagonalMatrix a, Matrix b)
        {
            if (a.Width != b.Height)
                return null;

            Matrix product = new Matrix(a.Height, b.Width);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                    product[i, j] = a[j] * b[i, j];
            return product;
        }
        public static Matrix operator *(Matrix a, DiagonalMatrix b)
        {
            if (a.Width != b.Height)
                return null;

            Matrix product = new Matrix(a.Height, b.Width);
            for (int i = 0; i < product.Width; i++)
                for (int j = 0; j < product.Height; j++)
                    product[i, j] = a[i, j] * b[i];
            return product;
        }
        public static DiagonalMatrix operator *(DiagonalMatrix a, DiagonalMatrix b)
        {
            if (a.Width != b.Height)
                return null;

            DiagonalMatrix product = new DiagonalMatrix(a.Height);
            for (int i = 0; i < product.Width; i++)
                    product[i] = a[i] * b[i];
            return product;
        }
        public static DiagonalMatrix operator *(double a, DiagonalMatrix b) => b * a;
        public static DiagonalMatrix operator *(DiagonalMatrix a, double b)
        {
            DiagonalMatrix product = new DiagonalMatrix(a.Height);
            for (int i = 0; i < product.Width; i++)
                product[i] = a[i] * b;
            return product;
        }
        public static DiagonalMatrix operator *(Complex a, DiagonalMatrix b) => b * a;
        public static DiagonalMatrix operator *(DiagonalMatrix a, Complex b)
        {
            DiagonalMatrix product = new DiagonalMatrix(a.Height);
            for (int i = 0; i < product.Width; i++)
                product[i] = a[i] * b;
            return product;
        }
        public static DiagonalMatrix operator -(DiagonalMatrix a)
        {
            DiagonalMatrix result = new DiagonalMatrix(a.Width);
            for (int i = 0; i < result.Width; i++)
                result[i] = -a[i];
            return result;
        }
        public static Matrix operator +(Matrix a, DiagonalMatrix b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
                throw new Exception("Matricies must have the same dimensions.");

            Matrix result = new Matrix(a.Height, b.Width);
            for (int i = 0; i < result.Width; i++)
                for (int j = 0; j < result.Height; j++)
                    result[i, j] = a[i, j];
            for (int i = 0; i < result.Width; i++)
                result[i, i] += b[i];
            return result;
        }
        public static Matrix operator -(Matrix a, DiagonalMatrix b) => a + -b;
        public static Matrix operator +(DiagonalMatrix a, Matrix b) => b + a;
        public void Round(int decimals)
        {
            for (int i = 0; i < Height; i++)
                this[i] = this[i].Round(decimals);
        }

        public static explicit operator Matrix(DiagonalMatrix m)
        {
            Matrix matrix = new Matrix(m.Width, m.Height);
            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                    matrix[i,j] = (Complex)0;
            return matrix + m;
        }

    }
}
