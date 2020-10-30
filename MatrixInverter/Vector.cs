using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    class Vector
    {
        public Vector(Complex[] elements)
        {
            Elements = elements;
        }
        public Complex this[int i]
        {
            get
            {
                return Elements[i];
            }
            set
            {
                Elements[i] = value;
            }
        }
        Complex[] Elements { get; set; }
        public int Dimensions => Elements.Length;
        public Complex Magnitude()
        {
            Complex magnitude = new Complex(0,0);
            foreach (var element in Elements)
                magnitude += Complex.Sqr(element);
            return Complex.Pow(magnitude, 0.5);
        }
        public Vector Normalize() => this / Magnitude();
        /// <summary>
        /// This is a dot product
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator *(Vector a, Vector b)
        {
            Complex result = new Complex(0,0);
            for (int i = 0; i < a.Elements.Length; i++)
                result += a.Elements[i] * b.Elements[i];
            return result;
        }
        public static Vector operator *(Vector a, Complex b)
        {
            Complex[] elements = new Complex[a.Elements.Length];
            for (int i = 0; i < a.Elements.Length; i++)
                elements[i] = a.Elements[i] * b;
            return new Vector(elements);
        }
        public static Vector operator *(Vector a, double b)
        {
            Complex[] elements = new Complex[a.Elements.Length];
            for (int i = 0; i < a.Elements.Length; i++)
                elements[i] = a.Elements[i] * b;
            return new Vector(elements);
        }
        public static Vector operator *(Complex a, Vector b) => b * a;
        public static Vector operator *(double a, Vector b) => b * a;
        public static Vector operator /(Vector a, Complex b) => a * (1/b);
        public static Vector operator /(Vector a, double b) => a * 1 / b;
        public static Vector operator +(Vector a, Vector b)
        {
            Complex[] elements = new Complex[a.Elements.Length];
            for (int i = 0; i < a.Elements.Length; i++)
                elements[i] = a.Elements[i] + b.Elements[i];
            return new Vector(elements);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            Complex[] elements = new Complex[a.Elements.Length];
            for (int i = 0; i < a.Elements.Length; i++)
                elements[i] = a.Elements[i] - b.Elements[i];
            return new Vector(elements);
        }
    }
}
