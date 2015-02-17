//---------------------------------------------------------------------
// <copyright file="DSPExpandProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.Linq;


    public class DSPExpandProvider : IExpandProvider
    {
        private IDataServiceQueryProvider queryProvider;

        public DSPExpandProvider(IDataServiceQueryProvider queryProvider)
        {
            this.queryProvider = queryProvider;
        }

        public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
        {
            Debug.Assert(expandPaths.Count == 1, "multiple expand paths not supported");
            Debug.Assert(expandPaths.First().Count == 1, "Deep or empty expands not supported segment collection");

            return new DSPExpandEnumerable(queryable);
        }
    }

    public class DSPExpandEnumerable : IEnumerable
    {
        private IEnumerable source;
        public DSPExpandEnumerable(IEnumerable source) { this.source = source; }

        public IEnumerator GetEnumerator()
        {
            return new DSPExpandEnumerator(this.source.GetEnumerator());
        }
    }

    public class DSPExpandEnumerator : IEnumerator, IExpandedResult
    {
        private IEnumerator source;
        public DSPExpandEnumerator(IEnumerator source) { this.source = source; }

        public object Current
        {
            get { return source.Current; }
        }

        public bool MoveNext()
        {
            return source.MoveNext();
        }

        public void Reset()
        {
            source.Reset();
        }

        public object ExpandedElement
        {
            get { return this.Current; }
        }

        public object GetExpandedPropertyValue(string name)
        {
            Debug.Assert(this.Current is DSPResource, "Only DSPResource supported");
            return ((DSPResource)this.Current).GetValue(name);
        }
    }
}