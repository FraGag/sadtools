//------------------------------------------------------------------------------
// <copyright file="IAnimFrameVisitor.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    /// <summary>
    /// Defines the interface for an <see cref="AnimFrame"/> visitor.
    /// </summary>
    /// <remarks>
    /// Implement this interface in a class, then instantiate this class and pass the instance as an argument to
    /// <see cref="AnimFrame.Accept"/>. Exactly one of the methods in this interface will be called with the
    /// <see cref="AnimFrame"/> object cast to its concrete type.
    /// </remarks>
    public interface IAnimFrameVisitor
    {
        /// <summary>
        /// Visits an instance of the <see cref="AnimFrame_PosRot"/> class.
        /// </summary>
        /// <param name="afpr">An instance of the <see cref="AnimFrame_PosRot"/> class.</param>
        void VisitAnimFrame_PosRot(AnimFrame_PosRot afpr);

        /// <summary>
        /// Visits an instance of the <see cref="AnimFrame_PosRotScale"/> class.
        /// </summary>
        /// <param name="afprs">An instance of the <see cref="AnimFrame_PosRotScale"/> class.</param>
        void VisitAnimFrame_PosRotScale(AnimFrame_PosRotScale afprs);

        /// <summary>
        /// Visits an instance of the <see cref="AnimFrame_VertNrm"/> class.
        /// </summary>
        /// <param name="afvn">An instance of the <see cref="AnimFrame_VertNrm"/> class.</param>
        void VisitAnimFrame_VertNrm(AnimFrame_VertNrm afvn);
    }
}
