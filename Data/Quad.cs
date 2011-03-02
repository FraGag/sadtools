//------------------------------------------------------------------------------
// <copyright file="Quad.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents a quadrilateral in a mesh.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class Quad : Poly
    {
        /// <summary>
        /// Gets the number of vertices in the polygon.
        /// </summary>
        /// <value>An integer equal to <c>4</c>.</value>
        public override int NumberOfVertices
        {
            get { return 4; }
        }

        /// <summary>
        /// Gets or sets the index of the first vertex.
        /// </summary>
        /// <value>The index of the first vertex.</value>
        public ushort Vertex1 { get; set; }

        /// <summary>
        /// Gets or sets the index of the second vertex.
        /// </summary>
        /// <value>The index of the second vertex.</value>
        public ushort Vertex2 { get; set; }

        /// <summary>
        /// Gets or sets the index of the third vertex.
        /// </summary>
        /// <value>The index of the third vertex.</value>
        public ushort Vertex3 { get; set; }

        /// <summary>
        /// Gets or sets the index of the fourth vertex.
        /// </summary>
        /// <value>The index of the fourth vertex.</value>
        public ushort Vertex4 { get; set; }

        /// <summary>
        /// Returns the polygon data as an array of <see cref="System.UInt16"/> values.
        /// </summary>
        /// <returns>
        /// An array of 4 <see cref="UInt16"/> values containing <see cref="Vertex1"/>, <see cref="Vertex2"/>,
        /// <see cref="Vertex3"/> and <see cref="Vertex4"/>.
        /// </returns>
        public override ushort[] ToArray()
        {
            return new[] { this.Vertex1, this.Vertex2, this.Vertex3, this.Vertex4 };
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

            visitor.VisitQuad(this);
        }
    }
}
