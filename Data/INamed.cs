//------------------------------------------------------------------------------
// <copyright file="INamed.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    /// <summary>
    /// Defines an interface for named objects.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        string Name { get; set; }
    }
}
