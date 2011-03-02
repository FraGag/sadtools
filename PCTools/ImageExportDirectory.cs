//------------------------------------------------------------------------------
// <copyright file="ImageExportDirectory.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the IMAGE_EXPORT_DIRECTORY structure.
    /// </summary>
    /// <remarks>
    /// <para>This structure lists the exported functions and variables of an executable (usually used for DLLs).
    /// <see cref="ImageOptionalHeader32.ExportDataDirectory"/> points to this structure.</para>
    /// <para>This structure has a size of 0x28 bytes.</para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageExportDirectory
    {
        /// <summary>
        /// Characteristics of the export directory.
        /// </summary>
        /// <remarks>This field seems to be largely unused and is often set to zero (0). This field is at offset 0x0.</remarks>
        public uint Characteristics;

        /// <summary>
        /// Date and time at which the executable was linked as a UNIX timestamp (time_t).
        /// </summary>
        /// <remarks>Some linkers set this field to zero (0). This field is at offset 0x4.</remarks>
        public uint TimeDateStamp;

        /// <summary>
        /// Major version of the linker used to generate this executable.
        /// </summary>
        /// <remarks>Some linkers set this field to zero (0). Note that the version is not very useful because there is no
        /// information about which linker was used. This field is at offset 0x8.</remarks>
        public ushort MajorVersion;

        /// <summary>
        /// Minor version of the linker used to generate this executable.
        /// </summary>
        /// <remarks>Some linkers set this field to zero (0). Note that the version is not very useful because there is no
        /// information about which linker was used. This field is at offset 0xA.</remarks>
        public ushort MinorVersion;

        /// <summary>
        /// Relative virtual address (RVA) to the name of the executable as a null-terminated single-byte per character string.
        /// </summary>
        /// <remarks>This field is at offset 0xC.</remarks>
        public uint Name;

        /// <summary>
        /// Value of the first ordinal.
        /// </summary>
        /// <remarks>
        /// The ordinal of a function is determined by its position in the list pointed to by <see cref="AddressOfFunctions"/>,
        /// to which the value of this field is added. This field is at offset 0x10.
        /// </remarks>
        public uint Base;

        /// <summary>
        /// The number of symbols (functions or variables) exported by the executable.
        /// </summary>
        /// <remarks>This field is at offset 0x14.</remarks>
        public uint NumberOfFunctions;

        /// <summary>
        /// The number of names for exported symbols and the number of name-to-ordinal mappings.
        /// </summary>
        /// <remarks>
        /// In general, each exported symbol will have exactly one name. However, it is possible to have unnamed exported
        /// symbols or exported symbols with more than one name. This field is at offset 0x18.
        /// </remarks>
        public uint NumberOfNames;

        /// <summary>
        /// Relative virtual address (RVA) of the list of pointers to the exported symbols.
        /// </summary>
        /// <remarks>
        /// The size of the list is specified by <see cref="NumberOfFunctions"/>. This field is at offset 0x1C.
        /// </remarks>
        public uint AddressOfFunctions;

        /// <summary>
        /// Relative virtual address (RVA) of the list of pointers to the names.
        /// </summary>
        /// <remarks>
        /// The size of the list is specified by <see cref="NumberOfNames"/>. This field is at offset 0x20.
        /// </remarks>
        public uint AddressOfNames;

        /// <summary>
        /// Relative virtual address (RVA) of the list of ordinals corresponding to each name.
        /// </summary>
        /// <remarks>
        /// The size of the list is specified by <see cref="NumberOfNames"/>. This field is at offset 0x24.
        /// </remarks>
        public uint AddressOfNameOrdinals;
    }
}
