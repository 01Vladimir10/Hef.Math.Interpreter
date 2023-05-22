// See https://aka.ms/new-console-template for more information

using Hef.Math;

internal class Program
{
    private const double FALSE = 0d;
    private const double TRUE = 1d;

    private static Player player;
    private static Interpreter interpreter;

    static void Main(string[] args)
    {
        double foo = 40d;
        double bar = 2d;
        double hundred = 100d;
        double global = 20d;

        player = new Player();
        interpreter = new Interpreter();
        interpreter.SetContext("player", new Player());
        interpreter.SetContext("World", new World());
        interpreter.SetVar("Foo", foo);
        interpreter.SetVar("bar", bar);
        interpreter.SetVar("hundred", hundred);
        Interpreter.SetGlobalVar("global", global);

        bool success = true;

        // Old tests.
        success &= Test("±1", -1d);
        success &= Test("sign(1)", -1d);
        success &= Test("1-1", 1d - 1d);
        success &= Test("sub(1, 1)", 1d - 1d);
        success &= Test("1-±1", 1d - -1d);
        success &= Test("2 + 2", 2 + 2d);
        success &= Test("add(2, 2)", 2 + 2d);
        success &= Test("2+2", 2d + 2d);
        success &= Test("(2+2)", 2d + 2d);
        success &= Test("2 * 2", 2d * 2d);
        success &= Test("mult(2, 2)", 2d * 2d);
        success &= Test("2 / 2", 2d / 2d);
        success &= Test("div(2, 2)", 2d / 2d);
        success &= Test("6 % 4", 6 % 4);
        success &= Test("mod(6, 4)", 6 % 4);
        success &= Test("sqrt 4 + 3 * 4", Math.Sqrt(4) + 3 * 4);
        success &= Test("sqrt 4+3*4", Math.Sqrt(4) + 3 * 4);
        success &= Test("(sqrt 4+3)*4", (Math.Sqrt(4) + 3) * 4);
        success &= Test("5 * ±1", 5 * -1d);
        success &= Test("abs ±1", Math.Abs(-1d));
        success &= Test("sin(1+2)", Math.Sin(1 + 2));
        success &= Test("sin 1+2", Math.Sin(1) + 2);
        success &= Test("sin 1*cos 2+cos 1*sin 2",
            Math.Sin(1) * Math.Cos(2) + Math.Cos(1) * Math.Sin(2));
        success &= Test("(2 * 5 == 10) * 5", (2d * 5d == 10 ? 1d : 0d) * 5d);
        success &= Test("min 4 6", Math.Min(4d, 6d));
        success &= Test("max 4 6", Math.Max(4d, 6d));
        success &= Test("(4 gte 4)", 4d >= 4d ? 1d : 0d);
        success &= Test("(4 gte 3)", 4d >= 3d ? 1d : 0d);
        success &= Test("(3 gte 4)", 3d >= 4d ? 1d : 0d);
        success &= Test("$player.Health / $player.MaxHealth", player.GetHealth() / player.MaxHealth);
        success &= Test("$bar", bar);
        success &= Test("sqrt($hundred^2)", Math.Sqrt(hundred * hundred));
        success &= Test("$Foo + $bar", foo + bar);
        success &= Test("round (rand * 10 + 90)");
        success &= Test("1 d 4+1 + 1 D 6+1");
        success &= Test("10^2", Math.Pow(10, 2));
        success &= Test("pow(10, 2)", Math.Pow(10, 2));
        success &= Test("$global", global);

        // Comparison.
        success &= Test("1 == 0", BoolToDouble(1d == 0d));
        success &= Test("1 == 1", BoolToDouble(1d == 1d));
        success &= Test("1 eq 0", BoolToDouble(1d == 0d));
        success &= Test("1 eq 1", BoolToDouble(1d == 1d));
        success &= Test("1 != 0", BoolToDouble(1d != 0d));
        success &= Test("1 != 1", BoolToDouble(1d != 1d));
        success &= Test("1 ne 0", BoolToDouble(1d != 0d));
        success &= Test("1 ne 1", BoolToDouble(1d != 1d));
        success &= Test("1 gt 0", BoolToDouble(1d > 0d));
        success &= Test("1 gt 1", BoolToDouble(1d > 1d));
        success &= Test("1 gt 2", BoolToDouble(1d > 2d));
        success &= Test("1 gte 0", BoolToDouble(1d >= 0d));
        success &= Test("1 gte 1", BoolToDouble(1d >= 1d));
        success &= Test("1 gte 2", BoolToDouble(1d >= 2d));
        success &= Test("1 lt 0", BoolToDouble(1d < 0d));
        success &= Test("1 lt 1", BoolToDouble(1d < 1d));
        success &= Test("1 lt 2", BoolToDouble(1d < 2d));
        success &= Test("1 lte 0", BoolToDouble(1d <= 0d));
        success &= Test("1 lte 1", BoolToDouble(1d <= 1d));
        success &= Test("1 lte 2", BoolToDouble(1d <= 2d));
        success &= Test("1 > 0", BoolToDouble(1d > 0d));
        success &= Test("1 > 1", BoolToDouble(1d > 1d));
        success &= Test("1 > 2", BoolToDouble(1d > 2d));
        success &= Test("1 >= 0", BoolToDouble(1d >= 0d));
        success &= Test("1 >= 1", BoolToDouble(1d >= 1d));
        success &= Test("1 >= 2", BoolToDouble(1d >= 2d));
        success &= Test("1 < 0", BoolToDouble(1d < 0d));
        success &= Test("1 < 1", BoolToDouble(1d < 1d));
        success &= Test("1 < 2", BoolToDouble(1d < 2d));
        success &= Test("1 <= 0", BoolToDouble(1d <= 0d));
        success &= Test("1 <= 1", BoolToDouble(1d <= 1d));
        success &= Test("1 <= 2", BoolToDouble(1d <= 2d));
        success &= Test("(1 eq 1) == (1 == 1)", TRUE);
        success &= Test("(1 eq 0) == (1 == 0)", TRUE);
        success &= Test("(1 eq 1) eq (0 == 0)", TRUE);
        success &= Test("(1 eq 0) eq (0 == 1)", TRUE);

        // Boolean.
        success &= Test("!1", FALSE);
        success &= Test("!0", TRUE);
        success &= Test("!2", FALSE);
        success &= Test("!0.5", FALSE);
        success &= Test("true", BoolToDouble(true));
        success &= Test("false", BoolToDouble(false));
        success &= Test("!true", BoolToDouble(!true));
        success &= Test("!false", BoolToDouble(!false));
        success &= Test("true && true", BoolToDouble(true && true));
        success &= Test("true && false", BoolToDouble(true && false));
        success &= Test("false && true", BoolToDouble(false && true));
        success &= Test("false && false", BoolToDouble(false && false));
        success &= Test("true and true", BoolToDouble(true && true));
        success &= Test("true and false", BoolToDouble(true && false));
        success &= Test("false and true", BoolToDouble(false && true));
        success &= Test("false and false", BoolToDouble(false && false));
        success &= Test("true || true", BoolToDouble(true || true));
        success &= Test("true || false", BoolToDouble(true || false));
        success &= Test("false || true", BoolToDouble(false || true));
        success &= Test("false || false", BoolToDouble(false || false));
        success &= Test("true or true", BoolToDouble(true || true));
        success &= Test("true or false", BoolToDouble(true || false));
        success &= Test("false or true", BoolToDouble(false || true));
        success &= Test("false or false", BoolToDouble(false || false));

        // Binary
        success &= Test("1 << 4", 1 << 4);
        success &= Test("32 >> 4", 32 >> 4);
        success &= Test("1 << 4 >> 4", 1 << 4 >> 4);
        success &= Test("32 >> 4 << 4", 32 >> 4 << 4);
        success &= Test("4 | 2", 4 | 2);
        success &= Test("6 | 2", 6 | 2);
        success &= Test("4 & 2", 4 & 2);
        success &= Test("6 & 2", 6 & 2);

        // Trigonometry.
        success &= Test("cos 0", Math.Cos(0d));
        success &= Test("cos (pi / 2)", Math.Cos(Math.PI / 2d));
        success &= Test("cos pi", Math.Cos(Math.PI));
        success &= Test("sin 0", Math.Sin(0d));
        success &= Test("sin (pi / 2)", Math.Sin(Math.PI / 2d));
        success &= Test("sin pi", Math.Sin(Math.PI));
        success &= Test("acos 0", Math.Acos(0d));
        success &= Test("acos 1", Math.Acos(1d));
        success &= Test("acos ±1", Math.Acos(-1d));
        success &= Test("asin 0", Math.Asin(0d));
        success &= Test("asin 1", Math.Asin(1d));
        success &= Test("asin ±1", Math.Asin(-1d));
        success &= Test("tan 0", Math.Tan(0));
        success &= Test("tan pi", Math.Tan(Math.PI));
        success &= Test("tan (pi / 4)", Math.Tan(Math.PI / 4d));
        success &= Test("tan (3 * pi / 4)", Math.Tan(3 * Math.PI / 4d));
        success &= Test("deg2rad 0", 0d);
        success &= Test("deg2rad 90", Math.PI * .5d);
        success &= Test("deg2rad 180", Math.PI);
        success &= Test("deg2rad 270", Math.PI * 1.5d);
        success &= Test("deg2rad 360", Math.PI * 2d);
        success &= Test("rad2deg (0)", 0d);
        success &= Test("rad2deg (pi * 0.5)", 90d);
        success &= Test("rad2deg (pi)", 180d);
        success &= Test("rad2deg (pi * 1.5)", 270d);
        success &= Test("rad2deg (pi * 2)", 360d);

        // Writing style and comma separator.
        success &= Test("min(1,2)", Math.Min(1, 2));
        success &= Test("min(1, 2)", Math.Min(1, 2));
        success &= Test("min(1 2)", Math.Min(1, 2));
        success &= Test("min 1 2", Math.Min(1, 2));
        success &= Test("min 1,2", Math.Min(1, 2));
        success &= Test("min 1, 2", Math.Min(1, 2));
        success &= Test("min(2,1)", Math.Min(1, 2));
        success &= Test("min(2, 1)", Math.Min(1, 2));
        success &= Test("min(2 1)", Math.Min(1, 2));
        success &= Test("min 2 1", Math.Min(1, 2));
        success &= Test("min 2,1", Math.Min(1, 2));
        success &= Test("min 2, 1", Math.Min(1, 2));

        // Rounding
        success &= Test("round(1.0)", Math.Round(1.0d));
        success &= Test("round(1.1)", Math.Round(1.1d));
        success &= Test("round(1.5)", Math.Round(1.5d));
        success &= Test("round(1.9)", Math.Round(1.9d));
        success &= Test("trunc(1.0)", Math.Truncate(1.0d));
        success &= Test("trunc(1.1)", Math.Truncate(1.1d));
        success &= Test("trunc(1.5)", Math.Truncate(1.5d));
        success &= Test("trunc(1.9)", Math.Truncate(1.9d));
        success &= Test("floor(1.0)", Math.Floor(1.0d));
        success &= Test("floor(1.1)", Math.Floor(1.1d));
        success &= Test("floor(1.5)", Math.Floor(1.5d));
        success &= Test("floor(1.9)", Math.Floor(1.9d));
        success &= Test("ceil(1.0)", Math.Ceiling(1.0d));
        success &= Test("ceil(1.1)", Math.Ceiling(1.1d));
        success &= Test("ceil(1.5)", Math.Ceiling(1.5d));
        success &= Test("ceil(1.9)", Math.Ceiling(1.9d));

        // Algebra.
        success &= Test("log(0.5)", Math.Log(.5d));
        success &= Test("log(1.0)", Math.Log(1d));
        success &= Test("log(2.0)", Math.Log(2d));
        success &= Test("log10(0.5)", Math.Log10(.5d));
        success &= Test("log10(1.0)", Math.Log10(1d));
        success &= Test("log10(2.0)", Math.Log10(2d));
        success &= Test("e(0.0)", Math.Exp(0d));
        success &= Test("e(0.5)", Math.Exp(.5d));
        success &= Test("e(1.0)", Math.Exp(1d));
        success &= Test("e(2.0)", Math.Exp(2d));
        success &= Test("exp(0.0)", Math.Exp(0d));
        success &= Test("exp(0.5)", Math.Exp(.5d));
        success &= Test("exp(1.0)", Math.Exp(1d));
        success &= Test("exp(2.0)", Math.Exp(2d));

        Console.WriteLine("--------------------\nOVERALL RESULT: " + success);

        interpreter.Dispose();
    }

    private static bool Test(string infix)
    {
        Console.WriteLine("{0} = {1} (nocheck)", infix, interpreter.Calculate(infix));

        return true;
    }

    private static bool Test(string infix, double intendedResult)
    {
        double result = interpreter.Calculate(infix);
        bool match = Math.Abs(intendedResult - result) < double.Epsilon;

        if (match)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} = {1} -> {2}", infix, result, match);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} = {1} (instead of {3}) -> {2}", infix, result, match, intendedResult);
        }

        Console.ForegroundColor = ConsoleColor.Gray;

        return match;
    }

    private static bool DoubleToBool(double value)
    {
        return Math.Abs(value - 1d) < double.Epsilon;
    }

    private static double BoolToDouble(bool value)
    {
        return value ? 1d : 0d;
    }
}

public class Player : IInterpreterContext
{
    public double MaxHealth
    {
        get { return 100d; }
    }

    public double GetHealth()
    {
        return 50d;
    }

    public bool TryGetVariable(string name, out double value)
    {
        value = 0d;
        if (name == "Health")
        {
            value = GetHealth();
            return true;
        }
        else if (name == "MaxHealth")
        {
            value = MaxHealth;
            return true;
        }

        return false;
    }
}

public class World : IInterpreterContext
{
    public bool TryGetVariable(string name, out double value)
    {
        value = 0d;
        if (name != "width" && name != "height") return false;
        value = 8d;
        return true;

    }
}