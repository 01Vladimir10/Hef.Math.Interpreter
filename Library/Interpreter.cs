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

namespace Hef.Math;

/// <summary>
/// An interpreter able to resolve a mathmatical formula.
/// </summary>
public partial class Interpreter : IDisposable
{
    #region Constants

    private const string VarPrefixStr       = "$";
    private const char   VarPrefixChar      = '$';
    private const string OpMarkStr          = "_";
    private const char   OpMarkChar         = '_';
    private const string LongOpMark0Str     = " _";
    private const string LongOpMark1Str     = "_ ";
    private const string OpenBracketStr     = "(";
    private const string ClosingBracketStr  = ")";
    private const char   OpenBracketChar    = '(';
    private const char   ClosingBracketChar = ')';
    private const string WhiteSpaceStr      = " ";
    private const char   WhiteSpaceChar     = ' ';
    private const char   CommaSeparatorChar = ',';

    #endregion

    #region Static

    private static readonly Cache<string, Node> Cache;
    private static readonly Dictionary<string, double> GlobalVariables;
    private static readonly Random Random;

    #endregion

    #region Members

    private Dictionary<string, double>? _variables;
    private Dictionary<string, IInterpreterContext>? _namedContext;
    private bool _disposed;

    #endregion

    #region Enumerations

    private enum OperatorType
    {
        Const = 0,
        Unary,
        Binary
    }

    #endregion

    #region Constructors

    static Interpreter()
    {
        Cache = new Cache<string, Node>(64);
        GlobalVariables = new Dictionary<string, double>();
        Random = new Random();

        LoadOperators();
    }

    /// <summary>
    /// Instantiates a new instance of Interpreter.
    /// </summary>
    public Interpreter()
    {
        _variables = new Dictionary<string, double>();
        _namedContext = new Dictionary<string, IInterpreterContext>();
    }

    #endregion

    #region Destructor

    /// <summary>
    /// Destructor.
    /// </summary>
    ~Interpreter()
    {
        Dispose(false);
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Sets a variable to be used in the formula.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The variable value.</param>
    public void SetVar(string name, double value)
    {
        name = name.StartsWith(VarPrefixStr) ? name : $"{VarPrefixStr}{name}";
        _variables ??= new Dictionary<string, double>();
        _variables[name] = value;
    }

    /// <summary>
    /// Sets a variable to be used in the formula. This variable will be global to ALL interpreters.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The variable value.</param>
    public static void SetGlobalVar(string name, double value)
    {
        if (!GlobalVariables.ContainsKey(name))
        {
            name = name.StartsWith(VarPrefixStr) ? name : $"{VarPrefixStr}{name}";
            GlobalVariables.Add(name, value);
        }
    }

    /// <summary>
    /// Sets an interpreter context to be use un variables resolution.
    /// </summary>
    /// <param name="name">The name of the context..</param>
    /// <param name="interpreterContext">An object that implements Hef.Math.IInterpreterContext. Null to re;ove context.</param>
    public void SetContext(string name, IInterpreterContext? interpreterContext)
    {
        if (_namedContext is null)
        {
            return;
        }
        if (interpreterContext == null)
        {
            _namedContext.Remove(name);
        }
        else
        {
            _namedContext[name] = interpreterContext;
        }
    }

    /// <summary>
    /// Compute the formula passed as argument.
    /// </summary>
    /// <param name="infix">The formula to resolve.</param>
    /// <returns></returns>
    public double Calculate(string infix)
    {
        var root = Cache.GetOrInitializeValue(infix, InfixToNode);
        return root.GetValue(this);
    }

    /// <summary>
    /// Forces the cache to be cleard.
    /// </summary>
    public static void ForceClearCache()
    {
        Cache.Clear();
    }

    /// <summary>
    /// Dispose this instance of Interpreter.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Private functions

    private static int ComparePrecedence(string a, string b)
    {
        if (!Operators.ContainsKey(a))
        {
            throw new Exception($"Operator '{a}' is not registered.");
        }

        if (!Operators.ContainsKey(b))
        {
            throw new Exception($"Operator '{b}' is not registered.");
        }

        return Operators[b].Priority - Operators[a].Priority;
    }

    private static bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private static bool IsNumeric(char c)
    {
        return c >= '0' && c <= '9';
    }

    private static int SkipString(string value, int index)
    {
        // Also allow dots for names context cariable access `$xxx.yyy`.
        // [#12] Operators with names containing digits fail -> Fixed by adding IsNumeric().
        while (index < value.Length && (IsAlpha(value[index]) || IsNumeric(value[index]) || value[index] == '.'))
        {
            ++index;
        }

        return index;
    }

    private static bool IsSpecial(char c)
    {
        return !IsNumeric(c) && !IsAlpha(c) && c != OpenBracketChar && c != ClosingBracketChar && c != VarPrefixChar && c != WhiteSpaceChar && c != '.' && c != '±';
    }

    private static int SkipSpecial(string value, int index)
    {
        while (index < value.Length && IsSpecial(value[index]))
        {
            ++index;
        }

        return index;
    }

    private static string InfixToRpn(string infix)
    {
        // Replace comma separator with white space for function-like use of operators.
        infix = infix.Replace(CommaSeparatorChar, WhiteSpaceChar);

        // Add operator markers.
        for (var index = 0; index < infix.Length; ++index)
        {
            if (infix[index] == VarPrefixChar)
            {
                index = SkipString(infix, index + 2);
            }
            else if (IsAlpha(infix[index]))
            {
                infix = infix.Insert(index, LongOpMark0Str);
                index = SkipString(infix, index + 2);
                infix = infix.Insert(index, LongOpMark1Str);
            }
            else if (IsSpecial(infix[index]))
            {
                infix = infix.Insert(index, LongOpMark0Str);
                index = SkipSpecial(infix, index + 2);
                infix = infix.Insert(index, LongOpMark1Str);
            }
        }

        // Add blank spaces where needed.
        for (var index = 0; index < infix.Length; ++index)
        {
            if (Operators.ContainsKey(infix[index].ToString()) || infix[index] == VarPrefixChar || infix[index] == OpMarkChar
                || infix[index] == OpenBracketChar || infix[index] == ClosingBracketChar)
            {
                // Ignore variable. It would be a mess to find an operator in the middle of a variable name...
                if (infix[index] == VarPrefixChar)
                {
                    index = SkipString(infix, index + 2);
                    //continue;
                }

                if (index != 0 && infix[index - 1] != WhiteSpaceChar)
                {
                    infix = infix.Insert(index, WhiteSpaceStr);
                }

                // Handle long operators.
                var jndex = index;
                if (infix[index] == OpMarkChar)
                {
                    jndex = infix.IndexOf(OpMarkChar, index + 1);
                }

                if (jndex != infix.Length - 1 && infix[jndex + 1] != OpMarkChar)
                {
                    infix = infix.Insert(jndex + 1, WhiteSpaceStr);
                }

                index = jndex;
            }
        }

        // Trim long op mark and white spaces.
        infix = System.Text.RegularExpressions.Regex.Replace(infix.Replace(OpMarkStr, string.Empty), @"\s+", " ");
        infix = infix.TrimStart(WhiteSpaceChar);
        infix = infix.TrimEnd(WhiteSpaceChar);

        var tokens = infix.Split(WhiteSpaceChar);
        var list = new List<string>();     //TODO: static
        var stack = new Stack<string>();  //TODO: static

        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token) || token == WhiteSpaceStr)
            {
                continue;
            }

            if (Operators.ContainsKey(token))
            {
                while (stack.Count > 0 && Operators.ContainsKey(stack.Peek()))
                {
                    if (ComparePrecedence(token, stack.Peek()) < 0)
                    {
                        list.Add(stack.Pop());
                        continue;
                    }

                    break;
                }

                stack.Push(token);
            }
            else if (token == OpenBracketStr)
            {
                stack.Push(token);
            }
            else if (token == ClosingBracketStr)
            {
                while (stack.Count > 0 && stack.Peek() != OpenBracketStr)
                {
                    list.Add(stack.Pop());
                }

                stack.Pop();
            }
            else
            {
                list.Add(token);
            }
        }

        while (stack.Count > 0)
        {
            list.Add(stack.Pop());
        }

        var rpn = string.Join(WhiteSpaceStr, list.ToArray());

        return rpn;
    }

    private static Node RpnToNode(string rpn)
    {
        var tokens = rpn.Split(WhiteSpaceChar);
        var values = new Stack<Node>();

        foreach (var token in tokens)
        {
            if (Operators.TryGetValue(token, out var @operator))
            {
                if (@operator.NodeType.IsSubclassOf(typeof (ZeroNode)))
                {
                    var constructorInfo = @operator.NodeType.GetConstructor(Type.EmptyTypes);
                    if (constructorInfo != null)
                    {
                        var node = (Node) constructorInfo.Invoke(Array.Empty<object>());
                        values.Push(node);
                    }
                }

                if (@operator.NodeType.IsSubclassOf(typeof (UnaryNode)))
                {
                    var constructorInfo = @operator.NodeType.GetConstructor(new [] {typeof (Node)});
                    if (constructorInfo != null)
                    {
                        var node = (Node) constructorInfo.Invoke(new object[] {values.Pop()});
                        values.Push(node);
                    }
                }

                if (@operator.NodeType.IsSubclassOf(typeof (BinaryNode)))
                {
                    var constructorInfo = @operator.NodeType.GetConstructor(new [] {typeof (Node), typeof (Node)});
                    if (constructorInfo != null)
                    {
                        var right = values.Pop();
                        var left = values.Pop();
                        var node = (Node) constructorInfo.Invoke(new object[] {left, right});
                        values.Push(node);
                    }
                }
            }
            else
            {
                if (double.TryParse(token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out var value))
                {
                    Node node = new ValueNode(value);
                    values.Push(node);
                }
                else
                {
                    Node node = new VarNode(token);
                    values.Push(node);
                }
            }
        }

        if (values.Count != 1)
        {
            throw new InvalidOperationException("Cannot calculate formula");
        }

        return values.Pop();
    }

    private static Node InfixToNode(string infix)
    {
        return RpnToNode(InfixToRpn(infix));
    }

    private static void LoadOperators()
    {
        var nodeType = typeof(Node);
        var assembly = System.Reflection.Assembly.GetAssembly(nodeType);
        var allTypes = assembly?.GetTypes();

        if (allTypes is null)
            return;
        foreach (var type in allTypes)
        {
            if (!type.IsSubclassOf(nodeType) || type.IsAbstract) continue;
            var attributes = (OperatorAttribute[])type.GetCustomAttributes(typeof(OperatorAttribute), true);
            foreach (var operatorAttribute in attributes)
            {
                Operators.Add(operatorAttribute.Symbol, new OperatorDescriptor(operatorAttribute.Priority, type));
            }
        }
    }

    private bool TryGetVariableValue(string varName, out double value)
    {
        // Look in local variables.
        if (_variables is null)
        {
            value = 0;
            return false;
        }
            
        if (_variables.TryGetValue(varName, out value))
        {
            return true;
        }

        // Look in named contexts.
        // Fixed #23 (bad regex).
        if (VariableRegex().IsMatch(varName))
        {
            var contextName = varName.Substring(varName.IndexOf('$') + 1, varName.IndexOf('.') - 1);
            var variableName = varName[(varName.IndexOf('.') + 1)..];

            if (_namedContext is not null && _namedContext.ContainsKey(contextName) && _namedContext[contextName].TryGetVariable(variableName, out value))
            {
                return true;
            }
        }

        // Look in global variables;
        return GlobalVariables.TryGetValue(varName, out value);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_variables != null)
            {
                _variables.Clear();
                _variables = null;
            }

            if (_namedContext != null)
            {
                _namedContext.Clear();
                _namedContext = null;
            }
        }

        _disposed = true;
    }

    #endregion

    #region Inner Types

    private struct OperatorDescriptor
    {
        public readonly int Priority;
        public readonly Type NodeType;

        public OperatorDescriptor(int priority, Type nodeType)
        {
            Priority = priority;
            NodeType = nodeType;
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex("\\$\\w+\\.\\w+")]
    private static partial System.Text.RegularExpressions.Regex VariableRegex();

    #endregion
}