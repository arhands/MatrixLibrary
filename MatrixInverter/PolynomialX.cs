using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    enum TextFormat
    {
        PlainText,
        Latex
    }
    class PolynomialX
    {
        public FractionX this[int i]
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
        public PolynomialX(int length)
            : this(new FractionX[length])
        {

        }
        PolynomialX(FractionX[] coefficients)
        {
            Coefficients = coefficients;
        }
        public FractionX[] Coefficients { get; set; }
        public PolynomialX Copy()
        {
            FractionX[] polynomial = new FractionX[Coefficients.Length];
            for (int i = 0; i < polynomial.Length; i++)
                polynomial[i] = Coefficients[i];
            return new PolynomialX(polynomial);
        }
        public PolynomialX DivideOutRoot(FractionX root)
        {
            var copy = Copy();
            PolynomialX result = new PolynomialX(Coefficients.Length - 1);
            FractionX last = Coefficients[Coefficients.Length - 1];
            result[result.Coefficients.Length - 1] = last;
            for (int i = Coefficients.Length - 3; i >= 0; i--)
            {
                copy[i + 1] += root * last;
                result[i] = last = copy[i + 1];
            }
            return result;
        }
        public FractionX F(FractionX x)
        {
            FractionX result = (FractionX)0;
            FractionX term = (FractionX)1;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * term;
                term *= x;
            }
            return result;
        }
        public PolynomialX Differentiate()
        {
            PolynomialX polynomial = new PolynomialX(Coefficients.Length - 1);
            for (int i = 1; i < Coefficients.Length; i++)
                polynomial[i - 1] = i * this[i];
            return polynomial;
        }
        //always starts looking at 0
        //public FractionX FindRoot()
        //{
        //    if (Coefficients.Length == 2)
        //        return -Coefficients[0] / Coefficients[1];
        //    else if (Coefficients.Length == 3)
        //        return (-Coefficients[1] + FractionX.Pow(FractionX.Sqr(Coefficients[1]) - 4 * Coefficients[0] * Coefficients[2], 0.5)) / (2 * Coefficients[2]);
        //    FractionX root = new FractionX(0, 0), last = new FractionX(double.NaN, double.NaN);
        //    PolynomialX derivative = Differentiate();
        //    var im = new FractionX(0, 1);
        //    for (int i = 0; i < 1000 && root != last; i++)
        //    {
        //        last = root;
        //        var div = derivative.F(root);
        //        if (div == 0)
        //            root += new FractionX(0.1, 0);
        //        else
        //            root -= F(root) / div;

        //        var complex = root * im;
        //        div = derivative.F(complex) * im;
        //        if (div == 0)
        //            root += new FractionX(0, 0.1);
        //        else
        //            root -= F(complex) / div;
        //    }
        //    return root;
        //}
        //public FractionX[] FindAllRoots()
        //{
        //    FractionX[] roots = new FractionX[Coefficients.Length - 1];
        //    PolynomialX poly = this;
        //    for (int i = 0; i < roots.Length; i++)
        //    {
        //        roots[i] = poly.FindRoot();
        //        poly = poly.DivideOutRoot(roots[i]);
        //    }
        //    return roots;
        //}
        public override string ToString() => ToString('x');
        public string ToString(char variable, TextFormat format = TextFormat.PlainText)
        {
            string str = null;
            for (int i = 0; i < Coefficients.Length; i++)
                if (Coefficients[i] != 0)
                {
                    if (str == null)
                        str = Coefficients[i].ToString(format);
                    else if (Coefficients[i] == 1)
                        str += " + " + Coefficients[i].ToString(format);
                    else if (0 > Coefficients[i])
                    {
                        str += " - ";
                        if (Coefficients[i] != -1 || i == 0)
                            str += (-Coefficients[i]).ToString(format);
                    }
                    else
                        str += " + " + Coefficients[i].ToString(format);
                    if (i > 0)
                    {
                        str += variable;
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
