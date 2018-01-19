namespace Qrakhen.Sqript
{
    internal class Reference : Value<Value>
    {        
        public Reference(Value value) : base(ValueType.REFERENCE, value) {

        }

        public Reference() : base(ValueType.REFERENCE, new Value(null, ValueType.NULL)) {

        }

        public virtual void assign(Value value) { 
            setReference(value);
        }

        public virtual Value getReference() {
            return value;
        }

        public virtual void setReference(Value value) {
            setValue(value, type);
        }

        public new virtual T getValue<T>() {
            return value.getValue<T>();
        }

        public new virtual object getValue() {
            return value?.getValue();
        }

        public ValueType getValueType() {
            return value.type;
        }

        public override string ToString() {
            return (getReference() == null ? NULL.ToString() : getReference().ToString());
        }
    }

    internal class TypeReference : Reference
    {
        public ValueType forcedType {
            get {
                return forcedType;
            }
            private set {
                if (forcedType == ValueType.NULL) forcedType = value;
                else throw new Exception("can not redefine static reference type");
            }
        }

        public TypeReference(Value value, ValueType forcedType) : base(value) {
            this.forcedType = forcedType;
        }
    }
}
