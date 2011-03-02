//------------------------------------------------------------------------------
// <copyright file="ImageSection.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

////#define IMAGE_SECTION_HAS_NAME

namespace SonicRetro.SonicAdventure.PCTools
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Represents a section in a Portable Executable (PE) image. This class is immutable.
    /// </summary>
    internal sealed class ImageSection : IComparable<ImageSection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSection"/> class with the specified <see cref="ImageSectionHeader"/>.
        /// </summary>
        /// <param name="imageSectionHeader">Pointer to a <see cref="ImageSectionHeader"/> structure that provides the initial values for the new instance.</param>
        public unsafe ImageSection(ImageSectionHeader* imageSectionHeader)
        {
#if IMAGE_SECTION_HAS_NAME
            byte[] name = new byte[8];
            Marshal.Copy(new IntPtr(imageSectionHeader->Name), name, 0, 8);
            this.Name = Encoding.Default.GetString(name).TrimEnd('\0');
#endif
            this.VirtualSize = imageSectionHeader->PhysicalAddressOrVirtualSize;
            this.VirtualAddress = imageSectionHeader->VirtualAddress;
            this.SizeOfRawData = imageSectionHeader->SizeOfRawData;
            this.PointerToRawData = imageSectionHeader->PointerToRawData;
        }

#if IMAGE_SECTION_HAS_NAME
        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <value>The name of the section.</value>
        public string Name { get; private set; }
#endif

        /// <summary>
        /// Gets the virtual size of the section.
        /// </summary>
        /// <value>The virtual size of the section.</value>
        public uint VirtualSize { get; private set; }

        /// <summary>
        /// Gets the relative virtual address (RVA) at which the section is to be loaded in memory.
        /// </summary>
        /// <value>The relative virtual address (RVA) of the section.</value>
        public uint VirtualAddress { get; private set; }

        /// <summary>
        /// Gets the size of the initial raw data in the section.
        /// </summary>
        /// <value>The size of the raw data.</value>
        public uint SizeOfRawData { get; private set; }

        /// <summary>
        /// Gets the address (in the image) of the initial raw data in the section.
        /// </summary>
        /// <value>The address (in the image) of the raw data.</value>
        public uint PointerToRawData { get; private set; }

        /// <summary>
        /// Compares the current instance to another instance.
        /// </summary>
        /// <param name="other"><see cref="ImageSection"/> to compare the current instance to.</param>
        /// <returns>The result of the comparison of the instances' <see cref="VirtualAddress"/> property.</returns>
        public int CompareTo(ImageSection other)
        {
            return this.VirtualAddress.CompareTo(other.VirtualAddress);
        }
    }
}
