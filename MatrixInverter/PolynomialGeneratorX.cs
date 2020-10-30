using IntXLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MatrixInverter
{
    class PolynomialGeneratorX
    {
        public enum InputTypes
        {
            Primes,
            Recursive,
            /// <summary>
            /// recursive, but to get the actual value, each value must be divided by the n'th prime (where n is the number of the value).
            /// </summary>
            RecursivePrimes,
            PreviousProduct,
        }
        public static IntX[] StrToIntX(string s)
        {
            IntX[] arr = new IntX[s.Length];
            for (int i = 0; i < s.Length; i++)
                arr[i] = s[i];
            return arr;
        }
        public static List<string> BatchGenerateAndBuild(string output, TextFormat format, char functionVariable = 'x', string functionName = "f", int startingValue = -1, bool verify = false) =>
            BatchGenerateAndBuild(output, (InputTypes[])Enum.GetValues(typeof(InputTypes)), format, functionVariable, functionName, startingValue,verify);
        public static List<string> BatchGenerateAndBuild(string output, InputTypes[] types, TextFormat format, char functionVariable = 'x', string functionName = "f", int startingValue = -1, bool verify = false)
        {
            IntX[] outputProc = StrToIntX(output);
            List<string> values = new List<string>();
            foreach(var type in types)
            {
                IntX[] input = GenerateInput(outputProc, type, startingValue);
                var poly = Generate(input, outputProc);
                if (poly != null)
                {
                    values.AddRange(BuildMessage(output, type, poly, format, functionVariable, functionName, startingValue));
                    values.Add("");
                    if (verify && !Verify(outputProc, poly, type, startingValue))
                    {
                        Console.WriteLine("Verification failed for type {0}, returning NULL",type);
                        return null;
                    }
                }
            }
            return values;
        }
        public static bool Verify(IntX[] output, PolynomialX poly, InputTypes type, int startingValue = -1, bool verify = false)
        {
            IntX[] input = GenerateInput(output, type, startingValue);
            for (int i = 0; i < output.Length; i++)
                if (output[i] != poly.F(input[i]))
                    return false;
            return true;
        }
        public static string[] GenerateAndBuild(string output, InputTypes type, TextFormat format, char functionVariable = 'x', string functionName = "f", int startingValue = -1, bool verify = false)
        {
            IntX[] outputProc = StrToIntX(output);
            IntX[] input = GenerateInput(outputProc,type,startingValue);
            var poly = Generate(input, outputProc);
            if (verify && !Verify(outputProc, poly, type, startingValue))
            {
                Console.WriteLine("Verification failed, returning NULL");
                return null;
            }
            return BuildMessage(output, type, poly, format, functionVariable, functionName, startingValue);
        }
        public static void Export(string path, string output, InputTypes type, PolynomialX poly, TextFormat format, char functionVariable = 'x', string functionName = "f", int startingValue = -1) =>
            File.WriteAllLines(path, BuildMessage(output, type, poly, format, functionVariable, functionName, startingValue));
        public static string[] BuildMessage(string output, InputTypes type, PolynomialX poly, TextFormat format, char functionVariable = 'x', string functionName = "f", int startingValue = -1)
        {
            string exportMessage;
            switch(type)
            {
                case InputTypes.Primes:
                    exportMessage = "Input the first " + output.Length + " consecutive primes to get the message ASCII values for the string: \"" + output + "\".";
                    break;
                case InputTypes.Recursive:
                    exportMessage = "Input " + startingValue + 
                        " into the following function and put the output into the function and repeat the recursive process to get the first " 
                        + output.Length + " outputs which will corrospond to the ASCII values for the string: \"" + output + "\".";
                    break;
                case InputTypes.RecursivePrimes:
                    exportMessage = "Input " + startingValue +
                        " into the following function and multiply the output by the first prime number" +
                        ", then put that product into the function, repeat with the next output and the second consecutive prime prime" +
                        "and so on until "
                        + output.Length + " output values have been obtained.  These values will corospond to the ASCII values which give the following string: \"" + output + "\".";
                    break;
                case InputTypes.PreviousProduct:
                    exportMessage = "Input " + startingValue +
                        " into the following function and multiply the output by the previous input and put that into the function a total of "
                        + (output.Length-1) + " times to get a total of " + output.Length + " output values will corrospond to the ASCII values for the string: \"" + output + "\".";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new string[]
            {
                exportMessage,
                functionName + "(" + functionVariable + ") = " + poly.ToString(functionVariable,format)
            };
        }
        public static IntX[] GenerateInput(IntX[] output, InputTypes type, int startingValue = -1)
        {
            IntX[] input = new IntX[output.Length];
            if (input.Length == 0)
                return input;
            switch (type)
            {
                case InputTypes.Primes:
                    input[0] = 2;
                    for (int i = 1; i < output.Length; i++)
                    {
                        for (IntX j = input[i - 1]; j < int.MaxValue; j++)
                        {
                            bool isPrime = true;
                            for (int k = 0; k < i; k++)
                                if (j % input[k] == 0)
                                {
                                    isPrime = false;
                                    break;
                                }
                            if (isPrime)
                            {
                                input[i] = j;
                                break;
                            }
                        }
                    }
                    break;
                case InputTypes.Recursive://Does not check if input is valid.
                    input[0] = startingValue;
                    for (int i = 1; i < output.Length; i++)
                        input[i] = output[i - 1];
                    break;
                case InputTypes.RecursivePrimes:
                    input = GenerateInput(output,InputTypes.Primes);
                    for (int i = output.Length - 1; i > 0; i--)
                        input[i] = output[i - 1]*input[i-1];
                    input[0] = startingValue;
                    break;
                case InputTypes.PreviousProduct:
                    input[0] = startingValue;
                    for (int i = 1; i < output.Length; i++)
                        input[i] = output[i - 1] * input[i - 1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return input;
        }
        public static PolynomialX Generate(IntX[] input, IntX[] output)
        {
            if (input.Length != output.Length)
                return null;
            MatrixX A = new MatrixX(input.Length, input.Length);

            MatrixX column = new MatrixX(1, input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                IntX prod = 1;
                for(int j = 0; j < input.Length; j++)
                {
                    A[j, i] = prod;
                    prod *= input[i];
                }
                column[0,i] = output[i];
            }
            var det = A.Determinant();
            if (det == 0)
                return null;
            var adjoint = A.GetAdjointMatrix();
            PolynomialX poly = new PolynomialX(input.Length);
            var result = adjoint * column;
            for (int i = 0; i < input.Length; i++)
                poly.Coefficients[i] = (FractionX)result[i,0]/det;
            return poly;
        }
    }
}
