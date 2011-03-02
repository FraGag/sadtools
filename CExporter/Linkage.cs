//------------------------------------------------------------------------------
// <copyright file="Linkage.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.CExporter
{
    /// <summary>
    /// Represents the linkage of a C global variable or function.
    /// </summary>
    public enum Linkage
    {
        /// <summary>
        /// Represents extern linkage (the default, visible across translation units).
        /// </summary>
        ExternLinkage,

        /// <summary>
        /// Represents static linkage (only visible to the translation unit).
        /// </summary>
        StaticLinkage
    }
}
