using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    public class Keyword
    {
        public struct Kwrd {
            public readonly int id;
            public readonly string name;
            public Kwrd(int id, string name) {
                this.id = id;
                this.name = name;
            }
        }

        public readonly static Kwrd
            REFERENCE = new Kwrd(0x000001, "REFERENCE"),
            DESTROY = new Kwrd(0x000002, "DESTROY"),
            NEW = new Kwrd(0x000004, "NEW"),
            RETURN = new Kwrd(0x000008, "RETURN"),
            QLASS = new Kwrd(0x000010, "QLASS"),
            FUNQTION = new Kwrd(0x000020, "FUNQTION"),
            NAMESPACE = new Kwrd(0x000040, "NAMESPACE"),
            CONDITION_IF = new Kwrd(0x000100, "CONDITION_IF"),
            BOOL_TRUE = new Kwrd(0x000101, "BOOL_TRUE"),
            BOOL_FALSE = new Kwrd(0x000102, "BOOL_FALSE"),
            CONDITION_ELSE = new Kwrd(0x000200, "CONDITION_ELSE"),
            CONDITION_LOOP = new Kwrd(0x000400, "CONDITION_LOOP"),
            PUBLIC = new Kwrd(0x001000, "PUBLIC"),
            PROTECTED = new Kwrd(0x002000, "PROTECTED"),
            PRIVATE = new Kwrd(0x003000, "PRIVATE"),
            CURRENT_CONTEXT = new Kwrd(0x010000, "CURRENT_CONTEXT"),
            PARENT_CONTEXT = new Kwrd(0x020000, "PARENT_CONTEXT");

        public enum KeywordType {
            FUNCTIONAL = 0x000F,
            DECLARATION = 0x0070,
            CONDITION = 0x0700,
            DECLARATION_MODIFIER = 0x7000,
            REFERENCIAL = 0x030000
        }

        public int id { get; private set; }
        public string name { get; private set; }
        public string[] aliases { get; private set; }

        public Keyword(int id, string name, string[] aliases) {
            this.id = id;
            this.name = name;
            this.aliases = aliases;
        }

        public override string ToString() {
            return name;
        }

        public bool isType(KeywordType type) {
            return ((id & (int)type) > 0);
        }

        public T getKey<T>() {
            return (T)(object)name;
        }

        public override bool Equals(object obj) {
            if (obj is Keyword) {
                return (id == (obj as Keyword).id);
            } else if (obj is Kwrd) {
                Kwrd k = (Kwrd)obj;
                return (id == k.id);
            } else return base.Equals(obj);
        }

        public static bool operator ==(Keyword a, Keyword b) {
            bool? r = (a?.Equals(b));
            if (r.HasValue) return r.Value;
            else return false;
        }

        public static bool operator ==(Keyword a, Kwrd b) {
            bool? r = (a?.Equals(b));
            if (r.HasValue) return r.Value;
            else return false;
        }

        public static bool operator !=(Keyword a, Keyword b) {
            bool? r = !(a?.Equals(b));
            if (r.HasValue) return r.Value;
            else return false;
        }

        public static bool operator !=(Keyword a, Kwrd b) {
            bool? r = !(a?.Equals(b));
            if (r.HasValue) return r.Value;
            else return false;
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

        public static bool isAlias(string key, Keyword.Kwrd kwrd) {
            return isAlias(key, kwrd.name);
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

        public static void define(Keyword.Kwrd kwrd, params string[] aliases) {
            if (get(kwrd.name) != null) throw new InvalidOperationException("keyword with given name already exists");
            foreach (string alias in aliases) if (get(alias) != null) throw new InvalidOperationException("keyword with given alias '" + alias + "' already exists");
            keywords.Add(kwrd.name, new Keyword(kwrd.id, kwrd.name, aliases));
        }
    }
}
