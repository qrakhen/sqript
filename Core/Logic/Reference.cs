namespace Qrakhen.Sqript
{
    internal class Reference : Value<Value>
    {
        public readonly ValueType acceptedType;

        public Reference(Value value, ValueType acceptedType = ValueType.Null) : base(ValueType.Reference, value) {
            this.acceptedType = acceptedType;
        }

        public Reference(ValueType acceptedType = ValueType.Null) : base(ValueType.Reference, new Value(null, ValueType.Null)) {
            this.acceptedType = acceptedType;
        }

        public virtual void assign(Value value) { 
            setReference(value);
        }

        public virtual Value getReference() {
            return value;
        }

        public virtual Value getTrueValue() {
            return (value is Reference ? (value as Reference).getTrueValue() : value);
        }

        public virtual void setReference(Value value) {
            if (acceptedType != ValueType.Null && !value.isType(acceptedType))
                throw new ReferenceException(
                    "can not assign value '" + value.str() + "': expected a value of type '" + acceptedType + "', got '" + value.toFullString() + "' instead", this);
            setValue(value, type);
        }

        public new virtual T getValue<T>() {
            return value.getValue<T>();
        }

        public new virtual object getValue() {
            return getTrueValue()?.getValue();
        }

        public ValueType getValueType() {
            return getTrueValue().type;
        }

        public override string ToString() {
            return 
                (acceptedType != ValueType.Null ? "<" + acceptedType + ":" : "") + 
                (getTrueValue() == null ? Null.ToString() : getTrueValue().ToString()) +
                (acceptedType != ValueType.Null ? ">" : "");
        }
    }

    internal class FloatingReference : Reference
    {
        public string name { get; private set; }
        public Qontext owner { get; private set; }

        public FloatingReference(string name, Qontext owner) : base(Null) {
            this.name = name;
            this.owner = owner;
        }

        public void bind() {
            if (value.type != ValueType.Null) owner.set(name, new Reference(value));
        }
    }
}
