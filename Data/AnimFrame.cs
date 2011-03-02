//------------------------------------------------------------------------------
// <copyright file="AnimFrame.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    /// <summary>
    /// Defines an abstract base class for animation frame data.
    /// </summary>
    public abstract class AnimFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is <c>internal</c> (<c>Friend</c> in Visual Basic) to prevent external projects from inheriting
        /// this class.
        /// </remarks>
        internal AnimFrame()
        {
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">An animation frame data visitor.</param>
        public abstract void Accept(IAnimFrameVisitor visitor);
    }
}
