//------------------------------------------------------------------------------
// <copyright file="Strip.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a strip in a mesh.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class Strip : Poly
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Strip" /> class.
        /// </summary>
        public Strip()
        {
            this.Vertices = new Collection<ushort>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Strip" /> class using the supplied values.
        /// </summary>
        /// <param name="isReversed">Value indicating whether the vertices should be read backwards.</param>
        /// <param name="vertices">Vertices in the strip.</param>
        public Strip(bool isReversed, Collection<ushort> vertices)
        {
            this.IsReversed = isReversed;
            this.Vertices = vertices;
        }

        /// <summary>
        /// Gets the number of vertices in the polygon.
        /// </summary>
        /// <value>The number of vertices in the strip.</value>
        public override int NumberOfVertices
        {
            get { return this.Vertices.Count; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the vertices should be read backwards.
        /// </summary>
        /// <value><c>true</c> if the vertices should be read backwards; otherwise, <c>false</c>.</value>
        public bool IsReversed { get; set; }

        /// <summary>
        /// Gets a collection of the vertices in the strip.
        /// </summary>
        /// <value>An instance of <see cref="Collection{T}"/> containing the vertex indices in the strip.</value>
        public Collection<ushort> Vertices { get; private set; }

        /// <summary>
        /// Returns the polygon data as an array of <see cref="System.UInt16"/> values.
        /// </summary>
        /// <returns>
        /// An array of <see cref="UInt16"/> values containing the number of vertices and the vertex order followed by the
        /// vertex indices.
        /// </returns>
        public override ushort[] ToArray()
        {
            ushort[] array = new ushort[this.Vertices.Count + 1];
            array[0] = (ushort)checked((short)this.Vertices.Count);
            if (this.IsReversed)
            {
                array[0] |= 0x8000;
            }

            for (int i = 0; i < this.Vertices.Count; i++)
            {
                array[i + 1] = this.Vertices[i];
            }

            return array;
        }

        /// <summary>
        /// Visits the specified visitor.
        /// </summary>
        /// <param name="visitor">A poly visitor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visitor"/> is <c>null</c> (<c>Nothing</c> in Visual Basic).
        /// </exception>
        public override void Accept(IPolyVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            visitor.VisitStrip(this);
        }
    }
}
