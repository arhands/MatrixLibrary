using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    class Polynomial
    {
        public Complex this[int i]
        {
            get
            {
                return Coefficients[i];
            }
            set
            {
                Coefficients[i] = value;
            }
        }
        public Polynomial(int length)
            :this(new Complex[length])
        {

        }
        Polynomial(Complex[] coefficients)
        {
            Coefficients = coefficients; 
        }
        public Complex[] Coefficients { get; set; }
        public Polynomial Copy()
        {
            Complex[] polynomial = new Complex[Coefficients.Length];
            for (int i = 0; i < polynomial.Length; i++)
                polynomial[i] = Coefficients[i];
            return new Polynomial(polynomial);
        }
        public Polynomial DivideOutRoot(Complex root)
        {
            var copy = Copy();
            Polynomial result = new Polynomial(Coefficients.Length - 1);
            Complex last = Coefficients[Coefficients.Length - 1];
            result[result.Coefficients.Length - 1] = last;
            for (int i = Coefficients.Length - 3; i >= 0; i--)
            {
                copy[i + 1] += root * last;
                result[i] = last = copy[i + 1];
            }
            return result;
        }
        public Complex F(double f)
        {
            Complex result = (Complex)0;
            double x = 1;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * x;
                x *= f;
            }
            return result;
        }
        public Complex F(Complex x)
        {
            Complex result = (Complex)0;
            Complex term = (Complex)1;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * term;
                term *= x;
            }
            return result;
        }
        public Polynomial Differentiate()
        {
            Polynomial polynomial = new Polynomial(Coefficients.Length - 1);
            for (int i = 1; i < Coefficients.Length; i++)
                polynomial[i - 1] = i * this[i];
            return polynomial;
        }
        //always starts looking at 0
        public Complex FindRoot()
        {
            if (Coefficients.Length == 2)
                return -Coefficients[0] / Coefficients[1];
            else if (Coefficients.Length == 3)
                return (-Coefficients[1] + Complex.Pow(Complex.Sqr(Coefficients[1]) - 4 * Coefficients[0] * Coefficients[2], 0.5)) / (2 * Coefficients[2]);
            Complex root = new Complex(0,0), last = new Complex(double.NaN, double.NaN);
            Polynomial derivative = Differentiate();
            var im = new Complex(0, 1);
            for (int i = 0; i < 1000 && root != last; i++)
            {
                last = root;
                var div = derivative.F(root);
                if (div == 0)
                    root += new Complex(0.1,0);
                else
                    root -= F(root)/div;

                var complex = root * im;
                div = derivative.F(complex)*im;
                if (div == 0)
                    root += new Complex(0, 0.1);
                else
                    root -= F(complex) / div;
            }
            return root;
        }
        public Complex[] FindAllRoots()
        {
            Complex[] roots = new Complex[Coefficients.Length - 1];
            Polynomial poly = this;
            for(int i = 0; i < roots.Length; i++)
            {
                roots[i] = poly.FindRoot();
                poly = poly.DivideOutRoot(roots[i]);
            }
            return roots;
        }
        public override string ToString()
        {
            string str = null;
            for(int i = 0; i < Coefficients.Length; i++)
                if(Coefficients[i] != 0)
                {
                    if (str == null)
                        str = Coefficients[i].ToString();
                    else if (Coefficients[i] == 1)
                        str += " + " + Coefficients[i];
                    else if (Coefficients[i].Real < 0)
                    {
                        str += " - ";
                        if (Coefficients[i] != -1 || i == 0)
                            str += -Coefficients[i];
                    }
                    else
                        str += " + " + Coefficients[i];
                    if (i > 0)
                    {
                        str += "X";
                        if (i > 1)
                            str += "^" + i;
                    }
                }
            if (str == null)
                str = "0";
            return str;
        }
    }
}
