//---------------------------------------------------------------------
// <copyright file="XmlElementParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
    internal static class XmlElementValueExtensions
    {
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

    internal abstract class XmlElementParser
    {
        private readonly Dictionary<string, XmlElementParser> childParsers;

        protected XmlElementParser(string elementName, Dictionary<string, XmlElementParser> children)
        {
            this.ElementName = elementName;
            this.childParsers = children;
        }

        internal string ElementName
        {
            get;
            private set;
        }

        public void AddChildParser(XmlElementParser child)
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

            return new XmlElementParser<TResult>(elementName, children, parserFunc);
        }
        #endregion

        internal abstract XmlElementValue Parse(XmlElementInfo element, IList<XmlElementValue> children);

        internal bool TryGetChildElementParser(string elementName, out XmlElementParser elementParser)
        {
            elementParser = null;
            return this.childParsers != null && this.childParsers.TryGetValue(elementName, out elementParser);
        }
    }

    internal class XmlElementParser<TResult> : XmlElementParser
    {
        private readonly Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc;

        internal XmlElementParser(
            string elementName,
            Dictionary<string, XmlElementParser> children,
            Func<XmlElementInfo, XmlElementValueCollection, TResult> parser)
            : base(elementName, children)
        {
            this.parserFunc = parser;
        }

        internal override XmlElementValue Parse(XmlElementInfo element, IList<XmlElementValue> children)
        {
            TResult result = this.parserFunc(element, XmlElementValueCollection.FromList(children));
            return new XmlElementValue<TResult>(element.Name, element.Location, result);
        }
    }

    internal class XmlElementValueCollection : IEnumerable<XmlElementValue>
    {
        private static readonly XmlElementValueCollection empty = new XmlElementValueCollection(new XmlElementValue[] { }, new XmlElementValue[] { }.ToLookup(value => value.Name));

        private readonly IList<XmlElementValue> values;
        private ILookup<string, XmlElementValue> nameLookup;

        private XmlElementValueCollection(IList<XmlElementValue> list, ILookup<string, XmlElementValue> nameMap)
        {
            Debug.Assert(list != null, "FromList should replace null list with XmlElementValueCollection.empty");
            this.values = list;
            this.nameLookup = nameMap;
        }

        internal XmlTextValue FirstText
        {
            get { return (this.values.OfText().FirstOrDefault() ?? XmlTextValue.Missing); }
        }

        internal XmlElementValue this[string elementName]
        {
            get
            {
                return this.EnsureLookup()[elementName].FirstOrDefault() ?? MissingXmlElementValue.Instance;
            }
        }

        public IEnumerator<XmlElementValue> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        internal bool Remove(XmlElementValue value)
        {
            if (value == null)
            {
                return false;
            }

            return this.values.Remove(value);
        }

        internal static XmlElementValueCollection FromList(IList<XmlElementValue> values)
        {
            if (values == null || values.Count == 0)
            {
                return XmlElementValueCollection.empty;
            }

            return new XmlElementValueCollection(values, null);
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

        private ILookup<string, XmlElementValue> EnsureLookup()
        {
            return this.nameLookup ?? (this.nameLookup = this.values.ToLookup(value => value.Name));
        }

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

            internal override bool IsUsed
            {
                get { return false; }
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
        private readonly TValue value;
        private bool isUsed;

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

        internal TValue Value
        {
            get
            {
                this.isUsed = true;
                return this.value;
            }
        }

        internal override T ValueAs<T>()
        {
            return this.Value as T;
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
    }
}
