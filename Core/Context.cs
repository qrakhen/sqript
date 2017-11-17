using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Context
    {
        public Dictionary<string, Reference> references { get; protected set; }
        public Context parent { get; protected set; }

        public Context(Context parent) {
            this.parent = parent;
            this.references = new Dictionary<string, Reference>();
        }

        public void createReference(Reference reference) {
            if (references.ContainsValue(reference)) throw new ReferenceException("can not redeclare reference", reference);
            references.Add(reference.name, reference);
        }

        public void destroyReference(Reference reference) {
            if (!references.ContainsValue(reference)) throw new ReferenceException("can not destroy undeclared reference", reference);
            references.Add(reference.name, reference);
        }

        public void destroyReference(string name) {
            destroyReference(getReference(name));
        }

        public Reference getReference(string name) {
            if (references.ContainsKey(name)) return references[name];
            else if (parent != null) return parent.getReference(name);
            else return null;
        }
    }
}
