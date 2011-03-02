//------------------------------------------------------------------------------
// <copyright file="ImageOptionalHeader32.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the IMAGE_OPTIONAL_HEADER32 structure.
    /// </summary>
    /// <remarks>
    /// In executables, the optional header is always present and immediately follows the IMAGE_FILE_HEADER
    /// (<see cref="ImageFileHeader"/>) structure. This structure has a size of 0xE0 bytes.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageOptionalHeader32
    {
        /// <summary>
        /// Identifies the structure of the optional header.
        /// </summary>
        /// <remarks>
        /// For 32-bit executables, the value of this field is 0x10B. This field is at offset 0x0.
        /// </remarks>
        public ushort Magic;

        /// <summary>
        /// Major version of the linker used to generate this executable.
        /// </summary>
        /// <remarks>Some linkers set this field to zero (0). Note that the version is not very useful because there is no
        /// information about which linker was used. This field is at offset 0x2.</remarks>
        public byte MajorLinkerVersion;

        /// <summary>
        /// Minor version of the linker used to generate this executable.
        /// </summary>
        /// <remarks>Some linkers set this field to zero (0). Note that the version is not very useful because there is no
        /// information about which linker was used. This field is at offset 0x3.</remarks>
        public byte MinorLinkerVersion;

        /// <summary>
        /// Size of the executable code in the executable.
        /// </summary>
        /// <remarks>
        /// This value is not reliable because all code may not be in one section. This field is at offset 0x4.
        /// </remarks>
        public uint SizeOfCode;

        /// <summary>
        /// Size of the initialized data in the executable.
        /// </summary>
        /// <remarks>
        /// This value is not reliable because all initialized data may not be in one section. This field is at offset 0x8.
        /// </remarks>
        public uint SizeOfInitializedData; // 08

        /// <summary>
        /// Size of the uninitialized data in the executable.
        /// </summary>
        /// <remarks>
        /// This value is not reliable because all uninitialized data may not be in one section. This field is at offset 0xC.
        /// </remarks>
        public uint SizeOfUninitializedData;

        /// <summary>
        /// Relative virtual address (RVA) of the entry point of the executable.
        /// </summary>
        /// <remarks>
        /// For stand-alone executables, this points to the startup routine. For dynamically linked libraries (DLLs), this
        /// points to the routine called when an event such as DLL_PROCESS_ATTACH or DLL_THREAD_DETACH occurs. This field is at
        /// offset 0x10.
        /// </remarks>
        public uint AddressOfEntryPoint;

        /// <summary>
        /// Relative virtual address (RVA) of the code.
        /// </summary>
        /// <remarks>
        /// This value is not reliable because all code may not be consecutive. This field is at offset 0x14.
        /// </remarks>
        public uint BaseOfCode;

        /// <summary>
        /// Relative virtual address (RVA) of the initialized data.
        /// </summary>
        /// <remarks>
        /// This value is not reliable because all initialized data may not be consecutive. This field is at offset 0x18.
        /// </remarks>
        public uint BaseOfData;

        /// <summary>
        /// Preferred load address of the executable.
        /// </summary>
        /// <remarks>
        /// If the executable cannot be loaded at the preferred address, it has to be relocated. This field is at offset 0x1C.
        /// </remarks>
        public uint ImageBase;

        /// <summary>
        /// Alignment of sections in main memory.
        /// </summary>
        /// <remarks>This field is at offset 0x20.</remarks>
        public uint SectionAlignment;

        /// <summary>
        /// Alignment of the file in main memory.
        /// </summary>
        /// <remarks>This field is at offset 0x24.</remarks>
        public uint FileAlignment;

        /// <summary>
        /// Major version of the target operating system.
        /// </summary>
        /// <remarks>The operating system doesn't check this value. This field is at offset 0x28.</remarks>
        public ushort MajorOperatingSystemVersion;

        /// <summary>
        /// Minor version of the target operating system.
        /// </summary>
        /// <remarks>The operating system doesn't check this value. This field is at offset 0x2A.</remarks>
        public ushort MinorOperatingSystemVersion;

        /// <summary>
        /// Major version of the executable.
        /// </summary>
        /// <remarks>
        /// This value may not correspond to the version indicated in the version resource. If a version resource is included,
        /// it should be preferred to this value. This field is at offset 0x2C.
        /// </remarks>
        public ushort MajorImageVersion;

        /// <summary>
        /// Minor version of the executable.
        /// </summary>
        /// <remarks>
        /// This value may not correspond to the version indicated in the version resource. If a version resource is included,
        /// it should be preferred to this value. This field is at offset 0x2E.
        /// </remarks>
        public ushort MinorImageVersion;

        /// <summary>
        /// Major version of the subsystem (Win16, Win32, OS/2 or POSIX).
        /// </summary>
        /// <remarks>
        /// The operating system may use this value (along with <see cref="MinorSubsystemVersion"/> to adjust the behavior of
        /// the application to the original behavior on the specified subsystem. This is usually 4 (and
        /// <see cref="MinorSubsystemVersion"/> is 0) for 32-bit executables targetting Windows 95 and later or Windows NT 4 and
        /// later. This field is at offset 0x30.
        /// </remarks>
        public ushort MajorSubsystemVersion;

        /// <summary>
        /// Minor version of the subsystem (Win16, Win32, OS/2 or POSIX).
        /// </summary>
        /// <remarks>
        /// The operating system may use this value (along with <see cref="MajorSubsystemVersion"/> to adjust the behavior of
        /// the application to the original behavior on the specified subsystem. This field is at offset 0x32.
        /// </remarks>
        public ushort MinorSubsystemVersion;

        /// <summary>
        /// Targetted version of Win32.
        /// </summary>
        /// <remarks>This value seems to be unused and always set to zero (0). This field is at offset 0x34.</remarks>
        public uint Win32VersionValue;

        /// <summary>
        /// Amount of memory required by the image, aligned to <see cref="SectionAlignment"/>.
        /// </summary>
        /// <remarks>This field is at offset 0x38.</remarks>
        public uint SizeOfImage;

        /// <summary>
        /// Size of the headers, including the data directories and the section headers.
        /// </summary>
        /// <remarks>This field is at offset 0x3C.</remarks>
        public uint SizeOfHeaders;

        /// <summary>
        /// Checksum of the image.
        /// </summary>
        /// <remarks>
        /// In practice, this value is often not computed and set to zero, as the operating system's loader doesn't actually
        /// check the checksum (except for drivers on Windows NT, and perhaps in a few other specific cases). This field is at
        /// offset 0x40.
        /// </remarks>
        public uint CheckSum;

        /// <summary>
        /// Subsystem the executable is intended to be run in.
        /// </summary>
        /// <remarks>This field is at offset 0x44.</remarks>
        public ushort Subsystem;

        /// <summary>
        /// Characteristics of the DLL.
        /// </summary>
        /// <remarks>This field is at offset 0x46.</remarks>
        public ushort DllCharacteristics;

        /// <summary>
        /// Amount of virtual memory reserved for the main thread's stack.
        /// </summary>
        /// <remarks>This field is at offset 0x48.</remarks>
        public uint SizeOfStackReserve;

        /// <summary>
        /// Amount of memory initially allocated for the main thread's stack.
        /// </summary>
        /// <remarks>This field is at offset 0x4C.</remarks>
        public uint SizeOfStackCommit;

        /// <summary>
        /// Amount of virtual memory reserved for the process's primary heap.
        /// </summary>
        /// <remarks>This field is at offset 0x50.</remarks>
        public uint SizeOfHeapReserve;

        /// <summary>
        /// Amount of memory initially allocated for the process's primary heap.
        /// </summary>
        /// <remarks>This field is at offset 0x54.</remarks>
        public uint SizeOfHeapCommit;

        /// <summary>
        /// This field is obsolete.
        /// </summary>
        /// <remarks>This field is at offset 0x58.</remarks>
        public uint LoaderFlags;

        /// <summary>
        /// Number of IMAGE_DATA_DIRECTORY (<see cref="ImageDataDirectory"/>) structures in the remainder of the optional
        /// header.
        /// </summary>
        /// <remarks>This field is at offset 0x5C.</remarks>
        public uint NumberOfRvaAndSizes;

        /// <summary>
        /// Location and size of the export table.
        /// </summary>
        /// <remarks>This field is at offset 0x60.</remarks>
        public ImageDataDirectory ExportDataDirectory;

        /// <summary>
        /// Location and size of the import table.
        /// </summary>
        /// <remarks>This field is at offset 0x68.</remarks>
        public ImageDataDirectory ImportDataDirectory;

        /// <summary>
        /// Location and size of the resource table.
        /// </summary>
        /// <remarks>This field is at offset 0x70.</remarks>
        public ImageDataDirectory ResourceDataDirectory;

        /// <summary>
        /// Location and size of the exception table.
        /// </summary>
        /// <remarks>This field is at offset 0x78.</remarks>
        public ImageDataDirectory ExceptionDataDirectory;

        /// <summary>
        /// Location and size of the certificate table.
        /// </summary>
        /// <remarks>This field is at offset 0x80.</remarks>
        public ImageDataDirectory SecurityDataDirectory;

        /// <summary>
        /// Location and size of the base relocation table.
        /// </summary>
        /// <remarks>This field is at offset 0x88.</remarks>
        public ImageDataDirectory BaseRelocationDataDirectory;

        /// <summary>
        /// Location and size of the debugging information table.
        /// </summary>
        /// <remarks>This field is at offset 0x90.</remarks>
        public ImageDataDirectory DebugDataDirectory;

        /// <summary>
        /// Location and size of architecture-specific information table.
        /// </summary>
        /// <remarks>This field is at offset 0x98.</remarks>
        public ImageDataDirectory ArchitectureDataDirectory;

        /// <summary>
        /// Global pointer register relative virtual address.
        /// </summary>
        /// <remarks>This field is at offset 0xA0.</remarks>
        public ImageDataDirectory GlobalPointerDataDirectory;

        /// <summary>
        /// Location and size of the thread-local storage table.
        /// </summary>
        /// <remarks>This field is at offset 0xA8.</remarks>
        public ImageDataDirectory TlsDataDirectory;

        /// <summary>
        /// Location and size of the load configuration table.
        /// </summary>
        /// <remarks>This field is at offset 0xB0.</remarks>
        public ImageDataDirectory LoadConfigurationDataDirectory;

        /// <summary>
        /// Location and size of the bound import table.
        /// </summary>
        /// <remarks>This field is at offset 0xB8.</remarks>
        public ImageDataDirectory BoundImportDataDirectory;

        /// <summary>
        /// Location and size of the import address table.
        /// </summary>
        /// <remarks>This field is at offset 0xC0.</remarks>
        public ImageDataDirectory ImportAddressTableDataDirectory;

        /// <summary>
        /// Location and size of the delay load import descriptor.
        /// </summary>
        /// <remarks>This field is at offset 0xC8.</remarks>
        public ImageDataDirectory DelayLoadImportDataDirectory;

        /// <summary>
        /// Location and size of the Common Language Runtime (CLR) header.
        /// </summary>
        /// <remarks>This field is at offset 0xD0.</remarks>
        public ImageDataDirectory ComRuntimeDescriptorDataDirectory;

        /// <summary>
        /// This field is reserved.
        /// </summary>
        /// <remarks>This field is at offset 0xD8.</remarks>
        public ImageDataDirectory ReservedDataDirectory;
    }
}
