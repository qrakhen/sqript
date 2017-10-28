using System;
using System.IO;

namespace Qrakhen.Sqript
{
    public class SqriptRuntime
    {
        static void Main(string[] args) {
            defineKeywords();
            defineOperators();
            string content = "";
            do {
                Console.Write(" ~> ");
                content = Console.ReadLine();
                if (content == "test") content = File.ReadAllText("TestScript.sq");
                else if (content == "exit") break;
                try {
                    var nizer = new Tokenizer(content);
                    var result = nizer.parse();
                } catch(Exception e) {
                    Console.WriteLine("exception thrown >> " + e.Message);
                }
            } while (content != "exit");
        }

        static void defineKeywords() {
            Keywords.define("newReference", "reference", "ref", "*~");
            Keywords.define("dereference", "dereference", "del", "~:");
        }

        static void defineOperators() {
            Operators.define("+", delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() + b.getValue<Decimal>()); });
            Operators.define("-", delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() - b.getValue<Decimal>()); });
            Operators.define("/", delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() / b.getValue<Decimal>()); });
            Operators.define("*", delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() * b.getValue<Decimal>()); });
        }
    }
}