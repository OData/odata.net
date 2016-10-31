//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

/*  DESIGN NOTES (pqian):
 *  a note on the primitive type parser/materializer on the client side
 *  Since the client type system allows multiple CLR types mapping to 
 *  a single EDM type (i.e., String, Char, Char[], XDocument and XElement 
 *  all maps to Edm.String). We cannot handle materialization based on the
 *  wire Edm type. The correct behavior would be to "tokenize" the wire data
 *  using the wire type, and them "materialize" the token using the CLR type
 *  declared on the entity type. However, there is a V1/V2 behavior where
 *  we DO NOT fail if the wire data type doesn't match the wire data (for example,
 *  you can have <d:Property type="Edm.Int32">3.2</Property>, as long as the CLR type
 *  can handle decimal numbers). Therefore, for all existing V1/V2 primitive types,
 *  the "tokenize" step should simply be storing the textual representation
 *  in an instance of TextPrimitiveParserToken, and use Parse() to materialize it.
 *  For new types, the TokenizeFromXml/TokenizeFromText on the PrimitiveTypeConverter
 *  should be overriden to give the correct behavior.
 */
namespace System.Data.Services.Client
{
    /// <summary>
    /// A parser token
    /// </summary>
    internal abstract class PrimitiveParserToken
    {
        /// <summary>
        /// Materialize this token using a PrimitiveTypeConverter
        /// </summary>
        /// <param name="clrType">The primitive type</param>
        /// <returns>A materialized instance</returns>
        internal abstract object Materialize(Type clrType);
    }

    /// <summary>
    /// Textual based parser token
    /// </summary>
    internal class TextPrimitiveParserToken : PrimitiveParserToken
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Textual value</param>
        internal TextPrimitiveParserToken(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// The text property
        /// </summary>
        internal string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Materialize by calling the Parse method on the converter
        /// </summary>
        /// <param name="clrType">clrType</param>
        /// <returns>A materialized instance</returns>
        internal override object Materialize(Type clrType)
        {
            return ClientConvert.ChangeType(this.Text, clrType);
        }
    }

    /// <summary>
    /// Instance based parser token, where the token is the materialized instance
    /// </summary>
    /// <typeparam name="T">The instance type</typeparam>
    internal class InstancePrimitiveParserToken<T> : PrimitiveParserToken
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance">The instance</param>
        internal InstancePrimitiveParserToken(T instance)
        {
            this.Instance = instance;
        }

        /// <summary>
        /// The instance
        /// </summary>
        internal T Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Materialize by returning the instance
        /// </summary>
        /// <param name="clrType">A primitive type converter</param>
        /// <returns>The instance</returns>
        internal override object Materialize(Type clrType)
        {
            return this.Instance;
        }
    }
}
