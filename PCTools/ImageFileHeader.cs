//------------------------------------------------------------------------------
// <copyright file="ImageFileHeader.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the IMAGE_FILE_HEADER structure.
    /// </summary>
    /// <remarks>
    /// <para>The IMAGE_FILE_HEADER corresponds to the Common Object File Format (COFF) header structure. This structure appears
    /// at the beginning of object files (.obj, .o, .res) and after the PE signature in Portable Executables.</para>
    /// <para>This structure has a size of 0x14 bytes.</para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageFileHeader
    {
        /// <summary>
        /// Identifies the target machine of the object or executable.
        /// </summary>
        /// <remarks>This field is at offset 0x0.</remarks>
        public ushort Machine;

        /// <summary>
        /// Number of sections in the object or executable.
        /// </summary>
        /// <remarks>This field is at offset 0x2.</remarks>
        public ushort NumberOfSections;

        /// <summary>
        /// Date and time at which the object or executable was compiled or linked as a UNIX timestamp (time_t).
        /// </summary>
        /// <remarks>This field is at offset 0x4.</remarks>
        public uint TimeDateStamp;

        /// <summary>
        /// Address of the symbol table.
        /// </summary>
        /// <remarks>This field is at offset 0x8.</remarks>
        public uint PointerToSymbolTable;

        /// <summary>
        /// Number of symbols in the symbol table.
        /// </summary>
        /// <remarks>This field is at offset 0xC.</remarks>
        public uint NumberOfSymbols;

        /// <summary>
        /// Size of the optional header, or 0 is the optional header is not present.
        /// </summary>
        /// <remarks>
        /// In objects, the optional header might not be present, but in executables, this header is always present. The
        /// optional header immediately follows this structure. In 32-bit executables, the size of the optional header is 0xE0.
        /// This field is at offset 0x10.
        /// </remarks>
        public ushort SizeOfOptionalHeader;

        /// <summary>
        /// Characteristics of the object or executable.
        /// </summary>
        /// <remarks>This field is at offset 0x12.</remarks>
        public ushort Characteristics;
    }
}
