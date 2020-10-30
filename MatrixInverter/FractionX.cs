using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntXLib;

namespace MatrixInverter
{
    struct FractionX
    {
        public FractionX(IntX numeric)
        {
            Numberator = numeric;
            Denominator = 1;
        }
        public FractionX(IntX numerator, IntX denominator)
        {
            Numberator = numerator;
            Denominator = denominator;
        }
        public IntX Numberator { get; set; }
        public IntX Denominator { get;
            set; }
        //
        //
        public static FractionX operator +(FractionX a, FractionX b)
        {
            FractionX ret = new FractionX((IntX)(a.Numberator * b.Denominator
                + b.Numberator * a.Denominator),
                a.Denominator * b.Denominator);
            ret.Simplify();
            return new FractionX(ret.Numberator, ret.Denominator);
        }
        public static FractionX operator -(FractionX a, FractionX b) => a + -b;
        public static FractionX operator -(FractionX a) =>
            new FractionX(-a.Numberator, a.Denominator);
        public static FractionX operator *(FractionX a, FractionX b)
        {
            var c = new FractionX(a.Numberator * b.Numberator, a.Denominator * b.Denominator);
            c.Simplify();
            return c;
        }
        public static FractionX operator /(FractionX a, FractionX b) =>
            a * new FractionX((IntX)b.Denominator, (IntX)b.Numberator);
        public static bool operator <(int a, FractionX b) =>
            a * b.Denominator < b.Numberator;
        public static bool operator >(int a, FractionX b) =>
            a * b.Denominator > b.Numberator;
        public static bool operator ==(int a, FractionX b) =>
            a * b.Denominator == b.Numberator;
        public static bool operator !=(int a, FractionX b) =>
            a * b.Denominator != b.Numberator;
        public static bool operator ==(FractionX a, int b) => b == a;
        public static bool operator !=(FractionX a, int b) => b != a;
        public static bool operator ==(IntX a, FractionX b) =>
            a * b.Denominator == b.Numberator;
        public static bool operator !=(IntX a, FractionX b) =>
            a * b.Denominator != b.Numberator;
        public static bool operator ==(FractionX a, IntX b) => b == a;
        public static bool operator !=(FractionX a, IntX b) => b != a;
        //
        public static implicit operator FractionX(IntX n) =>
            new FractionX(n, 1);
        public static implicit operator FractionX(int n) =>
            new FractionX(n, 1);
        public static explicit operator IntX(FractionX n) => IntX.Divide(n.Numberator,n.Denominator,DivideMode.AutoNewton);
        //
        //
        void Simplify()
        {
            if (Numberator == 0)
            {
                Denominator = 1;
                return;
            }
            IntX a = Numberator;
            IntX b = Denominator;
            IntX r = b % a;
            while (r != 0)
            {
                b = a;
                a = r;
                r = b % a;
            }
            Numberator /= (IntX)a;
            Denominator /= a;
            if (Denominator < 0)
            {
                Numberator = -Numberator;
                Denominator = -Denominator;
            }
        }
        public string ToString(TextFormat format)
        {
            if (Denominator != 1)
            {
                if(format == TextFormat.PlainText)
                    return Numberator + "/" + Denominator;
                return "\\frac{" + Numberator + "}{" + Denominator + "}";
            }
            else return Numberator.ToString();
        }
        public override string ToString() => ToString(TextFormat.PlainText);
        public static FractionX FromString(string str)
        {
            if (str.Contains('/'))
            {
                int index = str.IndexOf('/');
                return new FractionX(IntX.Parse(str.Substring(0, index)), IntX.Parse(str.Substring(index + 1)));
            }
            return new FractionX(IntX.Parse(str));
        }
    }
}
