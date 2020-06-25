//---------------------------------------------------------------------
// <copyright file="JsonParserContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides for the loading and conversion of one or more CSDL XML readers into Entity Data Model.
    /// </summary>
    internal class JsonParserContext
    {
        /// <summary>
        /// a value indicating whether this JSON path is changed outside.
        /// </summary>
        private bool _dirty;

        /// <summary>
        /// a value caching the calcuated path.
        /// </summary>
        private string _jsonPath;

        /// <summary>
        /// It's only include "string" for property, and Int32 for the index of array
        /// </summary>
        private Stack<object> _nodes;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonParserContext"/> class.
        /// </summary>
        /// <param name="settings">The CSDL-JSON reader settings.</param>
        /// <param name="source">The parsing source.</param>
        public JsonParserContext(CsdlJsonReaderSettings settings = null, string source = null)
        {
            Source = source;
            Settings = settings ?? CsdlJsonReaderSettings.Default;
            _nodes = new Stack<object>();
            Errors = new List<EdmError>();
            _dirty = true;
        }

        /// <summary>
        /// Gets the parsing source.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the CSDL-JSON reader settings.
        /// </summary>
        public CsdlJsonReaderSettings Settings { get; }

        #region JsonPath
        /// <summary>
        /// Enter a JSON property scope.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        public void EnterScope(string propertyName)
        {
            _nodes.Push(propertyName);
            _dirty = true;
        }

        /// <summary>
        /// Leave a JSON property/index scope.
        /// </summary>
        public void LeaveScope()
        {
            _nodes.Pop();
            _dirty = true;
        }

        /// <summary>
        /// Leave a JSON property scope.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        public void LeaveScope(string propertyName)
        {
            Debug.Assert(_nodes.Count > 0);
            Debug.Assert(_nodes.Peek().ToString() == propertyName);
            _nodes.Pop();
            _dirty = true;
        }

        /// <summary>
        /// Entry an array index scope
        /// </summary>
        /// <param name="index">The array index.</param>
        public void EnterScope(int index)
        {
            _nodes.Push(index);
            _dirty = true;
        }

        /// <summary>
        /// Leave a JSON array index scope.
        /// </summary>
        /// <param name="index">The array index.</param>
        public void LeaveScope(int index)
        {
            Debug.Assert(_nodes.Count > 0);
            Debug.Assert((int)_nodes.Peek() == index);
            _nodes.Pop();
            _dirty = true;
        }

        /// <summary>
        /// Gets current JSON path string
        /// </summary>
        public string Path => BuildJsonPath();

        /// <summary>
        /// Gets current CsdlLocation.
        /// </summary>
        /// <returns>The built CsdlLocation.</returns>
        public CsdlLocation Location()
        {
            return new CsdlLocation(BuildJsonPath());
        }
        #endregion

        #region EdmError

        /// <summary>
        /// Gets all parsing errors
        /// </summary>
        public List<EdmError> Errors { get; }

        /// <summary>
        /// Reports a certain parsing error.
        /// </summary>
        /// <param name="errorCode">The Edm Error Code.</param>
        /// <param name="errorMessage">The Error message.</param>
        public void ReportError(EdmErrorCode errorCode, string errorMessage)
        {
            Errors.Add(new EdmError(Location(), errorCode, errorMessage));
        }

        /// <summary>
        /// Check parsing status.
        /// </summary>
        /// <returns>True/false</returns>
        public bool IsSucceeded()
        {
            return Errors.Count == 0;
        }

        /// <summary>
        /// Determine if there is any error that could not be ignored.
        /// </summary>
        /// <returns>True if there is any error that could not be ignored.</returns>
        public bool HasIntolerableError()
        {
            if (Settings.IgnoreUnexpectedJsonElements)
            {
                return Errors.Any(error => error.ErrorCode != EdmErrorCode.UnexpectedElement && error.ErrorCode != EdmErrorCode.UnsupportedElement);
            }

            return Errors.Count != 0;
        }

        /// <summary>
        /// Add a range of errors.
        /// </summary>
        /// <param name="errors">The input errors.</param>
        public void AddRange(IList<EdmError> errors)
        {
            Errors.AddRange(errors);
        }
        #endregion

        /// <summary>
        /// Construct the json path string
        /// </summary>
        private string BuildJsonPath()
        {
            if (_dirty)
            {
                // $ for The root object or array.
                string root = "$";
                if (Source != null)
                {
                    root = "(" + Source + ")$";
                }

                string[] segments = new string[_nodes.Count + 1];
                segments[0] = root;
                int index = _nodes.Count;
                foreach (var segment in _nodes)
                {
                    segments[index--] = GetName(segment);
                }

                _jsonPath = string.Join("", segments);
                _dirty = false;
            }

            return _jsonPath;
        }

        private string GetName(object node)
        {
            string strNode = node as string;
            if (strNode != null)
            {
                if (Settings.IsBracketNotation)
                {
                    return "['" + strNode + "']";
                }

                return "." + strNode;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, "[{0}]", (int)node);
            }
        }
    }
}
#endif
