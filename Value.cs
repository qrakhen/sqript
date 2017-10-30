﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Value
    {
        public Type type { get; protected set; }
        public object value { get; protected set; }

        public Value(Type type, object value) {
            this.type = type;
            this.value = value;
        }

        public virtual object getValue() {
            return value;
        }

        public virtual T getValue<T>() {
            return (T) value;
        }

        public virtual void setValue(object value) {
            this.value = value;
        }

        public virtual void setValue(object value, Type type) {
            this.value = value;
            this.type = type;
        }

        public virtual void setValue<T>(T value) {
            this.value = value;
        }        

        public bool isType(int types) {
            return (((int) type & types) > 0);
        }
        
        public System.Type getSystemType(Type type) {
            switch (type) {
                case Type.KEYWORD: return typeof(Keyword);
                case Type.OPERATOR: return typeof(Operator);
                case Type.STRUCTURE: return typeof(string);
                case Type.INTEGER: return typeof(int);
                case Type.DECIMAL: return typeof(double);
                case Type.NUMBER: return typeof(double);
                case Type.STRING: return typeof(string);
                case Type.BOOLEAN: return typeof(bool);
                case Type.OBJECT: return typeof(Object);
                case Type.REFERENCE: return typeof(Reference);
                case Type.FUNCTION: return typeof(Function);
                default: return null;
            }
        }

        public enum Type
        {
            KEYWORD = 1,
            OPERATOR = 2,
            STRUCTURE = 4,
            IDENTIFIER = 8,
            INTEGER = 16,
            DECIMAL = 32,
            NUMBER = 48,
            STRING = 128,
            BOOLEAN = 256,
            OBJECT = 512,
            ARRAY = 1024,
            REFERENCE = 2048,
            FUNCTION = 4096,
            UNDEFINED = 8192
        }
    }
}
