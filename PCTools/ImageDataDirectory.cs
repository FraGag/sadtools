//------------------------------------------------------------------------------
// <copyright file="ImageDataDirectory.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the IMAGE_DATA_DIRECTORY structure from the Windows SDK.
    /// </summary>
    /// <remarks>This structure has a size of 0x8 bytes.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageDataDirectory
    {
        /// <summary>
        /// The relative virtual address (RVA) of the data.
        /// </summary>
        /// <remarks>This field is at offset 0x0.</remarks>
        public uint VirtualAddress;

        /// <summary>
        /// The size of the data.
        /// </summary>
        /// <remarks>This field is at offset 0x4.</remarks>
        public uint Size;
    }
}
