//------------------------------------------------------------------------------
// <copyright file="MESH.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    // TODO: document field MESH.PolyAttributes
    // TODO: document field MESH.PolyNormals

    /// <summary>
    /// Represents a 3D mesh.
    /// </summary>
    public struct MESH : IEquatable<MESH>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MESH" /> structure using the supplied values.
        /// </summary>
        /// <param name="materialIdAndPolyType">Value containing the material ID and the polygon type.</param>
        /// <param name="polys">Polygons in the mesh.</param>
        /// <param name="polyAttributes">Unknown value at offset 0x08.</param>
        /// <param name="polyNormals">Unknown value at offset 0x0C.</param>
        /// <param name="vertexColors">Vertex colors for each vertex in the mesh.</param>
        /// <param name="uv">UV coordinates for each vertex in the mesh.</param>
        /// <param name="nullMember">Unused value. Set this value to 0.</param>
        [CLSCompliant(false)]
        public MESH(ushort materialIdAndPolyType, NamedCollection<Poly> polys, int polyAttributes, NamedCollection<PolyNormal> polyNormals, NamedCollection<uint> vertexColors, NamedCollection<UV> uv, int nullMember)
            : this()
        {
            this.MaterialIdAndPolyType = materialIdAndPolyType;
            this.Polys = polys;
            this.PolyAttributes = polyAttributes;
            this.PolyNormals = polyNormals;
            this.VertexColors = vertexColors;
            this.UV = uv;
            this.Null = nullMember;
        }

        /// <summary>
        /// Gets or sets a value containing the material ID and the polygon type.
        /// </summary>
        /// <value>
        /// An <see cref="System.UInt16"/> value whose 14 low bits represent the material ID and whose 2 high bits represent the
        /// polygon type (0 = triangles, 1 = quads, 2 or 3 = strips).
        /// </value>
        [CLSCompliant(false)]
        public ushort MaterialIdAndPolyType { get; set; }

        /// <summary>
        /// Gets a collection of the polygons in the mesh.
        /// </summary>
        /// <value>An instance of <see cref="NamedCollection{T}"/> containing <see cref="Poly"/> objects.</value>
        [CLSCompliant(false)]
        public NamedCollection<Poly> Polys { get; private set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        public int PolyAttributes { get; set; }

        /// <summary>
        /// Gets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        public NamedCollection<PolyNormal> PolyNormals { get; private set; }

        /// <summary>
        /// Gets a collection of the vertex colors for each vertex in the mesh.
        /// </summary>
        /// <value>
        /// An instance of <see cref="NamedCollection{T}"/> containing the color of each vertex in the mesh, in the format
        /// 0xAARRGGBB.
        /// </value>
        [CLSCompliant(false)]
        public NamedCollection<uint> VertexColors { get; private set; }

        /// <summary>
        /// Gets a collection of the UV coordinates for each vertex in the mesh.
        /// </summary>
        /// <value>
        /// An instance of <see cref="NamedCollection{T}"/> containing the UV coordinates of each vertex in the mesh.
        /// </value>
        public NamedCollection<UV> UV { get; private set; }

        /// <summary>
        /// Gets or sets an unused value.
        /// </summary>
        /// <value>An unused value. Set this value to 0.</value>
        /// <remarks>This field is unused.</remarks>
        public int Null { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="MESH"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="MESH"/> object.</param>
        /// <param name="right">Second <see cref="MESH"/> object.</param>
        /// <returns><c>true</c> if both <see cref="MESH"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MESH left, MESH right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="MESH"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="MESH"/> object.</param>
        /// <param name="right">Second <see cref="MESH"/> object.</param>
        /// <returns><c>true</c> if both <see cref="MESH"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MESH left, MESH right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise,
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            MESH? mesh = obj as MESH?;
            if (mesh.HasValue)
            {
                return this.Equals(mesh.GetValueOrDefault());
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(MESH other)
        {
            return this.MaterialIdAndPolyType == other.MaterialIdAndPolyType &&
                this.Polys == other.Polys &&
                this.PolyAttributes == other.PolyAttributes &&
                this.PolyNormals == other.PolyNormals &&
                this.VertexColors == other.VertexColors &&
                this.UV == other.UV &&
                this.Null == other.Null;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
