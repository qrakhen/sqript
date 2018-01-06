using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Keyword
    {
        public const string
            REFERENCE = "REFERENCE",
            DESTROY = "DESTROY",
            NEW = "NEW",
            QLASS = "QLASS",
            FUNQTION = "FUNQTION",
            RETURN = "RETURN",
            CONDITION_IF = "IF",
            CONDITION_ELSE = "ELSE",
            CONDITION_LOOP = "LOOP",
            PUBLIC = "PUBLIC",
            PROTECTED = "PROTECTED",
            PRIVATE = "PRIVATE",
            CURRENT_CONTEXT = "CURRENT_CONTEXT",
            PARENT_CONTEXT = "PARENT_CONTEXT";


        public string name { get; private set; }
        public string[] aliases { get; private set; }

        public Keyword(string name, string[] aliases) {
            this.name = name;
            this.aliases = aliases;
        }

        public override string ToString() {
            return name;
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

        public static Keyword[] get() {
            List<Keyword> keywords = new List<Keyword>();
            foreach (var keyword in Keywords.keywords) keywords.Add(keyword.Value);
            return keywords.ToArray();
        }

        public static bool isAlias(string key, string name) {
            if (keywords.ContainsKey(key)) return (keywords[key].name == name);
            foreach (Keyword keyword in keywords.Values) {
                foreach (string alias in keyword.aliases) {
                    if (alias == key) return (keyword.name == name);
                }
            }
            return false;
        }

        public static void define(string name, params string[] aliases) {
            if (get(name) != null) throw new InvalidOperationException("keyword with given name already exists");
            foreach (string alias in aliases) if (get(alias) != null) throw new InvalidOperationException("keyword with given alias '" + alias + "' already exists");
            keywords.Add(name, new Keyword(name, aliases));
        }
    }
}
