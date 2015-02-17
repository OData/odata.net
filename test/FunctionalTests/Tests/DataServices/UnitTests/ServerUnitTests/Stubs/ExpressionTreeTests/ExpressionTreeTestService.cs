//---------------------------------------------------------------------
// <copyright file="ExpressionTreeTestService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces
    
    /// <summary>
    /// Test toolset for Expression Tree verification and validation 
    /// </summary>
    public class ExpressionTreeTestUtils
    {
        /// <summary>
        /// This field is used to pass queryable from the service back to the calling methods
        /// </summary>
        private static IQueryable lastQueryable;

        /// <summary>
        /// Create a TestWebRequest using <paramref name="dataServiceType"/> and <paramref name="uri"/>.
        /// Execute the request and extract the IQueryable generated from the service
        /// </summary>
        /// <param name="dataServiceType">The context type for the service</param>
        /// <param name="uri">The uri for this request</param>
        /// <returns>An IQueryable object representing the Queryable for this request</returns>
        public static IQueryable CreateRequestAndGetQueryable(Type dataServiceType, string uri)
        {
            TestUtil.ClearMetadataCache();
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(ExpressionDataService<>).MakeGenericType(dataServiceType);
                request.RequestUriString = uri;
                request.SendRequest();
                return lastQueryable;
            }
        }

        /// <summary>
        /// Create a TestWebRequest using <paramref name="dataServiceType"/> and <paramref name="uri"/>.
        /// Execute the request and extract the IQueryable generated from the service, then serialize the expression tree into XML
        /// </summary>
        /// <param name="dataServiceType">The context type for the service</param>
        /// <param name="uri">The uri for this request</param>
        /// <returns>An XmlDocument object serialized from the Queryable for this request</returns>
        public static XmlDocument CreateRequestAndGetExpressionTreeXml(Type dataServiceType, string uri)
        {
            IQueryable queryable = CreateRequestAndGetQueryable(dataServiceType, uri);
            return ExpressionTreeToXmlSerializer.SerializeToXml(queryable.Expression);
        }

        /// <summary>
        /// Returns the last queryable executed.
        /// </summary>
        /// <returns>The last IQueryable executed.</returns>
        public static IQueryable GetLastQueryable()
        {
            return lastQueryable;
        }

        /// <summary>
        /// Returns the last expression tree executed.
        /// </summary>
        /// <returns>The expression tree executed serialized into XML.</returns>
        public static XmlDocument GetLastExpressionTreeXml()
        {
            return ExpressionTreeToXmlSerializer.SerializeToXml(GetLastQueryable().Expression);
        }

        /// <summary>
        /// Registers the expression tree capture on a specified service object
        /// </summary>
        /// <param name="serviceObject">The service object - DataService of T - to register for expression tree capture.</param>
        public static void RegisterOnService(object serviceObject)
        {
            Type dataServiceOfT = serviceObject.GetType();
            while (dataServiceOfT != null)
            {
                if (dataServiceOfT.IsGenericType && dataServiceOfT.GetGenericTypeDefinition() == typeof(DataService<>))
                {
                    break;
                }
                dataServiceOfT = dataServiceOfT.BaseType;
            }
            Assert.IsNotNull(dataServiceOfT, "The service object doesn't derive from DataService<T>.");

            FieldInfo fi = dataServiceOfT.GetField("requestQueryableConstructed", BindingFlags.Instance | BindingFlags.SetField | BindingFlags.NonPublic);
            if (fi != null)
            {
                fi.SetValue(serviceObject, new Action<IQueryable>(OnQueryableConstructed));
            }
        }

        /// <summary>
        /// Hook called from the product whenever a query is constructed.
        /// </summary>
        /// <param name="result">The query about to execute.</param>
        private static void OnQueryableConstructed(IQueryable result)
        {
            ExpressionTreeTestUtils.lastQueryable = result;
        }

        #region Service
        public class ExpressionDataService<T> : OpenWebDataService<T>
        {
            public ExpressionDataService()
            {
                ExpressionTreeTestUtils.RegisterOnService(this);
            }
        }
        #endregion
    }    
}
