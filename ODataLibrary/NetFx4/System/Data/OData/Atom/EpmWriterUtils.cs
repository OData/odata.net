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

namespace System.Data.OData.Atom
{
    /// <summary>
    /// Helper methods for EPM writers.
    /// </summary>
    internal static class EpmWriterUtils
    {
        /// <summary>
        /// Given a property value returns the text value to be used in EPM.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The text representation of the value, or the method throws if the text representation was not possible to obtain.</returns>
        internal static string GetPropertyValueAsText(object propertyValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (propertyValue == null)
            {
                return null;
            }

            string textPropertyValue;
            bool preserveWhitespace;
            if (!AtomValueUtils.TryConvertPrimitiveToString(propertyValue, out textPropertyValue, out preserveWhitespace))
            {
                throw new ODataException(Strings.AtomValueUtils_CannotConvertValueToAtomPrimitive(propertyValue.GetType().FullName));
            }

            return textPropertyValue;
        }
    }
}
