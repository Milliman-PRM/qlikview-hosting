using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost.Config {
    public class FileMonitorCollection: ConfigurationElementCollection {
        public FileMonitorCollection() {}
        protected override string ElementName { get { return "FileMonitor"; } }
        public void Add(FileMonitorElement customElement) {
            BaseAdd(customElement);
        }

        protected override void BaseAdd(ConfigurationElement element) {
            base.BaseAdd(element, false);
        }

        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new FileMonitorElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((FileMonitorElement) element).Name;
        }

        public FileMonitorElement this[int Index] {
            get { return (FileMonitorElement) BaseGet(Index); }
            set {
                if( BaseGet(Index) != null ) {
                    BaseRemoveAt(Index);
                }
                BaseAdd(Index, value);
            }
        }

        new public FileMonitorElement this[string Name] {
            get {
                return (FileMonitorElement) BaseGet(Name);
            }
        }

        public int indexof(FileMonitorElement element) {
            return BaseIndexOf(element);
        }

        public void Remove(FileMonitorElement element) {
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
