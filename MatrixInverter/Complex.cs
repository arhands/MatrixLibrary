using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    class Complex
    {
        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public static bool operator ==(Complex a, Complex b) => a.Real == b.Real && a.Imaginary == b.Imaginary;
        public static bool operator !=(Complex a, Complex b) => a.Real != b.Real || a.Imaginary != b.Imaginary;
        public static bool operator ==(Complex a, double b) => a.Real == b && a.Imaginary == 0;
        public static bool operator ==(double a, Complex b) => b == a;
        public static bool operator !=(Complex a, double b) => a.Real != b || a.Imaginary != 0;
        public static bool operator !=(double a, Complex b) => b != a;


        public static Complex operator +(Complex a, Complex b) => new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        public static Complex operator -(Complex a) => new Complex(-a.Real, -a.Imaginary);
        public static Complex operator -(Complex a, Complex b) => new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        public static Complex operator *(Complex a, Complex b) => new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Real * b.Imaginary + b.Real * a.Imaginary);
        public static Complex operator *(Complex a, double b) => new Complex(a.Real * b, a.Imaginary * b);
        public static Complex operator *(double a, Complex b) => new Complex(b.Real * a, b.Imaginary * a);
        public static Complex operator /(Complex a, Complex b)
        {
            if (b.Imaginary == 0)
                return new Complex(a.Real / b.Real, a.Imaginary / b.Real);
            return FromPolar(a.Length / b.Length, a.Angle - b.Angle);
        }
        public static Complex operator /(double a, Complex b)
        {
            if (b.Imaginary == 0)
                return new Complex(a / b.Real, 0);
            return FromPolar(a / b.Length, -b.Angle);
        }
        public static Complex operator /(Complex a, double b) => new Complex(a.Real / b, a.Imaginary / b);
        public static explicit operator Complex(double a) => new Complex(a, 0);
        public static Complex Sqr(Complex n) => new Complex(n.Real * n.Real - n.Imaginary * n.Imaginary, n.Real * n.Imaginary * 2);
        public static Complex i => new Complex(0, -1);
        public static Complex Cos(Complex n) =>
            new Complex(Math.Cos(n.Real) * Math.Cosh(n.Imaginary), -Math.Sin(n.Real) * Math.Sinh(n.Imaginary));
        public static Complex Sin(Complex n) =>
            new Complex(Math.Sin(n.Real) * Math.Cosh(n.Imaginary), Math.Cos(n.Real) * Math.Sinh(n.Imaginary));
        public static Complex Tan(Complex n) => Sin(n) / Cos(n);
        public static Complex Pow(Complex n, int exp)
        {
            Complex val = new Complex(1, 0);
            for (int i = 0; i < exp; i++)
                val *= n;
            return val;
        }
        public static Complex Pow(Complex n, double exp) => FromPolar(Math.Pow(n.Length, exp), n.Angle * exp);
        public static Complex Pow(Complex b, Complex exp)
        {
            var length = b.Length;
            var angle = b.Angle;
            return FromPolar(Math.Pow(length,exp.Real-exp.Imaginary*angle),exp.Real*angle+exp.Imaginary*Math.Log(length));
        }
        public static Complex Pow(double real, Complex exp)
        {
            if (exp.Imaginary == 0)
                return new Complex(Math.Pow(real, exp.Real), 0);
            var angle = exp.Imaginary * Math.Log(real);
            return Math.Pow(real, exp.Real) * new Complex(Math.Cos(angle), Math.Sin(angle));
        }
        public static Complex Log(Complex c) =>
            new Complex(Math.Log(c.Real), c.Angle);
        public static Complex FromPolar(double length, double angle) =>
            new Complex(length * Math.Cos(angle), length * Math.Sin(angle));
        public double Length => Math.Sqrt(LengthSquared);
        public double LengthSquared => Real * Real + Imaginary * Imaginary;
        public double Angle => Math.Atan2(Imaginary, Real);
        public Complex Round(int decimals) => new Complex(Math.Round(Real, decimals), Math.Round(Imaginary, decimals));
        public bool IsZero => Real == 0 && Imaginary == 0;
        public override string ToString() {
            if (Real == 0) {
                if (Imaginary == 0)
                    return "0";
                else if (Imaginary == 1)
                    return "i";
                else if (Imaginary == -1)
                    return "-i";
                else
                    return Imaginary + " i";
            }
            else
            {
                if (Imaginary == 0)
                    return Real.ToString();
                else if(Imaginary == 1)
                    return "(" + Real + " + i)";
                else if (Imaginary == -1)
                    return "(" + Real + " - i)";
                else
                    return "(" + Real + " + " + Imaginary + " i" + ")";
            }
        }
    }
}
