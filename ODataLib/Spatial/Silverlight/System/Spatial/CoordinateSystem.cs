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

namespace System.Spatial
{
    using Collections.Generic;
    using Diagnostics.CodeAnalysis;
    using Globalization;
    using Microsoft.Data.Spatial;

    /// <summary>
    ///   Coordinate System Reference
    /// </summary>
    public class CoordinateSystem
    {
        /// <summary>
        ///   Default Geometry Reference
        /// </summary>
        public static readonly CoordinateSystem DefaultGeometry = new CoordinateSystem(0, "Unitless Plane", Topology.Geometry);

        /// <summary>
        ///   Default Geography Reference (SRID 4326, WGS84)
        /// </summary>
        public static readonly CoordinateSystem DefaultGeography = new CoordinateSystem(4326, "WGS84", Topology.Geography);

        /// <summary>
        ///   List of registered references
        /// </summary>
        private static readonly Dictionary<CompositeKey<int, Topology>, CoordinateSystem> References;

        /// <summary>
        ///   A lock object for the References static dict
        /// </summary>
        private static readonly object referencesLock = new object();

        /// <summary>
        ///   The shape of the space that this coordinate system measures.
        /// </summary>
        private readonly Topology topology;

        /// <summary>
        ///   Static Constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810", Justification = "Static Constructor required")]
        [SuppressMessage("Microsoft.MSInternal", "CA908", Justification = "generic of int required")]
        static CoordinateSystem()
        {
            References = new Dictionary<CompositeKey<int, Topology>, CoordinateSystem>(EqualityComparer<CompositeKey<int, Topology>>.Default);
            AddRef(DefaultGeometry);
            AddRef(DefaultGeography);
        }

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name = "epsgId">The coordinate system ID, according to the EPSG</param>
        /// <param name = "name">The Name of the system</param>
        /// <param name = "topology">The topology of this coordinate system</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
        internal CoordinateSystem(int epsgId, string name, Topology topology)
        {
            this.topology = topology;
            this.EpsgId = epsgId;
            this.Name = name;
        }

        /// <summary>
        ///   The shapes of the spaces measured by coordinate systems.
        /// </summary>
        internal enum Topology
        {
            /// <summary>
            ///   Ellipsoidal coordinates
            /// </summary>
            Geography = 0,

            /// <summary>
            ///   Planar coordinates
            /// </summary>
            Geometry
        }

        /// <summary>
        ///   The coordinate system ID according to the EPSG, or NULL if this is not an EPSG coordinate system.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
        public int? EpsgId { get; private set; }

        /// <summary>
        ///   The coordinate system Id, no matter what scheme is used.
        /// </summary>
        public string Id
        {
            get { return EpsgId.Value.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        ///   The Name of the Reference
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///   Gets or creates a Geography coordinate system with the ID, or the default if null is given.
        /// </summary>
        /// <param name = "epsgId">The coordinate system id, according to the EPSG. Null indicates the default should be returned</param>
        /// <returns>The coordinate system</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
        public static CoordinateSystem Geography(int? epsgId)
        {
            return epsgId.HasValue ? GetOrCreate(epsgId.Value, Topology.Geography) : DefaultGeography;
        }

        /// <summary>
        ///   Gets or creates a Geometry coordinate system with the ID, or the default if null is given.
        /// </summary>
        /// <param name = "epsgId">The coordinate system id, according to the EPSG. Null indicates the default should be returned</param>
        /// <returns>The coordinate system</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
        public static CoordinateSystem Geometry(int? epsgId)
        {
            return epsgId.HasValue ? GetOrCreate(epsgId.Value, Topology.Geometry) : DefaultGeometry;
        }

        /// <summary>
        ///   Display the coordinate system for debugging
        /// </summary>
        /// <returns>String representation of the coordinate system, for debugging</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}CoordinateSystem(EpsgId={1})", this.topology, this.EpsgId);
        }

        /// <summary>
        ///   To a string that can be used with extended WKT.
        /// </summary>
        /// <returns>String representation in the form of SRID=#;</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wkt", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
        public string ToWktId()
        {
            return WellKnownTextConstants.WktSrid + WellKnownTextConstants.WktEquals + this.EpsgId + WellKnownTextConstants.WktSemiColon;
        }

        /// <summary>
        ///   Equals overload
        /// </summary>
        /// <param name = "obj">The other CoordinateSystem</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as CoordinateSystem);
        }

        /// <summary>
        ///   Equals overload
        /// </summary>
        /// <param name = "other">The other CoordinateSystem</param>
        /// <returns>True if equal</returns>
        public bool Equals(CoordinateSystem other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.topology, this.topology) && other.EpsgId.Equals(this.EpsgId);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.topology.GetHashCode() * 397) ^ (this.EpsgId.HasValue ? this.EpsgId.Value : 0);
            }
        }

        /// <summary>
        ///   For tests only. Identifies whether the coordinate system is of the designated topology.
        /// </summary>
        /// <param name = "expected">The expected topology.</param>
        /// <returns>True if this coordinate system is of the expected topology.</returns>
        internal bool TopologyIs(Topology expected)
        {
            return this.topology == expected;
        }

        /// <summary>
        ///   Get or create a CoordinateSystem with ID
        /// </summary>
        /// <param name = "epsgId">The SRID</param>
        /// <param name = "topology">The topology.</param>
        /// <returns>
        ///   A CoordinateSystem object
        /// </returns>
        private static CoordinateSystem GetOrCreate(int epsgId, Topology topology)
        {
            CoordinateSystem r;
            lock (referencesLock)
            {
                if (References.TryGetValue(KeyFor(epsgId, topology), out r))
                {
                    return r;
                }

                r = new CoordinateSystem(epsgId, "ID " + epsgId, topology);
                AddRef(r);
            }

            return r;
        }

        /// <summary>
        ///   Remember this coordinate system in the references dictionary.
        /// </summary>
        /// <param name = "coords">The coords.</param>
        private static void AddRef(CoordinateSystem coords)
        {
            References.Add(KeyFor(coords.EpsgId.Value, coords.topology), coords);
        }

        /// <summary>
        ///   Gets the key for a coordinate system
        /// </summary>
        /// <param name = "epsgId">ID</param>
        /// <param name = "topology">topology</param>
        /// <returns>The key to use with the references dict.</returns>
        private static CompositeKey<int, Topology> KeyFor(int epsgId, Topology topology)
        {
            return new CompositeKey<int, Topology>(epsgId, topology);
        }
    }
}
