//------------------------------------------------------------------------------
// <copyright file="IPolyVisitor.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Defines the interface for a <see cref="Poly"/> visitor.
    /// </summary>
    /// <remarks>
    /// Implement this interface in a class, then instantiate this class and pass the instance as an argument to
    /// <see cref="Poly.Accept"/>. Exactly one of the methods in this interface will be called with the <see cref="Poly"/> object
    /// cast to its concrete type.
    /// </remarks>
    [CLSCompliant(false)]
    public interface IPolyVisitor
    {
        /// <summary>
        /// Visits an instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="triangle">An instance of the <see cref="Triangle"/> class.</param>
        void VisitTriangle(Triangle triangle);

        /// <summary>
        /// Visits an instance of the <see cref="Quad"/> class.
        /// </summary>
        /// <param name="quad">An instance of the <see cref="Quad"/> class.</param>
        void VisitQuad(Quad quad);

        /// <summary>
        /// Visits an instance of the <see cref="Strip"/> class.
        /// </summary>
        /// <param name="strip">An instance of the <see cref="Strip"/> class.</param>
        void VisitStrip(Strip strip);
    }
}
