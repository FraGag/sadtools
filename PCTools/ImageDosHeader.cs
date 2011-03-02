//------------------------------------------------------------------------------
// <copyright file="ImageDosHeader.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the IMAGE_DOS_HEADER structure from the Windows SDK.
    /// </summary>
    /// <remarks>
    /// <para>This structure appears at the very beginning of every MS-DOS-compatible executable. This includes actual MS-DOS
    /// executables, New Executables (16-bit Windows and DOS 4.0), IBM OS/2 Executables and Portable Executable (32-bit and
    /// later Windows). For the latter executable formats, this header has remained mainly for compatibility, as most fields are
    /// unused.</para>
    /// <para>This structure has a size of 0x40 bytes.</para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageDosHeader
    {
        /// <summary>
        /// Magic number identifying the image as a DOS executable. Its value must be 'MZ' (big endian: 0x4D5A, little endian:
        /// 0x5A4D).
        /// </summary>
        /// <remarks>This field is at offset 0x0.</remarks>
        public ushort Magic;

        /// <summary>
        /// The number of bytes in the last page of the program that are actually used.
        /// </summary>
        /// <remarks>
        /// The size of a page is 512 bytes. If this value is zero, that means the entire last page is used (i.e. the effective
        /// value is 512). This field is at offset 0x2.
        /// </remarks>
        public ushort BytesOnLastPageOfFile;

        /// <summary>
        /// The number of pages in the image that are part of the EXE file.
        /// </summary>
        /// <remarks>
        /// The size of a page is 512 bytes. If <see cref="BytesOnLastPageOfFile"/> is not zero, then the last page is only
        /// partially used. This field is at offset 0x4.
        /// </remarks>
        public ushort PageCount;

        /// <summary>
        /// The number of relocation entries stored after the header.
        /// </summary>
        /// <remarks>
        /// In Portable Executables, this is often zero (0). This field is at offset 0x6.
        /// </remarks>
        public ushort RelocationCount;

        /// <summary>
        /// The size of the header, in paragraphs.
        /// </summary>
        /// <remarks>
        /// The size of a paragraph is 16 bytes. The program's data begins just after the header, and this field can be used to
        /// calculate the appropriate file offset. The header includes the relocation entries. Note that some OSs and/or
        /// programs may fail if the header is not a multiple of 512 bytes. This field is at offset 0x8.
        /// </remarks>
        public ushort HeaderSizeInParagraphs;

        /// <summary>
        /// Number of paragraphs of additional memory that the program will need.
        /// </summary>
        /// <remarks>
        /// This is the equivalent of the BSS size in a Unix program. The program can't be loaded if there isn't at least this
        /// much memory available to it. This field is at offset 0xA.
        /// </remarks>
        public ushort MinimumExtraParagraphsNeeded;

        /// <summary>
        /// Maximum number of paragraphs of additional memory.
        /// </summary>
        /// <remarks>
        /// In DOS, the OS reserves all the remaining conventional memory for the program, but you can limit it with this field.
        /// This field is at offset 0xC.
        /// </remarks>
        public ushort MaximumExtraParagraphsNeeded;

        /// <summary>
        /// Relative value of the stack segment. This value is added to the segment the program was loaded at, and the result is
        /// used to initialize the SS register. This field is at offset 0xE.
        /// </summary>
        public ushort InitialSS;

        /// <summary>
        /// Initial value of the SP (stack pointer) register. This field is at offset 0x10.
        /// </summary>
        public ushort InitialSP;

        /// <summary>
        /// Value used to make the checksum of the words in the image equal to zero (0).
        /// </summary>
        /// <remarks>
        /// In practice, this value is often not computed and set to zero, as the operating system's loader doesn't actually
        /// check the checksum. This field is at offset 0x12.
        /// </remarks>
        public ushort Checksum;

        /// <summary>
        /// Initial value of the IP (instruction pointer) register.
        /// </summary>
        /// <remarks>
        /// In most Portable Executables, this points to a "DOS stub", which usually only shows a message similar to "This
        /// program cannot be run in DOS mode." This field is at offset 0x14.
        /// </remarks>
        public ushort InitialIP;

        /// <summary>
        /// Initial value of the CS register, relative to the segment the program was loaded at. This field is at offset 0x16.
        /// </summary>
        public ushort InitialCS;

        /// <summary>
        /// Address of the relocation table.
        /// </summary>
        /// <remarks>
        /// In Portable Executables, this value is often meaningless, because the <see cref="RelocationCount"/> is zero (0).
        /// This field is at offset 0x18.
        /// </remarks>
        public ushort AddressOfRelocationTable;

        /// <summary>
        /// Overlay number. Normally zero, meaning that it's the main program.
        /// </summary>
        /// <remarks>This field is at offset 0x1A.</remarks>
        public ushort OverlayNumber;

        /// <summary>
        /// Reserved space. Do not use.
        /// </summary>
        /// <remarks>This reserved space starts at offset 0x1C and extends up to offset 0x23 (inclusively).</remarks>
        public unsafe fixed ushort Reserved1[4];

        /// <summary>
        /// OEM identifier.
        /// </summary>
        /// <remarks>This field is at offset 0x24.</remarks>
        public ushort OemIdentifier;

        /// <summary>
        /// Information specific to the OEM.
        /// </summary>
        /// <remarks>This field is at offset 0x26.</remarks>
        public ushort OemInformation;

        /// <summary>
        /// Reserved space. Do not use.
        /// </summary>
        /// <remarks>This reserved space starts at offset 0x28 and extends up to offset 0x3B (inclusively).</remarks>
        public unsafe fixed ushort Reserved2[10];

        /// <summary>
        /// Address of the new executable header.
        /// </summary>
        /// <remarks>
        /// In Portable Executables, this points to the PE signature ('PE', 0, 0), which is followed by an IMAGE_FILE_HEADER
        /// (<see cref="ImageFileHeader"/>). This field is at offset 0x3C.
        /// </remarks>
        public uint AddressOfNewExeHeader;
    }
}
