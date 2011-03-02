//------------------------------------------------------------------------------
// <copyright file="Triangle.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents a triangle in a mesh.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class Triangle : Poly
    {
        /// <summary>
        /// Gets the number of vertices in the polygon.
        /// </summary>
        /// <value>An integer equal to <c>3</c>.</value>
        public override int NumberOfVertices
        {
            get { return 3; }
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
        /// Returns the polygon data as an array of <see cref="System.UInt16"/> values.
        /// </summary>
        /// <returns>
        /// An array of 3 <see cref="UInt16"/> values containing <see cref="Vertex1"/>, <see cref="Vertex2"/>
        /// and <see cref="Vertex3"/>.
        /// </returns>
        public override ushort[] ToArray()
        {
            return new ushort[] { this.Vertex1, this.Vertex2, this.Vertex3 };
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

            visitor.VisitTriangle(this);
        }
    }
}
