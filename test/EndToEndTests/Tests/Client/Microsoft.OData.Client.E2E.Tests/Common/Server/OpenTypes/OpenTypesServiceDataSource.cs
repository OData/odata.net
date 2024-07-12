﻿//-----------------------------------------------------------------------------
// <copyright file="OpenTypesServiceDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Spatial;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes
{
    public class OpenTypesServiceDataSource
    {
        static OpenTypesServiceDataSource()
        {
            Initialize();
        }

        public static IList<Row>? Row { get; private set; }
        public static IList<RowIndex>? RowIndex { get; private set; }

        private static void Initialize()
        {
            Row = new List<Row>()
                {
                    new IndexedRow
                    {
                        Id = new Guid("432f0da9-806e-4a2f-b708-dbd1c57a1c21"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            { "Name", "Chris" }
                        }
                    },
                    new IndexedRow()
                    {
                        Id = new Guid("02d5d465-edb3-4169-9176-89dd7c86535e"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            { "Description", "Excellent" }
                        }
                    },
                    new IndexedRow()
                    {
                        Id = new Guid("8f59bcb4-1bed-4b91-ab74-44628f57f160"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            { "Count", 1 }
                        }
                    },
                    new IndexedRow()
                    {
                        Id = new Guid("5dcbef86-a002-4121-8087-f6160fe9a1ed"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            {
                                "Occurred", new DateTimeOffset(2001, 4, 5, 5, 5, 5, 1, new TimeSpan(0, 1, 0))
                            }
                        }
                    },
                    new Row()
                    {
                        Id = new Guid("71f7d0dc-ede4-45eb-b421-555a2aa1e58f"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            {
                                "Double", 1.2626d
                            }
                        }
                    },
                    new Row()
                    {
                        Id = new Guid("672b8250-1e6e-4785-80cf-b94b572e42b3"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            {
                                "Decimal", new Decimal(1.26d)
                            }
                        }
                    },
                    new Row()
                    {
                        Id = new Guid("814d505b-6b6a-45a0-9de0-153b16149d56"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            {
                                "Date", new DateTime(1999, 2, 4)
                            }
                        }
                    },
                    new Row()
                    {
                        Id = new Guid("2e4904b4-00b0-4e37-9f44-b99a6b208dba"),
                        DynamicProperties = new Dictionary<string, object>
                        {
                            {
                                "GeomPolygon", WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new StringReader("SRID=0;POLYGON EMPTY"))
                            }
                        }
                    },
                    new Row()
                    {
                        Id = new Guid("5a76c54e-4553-4bf6-8592-04cbcbfb1e65")
                    },
                    new IndexedRow()
                    {
                        Id = new Guid("9f9c963b-5c2f-4e39-8bec-b45d19c5dc85")
                    }
                };

            RowIndex = new List<RowIndex>();

            for (int i = -10; i <= -1; i++)
            {
                RowIndex.Add(new RowIndex()
                {
                    Id = i
                });
            }
        }
    }
}
