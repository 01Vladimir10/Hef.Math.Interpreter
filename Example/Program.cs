// See https://aka.ms/new-console-template for more information

using Hef.Math;

Console.WriteLine("Calculator \n" +
                         "'setv key value' to set variable\n" +
                         "'q' to quit\n----------");

var interpreter = new Interpreter();

var lastResultIdx = 0;
var stop = false;
while (!stop)
{
    try
    {
        Console.Write("  : ");
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
        {
        }
        else if (input == "q" || input == "Q")
        {
            Console.WriteLine("Bood Bye :)");
            stop = true;
        }
        else if (input.StartsWith("setv"))
        {
            var tokens = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            double value;
            if (tokens.Length == 3 && double.TryParse(tokens[2], out value))
            {
                interpreter.SetVar(tokens[1], value);
            }
            else
            {
                Console.WriteLine("Syntax: setv key value");
            }
        }
        else
        {
            var result = interpreter.Calculate(input);
            interpreter.SetVar(lastResultIdx.ToString(), result);
            Console.WriteLine("${1}> {0}", result, lastResultIdx);
            lastResultIdx++;
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e);
    }
}

Console.WriteLine("Hello, World!");