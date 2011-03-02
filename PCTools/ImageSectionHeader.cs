//------------------------------------------------------------------------------
// <copyright file="ImageSectionHeader.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the IMAGE_SECTION_HEADER structure.
    /// </summary>
    /// <remarks>This structure has a size of 0x28 bytes.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ImageSectionHeader
    {
        /// <summary>
        /// Name of the section.
        /// </summary>
        /// <remarks>
        /// The name is a fixed-length string of 8 single-byte characters. Unused characters are set to null (\0). This field is
        /// at offset 0x0.
        /// </remarks>
        public fixed byte Name[8];

        /// <summary>
        /// In object files, physical address where the section is relocated to. In executable images, size of the section.
        /// </summary>
        /// <remarks>This field is at offset 0x8.</remarks>
        public uint PhysicalAddressOrVirtualSize;

        /// <summary>
        /// Relative virtual address (RVA) where the section is to be loaded to in main memory.
        /// </summary>
        /// <remarks>This field is at offset 0xC.</remarks>
        public uint VirtualAddress;

        /// <summary>
        /// Size of the section's data, aligned to <see cref="ImageOptionalHeader32.FileAlignment"/>.
        /// </summary>
        /// <remarks>This field is at offset 0x10.</remarks>
        public uint SizeOfRawData;

        /// <summary>
        /// Offset from the beginning of the file to the raw data.
        /// </summary>
        /// <remarks>This field is at offset 0x14.</remarks>
        public uint PointerToRawData;

        /// <summary>
        /// Offset from the beginning of the file to the relocation table.
        /// </summary>
        /// <remarks>The value of this field is only relevant for object files. This field is at offset 0x18.</remarks>
        public uint PointerToRelocations;

        /// <summary>
        /// Offset from the beginning of the file to the line numbers table.
        /// </summary>
        /// <remarks>The value of this field is only relevant for object files. This field is at offset 0x1C.</remarks>
        public uint PointerToLinenumbers;

        /// <summary>
        /// Number of items in the relocation table.
        /// </summary>
        /// <remarks>The value of this field is only relevant for object files. This field is at offset 0x20.</remarks>
        public ushort NumberOfRelocations;

        /// <summary>
        /// Number of items in the line numbers table.
        /// </summary>
        /// <remarks>The value of this field is only relevant for object files. This field is at offset 0x22.</remarks>
        public ushort NumberOfLinenumbers;

        /// <summary>
        /// Bitfield describing the characteristics of the memory for the section.
        /// </summary>
        /// <remarks>The value of this field is only relevant for object files. This field is at offset 0x24.</remarks>
        public uint Characteristics;
    }
}
