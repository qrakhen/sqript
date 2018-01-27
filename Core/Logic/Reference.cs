namespace Qrakhen.Sqript
{
    internal class Reference : Value<Value>
    {
        public readonly ValueType acceptedType;
        public readonly Qontext owner;
        public readonly Access access;
        public readonly bool isReadonly;
        public readonly string name;

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
            return (getReference() is Reference ? (value as Reference).getTrueValue() : getReference());
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

    internal class FloatingReference<T> : Reference
    {
        public T key { get; private set; }
        public Collection<T> owner { get; private set; }

        public FloatingReference(T key, Collection<T> owner) : base(Null) {
            this.key = key;
            this.owner = owner;
        }

        public void bind() {
            if (value.type != ValueType.Null) owner.set(key, new Reference(value));
        }
    }
}
