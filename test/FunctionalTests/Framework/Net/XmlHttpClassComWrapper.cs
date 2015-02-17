//---------------------------------------------------------------------
// <copyright file="XmlHttpClassComWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Data.Test.Astoria
{
    /// <summary>
    /// Wrapper used to create an instance of an XmlHttpClass using msxml6
    /// </summary>
    internal class XmlHttpClassComWrapper : IDisposable
    {
        private bool disposed = false;
        private object _object;

        private XmlHttpClassComWrapper(object o)
        {
            _object = o;
        }

        public static XmlHttpClassComWrapper CreateXmlHttpClassWrapper()
        {
            Type t = Type.GetTypeFromProgID("Msxml2.XMLHTTP.6.0");
            object o = Activator.CreateInstance(t);
            XmlHttpClassComWrapper wrapper = new XmlHttpClassComWrapper(o);
            return wrapper;
        }

        public void open(string bstrMethod, string bstrUrl, object varAsync, object bstrUser, object bstrPassword)
        {
            _object.GetType().InvokeMember("open", BindingFlags.InvokeMethod, null, _object, new object[] { bstrMethod, bstrUrl, varAsync, bstrUser, bstrPassword });
        }

        public void setRequestHeader(string bstrHeader, string bstrValue)
        {
            _object.GetType().InvokeMember("setRequestHeader", BindingFlags.InvokeMethod, null, _object, new object[] { bstrHeader, bstrValue });
        }

        public void send(object varBody)
        {
            _object.GetType().InvokeMember("send", BindingFlags.InvokeMethod, null, _object, new object[] { varBody });
           
        }

        public string responseText
        {
            get
            {

                return (string)_object.GetType().InvokeMember("responseText", BindingFlags.GetProperty, null, _object, new object[] { });
            }
        }

        public byte[] responseBody
        {
            get
            {
                return (byte[])_object.GetType().InvokeMember("responseBody", BindingFlags.GetProperty, null, _object, new object[] { });
            }
        }

        //public string responseHeader
        //{
        //    get 
        //    {
        //        // TODO: does this member even exist?
        //        return (string)_object.GetType().InvokeMember("responseHeader", BindingFlags.GetProperty, null, _object, new object[] { });
        //    } 
        //}

        public string AllResponseHeaders
        {
            get
            {
                return (string)_object.GetType().InvokeMember("getAllResponseHeaders", BindingFlags.InvokeMethod, null, _object, new object[] { });
            }
        }

        public int status
        {
            get
            {
                return (int)_object.GetType().InvokeMember("status", BindingFlags.GetProperty, null, _object, new object[] { });
            }
        }

        public string statusText
        {
            get
            {
                return (string)_object.GetType().InvokeMember("statusText", BindingFlags.GetProperty, null, _object, new object[] { });
            }
        }

        internal string getResponseHeader(string p)
        {
            return (string)_object.GetType().InvokeMember("getResponseHeader", BindingFlags.InvokeMethod, null, _object, new object[] { p });
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!this.disposed)
            {
                if (disposeManagedResources)
                {}

                System.Runtime.InteropServices.Marshal.ReleaseComObject(_object);
                _object = null;

                disposed = true;
            }
            else
            {
                // Do nothin
            }
        }

        #endregion
    }
}
