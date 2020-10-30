using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixInverter
{
    struct Fraction
    {
        public Fraction(int numeric)
        {
            Numberator = numeric;
            Denominator = 1;
        }
        public Fraction(int numerator, uint denominator)
        {
            Numberator = numerator;
            Denominator = denominator;
        }
        public int Numberator { get; set; }
        public uint Denominator { get; set; }
        //
        //
        public static Fraction operator +(Fraction a, Fraction b)
        {
            Factor(ref a,ref b, out Fraction common);
            Fraction ret = new Fraction((int)(a.Numberator * b.Denominator 
                + b.Numberator * a.Denominator), 
                a.Denominator * b.Denominator);
            ret.Simplify();
            return new Fraction(ret.Numberator * common.Numberator, ret.Denominator * common.Denominator);
        }
        public static Fraction operator -(Fraction a, Fraction b) => a + -b;
        public static Fraction operator -(Fraction a) =>
            new Fraction(-a.Numberator, a.Denominator);
        public static Fraction operator *(Fraction a, Fraction b)
        {
            Fraction c = new Fraction(a.Numberator, b.Denominator),
                d = new Fraction(b.Numberator, a.Denominator);
            c.Simplify();
            d.Simplify();
            return new Fraction(c.Numberator * d.Numberator, c.Denominator * d.Denominator);
        }
        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.Numberator < 0)
                return a * new Fraction(-(int)b.Denominator,(uint)(-b.Numberator));
            return a * new Fraction((int)b.Denominator, (uint)b.Numberator);
        }
        public static bool operator <(int a, Fraction b) =>
            (long)a * b.Denominator < b.Numberator;
        public static bool operator >(int a, Fraction b) =>
            (long)a * b.Denominator > b.Numberator;
        public static bool operator ==(int a, Fraction b) =>
            (long)a * b.Denominator == b.Numberator;
        public static bool operator !=(int a, Fraction b) =>
            (long)a * b.Denominator != b.Numberator;
        //
        public static implicit operator Fraction(int n) =>
            new Fraction(n, 1);
        //
        static Fraction Factor(Fraction a, Fraction b)
        {
            Factor(ref a, ref b, out Fraction factor);
            return factor;
        }
        //
        static void Factor(ref Fraction a, ref Fraction b, out Fraction factor)
        {
            
            uint min = Math.Min(Math.Min((uint)Math.Abs(a.Numberator), (uint)Math.Abs(b.Numberator)), 
                Math.Min(a.Denominator,b.Denominator));
            factor = 1;
            //2's
            if (a.Numberator % 2 == 0)
            {
                while(a.Numberator % 2 == 0 && b.Numberator % 2 == 0)
                {
                    a.Numberator >>= 2;
                    b.Numberator >>= 2;
                    factor.Numberator = factor.Numberator << 2;
                }
            }
            else if (a.Denominator % 2 == 0)
            {
                while (a.Denominator % 2 == 0 && b.Denominator % 2 == 0)
                {
                    a.Denominator >>= 2;
                    b.Denominator >>= 2;
                    factor.Denominator = factor.Denominator << 2;
                }
            }
            //everything else
            for (int i = 3; i <= min; i += 2)//NOTE: because of the way we are doing this,
            {                                //we should only find prime factors. (my be useful eventually)
                if(a.Numberator % i == 0)
                {
                    while(b.Numberator % i == 0 && a.Numberator % i == 0)
                    {
                        a.Numberator /= i;
                        b.Numberator /= i;
                        factor.Numberator *= i;
                    }
                }
                else if (a.Denominator % i == 0)
                {
                    if (b.Denominator % i == 0 && a.Denominator % i == 0)
                    {
                        a.Denominator /= (uint)i;
                        b.Denominator /= (uint)i;
                        factor.Denominator *= (uint)i;
                    }
                }
            }
        }
        void Simplify()
        {
            if(Numberator == 0)
            {
                Denominator = 1;
                return;
            }
            uint a = (uint)(Numberator > 1 ? Numberator : -Numberator);
            uint b = Denominator;
            uint r = a % b;
            while(r != 0)
            {
                b = a;
                a = r;
                r = a % b;
            }
            Numberator /= (int)a;
            Denominator /= a;
        }
        public override string ToString()
        {
            if (Denominator != 1)
                return Numberator + "/" + Denominator;
            else return Numberator.ToString();
        }
        public static Fraction FromString(string str)
        {
            if (str.Contains('/'))
            {
                int index = str.IndexOf('/');
                return new Fraction(int.Parse(str.Substring(0, index)), uint.Parse(str.Substring(index + 1)));
            }
            return new Fraction(int.Parse(str));
        }
    }
}
