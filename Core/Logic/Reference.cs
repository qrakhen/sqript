namespace Qrakhen.Sqript
{

	internal class Reference : QValue<QValue>
	{

		public readonly ValueType acceptedType;
		public readonly Qontext owner;
		public readonly Access access;
		public readonly bool isReadonly;
		public readonly string name;


		public Reference(
				QValue value,
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


		public virtual void Assign(QValue value) {
			SetReference(value);
		}

		public virtual QValue GetReference() {
			return Value;
		}

		public virtual QValue GetTrueValue() {
			return (GetReference() is Reference ? (Value as Reference).GetTrueValue() : GetReference());
		}

		public virtual void SetReference(QValue value) {
			if (isReadonly)
				throw new ReferenceException("can not set the value of read-only reference '" + name?.ToString() + "'", this);
			else if (acceptedType != ValueType.Null && !value.IsType(acceptedType))
				throw new ReferenceException("can not assign value '" + value.Str() + "': expected a value of type '" + acceptedType + "', got '" + value.ToFullString() + "' instead", this);
			else if (owner != null) {

			} else {
				base.SetValue((object) value, Type);
			}
		}

		public new virtual T GetValue<T>() {
			return Value.GetValue<T>();
		}

		public new virtual object GetValue() {
			return GetTrueValue()?.GetValue();
		}

		public ValueType GetValueType() {
			return GetTrueValue().Type;
		}

		public override string ToString() {
			return
				this.name ?? "" + " " +
				(acceptedType != ValueType.Null ? "<" + acceptedType + ":" : "") +
				(GetTrueValue() == null ? Null.ToString() : GetTrueValue().ToString()) +
				(acceptedType != ValueType.Null ? ">" : "");
		}
	}

	internal class FloatingReference<T> : Reference
	{

		public T Key { get; private set; }
		public Collection<T> Target { get; private set; }


		public FloatingReference(T key, Collection<T> target) : base(Null) {
			this.Key = key;
			this.Target = target;
		}


		public void Bind() {
			if (Target is Qontext && !(Target as Qontext).extendable)
				throw new QontextException("can not assign new reference to context: context not extendable or read-only");
			if (Value.Type != ValueType.Null)
				Target.Set(Key, new Reference(Value));
		}
	}
}
