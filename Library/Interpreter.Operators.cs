#region License
// Copyright(c) 2017 François Ségaud
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System.Diagnostics.CodeAnalysis;

namespace Hef.Math;

// TODO: Move interface somewhere else.
public partial class Interpreter
{
    #region Static

    private static readonly Dictionary<string, OperatorDescriptor> Operators = new();

    #endregion
        
    private abstract class Node
    {
        public abstract double GetValue(Interpreter interpreter);
    }

    private abstract class ZeroNode : Node
    {
    }

    private abstract class UnaryNode : Node
    {
        protected readonly Node Input;

        protected UnaryNode(Node input)
        {
            Input = input;
        }
    }

    private abstract class BinaryNode : Node
    {
        protected readonly Node LeftInput;
        protected readonly Node RightInput;

        protected BinaryNode(Node leftInput, Node rightInput)
        {
            LeftInput = leftInput;
            RightInput = rightInput;
        }
    }
        
    #region ZeroNode

    private class ValueNode : ZeroNode
    {
        private readonly double _value;

        public ValueNode(double value)
        {
            _value = value;
        }

        public override double GetValue(Interpreter interpreter)
        {
            return _value;
        }
    }

    private class VarNode : ZeroNode
    {
        private readonly string _varName;

        public VarNode(string varName)
        {
            _varName = varName;
        }

        public override double GetValue(Interpreter interpreter)
        {
            if (interpreter.TryGetVariableValue(_varName, out var value))
            {
                return value;
            }

            throw new Exception($"Could not parse variable '{_varName}'");
        }
    }

    [Operator("pi", 0)]
    private class PiNode : ZeroNode
    {
        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.PI;
        }
    }

    [Operator("rand", 0)]
    private class RandNode : ZeroNode
    {
        public override double GetValue(Interpreter interpreter)
        {
            return Random.NextDouble();
        }
    }

    [Operator("true", 0)]
    private class TrueNode : ZeroNode
    {
        public override double GetValue(Interpreter interpreter)
        {
            return True;
        }
    }

    [Operator("false", 0)]
    private class FalseNode : ZeroNode
    {
        public override double GetValue(Interpreter interpreter)
        {
            return False;
        }
    }

    #endregion

    #region UnaryNode

    [Operator("±", 1)]
    [Operator("sign", 1)]
    private class SignNode : UnaryNode
    {
        public SignNode(Node input) : 
            base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return -Input.GetValue(interpreter);
        }
    }

    [Operator("sqrt")]
    private class SqrtNode : UnaryNode
    {
        public SqrtNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Sqrt(Input.GetValue(interpreter));
        }
    }

    [Operator("cos")]
    private class CosNode : UnaryNode
    {
        public CosNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Cos(Input.GetValue(interpreter));
        }
    }

    [Operator("sin")]
    private class SinNode : UnaryNode
    {
        public SinNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Sin(Input.GetValue(interpreter));
        }
    }

    [Operator("tan")]
    private class TanNode : UnaryNode
    {
        public TanNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Tan(Input.GetValue(interpreter));
        }
    }

    [Operator("acos")]
    private class AcosNode : UnaryNode
    {
        public AcosNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Acos(Input.GetValue(interpreter));
        }
    }

    [Operator("asin")]
    private class AsinNode : UnaryNode
    {
        public AsinNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Asin(Input.GetValue(interpreter));
        }
    }

    [Operator("atan")]
    private class AtanNode : UnaryNode
    {
        public AtanNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Atan(Input.GetValue(interpreter));
        }
    }

    [Operator("cosh")]
    private class CoshNode : UnaryNode
    {
        public CoshNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Cosh(Input.GetValue(interpreter));
        }
    }

    [Operator("sinh")]
    private class SinhNode : UnaryNode
    {
        public SinhNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Sinh(Input.GetValue(interpreter));
        }
    }

    [Operator("tanh")]
    private class TanhNode : UnaryNode
    {
        public TanhNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Tanh(Input.GetValue(interpreter));
        }
    }

    [Operator("deg2rad")]
    private class Deg2RadNode : UnaryNode
    {
        public Deg2RadNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (Input.GetValue(interpreter) * System.Math.PI) / 180d;
        }
    }

    [Operator("rad2deg")]
    private class Rad2DegNode : UnaryNode
    {
        public Rad2DegNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (Input.GetValue(interpreter) * 180d) / System.Math.PI;
        }
    }

    [Operator("abs")]
    private class AbsNode : UnaryNode
    {
        public AbsNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Abs(Input.GetValue(interpreter));
        }
    }

    [Operator("round")]
    private class RoundNode : UnaryNode
    {
        public RoundNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Round(Input.GetValue(interpreter));
        }
    }

    [Operator("!", 3)]
    [Operator("not", 3)]
    private class NegNode : UnaryNode
    {
        public NegNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Abs(Input.GetValue(interpreter)) < double.Epsilon ? True : False;
        }
    }

    [Operator("ceil")]
    private class CeilNode : UnaryNode
    {
        public CeilNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Ceiling(Input.GetValue(interpreter));
        }
    }

    [Operator("floor")]
    private class FlorrNode : UnaryNode
    {
        public FlorrNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Floor(Input.GetValue(interpreter));
        }
    }

    [Operator("trunc")]
    private class TruncNode : UnaryNode
    {
        public TruncNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Truncate(Input.GetValue(interpreter));
        }
    }

    [Operator("log")]
    private class LogNode : UnaryNode
    {
        public LogNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Log(Input.GetValue(interpreter));
        }
    }

    [Operator("log10")]
    private class Log10Node : UnaryNode
    {
        public Log10Node(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Log10(Input.GetValue(interpreter));
        }
    }

    [Operator("e")]
    [Operator("exp")]
    private class ExpNode : UnaryNode
    {
        public ExpNode(Node input)
            : base(input)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Exp(Input.GetValue(interpreter));
        }
    }

    #endregion

    #region  BinaryNode

    [Operator("+", 6)]
    [Operator("add", 6)]
    private class AddNode : BinaryNode
    {
        public AddNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) + RightInput.GetValue(interpreter);
        }
    }

    [Operator("-", 6)]
    [Operator("sub", 6)]
    private class SubNode : BinaryNode
    {
        public SubNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) - RightInput.GetValue(interpreter);
        }
    }

    [Operator("*", 5)]
    [Operator("mult", 5)]
    private class MultNode : BinaryNode
    {
        public MultNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) * RightInput.GetValue(interpreter);
        }
    }

    [Operator("/", 5)]
    [Operator("div", 5)]
    private class DivNode : BinaryNode
    {
        public DivNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) / RightInput.GetValue(interpreter);
        }
    }

    [Operator("%", 5)]
    [Operator("mod", 5)]
    private class ModNode : BinaryNode
    {
        public ModNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (int)LeftInput.GetValue(interpreter) % (int)RightInput.GetValue(interpreter);
        }
    }

    [Operator("^")]
    [Operator("pow")]
    private class PowNode : BinaryNode
    {
        public PowNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Pow(LeftInput.GetValue(interpreter), RightInput.GetValue(interpreter));
        }
    }

    [Operator("min")]
    private class MinNode : BinaryNode
    {
        public MinNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Min(LeftInput.GetValue(interpreter), RightInput.GetValue(interpreter));
        }
    }

    [Operator("max")]
    private class MaxNode : BinaryNode
    {
        public MaxNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Max(LeftInput.GetValue(interpreter), RightInput.GetValue(interpreter));
        }
    }

    [Operator("==", 9)]
    [Operator("eq", 9)]
    private class EqualNode : BinaryNode
    {
        public EqualNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Abs(LeftInput.GetValue(interpreter) - RightInput.GetValue(interpreter)) < double.Epsilon ? True : False;
        }
    }

    [Operator("!=", 9)]
    [Operator("ne", 9)]
    private class NonEqualNode : BinaryNode
    {
        public NonEqualNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return System.Math.Abs(LeftInput.GetValue(interpreter) - RightInput.GetValue(interpreter)) < double.Epsilon ? False : True;
        }
    }

    [Operator("lt", 8)]
    [Operator("<", 8)]
    private class LtNode : BinaryNode
    {
        public LtNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) < RightInput.GetValue(interpreter) ? True : False;
        }
    }

    [Operator("lte", 8)]
    [Operator("<=", 8)]
    private class LteNode : BinaryNode
    {
        public LteNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) <= RightInput.GetValue(interpreter) ? True : False;
        }
    }

    [Operator("gt", 8)]
    [Operator(">", 8)]
    private class GtNode : BinaryNode
    {
        public GtNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) > RightInput.GetValue(interpreter) ? True : False;
        }
    }

    [Operator("gte", 8)]
    [Operator(">=", 8)]
    private class GteNode : BinaryNode
    {
        public GteNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return LeftInput.GetValue(interpreter) >= RightInput.GetValue(interpreter) ? True : False;
        }
    }

    [Operator("dice")]
    [Operator("D")]
    [Operator("d")]
    private class DiceNode : BinaryNode
    {
        public DiceNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            int left = (int)LeftInput.GetValue(interpreter);
            int right = (int)RightInput.GetValue(interpreter);

            int value = 0;
            for (int i = 0; i < left; ++i)
            {
                value += Random.Next(1, right + 1);
            }

            return value;
        }
    }

    [Operator("&&", 13)]
    [Operator("and", 13)]
    private class AndNode : BinaryNode
    {
        public AndNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return BoolToDouble(DoubleToBool(LeftInput.GetValue(interpreter)) && DoubleToBool(RightInput.GetValue(interpreter)));
        }
    }

    [Operator("||", 14)]
    [Operator("or", 14)]
    private class OrNode : BinaryNode
    {
        public OrNode(Node leftInput, Node rightInput)
            : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return BoolToDouble(DoubleToBool(LeftInput.GetValue(interpreter)) || DoubleToBool(RightInput.GetValue(interpreter)));
        }
    }

    [Operator("<<", 7)]
    private class LeftShiftNode : BinaryNode
    {
        public LeftShiftNode(Node leftInput, Node rightInput) : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (int)LeftInput.GetValue(interpreter) << (int)RightInput.GetValue(interpreter);
        }
    }

    [Operator(">>", 7)]
    private class RightShiftNode : BinaryNode
    {
        public RightShiftNode(Node leftInput, Node rightInput) : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (int)LeftInput.GetValue(interpreter) >> (int)RightInput.GetValue(interpreter);
        }
    }

    [Operator("|", 12)]
    private class BitOrNode : BinaryNode
    {
        public BitOrNode(Node leftInput, Node rightInput) : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (int)LeftInput.GetValue(interpreter) | (int)RightInput.GetValue(interpreter);
        }
    }

    [Operator("&", 10)]
    private class BitAndNode : BinaryNode
    {
        public BitAndNode(Node leftInput, Node rightInput) : base(leftInput, rightInput)
        {
        }

        public override double GetValue(Interpreter interpreter)
        {
            return (int)LeftInput.GetValue(interpreter) & (int)RightInput.GetValue(interpreter);
        }
    }

    #endregion

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)]
    private class OperatorAttribute : Attribute
    {
        private const int FunctionPriority = 2;

        public readonly string Symbol;
        public readonly int Priority;

        public OperatorAttribute(string symbol, int priority = FunctionPriority)
        {
            Symbol = symbol;
            Priority = priority;
        }
    }
}