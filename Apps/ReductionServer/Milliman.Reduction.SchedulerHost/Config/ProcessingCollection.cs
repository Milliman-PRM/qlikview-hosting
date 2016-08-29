using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost.Config {
    public class ProcessingCollection: System.Configuration.ConfigurationElementCollection {
        protected override string ElementName {get {return "Process";}}
        public ProcessingCollection() {}

        public void Add(ProcessingElement customElement) {
            BaseAdd(customElement);
        }

        protected override void BaseAdd(ConfigurationElement element) {
            base.BaseAdd(element, false);
        }

        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new ProcessingElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((ProcessingElement) element).Name;
        }

        public ProcessingElement this[int Index] {
            get { return (ProcessingElement) BaseGet(Index); }
            set {
                if( BaseGet(Index) != null ) {
                    BaseRemoveAt(Index);
                }
                BaseAdd(Index, value);
            }
        }

        new public ProcessingElement this[string Name] {
            get {
                return (ProcessingElement) BaseGet(Name);
            }
        }

        public int indexof(ProcessingElement element) {
            return BaseIndexOf(element);
        }

        public void Remove(ProcessingElement element) {
            if( BaseIndexOf(element) >= 0 )
                BaseRemove(element.Name);
        }

        public void RemoveAt(int index) {
            BaseRemoveAt(index);
        }

        public void Remove(string name) {
            BaseRemove(name);
        }

        public void Clear() {
            BaseClear();
        }
    }
}
