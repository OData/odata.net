//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Common
{
    internal abstract class XmlElementParser
    {
        private readonly Dictionary<string, XmlElementParser> childParsers;
        private readonly Dictionary<string, XmlElementParser> descendantParsers;

        protected XmlElementParser(string elementName, Dictionary<string, XmlElementParser> children, Dictionary<string, XmlElementParser> descendants)
        {
            this.ElementName = elementName;
            this.childParsers = children;
            this.descendantParsers = descendants;
        }

        internal string ElementName
        {
            get; 
            private set;
        }

        internal abstract XmlElementValue Parse(XmlElementInfo element, IList<XmlElementValue> children);
        
        internal bool TryGetChildElementParser(string elementName, out XmlElementParser elementParser)
        {
            elementParser = null;
            return this.childParsers != null && this.childParsers.TryGetValue(elementName, out elementParser);
        }

        internal bool TryGetDescendantElementParser(string elementName, out XmlElementParser elementParser)
        {
            elementParser = null;
            return this.descendantParsers != null && this.descendantParsers.TryGetValue(elementName, out elementParser);
        }

        protected void AddChildParser(XmlElementParser child)
        {
            this.childParsers[child.ElementName] = child;
        }

        #region Factory Methods

        internal static XmlElementParser<TResult> Create<TResult>(string elementName, Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc, IEnumerable<XmlElementParser> childParsers, IEnumerable<XmlElementParser> descendantParsers)
        {
            Dictionary<string, XmlElementParser> children = null;
            if (childParsers != null)
            {
                children = childParsers.ToDictionary(p => p.ElementName);
            }

            Dictionary<string, XmlElementParser> descendants = null;
            if (descendants != null)
            {
                descendants = descendantParsers.ToDictionary(p => p.ElementName);
            }

            return new XmlElementParser<TResult>(elementName, children, descendants, parserFunc);
        }
        #endregion
    }

    internal class XmlElementParser<TResult> : XmlElementParser
    {
        private readonly Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc;

        internal XmlElementParser(string elementName,
                                  Dictionary<string, XmlElementParser> children,
                                  Dictionary<string, XmlElementParser> descendants,
                                  Func<XmlElementInfo, XmlElementValueCollection, TResult> parser)
            : base(elementName, children, descendants)
        {
            this.parserFunc = parser;
        }

        internal override XmlElementValue Parse(XmlElementInfo element, IList<XmlElementValue> children)
        {
            TResult result = this.parserFunc(element, XmlElementValueCollection.FromList(children));
            return new XmlElementValue<TResult>(element.Name, element.Location, result);
        }

        public void AddChildParser<ChildElement>(XmlElementParser<ChildElement> child)
        {
            base.AddChildParser(child);
        }
    }

    internal class XmlElementValueCollection : IEnumerable<XmlElementValue>
    {
        internal sealed class MissingXmlElementValue : XmlElementValue
        {
            internal static readonly MissingXmlElementValue Instance = new MissingXmlElementValue();

            private MissingXmlElementValue()
                : base(null, default(CsdlLocation))
            {
            }

            internal override object UntypedValue
            {
                get { return null; }
            }

            internal override bool IsMissing
            {
                get
                {
                    return true;
                }
            }

            internal override bool IsUsed
            {
                get { return false; }
            }
        }

        private static readonly XmlElementValueCollection empty = new XmlElementValueCollection(new XmlElementValue[] { }, new XmlElementValue[] { }.ToLookup(value => value.Name));

        private readonly IList<XmlElementValue> values;
        private ILookup<string, XmlElementValue> nameLookup;

        private XmlElementValueCollection(IList<XmlElementValue> list, ILookup<string, XmlElementValue> nameMap)
        {
            Debug.Assert(list != null, "FromList should replace null list with XmlElementValueCollection.empty");
            this.values = list;
            this.nameLookup = nameMap;
        }

        private ILookup<string, XmlElementValue> EnsureLookup()
        {
            return this.nameLookup ?? (this.nameLookup = this.values.ToLookup(value => value.Name));
        }

        internal static XmlElementValueCollection FromList(IList<XmlElementValue> values)
        {
            if (values == null || values.Count == 0)
            {
                return XmlElementValueCollection.empty;
            }

            return new XmlElementValueCollection(values, null);
        }

        internal XmlElementValue this[string elementName]
        {
            get
            {
                return this.EnsureLookup()[elementName].FirstOrDefault() ?? MissingXmlElementValue.Instance;
            }
        }

        internal IEnumerable<XmlElementValue> FindByName(string elementName)
        {
            return this.EnsureLookup()[elementName];
        }

        internal IEnumerable<XmlElementValue<TResult>> FindByName<TResult>(string elementName)
            where TResult : class
        {
            return this.FindByName(elementName).OfResultType<TResult>();
        }

        internal XmlTextValue FirstText
        {
            get { return (this.values.OfText().FirstOrDefault() ?? XmlTextValue.Missing); }
        }

        public IEnumerator<XmlElementValue> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }
    }

    internal static class XmlElementValueExtensions
    {
        // TODO: Move this to EdmUtil?
        internal static IEnumerator<T> Do<T>(this IEnumerable<T> source, params Action<T>[] toDo)
        {
            IEnumerator<T> enumerator = source.GetEnumerator();
            int actionPos = 0;
            while (actionPos < toDo.Length)
            {
                if (enumerator.MoveNext())
                {
                    toDo[actionPos](enumerator.Current);
                    actionPos++;
                }
                else
                {
                    break;
                }
            }

            return enumerator;
        }

        internal static void Then<T>(this IEnumerator<T> source, Action<T> toDo)
        {
            while (source.MoveNext())
            {
                toDo(source.Current);
            }
        }

        internal static IEnumerable<XmlElementValue<T>> OfResultType<T>(this IEnumerable<XmlElementValue> elements)
            where T : class
        {
            foreach (var element in elements)
            {
                XmlElementValue<T> result = element as XmlElementValue<T>;
                if (result != null)
                {
                    yield return result;
                }
                else if (element.UntypedValue is T)
                {
                    yield return new XmlElementValue<T>(element.Name, element.Location, element.ValueAs<T>());
                }
            }
        }

        internal static IEnumerable<T> ValuesOfType<T>(this IEnumerable<XmlElementValue> elements)
             where T : class
        {
            return elements.OfResultType<T>().Select(ev => ev.Value);
        }

        internal static IEnumerable<XmlTextValue> OfText(this IEnumerable<XmlElementValue> elements)
        {
            foreach (var element in elements)
            {
                if (element.IsText)
                {
                    yield return (XmlTextValue)element;
                }
            }
        }
    }

    internal abstract class XmlElementValue
    {
        internal XmlElementValue(string elementName, CsdlLocation elementLocation)
        {
            this.Name = elementName;
            this.Location = elementLocation;
        }

        internal string Name
        {
            get; 
            private set;
        }

        internal CsdlLocation Location
        {
            get; 
            private set;
        }

        internal abstract object UntypedValue
        {
            get;
        }

        internal abstract bool IsUsed
        {
            get;
        }

        internal virtual bool IsMissing
        {
            get { return false; }
        }

        internal virtual bool IsText
        {
            get { return false; }
        }

        internal virtual string TextValue
        {
            get { return this.ValueAs<string>(); }
        }

        internal virtual TValue ValueAs<TValue>() where TValue : class
        {
            return this.UntypedValue as TValue;
        } 
    }

    internal class XmlElementValue<TValue> : XmlElementValue
    {
        private bool isUsed;
        private readonly TValue value;

        internal XmlElementValue(string name, CsdlLocation location, TValue newValue)
            : base(name, location)
        {
            this.value = newValue;
        }

        internal override bool IsText
        {
            get { return false; }
        }

        internal override bool IsUsed
        {
            get { return this.isUsed; }
        }

        internal override object UntypedValue
        {
            get { return this.value; }
        }

        internal override T ValueAs<T>()
        {
            return this.Value as T;
        }

        internal TValue Value
        {
            get { this.isUsed = true; return this.value; }
        }
    }

    internal class XmlTextValue : XmlElementValue<string>
    {
        internal static readonly XmlTextValue Missing = new XmlTextValue(default(CsdlLocation), null);

        internal const string ElementName = "<\"Text\">";

        internal XmlTextValue(CsdlLocation textLocation, string textValue)
            : base(ElementName, textLocation, textValue)
        {
        }

        internal override bool IsText
        {
            get { return true; }
        }

        internal override string TextValue
        {
            get { return this.Value; }
        }

        internal override bool IsMissing
        {
            get { return object.ReferenceEquals(this, XmlTextValue.Missing); }
        }
    }
}
