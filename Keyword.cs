using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Keyword
    {
        public const string
            DECLARE = "DECLARE",
            DESTROY = "DESTROY",
            NEW = "NEW",
            CLASS = "CLASS",
            FUNCTION = "FUNCTION",
            OBJECT = "OBJECT",
            ARRAY = "ARRAY",
            RETURN = "RETURN";


        public string name { get; private set; }
        public string[] aliases { get; private set; }

        public Keyword(string name, string[] aliases) {
            this.name = name;
            this.aliases = aliases;
        }
    } 


    public static class Keywords
    {
        private static Dictionary<string, Keyword> keywords = new Dictionary<string, Keyword>();

        /// <summary>
        /// Should maybe return an object at some point
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Keyword get(string name) {
            if (keywords.ContainsKey(name)) return keywords[name];
            foreach (Keyword keyword in keywords.Values) {
                foreach (string alias in keyword.aliases){
                    if (alias == name) return keyword;
                }
            }
            return null;
        }

        public static void define(string name, params string[] aliases) {
            if (get(name) != null) throw new InvalidOperationException("keyword with given name already exists");
            foreach (string alias in aliases) if (get(alias) != null) throw new InvalidOperationException("keyword with given alias '" + alias + "' already exists");
            keywords.Add(name, new Keyword(name, aliases));
        }
    }
}
