using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigComparisonGui
{
    class NameValueCfgElementCollection : ConfigurationElementCollection
    {
        public NameValueCfgElementCollection()
        {
            Console.WriteLine("NameValueCfgElementCollection Default Constructor");
        }

        public NameValueCfgElement this[int index]
        {
            get { return (NameValueCfgElement)BaseGet(index); }
            set {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(NameValueCfgElement NewElement)
        {
            BaseAdd(NewElement);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new NameValueCfgElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NameValueCfgElement)element).KeyName;
        }

        public void Remove(NameValueCfgElement DeadElement)
        {
            BaseRemove(DeadElement.KeyName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

    }
}
