namespace Qrakhen.Sqript {
	internal class Reference : Value<Value> {
		public readonly ValueType acceptedType;
		public readonly Qontext owner;
		public readonly Access access;
		public readonly bool isReadonly;
		public readonly string name;

		public Reference(
				Value value,
				Qontext owner = null,
				string name = null,
				Access access = Access.PUBLIC,
				bool isReadonly = false,
				ValueType acceptedType = ValueType.Null) : base(ValueType.Reference, value) {
			this.owner = owner;
			this.name = name;
			this.access = access;
			this.isReadonly = isReadonly;
			this.acceptedType = acceptedType;
		}

		public Reference(ValueType acceptedType = ValueType.Null) : base(ValueType.Reference, Null) {
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
			if(isReadonly)
				throw new ReferenceException("can not set the value of read-only reference '" + name?.ToString() + "'", this);
			else if(acceptedType != ValueType.Null && !value.isType(acceptedType))
				throw new ReferenceException("can not assign value '" + value.str() + "': expected a value of type '" + acceptedType + "', got '" + value.toFullString() + "' instead", this);
			else if(owner != null) {

			} else {
				setValue(value, type);
			}
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
				name != null ? name : "" + " " +
				(acceptedType != ValueType.Null ? "<" + acceptedType + ":" : "") +
				(getTrueValue() == null ? Null.ToString() : getTrueValue().ToString()) +
				(acceptedType != ValueType.Null ? ">" : "");
		}
	}

	internal class FloatingReference<T> : Reference {
		public T key { get; private set; }
		public Collection<T> target { get; private set; }

		public FloatingReference(T key, Collection<T> target) : base(Null) {
			this.key = key;
			this.target = target;
		}

		public void bind() {
			if(target is Qontext && !(target as Qontext).extendable)
				throw new QontextException("can not assign new reference to context: context not extendable or read-only");
			if(value.type != ValueType.Null)
				target.set(key, new Reference(value));
		}
	}
}
