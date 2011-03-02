//------------------------------------------------------------------------------
// <copyright file="PEReader.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using SonicRetro.SonicAdventure.Data;

    /// <summary>
    /// Contains logic to read data from a Portable Executable (PE) stream.
    /// </summary>
    public sealed class PEReader : IDisposable
    {
        /// <summary>
        /// <see cref="BinaryReader"/> around the <see cref="VirtualMemoryStream"/> for the image.
        /// </summary>
        private BinaryReader reader;

        /// <summary>
        /// Base address of the image.
        /// </summary>
        private long imageBase;

        /// <summary>
        /// Map of exported symbol names to their target address.
        /// </summary>
        private Dictionary<string, uint> exports;

        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> of objects that have been read using one of the methods of this class on the
        /// current instance.
        /// </summary>
        private Dictionary<long, INamed> knownObjects;

        /// <summary>
        /// Flag indicating whether the object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PEReader"/> class with the data from the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream giving access to a Portable Executable (PE) image.</param>
        public PEReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            stream.Seek(0, SeekOrigin.Begin);
            VirtualMemoryStream virtualMemoryStream;
            BinaryReader reader = new BinaryReader(stream);
            SortedSet<ImageSection> sections = new SortedSet<ImageSection>();
            this.exports = new Dictionary<string, uint>();
            this.knownObjects = new Dictionary<long, INamed>();

            unsafe
            {
                // Read DOS header
                byte[] bytes = new byte[sizeof(ImageDosHeader)];
                stream.Read(bytes, 0, sizeof(ImageDosHeader));
                fixed (byte* dosHeaderBytePtr = bytes)
                {
                    const ushort MZMagic = 0x5A4D; // MZ
                    ImageDosHeader* dosHeader = (ImageDosHeader*)dosHeaderBytePtr;
                    if (dosHeader->Magic != MZMagic)
                    {
                        throw new ArgumentException("The stream does not contain a DOS image header.", "stream");
                    }

                    // Read PE signature
                    const uint PEMagic = 0x4550; // PE<nul><nul>
                    stream.Seek(dosHeader->AddressOfNewExeHeader, SeekOrigin.Begin);
                    if (reader.ReadUInt32() != PEMagic)
                    {
                        throw new ArgumentException("The stream does not contain a PE header.", "stream");
                    }
                }

                // Read COFF header
                bytes = new byte[sizeof(ImageFileHeader)];
                stream.Read(bytes, 0, sizeof(ImageFileHeader));
                fixed (byte* peHeaderBytePtr = bytes)
                {
                    ImageFileHeader* peHeader = (ImageFileHeader*)peHeaderBytePtr;
                    if (peHeader->SizeOfOptionalHeader != sizeof(ImageOptionalHeader32))
                    {
                        throw new ArgumentException("The stream's PE header contains an optional header of an expected size.", "stream");
                    }

                    // Read optional header
                    bytes = new byte[sizeof(ImageOptionalHeader32)];
                    stream.Read(bytes, 0, sizeof(ImageOptionalHeader32));
                    fixed (byte* optionalHeaderBytePtr = bytes)
                    {
                        ImageOptionalHeader32* optionalHeader = (ImageOptionalHeader32*)optionalHeaderBytePtr;
                        if (optionalHeader->Magic != 0x10b)
                        {
                            throw new ArgumentException("The stream's optional header has an unexpected magic number.", "stream");
                        }

                        this.imageBase = optionalHeader->ImageBase;

                        // Read sections
                        bytes = new byte[sizeof(ImageSectionHeader) * peHeader->NumberOfSections];
                        stream.Read(bytes, 0, sizeof(ImageSectionHeader) * peHeader->NumberOfSections);
                        fixed (byte* sectionHeadersBytePtr = bytes)
                        {
                            ImageSectionHeader* sectionHeaders = (ImageSectionHeader*)sectionHeadersBytePtr;
                            for (int i = 0; i < peHeader->NumberOfSections; i++)
                            {
                                sections.Add(new ImageSection(sectionHeaders + i));
                            }
                        }

                        // Initialize virtual memory stream
                        virtualMemoryStream = new VirtualMemoryStream(stream, this.imageBase, sections);

                        // Read exports
                        virtualMemoryStream.Seek(this.imageBase + optionalHeader->ExportDataDirectory.VirtualAddress, SeekOrigin.Begin);
                        bytes = new byte[sizeof(ImageExportDirectory)];
                        virtualMemoryStream.Read(bytes, 0, sizeof(ImageExportDirectory));
                        fixed (byte* exportDirectoryBytePtr = bytes)
                        {
                            ImageExportDirectory* exportDirectory = (ImageExportDirectory*)exportDirectoryBytePtr;

                            // Read function pointers
                            virtualMemoryStream.Seek(this.imageBase + exportDirectory->AddressOfFunctions, SeekOrigin.Begin);
                            bytes = new byte[sizeof(uint) * exportDirectory->NumberOfFunctions];
                            virtualMemoryStream.Read(bytes, 0, (int)(sizeof(uint) * exportDirectory->NumberOfFunctions));
                            fixed (byte* functionPointersBytePtr = bytes)
                            {
                                uint* functionPointers = (uint*)functionPointersBytePtr;

                                // Read function names pointers
                                virtualMemoryStream.Seek(this.imageBase + exportDirectory->AddressOfNames, SeekOrigin.Begin);
                                bytes = new byte[sizeof(uint) * exportDirectory->NumberOfNames];
                                virtualMemoryStream.Read(bytes, 0, (int)(sizeof(uint) * exportDirectory->NumberOfNames));
                                fixed (byte* functionNamesPointersBytePtr = bytes)
                                {
                                    uint* functionNamesPointers = (uint*)functionNamesPointersBytePtr;

                                    // Read function name ordinals pointers
                                    virtualMemoryStream.Seek(this.imageBase + exportDirectory->AddressOfNameOrdinals, SeekOrigin.Begin);
                                    bytes = new byte[sizeof(uint) * exportDirectory->NumberOfNames];
                                    virtualMemoryStream.Read(bytes, 0, (int)(sizeof(ushort) * exportDirectory->NumberOfNames));
                                    fixed (byte* functionNameOrdinalsPointersBytePtr = bytes)
                                    {
                                        ushort* functionNameOrdinalsPointers = (ushort*)functionNameOrdinalsPointersBytePtr;

                                        // Build export dictionary
                                        for (int i = 0; i < exportDirectory->NumberOfNames; i++)
                                        {
                                            virtualMemoryStream.Seek(this.imageBase + functionNamesPointers[i], SeekOrigin.Begin);

                                            List<byte> name = new List<byte>();
                                            int byteRead;

                                            // Loop until null character (0) or end of stream (-1)
                                            while ((byteRead = virtualMemoryStream.ReadByte()) > 0)
                                            {
                                                name.Add((byte)byteRead);
                                            }

                                            this.exports[Encoding.Default.GetString(name.ToArray())] = optionalHeader->ImageBase + functionPointers[functionNameOrdinalsPointers[i]];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.reader = new BinaryReader(virtualMemoryStream);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="PEReader"/> class.
        /// </summary>
        public void Dispose()
        {
            /* The PEReader class is sealed, so we don't need to bother implementing the "Dispose" pattern as described at
               http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx. */

            if (!this.disposed)
            {
                if (this.reader != null)
                {
                    this.reader.Dispose();
                    this.reader = null;
                }

                this.exports = null;
                this.knownObjects = null;

                this.disposed = true;
            }
        }

        /// <summary>
        /// Returns the virtual address of an exported function or variable in the image.
        /// </summary>
        /// <param name="name">The name of the export.</param>
        /// <returns>The virtual address of the export, or 0 if the export doesn't exist.</returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        [CLSCompliant(false)]
        public uint GetExport(string name)
        {
            this.ThrowIfDisposed();

            uint value;
            this.exports.TryGetValue(name, out value);
            return value;
        }

        /// <summary>
        /// Returns a named object that has already been analyzed by the current instance of <see cref="PEReader"/>.
        /// </summary>
        /// <param name="address">The address of the object to retrieve.</param>
        /// <returns>
        /// A reference to the named object, or <c>null</c> (<c>Nothing</c> in Visual Basic) if no object has been analyzed at
        /// the specified address yet.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public INamed GetKnownObject(long address)
        {
            this.ThrowIfDisposed();

            INamed value;
            this.knownObjects.TryGetValue(address, out value);
            return value;
        }

        /// <summary>
        /// Reads an <see cref="ATTACH"/> structure at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="ATTACH"/> structure starts.</param>
        /// <returns>
        /// An instance of the <see cref="ATTACH"/> class containing the data of the ATTACH structure at the given address in
        /// the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <remarks>
        /// Every time this method is invoked with the same address on the same instance of the <see cref="PEReader"/> class,
        /// the same instance is returned.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public ATTACH ReadAttach(long address)
        {
            return this.Extract(
                address,
                delegate
                {
                    string name = "attach_" + address.ToString("X8", CultureInfo.InvariantCulture);

                    // Vertices and normals
                    uint addressOfVertices = this.reader.ReadUInt32();
                    uint addressOfNormals = this.reader.ReadUInt32();
                    int verticesCount = this.reader.ReadInt32();
                    var vertices = this.ReadVector3Array(addressOfVertices, verticesCount);
                    var normals = this.ReadVector3Array(addressOfNormals, verticesCount);

                    // Meshes and materials
                    uint addressOfMeshes = this.reader.ReadUInt32();
                    uint addressOfMaterials = this.reader.ReadUInt32();
                    var meshes = this.ReadMeshArray(addressOfMeshes, this.reader.ReadUInt16());
                    var materials = this.ReadMaterialArray(addressOfMaterials, this.reader.ReadUInt16());

                    Vector3 center = this.ReadVector3();
                    float radius = this.reader.ReadSingle();
                    int nullMember = this.reader.ReadInt32();

                    return new ATTACH(name, vertices, normals, meshes, materials, center, radius, nullMember);
                });
        }

        /// <summary>
        /// Reads an array of pointers to <see cref="ATTACH"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="ATTACH"/> pointers start.</param>
        /// <param name="count">The number of pointers.</param>
        /// <returns>
        /// A collection of <see cref="ATTACH"/> objects containing the data of the ATTACH structures referenced by the pointers
        /// in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<ATTACH> ReadAttachPointerArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<ATTACH> attaches = new List<ATTACH>(count);
                    for (int i = 0; i < count; i++)
                    {
                        attaches.Add(this.ReadAttach(this.reader.ReadUInt32()));
                    }

                    return new NamedCollection<ATTACH>(attaches) { Name = "MODELS_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an <see cref="AnimHead"/> structure at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimHead"/> structure starts.</param>
        /// <returns>
        /// An instance of the <see cref="AnimHead"/> class containing the data of the AnimHead structure at the given address
        /// in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <remarks>When this method is invoked more than once with the same address, the same object is returned.</remarks>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public AnimHead ReadAnimHead(long address)
        {
            return this.Extract(
                address,
                delegate
                {
                    AnimHead animHead = new AnimHead();
                    animHead.Name = "ah_" + address.ToString("X8", CultureInfo.InvariantCulture);
                    animHead.Model = this.ReadObject(this.reader.ReadUInt32());
                    animHead.Motion = this.ReadAnimHead2(this.reader.ReadUInt32(), CountVerticesInObject(animHead.Model).ToArray());
                    return animHead;
                });
        }

        /// <summary>
        /// Reads an <see cref="AnimHead2"/> structure at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimHead2"/> structure starts.</param>
        /// <param name="vector3CountsByModel">
        /// Array containing the number of <see cref="Vector3"/> structures contained in the vertex animation data arrays for
        /// each animatable model in the object tree corresponding to the animation.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="AnimHead2"/> class containing the data of the AnimHead2 structure at the given address
        /// in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <remarks>When this method is invoked more than once with the same address, the same object is returned.</remarks>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public AnimHead2 ReadAnimHead2(long address, int[] vector3CountsByModel)
        {
            return this.Extract(
                address,
                delegate
                {
                    string name = "ah2_" + address.ToString("X8", CultureInfo.InvariantCulture);

                    uint addressOfFrameData = this.reader.ReadUInt32();
                    int frameCount = this.reader.ReadInt32();
                    ushort flags = this.reader.ReadUInt16();
                    NamedCollection<AnimFrame> frameData;

                    switch (flags)
                    {
                        case 3:
                            frameData = this.ReadAnimFrame_PosRotArray(addressOfFrameData, vector3CountsByModel.Length);
                            break;
                        case 7:
                            frameData = this.ReadAnimFrame_PosRotScaleArray(addressOfFrameData, vector3CountsByModel.Length);
                            break;
                        case 0x30:
                            frameData = this.ReadAnimFrame_VertNrmArray(addressOfFrameData, vector3CountsByModel);
                            break;
                        default:
                            throw new UnexpectedDataException("Unexpected flags value in AnimHead2 @ 0x" + address.ToString("X8", CultureInfo.InvariantCulture) + ": 0x" + flags.ToString("X4", CultureInfo.InvariantCulture));
                    }

                    ushort unknown0A = this.reader.ReadUInt16();
                    return new AnimHead2(name, frameData, frameCount, flags, unknown0A);
                });
        }

        /// <summary>
        /// Reads an array of pointers to <see cref="AnimHead"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimHead"/> pointers start.</param>
        /// <param name="count">The number of pointers.</param>
        /// <returns>
        /// A collection of <see cref="AnimHead"/> objects containing the data of the AnimHead structures referenced by the
        /// pointers in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<AnimHead> ReadAnimHeadPointerArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<AnimHead> animHeads = new List<AnimHead>(count);
                    for (int i = 0; i < count; i++)
                    {
                        animHeads.Add(this.ReadAnimHead(this.reader.ReadUInt32()));
                    }

                    return new NamedCollection<AnimHead>(animHeads) { Name = "ACTIONS_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of pointers to <see cref="AnimHead2"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimHead2"/> pointers start.</param>
        /// <param name="vector3CountsByModel">
        /// Array of arrays containing the number of <see cref="Vector3"/> structures contained in the vertex animation data
        /// arrays for each animatable model in the corresponding object tree and for each AnimHead2 structure.
        /// </param>
        /// <returns>
        /// A collection of <see cref="AnimHead"/> objects containing the data of the AnimHead structures referenced by the
        /// pointers in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<AnimHead2> ReadAnimHead2PointerArray(long address, int[][] vector3CountsByModel)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<AnimHead2> animHead2s = new List<AnimHead2>(vector3CountsByModel.Length);
                    for (int i = 0; i < vector3CountsByModel.Length; i++)
                    {
                        animHead2s.Add(this.ReadAnimHead2(this.reader.ReadUInt32(), vector3CountsByModel[i]));
                    }

                    return new NamedCollection<AnimHead2>(animHead2s) { Name = "MOTIONS_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of ARGB colors at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the ARGB colors start.</param>
        /// <param name="count">The number of colors.</param>
        /// <returns>
        /// A collection of <see cref="UInt32"/> structures containing the color data from the image, or <c>null</c> if
        /// <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        [CLSCompliant(false)]
        public NamedCollection<uint> ReadColorArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<uint> colors = new List<uint>(count);
                    for (int i = 0; i < count; i++)
                    {
                        colors.Add(this.reader.ReadUInt32());
                    }

                    return new NamedCollection<uint>(colors) { Name = "colors_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of pointers to <see cref="MATERIAL"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="MATERIAL"/> pointers start.</param>
        /// <param name="count">The number of pointers.</param>
        /// <returns>
        /// A collection of <see cref="MATERIAL"/> objects containing the data of the MATERIAL structures referenced by the
        /// pointers in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<MATERIAL> ReadMaterialArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<MATERIAL> materials = new List<MATERIAL>(count);
                    for (int i = 0; i < count; i++)
                    {
                        materials.Add(this.ReadMaterial());
                    }

                    return new NamedCollection<MATERIAL>(materials) { Name = "mat_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of pointers to arrays of <see cref="MATERIAL"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="MATERIAL"/> array pointers start.</param>
        /// <param name="materialCounts">The number of <see cref="MATERIAL"/> structures in each array.</param>
        /// <returns>
        /// A collection of collections of <see cref="MATERIAL"/> objects containing the data of the MATERIAL arrays referenced
        /// by the pointers in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<NamedCollection<MATERIAL>> ReadMaterialArrayPointerArray(long address, int[] materialCounts)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<NamedCollection<MATERIAL>> objects = new List<NamedCollection<MATERIAL>>(materialCounts.Length);
                    for (int i = 0; i < materialCounts.Length; i++)
                    {
                        objects.Add(this.ReadMaterialArray(this.reader.ReadUInt32(), materialCounts[i]));
                    }

                    return new NamedCollection<NamedCollection<MATERIAL>>(objects) { Name = "MATERIALS_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of pointers to <see cref="MESH"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="MESH"/> pointers start.</param>
        /// <param name="count">The number of pointers.</param>
        /// <returns>
        /// A collection of <see cref="MESH"/> objects containing the data of the MESH structures referenced by the pointers in
        /// the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<MESH> ReadMeshArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<MESH> meshes = new List<MESH>(count);
                    for (int i = 0; i < count; i++)
                    {
                        meshes.Add(this.ReadMesh());
                    }

                    return new NamedCollection<MESH>(meshes) { Name = "mesh_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an <see cref="OBJECT"/> structure at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="OBJECT"/> structure starts.</param>
        /// <returns>
        /// An instance of the <see cref="OBJECT"/> class containing the data of the OBJECT structure at the given address in
        /// the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <remarks>When this method is invoked more than once with the same address, the same object is returned.</remarks>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public OBJECT ReadObject(long address)
        {
            return this.Extract(
                address,
                delegate
                {
                    OBJECT obj = new OBJECT();
                    obj.Name = "obj_" + address.ToString("X8", CultureInfo.InvariantCulture);
                    obj.Flags = this.reader.ReadUInt32();
                    obj.Attach = this.ReadAttach(this.reader.ReadUInt32());
                    obj.Position = this.ReadVector3();
                    obj.Rotation = this.ReadRotation3();
                    obj.Scale = this.ReadVector3();
                    obj.Child = this.ReadObject(this.reader.ReadUInt32());
                    obj.Sibling = this.ReadObject(this.reader.ReadUInt32());
                    return obj;
                });
        }

        /// <summary>
        /// Reads an array of pointers to <see cref="OBJECT"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="OBJECT"/> pointers start.</param>
        /// <param name="count">The number of pointers.</param>
        /// <returns>
        /// A collection of <see cref="OBJECT"/> objects containing the data of the OBJECT structures referenced by the pointers
        /// in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<OBJECT> ReadObjectPointerArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<OBJECT> objects = new List<OBJECT>(count);
                    for (int i = 0; i < count; i++)
                    {
                        objects.Add(this.ReadObject(this.reader.ReadUInt32()));
                    }

                    return new NamedCollection<OBJECT>(objects) { Name = "OBJECTS_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="PolyNormal"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="PolyNormal"/> structures start.</param>
        /// <param name="count">The number of <see cref="PolyNormal"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="PolyNormal"/> objects containing the data of the PolyNormal structures from the image, or
        /// <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<PolyNormal> ReadPolyNormalArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<PolyNormal> polyNormals = new List<PolyNormal>(count);
                    for (int i = 0; i < count; i++)
                    {
                        polyNormals.Add(this.ReadPolyNormal());
                    }

                    return new NamedCollection<PolyNormal>(polyNormals) { Name = "pn_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="Rotation3AnimData"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="Rotation3AnimData"/> structures start.</param>
        /// <param name="count">The number of <see cref="Rotation3AnimData"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="Rotation3AnimData"/> objects containing the data of the Rotation3AnimData structures from
        /// the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<Rotation3AnimData> ReadRotation3AnimDataArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<Rotation3AnimData> animData = new List<Rotation3AnimData>(count);
                    for (int i = 0; i < count; i++)
                    {
                        Rotation3AnimData rotation3AnimData = new Rotation3AnimData();
                        rotation3AnimData.FrameNumber = this.reader.ReadInt32();
                        rotation3AnimData.Rotation = this.ReadRotation3();
                        animData.Add(rotation3AnimData);
                    }

                    return new NamedCollection<Rotation3AnimData>(animData) { Name = "r3ad_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="UV"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="UV"/> structures start.</param>
        /// <param name="count">The number of <see cref="UV"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="UV"/> objects containing the data of the UV structures from the image, or <c>null</c> if
        /// <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<UV> ReadUVArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<UV> uvs = new List<UV>(count);
                    for (int i = 0; i < count; i++)
                    {
                        uvs.Add(this.ReadUV());
                    }

                    return new NamedCollection<UV>(uvs) { Name = "uv_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="Vector3"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="Vector3"/> structures start.</param>
        /// <param name="count">The number of <see cref="Vector3"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="Vector3"/> objects containing the data of the Vector3 structures from the image, or
        /// <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<Vector3> ReadVector3Array(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<Vector3> vector3s = new List<Vector3>(count);
                    for (int i = 0; i < count; i++)
                    {
                        vector3s.Add(this.ReadVector3());
                    }

                    return new NamedCollection<Vector3>(vector3s) { Name = "vec3_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="Vector3AnimData"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="Vector3AnimData"/> structures start.</param>
        /// <param name="count">The number of <see cref="Vector3AnimData"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="Vector3AnimData"/> objects containing the data of the Vector3AnimData structures from the
        /// image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<Vector3AnimData> ReadVector3AnimDataArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<Vector3AnimData> animData = new List<Vector3AnimData>(count);
                    for (int i = 0; i < count; i++)
                    {
                        Vector3AnimData vector3AnimData = new Vector3AnimData();
                        vector3AnimData.FrameNumber = this.reader.ReadInt32();
                        vector3AnimData.Vector = this.ReadVector3();
                        animData.Add(vector3AnimData);
                    }

                    return new NamedCollection<Vector3AnimData>(animData) { Name = "v3ad_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="Vector3ArrayAnimData"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="Vector3ArrayAnimData"/> structures start.</param>
        /// <param name="count">The number of <see cref="Vector3ArrayAnimData"/> structures.</param>
        /// <param name="vector3Count">The number of <see cref="Vector3"/> structures in the <see cref="Vector3"/> array.</param>
        /// <returns>
        /// A collection of <see cref="Vector3ArrayAnimData"/> objects containing the data of the Vector3ArrayAnimData
        /// structures from the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<Vector3ArrayAnimData> ReadVector3ArrayAnimDataArray(long address, int count, int vector3Count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<Vector3ArrayAnimData> animData = new List<Vector3ArrayAnimData>(count);
                    for (int i = 0; i < count; i++)
                    {
                        int frameNumber = this.reader.ReadInt32();
                        var vectors = this.ReadVector3Array(this.reader.ReadUInt32(), vector3Count);
                        animData.Add(new Vector3ArrayAnimData(frameNumber, vectors));
                    }

                    return new NamedCollection<Vector3ArrayAnimData>(animData) { Name = "v3aad_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of pointers to arrays of <see cref="Vector3"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="Vector3"/> arrays start.</param>
        /// <param name="vector3Counts">The number of <see cref="Vector3"/> structures in each array.</param>
        /// <returns>
        /// A collection of collections of <see cref="Vector3"/> objects containing the data of the Vector3 arrays referenced by
        /// the pointers in the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Operations on a disposed PEReader are not allowed.</exception>
        public NamedCollection<NamedCollection<Vector3>> ReadVector3ArrayPointerArray(long address, int[] vector3Counts)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<NamedCollection<Vector3>> vector3Collections = new List<NamedCollection<Vector3>>();
                    for (int i = 0; i < vector3Counts.Length; i++)
                    {
                        vector3Collections.Add(this.ReadVector3Array(this.reader.ReadUInt32(), vector3Counts[i]));
                    }

                    return new NamedCollection<NamedCollection<Vector3>>(vector3Collections) { Name = "POINTS_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Counts the number of vertices for each animatable model in the specified object tree.
        /// </summary>
        /// <param name="obj">The root of the object tree.</param>
        /// <returns>
        /// <see cref="List{T}"/> containing the number of vertices for each animatable model in the object tree.
        /// </returns>
        private static List<int> CountVerticesInObject(OBJECT obj)
        {
            List<int> numVertices = new List<int>();

            // If this flag is set, ignore the model
            if ((obj.Flags & 0x40) == 0)
            {
                if (obj.Attach != null)
                {
                    numVertices.Add(obj.Attach.Vertices.Count);
                }
                else
                {
                    numVertices.Add(0);
                }
            }

            if (obj.Child != null)
            {
                numVertices.AddRange(CountVerticesInObject(obj.Child));
            }

            if (obj.Sibling != null)
            {
                numVertices.AddRange(CountVerticesInObject(obj.Sibling));
            }

            return numVertices;
        }

        /// <summary>
        /// Extracts an object from the image.
        /// </summary>
        /// <typeparam name="T">Type of the object to extract.</typeparam>
        /// <param name="address">Virtual address of the object to extract.</param>
        /// <param name="callback">Callback method to invoke to extract the object.</param>
        /// <returns>
        /// The object located at <paramref name="address"/>, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        /// <remarks>
        /// The <see cref="PEReader"/> class maintains a list of objects that it has already read. If <paramref name="address"/>
        /// is found in this list, the corresponding object is returned and <paramref name="callback"/> is not called.
        /// Otherwise, <see cref="reader"/> is positioned at the address in the image corresponding to the virtual address given
        /// in <paramref name="address"/>, then <paramref name="callback"/> is invoked, then <see cref="reader"/> is restored to
        /// its previous position.
        /// </remarks>
        private T Extract<T>(long address, Func<T> callback)
            where T : INamed
        {
            this.ThrowIfDisposed();

            if (address == 0)
            {
                return default(T);
            }

            INamed knownObject;
            if (this.knownObjects.TryGetValue(address, out knownObject) && (knownObject == null || typeof(T).IsAssignableFrom(knownObject.GetType())))
            {
                return (T)knownObject;
            }

            long oldPosition = this.reader.BaseStream.Position;
            this.reader.BaseStream.Seek(address, SeekOrigin.Begin); // BaseStream is a VirtualMemoryStream
            try
            {
                T obj = callback();
                this.knownObjects[address] = obj;
                return obj;
            }
            finally
            {
                this.reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Reads an array of <see cref="AnimFrame_PosRot"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimFrame_PosRot"/> structures start.</param>
        /// <param name="count">The number of <see cref="AnimFrame_PosRot"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="AnimFrame_PosRot"/> objects containing the data of the AnimFrame_PosRot structures from
        /// the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        private NamedCollection<AnimFrame> ReadAnimFrame_PosRotArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<AnimFrame> animFrames = new List<AnimFrame>(count);
                    for (int i = 0; i < count; i++)
                    {
                        uint addressOfPositions = this.reader.ReadUInt32();
                        uint addressOfRotations = this.reader.ReadUInt32();
                        var positions = this.ReadVector3AnimDataArray(addressOfPositions, this.reader.ReadInt32());
                        var rotations = this.ReadRotation3AnimDataArray(addressOfRotations, this.reader.ReadInt32());
                        animFrames.Add(new AnimFrame_PosRot(positions, rotations));
                    }

                    return new NamedCollection<AnimFrame>(animFrames) { Name = "afpr_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="AnimFrame_PosRotScale"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimFrame_PosRotScale"/> structures start.</param>
        /// <param name="count">The number of <see cref="AnimFrame_PosRotScale"/> structures.</param>
        /// <returns>
        /// A collection of <see cref="AnimFrame_PosRotScale"/> objects containing the data of the AnimFrame_PosRotScale
        /// structures from the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        private NamedCollection<AnimFrame> ReadAnimFrame_PosRotScaleArray(long address, int count)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<AnimFrame> animFrames = new List<AnimFrame>(count);
                    for (int i = 0; i < count; i++)
                    {
                        uint addressOfPositions = this.reader.ReadUInt32();
                        uint addressOfRotations = this.reader.ReadUInt32();
                        uint addressOfScales = this.reader.ReadUInt32();
                        var positions = this.ReadVector3AnimDataArray(addressOfPositions, this.reader.ReadInt32());
                        var rotations = this.ReadRotation3AnimDataArray(addressOfRotations, this.reader.ReadInt32());
                        var scales = this.ReadVector3AnimDataArray(addressOfScales, this.reader.ReadInt32());
                        animFrames.Add(new AnimFrame_PosRotScale(positions, rotations, scales));
                    }

                    return new NamedCollection<AnimFrame>(animFrames) { Name = "afprs_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads an array of <see cref="AnimFrame_VertNrm"/> structures at <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the <see cref="AnimFrame_VertNrm"/> structures start.</param>
        /// <param name="vector3CountsByModel">
        /// Array containing the number of <see cref="Vector3"/> structures for each animatable model in the object tree.
        /// </param>
        /// <returns>
        /// A collection of <see cref="AnimFrame_VertNrm"/> objects containing the data of the AnimFrame_VertNrm structures from
        /// the image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        private NamedCollection<AnimFrame> ReadAnimFrame_VertNrmArray(long address, int[] vector3CountsByModel)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<AnimFrame> animFrames = new List<AnimFrame>(vector3CountsByModel.Length);
                    for (int i = 0; i < vector3CountsByModel.Length; i++)
                    {
                        uint addressOfVertices = this.reader.ReadUInt32();
                        uint addressOfNormals = this.reader.ReadUInt32();
                        var vertices = this.ReadVector3ArrayAnimDataArray(addressOfVertices, this.reader.ReadInt32(), vector3CountsByModel[i]);
                        var normals = this.ReadVector3ArrayAnimDataArray(addressOfNormals, this.reader.ReadInt32(), 1);
                        animFrames.Add(new AnimFrame_VertNrm(vertices, normals));
                    }

                    return new NamedCollection<AnimFrame>(animFrames) { Name = "afpr_" + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads a <see cref="MATERIAL"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="MATERIAL"/> structure containing the data of the ATTACH structure at the current
        /// position of <see cref="reader"/>.
        /// </returns>
        private MATERIAL ReadMaterial()
        {
            MATERIAL material = new MATERIAL();
            material.DiffuseColor = this.reader.ReadUInt32();
            material.SpecularColor = this.reader.ReadUInt32();
            material.Unknown08 = this.reader.ReadSingle();
            material.TextureId = this.reader.ReadUInt32();
            material.Unknown10 = this.reader.ReadUInt16();
            material.Flags = this.reader.ReadByte();
            material.Unknown13 = this.reader.ReadByte();
            return material;
        }

        /// <summary>
        /// Reads a <see cref="MESH"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="MESH"/> structure containing the data of the MESH structure at the current position of
        /// <see cref="reader"/>.
        /// </returns>
        private MESH ReadMesh()
        {
            ushort materialIdAndPolyType = this.reader.ReadUInt16();
            ushort polyCount = this.reader.ReadUInt16();
            var polys = this.ReadPolyArray(this.reader.ReadUInt32(), polyCount, materialIdAndPolyType >> 14);
            int polyVertexCount = 0;
            for (int i = 0; i < polys.Count; i++)
            {
                polyVertexCount += polys[i].NumberOfVertices;
            }

            int polyAttributes = this.reader.ReadInt32();
            var polyNormals = this.ReadPolyNormalArray(this.reader.ReadUInt32(), polyVertexCount);
            var vertexColors = this.ReadColorArray(this.reader.ReadUInt32(), polyVertexCount);
            var uv = this.ReadUVArray(this.reader.ReadUInt32(), polyVertexCount);
            int nullMember = this.reader.ReadInt32();
            return new MESH(materialIdAndPolyType, polys, polyAttributes, polyNormals, vertexColors, uv, nullMember);
        }

        /// <summary>
        /// Reads an array of <see cref="Triangle"/>, <see cref="Quad"/> or <see cref="Strip"/> structures at
        /// <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The address at which the structures start.</param>
        /// <param name="count">The number of structures.</param>
        /// <param name="polyType">
        /// Specifies the type of polygon to read.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <description>Meaning</description>
        ///     </listheader>
        ///     <item>
        ///         <term>0</term>
        ///         <description><see cref="Triangle"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>1</term>
        ///         <description><see cref="Quad"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>2 or 3</term>
        ///         <description><see cref="Strip"/>.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>
        /// A collection of <see cref="Poly"/> objects containing the data of the Triangle, Quad or Strip structures from the
        /// image, or <c>null</c> if <paramref name="address"/> is 0.
        /// </returns>
        private NamedCollection<Poly> ReadPolyArray(long address, int count, int polyType)
        {
            return this.Extract(
                address,
                delegate
                {
                    List<Poly> polys = new List<Poly>(count);
                    string prefix;
                    Func<Poly> polyReader;
                    switch (polyType)
                    {
                        case 0:
                            prefix = "tris_";
                            polyReader = this.ReadTriangle;
                            break;
                        case 1:
                            prefix = "quads_";
                            polyReader = this.ReadQuad;
                            break;
                        default:
                            prefix = "strips_";
                            polyReader = this.ReadStrip;
                            break;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        polys.Add(polyReader());
                    }

                    return new NamedCollection<Poly>(polys) { Name = prefix + address.ToString("X8", CultureInfo.InvariantCulture) };
                });
        }

        /// <summary>
        /// Reads a <see cref="PolyNormal"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="PolyNormal"/> structure containing the data of the PolyNormal structure at the current
        /// position of <see cref="reader"/>.
        /// </returns>
        private PolyNormal ReadPolyNormal()
        {
            PolyNormal polyNormal = new PolyNormal();
            polyNormal.Unknown00 = this.reader.ReadSingle();
            polyNormal.Unknown04 = this.reader.ReadSingle();
            return polyNormal;
        }

        /// <summary>
        /// Reads a <see cref="Quad"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Quad"/> class containing the data of the Quad structure at the current position of
        /// <see cref="reader"/>.
        /// </returns>
        private Poly ReadQuad()
        {
            Quad quad = new Quad();
            quad.Vertex1 = this.reader.ReadUInt16();
            quad.Vertex2 = this.reader.ReadUInt16();
            quad.Vertex3 = this.reader.ReadUInt16();
            quad.Vertex4 = this.reader.ReadUInt16();
            return quad;
        }

        /// <summary>
        /// Reads a <see cref="Rotation3"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Rotation3"/> structure containing the data of the Rotation3 structure at the current
        /// position of <see cref="reader"/>.
        /// </returns>
        private Rotation3 ReadRotation3()
        {
            Rotation3 rot = new Rotation3();
            rot.X = this.reader.ReadInt32();
            rot.Y = this.reader.ReadInt32();
            rot.Z = this.reader.ReadInt32();
            return rot;
        }

        /// <summary>
        /// Reads a <see cref="Strip"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Strip"/> class containing the data of the Strip structure at the current position of
        /// <see cref="reader"/>.
        /// </returns>
        private Poly ReadStrip()
        {
            ushort countAndDirection = this.reader.ReadUInt16();
            ushort count = (ushort)(countAndDirection & 0x7FFFu);
            List<ushort> vertices = new List<ushort>(count);
            bool isReversed = (countAndDirection & 0x8000) != 0;
            for (ushort i = 0; i < count; i++)
            {
                vertices.Add(this.reader.ReadUInt16());
            }

            return new Strip(isReversed, new Collection<ushort>(vertices));
        }

        /// <summary>
        /// Reads a <see cref="Triangle"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Triangle"/> class containing the data of the Triangle structure at the current
        /// position of <see cref="reader"/>.
        /// </returns>
        private Poly ReadTriangle()
        {
            Triangle triangle = new Triangle();
            triangle.Vertex1 = this.reader.ReadUInt16();
            triangle.Vertex2 = this.reader.ReadUInt16();
            triangle.Vertex3 = this.reader.ReadUInt16();
            return triangle;
        }

        /// <summary>
        /// Reads a <see cref="UV"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="UV"/> structure containing the data of the UV structure at the current position of
        /// <see cref="reader"/>.
        /// </returns>
        private UV ReadUV()
        {
            UV uv = new UV();
            uv.U = this.reader.ReadInt16();
            uv.V = this.reader.ReadInt16();
            return uv;
        }

        /// <summary>
        /// Reads a <see cref="Vector3"/> structure at the current position of <see cref="reader"/>.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Vector3"/> structure containing the data of the Vector3 structure at the current
        /// position of <see cref="reader"/>.
        /// </returns>
        private Vector3 ReadVector3()
        {
            Vector3 vec = new Vector3();
            vec.X = this.reader.ReadSingle();
            vec.Y = this.reader.ReadSingle();
            vec.Z = this.reader.ReadSingle();
            return vec;
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the <see cref="PEReader"/> instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Operations on a disposed <see cref="PEReader"/> instance are not allowed.
        /// </exception>
        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(Properties.Resources.PEReaderDisposed);
            }
        }
    }
}
