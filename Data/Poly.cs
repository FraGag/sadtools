//------------------------------------------------------------------------------
// <copyright file="Poly.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents a polygon in a mesh.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Poly
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Poly"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is <c>internal</c> (<c>Friend</c> in Visual Basic) to prevent external projects from inheriting
        /// this class.
        /// </remarks>
        internal Poly()
        {
        }

        /// <summary>
        /// Gets the number of vertices in the polygon.
        /// </summary>
        /// <value>The number of vertices in the polygon.</value>
        public abstract int NumberOfVertices { get; }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">A poly visitor.</param>
        public abstract void Accept(IPolyVisitor visitor);

        /// <summary>
        /// Returns the polygon data as an array of <see cref="System.UInt16"/> values.
        /// </summary>
        /// <returns>An array of <see cref="System.UInt16"/> values making up the original data.</returns>
        public abstract ushort[] ToArray();
    }
}
