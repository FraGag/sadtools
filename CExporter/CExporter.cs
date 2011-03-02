//------------------------------------------------------------------------------
// <copyright file="CExporter.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.CExporter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using SonicRetro.SonicAdventure.Data;

    /// <summary>
    /// Defines static methods that generate C code representing data structures used in Sonic Adventure.
    /// </summary>
    public static class CExporter
    {
        /// <summary>
        /// Exports a collection of <see cref="AnimFrame"/> items.
        /// </summary>
        /// <param name="animFrames">Collection of <see cref="AnimFrame"/> items to export.</param>
        /// <param name="exportTracker">
        /// <see cref="ExportTracker"/> to use to track exported <see cref="Vector3"/> collections. Specify a valid reference
        /// to export all dependent data recursively, or <c>null</c> to export only the collection.
        /// </param>
        /// <returns>A string containing the C code representing the collection of <see cref="AnimFrame"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="animFrames"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="animFrames"/> is an empty collection, contains a <c>null</c> item or is not homogeneous (that is,
        /// it contains items of different concrete types).
        /// </exception>
        public static string Export(NamedCollection<AnimFrame> animFrames, ExportTracker exportTracker)
        {
            if (animFrames == null)
            {
                throw new ArgumentNullException("animFrames");
            }

            if (animFrames.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "animFrames");
            }

            if (animFrames[0] == null)
            {
                throw new ArgumentException(Properties.Resources.AnimFrameCollectionContainsNull, "animFrames");
            }

            Type typeOfAnimFrame = animFrames[0].GetType();
            for (int i = 1; i < animFrames.Count; i++)
            {
                if (animFrames[i] == null)
                {
                    throw new ArgumentException(Properties.Resources.AnimFrameCollectionContainsNull, "animFrames");
                }

                if (animFrames[i].GetType() != typeOfAnimFrame)
                {
                    throw new ArgumentException(Properties.Resources.AnimFrameCollectionIsNotHomogeneous, "animFrames");
                }
            }

            StringBuilder previousDefinitions = null;
            if (exportTracker != null)
            {
                previousDefinitions = new StringBuilder();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct ").Append(GetCTypeFromAnimFrameType(typeOfAnimFrame)).Append(" ").Append(animFrames.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                animFrames[i].Accept(new AnimFrameExporterVisitor(sb, exportTracker, previousDefinitions));
                if (++i >= animFrames.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            sb.AppendLine("};");
            if (previousDefinitions != null)
            {
                return previousDefinitions.Append(sb.ToString()).ToString();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Exports an <see cref="AnimHead"/>.
        /// </summary>
        /// <param name="action"><see cref="AnimHead"/> to export.</param>
        /// <returns>A string containing the C code representing the <see cref="AnimHead"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// A named item referenced by <paramref name="action"/> has a name that isn't a valid C identifier.
        /// </exception>
        public static string Export(AnimHead action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ValidateName(action.Name);
            if (action.Model != null)
            {
                ValidateName(action.Model.Name);
            }

            if (action.Motion != null)
            {
                ValidateName(action.Motion.Name);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("struct AnimHead " + action.Name + " = { ");
            if (action.Model == null)
            {
                sb.Append("NULL, ");
            }
            else
            {
                sb.Append("&").Append(action.Model.Name).Append(", ");
            }

            if (action.Motion == null)
            {
                sb.Append("NULL");
            }
            else
            {
                sb.Append("&").Append(action.Motion.Name);
            }

            return sb.AppendLine(" };").ToString();
        }

        /// <summary>
        /// Exports a collection of pointers to <see cref="AnimHead"/> items.
        /// </summary>
        /// <param name="actionList">Collection of <see cref="AnimHead"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="AnimHead"/> pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="actionList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="actionList"/> is empty.</exception>
        public static string Export(NamedCollection<AnimHead> actionList)
        {
            return ExportPointerCollection(actionList, "AnimHead", "actionList");
        }

        /// <summary>
        /// Exports an <see cref="AnimHead2"/>.
        /// </summary>
        /// <param name="motion"><see cref="AnimHead2"/> to export.</param>
        /// <returns>A string containing the C code representing the <see cref="AnimHead2"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="motion"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// A named item referenced by <paramref name="motion"/> has a name that isn't a valid C identifier.
        /// </exception>
        public static string Export(AnimHead2 motion)
        {
            if (motion == null)
            {
                throw new ArgumentNullException("motion");
            }

            ValidateName(motion.Name);
            if (motion.FrameData != null)
            {
                ValidateName(motion.FrameData.Name);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("struct AnimHead2 " + motion.Name + " =").AppendLine("{");
            if (motion.FrameData == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    ").Append(motion.FrameData.Name).AppendLine(",");
            }

            return sb.Append("    ").Append(FormatInt32(motion.FrameCount)).AppendLine(",")
                .Append("    ").Append(FormatAnimHead2Flags(motion.Flags)).AppendLine(",")
                .Append("    ").AppendLine(FormatUInt16Hex(motion.Unknown0A))
                .AppendLine("};")
                .ToString();
        }

        /// <summary>
        /// Exports a collection of pointers to <see cref="AnimHead2"/> items.
        /// </summary>
        /// <param name="motionList">Collection of <see cref="AnimHead2"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="AnimHead2"/> pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="motionList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="motionList"/> is empty.</exception>
        public static string Export(NamedCollection<AnimHead2> motionList)
        {
            return ExportPointerCollection(motionList, "AnimHead2", "motionList");
        }

        /// <summary>
        /// Exports an <see cref="ATTACH"/>.
        /// </summary>
        /// <param name="attach"><see cref="ATTACH"/> to export.</param>
        /// <returns>A string containing the C code representing the <see cref="ATTACH"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attach"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// The number of vertices is not equal to the number of normals or a named item referenced by <paramref name="attach"/>
        /// has a name that isn't a valid C identifier.
        /// </exception>
        public static string Export(ATTACH attach)
        {
            return Export(attach, null);
        }

        /// <summary>
        /// Exports an <see cref="ATTACH"/>.
        /// </summary>
        /// <param name="attach"><see cref="ATTACH"/> to export.</param>
        /// <param name="overriddenMaterialCount">A value that will override the actual number of materials.</param>
        /// <returns>A string containing the C code representing the <see cref="ATTACH"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attach"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// The number of vertices is not equal to the number of normals or a named item referenced by <paramref name="attach"/>
        /// has a name that isn't a valid C identifier.
        /// </exception>
        public static string Export(ATTACH attach, int overriddenMaterialCount)
        {
            return Export(attach, new int?(overriddenMaterialCount));
        }

        /// <summary>
        /// Exports a collection of pointers to <see cref="ATTACH"/> items.
        /// </summary>
        /// <param name="modelList">Collection of <see cref="ATTACH"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="ATTACH"/> pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="modelList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="modelList"/> is empty.</exception>
        public static string Export(NamedCollection<ATTACH> modelList)
        {
            return ExportPointerCollection(modelList, "ATTACH", "modelList");
        }

        /// <summary>
        /// Exports a collection of <see cref="MATERIAL"/> items.
        /// </summary>
        /// <param name="materials">Collection of <see cref="MATERIAL"/> items to export.</param>
        /// <param name="linkage">Specifies either extern or static linkage for the array.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="MATERIAL"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="materials"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="materials"/> is empty or a named item referenced by <paramref name="materials"/> has a name that
        /// isn't a valid C identifier.
        /// </exception>
        public static string Export(NamedCollection<MATERIAL> materials, Linkage linkage)
        {
            if (materials == null)
            {
                throw new ArgumentNullException("materials");
            }

            if (materials.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "materials");
            }

            ValidateName(materials.Name);

            StringBuilder sb = new StringBuilder();
            if (linkage == Linkage.StaticLinkage)
            {
                sb.Append("static ");
            }

            sb.Append("struct MATERIAL ").Append(materials.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                MATERIAL material = materials[i];
                sb.AppendLine("    {")
                    .Append("        ").Append(FormatUInt32Hex(material.DiffuseColor)).AppendLine(",")
                    .Append("        ").Append(FormatUInt32Hex(material.SpecularColor)).AppendLine(",")
                    .Append("        ").Append(FormatSingle(material.Unknown08)).AppendLine(",")
                    .Append("        ").Append(FormatUInt32Hex(material.TextureId)).AppendLine(",")
                    .Append("        ").Append(FormatUInt16Hex(material.Unknown10)).AppendLine(",")
                    .Append("        ").Append(FormatUInt8Hex(material.Flags)).AppendLine(",")
                    .Append("        ").AppendLine(FormatUInt8Hex(material.Unknown13));

                if (++i >= materials.Count)
                {
                    sb.AppendLine("    }");
                    break;
                }

                sb.AppendLine("    },");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of pointers to <see cref="MATERIAL"/> arrays.
        /// </summary>
        /// <param name="materialList">Collection of <see cref="MATERIAL"/> arrays to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="MATERIAL"/> pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="materialList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="materialList"/> is empty.</exception>
        public static string Export(NamedCollection<NamedCollection<MATERIAL>> materialList)
        {
            return ExportPointerCollection(materialList, "MATERIAL", "materialList");
        }

        /// <summary>
        /// Exports a collection of <see cref="MESH"/> items.
        /// </summary>
        /// <param name="meshes">Collection of <see cref="MESH"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="MESH"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="meshes"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="meshes"/> is empty or a named item referenced by <paramref name="meshes"/> has a name that isn't
        /// a valid C identifier.
        /// </exception>
        public static string Export(NamedCollection<MESH> meshes)
        {
            if (meshes == null)
            {
                throw new ArgumentNullException("meshes");
            }

            if (meshes.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "meshes");
            }

            ValidateName(meshes.Name);

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct MESH ").Append(meshes.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                MESH mesh = meshes[i];
                if (mesh.Polys != null)
                {
                    ValidateName(mesh.Polys.Name);
                }

                if (mesh.PolyNormals != null)
                {
                    ValidateName(mesh.PolyNormals.Name);
                }

                if (mesh.VertexColors != null)
                {
                    ValidateName(mesh.VertexColors.Name);
                }

                if (mesh.UV != null)
                {
                    ValidateName(mesh.UV.Name);
                }

                sb.Append("    { ").Append((mesh.MaterialIdAndPolyType & 0x3FFF).ToString(CultureInfo.InvariantCulture)).Append(" | ");
                switch (mesh.MaterialIdAndPolyType & 0xC000)
                {
                    case 0:
                        sb.Append("MeshPolyType_Triangles");
                        break;
                    case 0x4000:
                        sb.Append("MeshPolyType_Quads");
                        break;
                    case 0x8000:
                        sb.Append("MeshPolyType_StripsA");
                        break;
                    case 0xC000:
                    default:
                        sb.Append("MeshPolyType_StripsB");
                        break;
                }

                sb.Append(", ");

                if (mesh.Polys.Count < 0 || mesh.Polys.Count > ushort.MaxValue)
                {
                    throw new ArgumentException(Properties.Resources.PolyCountInMeshTooHigh, "meshes");
                }

                sb.Append(FormatUInt16((ushort)mesh.Polys.Count))
                    .Append(", ")
                    .Append(mesh.Polys.Name)
                    .Append(", ")
                    .Append(FormatInt32(mesh.PolyAttributes))
                    .Append(", ");
                if (mesh.PolyNormals == null)
                {
                    sb.Append("NULL, ");
                }
                else
                {
                    sb.Append(mesh.PolyNormals.Name).Append(", ");
                }

                if (mesh.VertexColors == null)
                {
                    sb.Append("NULL, ");
                }
                else
                {
                    sb.Append(mesh.VertexColors.Name).Append(", ");
                }

                if (mesh.UV == null)
                {
                    sb.Append("NULL, ");
                }
                else
                {
                    sb.Append(mesh.UV.Name).Append(", ");
                }

                sb.Append(FormatInt32(mesh.Null)).Append(" }");
                if (++i >= meshes.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports an <see cref="OBJECT"/>.
        /// </summary>
        /// <param name="obj"><see cref="OBJECT"/> to export.</param>
        /// <returns>A string containing the C code representing the <see cref="OBJECT"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// A named item referenced by <paramref name="obj"/> has a name that isn't a valid C identifier.
        /// </exception>
        public static string Export(OBJECT obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            ValidateName(obj.Name);
            if (obj.Attach != null)
            {
                ValidateName(obj.Attach.Name);
            }

            if (obj.Child != null)
            {
                ValidateName(obj.Child.Name);
            }

            if (obj.Sibling != null)
            {
                ValidateName(obj.Sibling.Name);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("struct OBJECT ").Append(obj.Name).AppendLine(" =")
                .AppendLine("{")
                .Append("    ").Append(FormatObjectFlags(obj.Flags)).Append(",");
            if (obj.Attach == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    &").Append(obj.Attach.Name).AppendLine(",");
            }

            sb.Append("    ").Append(FormatVector3(obj.Position)).AppendLine(",")
                .Append("    ").Append(FormatRotation3(obj.Rotation)).AppendLine(",")
                .Append("    ").Append(FormatVector3(obj.Scale)).AppendLine(",");
            if (obj.Child == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    &").Append(obj.Child.Name).AppendLine(",");
            }

            if (obj.Sibling == null)
            {
                sb.AppendLine("    NULL");
            }
            else
            {
                sb.Append("    &").AppendLine(obj.Sibling.Name);
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of pointers to <see cref="OBJECT"/> items.
        /// </summary>
        /// <param name="objectList">Collection of <see cref="OBJECT"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="OBJECT"/> pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="objectList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="objectList"/> is empty.</exception>
        public static string Export(NamedCollection<OBJECT> objectList)
        {
            return ExportPointerCollection(objectList, "OBJECT", "objectList");
        }

        /// <summary>
        /// Exports a collection of <see cref="Poly"/> items and all the data structures they reference.
        /// </summary>
        /// <param name="polys">Collection of <see cref="Poly"/> items to export.</param>
        /// <returns>
        /// A string containing the C code representing the collection of <see cref="Poly"/> items as well as the data they
        /// reference.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="polys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="polys"/> is an empty collection, contains a <c>null</c> item, is not homogeneous (that is,
        /// it contains items of different concrete types) or contains a strip that contains more than 32767 vertices.
        /// </exception>
        [CLSCompliant(false)]
        public static string Export(NamedCollection<Poly> polys)
        {
            if (polys == null)
            {
                throw new ArgumentNullException("polys");
            }

            if (polys.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "polys");
            }

            if (polys[0] == null)
            {
                throw new ArgumentException(Properties.Resources.PolyCollectionContainsNull, "polys");
            }

            Type typeOfPoly = polys[0].GetType();
            for (int i = 1; i < polys.Count; i++)
            {
                if (polys[i] == null)
                {
                    throw new ArgumentException(Properties.Resources.PolyCollectionContainsNull, "polys");
                }

                if (polys[i].GetType() != typeOfPoly)
                {
                    throw new ArgumentException(Properties.Resources.PolyCollectionIsNotHomogeneous, "polys");
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static short unsigned int ").Append(polys.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                polys[i].Accept(new PolyExporterVisitor(sb));
                if (++i >= polys.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of <see cref="PolyNormal"/> items.
        /// </summary>
        /// <param name="polyNormals">Collection of <see cref="PolyNormal"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="PolyNormal"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="polyNormals"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="polyNormals"/> is empty.</exception>
        public static string Export(NamedCollection<PolyNormal> polyNormals)
        {
            if (polyNormals == null)
            {
                throw new ArgumentNullException("polyNormals");
            }

            if (polyNormals.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "polyNormals");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct PolyNormal ").Append(polyNormals.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    { ")
                    .Append(FormatSingle(polyNormals[i].Unknown00))
                    .Append(", ")
                    .Append(FormatSingle(polyNormals[i].Unknown04))
                    .Append(" }");
                if (++i >= polyNormals.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of <see cref="Rotation3AnimData"/> items.
        /// </summary>
        /// <param name="rotation3AnimDataCollection">Collection of <see cref="Rotation3AnimData"/> items to export.</param>
        /// <returns>
        /// A string containing the C code representing the collection of <see cref="Rotation3AnimData"/> items.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rotation3AnimDataCollection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="rotation3AnimDataCollection"/> is empty.</exception>
        public static string Export(NamedCollection<Rotation3AnimData> rotation3AnimDataCollection)
        {
            if (rotation3AnimDataCollection == null)
            {
                throw new ArgumentNullException("rotation3AnimDataCollection");
            }

            if (rotation3AnimDataCollection.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "rotation3AnimDataCollection");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct Rotation3AnimData ").Append(rotation3AnimDataCollection.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    { ")
                    .Append(FormatInt32(rotation3AnimDataCollection[i].FrameNumber))
                    .Append(", ")
                    .Append(FormatRotation3(rotation3AnimDataCollection[i].Rotation))
                    .Append(" }");
                if (++i >= rotation3AnimDataCollection.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of <see cref="UInt32"/> items.
        /// </summary>
        /// <param name="uint32Collection">Collection of <see cref="UInt32"/> items.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="UInt32"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uint32Collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="uint32Collection"/> is empty.</exception>
        [CLSCompliant(false)]
        public static string Export(NamedCollection<uint> uint32Collection)
        {
            if (uint32Collection == null)
            {
                throw new ArgumentNullException("uint32Collection");
            }

            if (uint32Collection.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "uint32Collection");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static unsigned int ").Append(uint32Collection.Name).AppendLine("[] =");
            sb.AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    ").Append(FormatUInt32Hex(uint32Collection[i]));
                if (++i >= uint32Collection.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of <see cref="UV"/> items.
        /// </summary>
        /// <param name="uv">Collection of <see cref="UV"/> items to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="UV"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uv"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="uv"/> is empty.</exception>
        public static string Export(NamedCollection<UV> uv)
        {
            if (uv == null)
            {
                throw new ArgumentNullException("uv");
            }

            if (uv.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "uv");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct UV ").Append(uv.Name).AppendLine("[] =");
            sb.AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    { ")
                    .Append(FormatInt16(uv[i].U))
                    .Append(", ")
                    .Append(FormatInt16(uv[i].V))
                    .Append(" }");
                if (++i >= uv.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of <see cref="Vector3"/> items.
        /// </summary>
        /// <param name="vectors">Collection of <see cref="Vector3"/> items to export.</param>
        /// <param name="linkage">Specifies either extern or static linkage for the array.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="Vector3"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        public static string Export(NamedCollection<Vector3> vectors, Linkage linkage)
        {
            if (vectors == null)
            {
                throw new ArgumentNullException("vectors");
            }

            if (vectors.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "vectors");
            }

            StringBuilder sb = new StringBuilder();
            if (linkage == Linkage.StaticLinkage)
            {
                sb.Append("static ");
            }

            sb.Append("struct Vector3 ").Append(vectors.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    ").Append(FormatVector3(vectors[i]));

                if (++i >= vectors.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of pointers to <see cref="Vector3"/> arrays.
        /// </summary>
        /// <param name="pointList">Collection of <see cref="Vector3"/> arrays to export.</param>
        /// <returns>A string containing the C code representing the collection of <see cref="Vector3"/> pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pointList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="pointList"/> is empty.</exception>
        public static string Export(NamedCollection<NamedCollection<Vector3>> pointList)
        {
            return ExportPointerCollection(pointList, "Vector3", "pointList");
        }

        /// <summary>
        /// Exports a collection of <see cref="Vector3AnimData"/> items.
        /// </summary>
        /// <param name="vector3AnimDataCollection">Collection of <see cref="Vector3AnimData"/> items to export.</param>
        /// <returns>
        /// A string containing the C code representing the collection of <see cref="Vector3AnimData"/> items.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="vector3AnimDataCollection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="vector3AnimDataCollection"/> is empty.</exception>
        public static string Export(NamedCollection<Vector3AnimData> vector3AnimDataCollection)
        {
            if (vector3AnimDataCollection == null)
            {
                throw new ArgumentNullException("vector3AnimDataCollection");
            }

            if (vector3AnimDataCollection.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "vector3AnimDataCollection");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct Vector3AnimData ").Append(vector3AnimDataCollection.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    { ")
                    .Append(FormatInt32(vector3AnimDataCollection[i].FrameNumber))
                    .Append(", ")
                    .Append(FormatVector3(vector3AnimDataCollection[i].Vector))
                    .Append(" }");
                if (++i >= vector3AnimDataCollection.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of <see cref="Vector3ArrayAnimData"/> items.
        /// </summary>
        /// <param name="vector3ArrayAnimDataCollection">
        /// Collection of <see cref="Vector3ArrayAnimData"/> items to export.
        /// </param>
        /// <param name="exportTracker">
        /// <see cref="ExportTracker"/> to use to track exported <see cref="Vector3"/> collections. Specify a valid reference
        /// to export all dependent data recursively, or <c>null</c> to export only the collection.
        /// </param>
        /// <returns>
        /// A string containing the C code representing the collection of <see cref="Vector3ArrayAnimData"/> items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="vector3ArrayAnimDataCollection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="vector3ArrayAnimDataCollection"/> is empty.</exception>
        public static string Export(NamedCollection<Vector3ArrayAnimData> vector3ArrayAnimDataCollection, ExportTracker exportTracker)
        {
            if (vector3ArrayAnimDataCollection == null)
            {
                throw new ArgumentNullException("vector3ArrayAnimDataCollection");
            }

            if (vector3ArrayAnimDataCollection.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, "vector3ArrayAnimDataCollection");
            }

            StringBuilder previousDefinitions = null;
            if (exportTracker != null)
            {
                previousDefinitions = new StringBuilder();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("static struct Vector3ArrayAnimData ").Append(vector3ArrayAnimDataCollection.Name).AppendLine("[] =")
                .AppendLine("{");
            for (int i = 0;;)
            {
                sb.Append("    { ")
                    .Append(FormatInt32(vector3ArrayAnimDataCollection[i].FrameNumber))
                    .Append(", ");
                NamedCollection<Vector3> vectors = vector3ArrayAnimDataCollection[i].Vectors;
                if (vectors == null)
                {
                    sb.Append("NULL }");
                }
                else
                {
                    if (exportTracker != null && exportTracker.Add(vectors))
                    {
                        previousDefinitions.AppendLine(Export(vectors, Linkage.StaticLinkage));
                    }

                    sb.Append(vector3ArrayAnimDataCollection[i].Vectors.Name).Append(" }");
                }

                if (++i >= vector3ArrayAnimDataCollection.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            sb.AppendLine("};");
            if (previousDefinitions != null)
            {
                return previousDefinitions.Append(sb.ToString()).ToString();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Exports an <see cref="ATTACH"/>.
        /// </summary>
        /// <param name="attach"><see cref="ATTACH"/> to export.</param>
        /// <param name="overriddenMaterialCount">
        /// A value that will override the actual number of materials. Specify <c>null</c> to use the real material count from
        /// the <see cref="ATTACH.Materials"/> collection.
        /// </param>
        /// <returns>A string containing the C code representing the <see cref="ATTACH"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attach"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// The number of vertices is not equal to the number of normals or a named item referenced by <paramref name="attach"/>
        /// has a name that isn't a valid C identifier.
        /// </exception>
        private static string Export(ATTACH attach, int? overriddenMaterialCount)
        {
            if (attach == null)
            {
                throw new ArgumentNullException("attach");
            }

            if (attach.Vertices != null && attach.Normals != null && attach.Vertices.Count != attach.Normals.Count)
            {
                throw new ArgumentException("The number of vertices must be equal to the number of normals.", "attach");
            }

            ValidateName(attach.Name);
            if (attach.Vertices != null)
            {
                ValidateName(attach.Vertices.Name);
            }

            if (attach.Normals != null)
            {
                ValidateName(attach.Normals.Name);
            }

            if (attach.Meshes != null)
            {
                ValidateName(attach.Meshes.Name);
            }

            if (attach.Materials != null)
            {
                ValidateName(attach.Materials.Name);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("struct ATTACH ").Append(attach.Name).AppendLine(" =")
                .AppendLine("{");
            if (attach.Vertices == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    ").Append(attach.Vertices.Name).AppendLine(",");
            }

            if (attach.Normals == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    ").Append(attach.Normals.Name).AppendLine(",");
            }

            if (attach.Vertices == null)
            {
                if (attach.Normals == null)
                {
                    sb.AppendLine("    0,");
                }
                else
                {
                    sb.Append("    ARRAYSIZE(").Append(attach.Normals.Name).AppendLine("),");
                }
            }
            else
            {
                sb.Append("    ARRAYSIZE(").Append(attach.Vertices.Name).AppendLine("),");
            }

            if (attach.Meshes == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    ").Append(attach.Meshes.Name).AppendLine(",");
            }

            if (attach.Materials == null)
            {
                sb.AppendLine("    NULL,");
            }
            else
            {
                sb.Append("    ").Append(attach.Materials.Name).AppendLine(",");
            }

            if (attach.Meshes == null)
            {
                sb.AppendLine("    0,");
            }
            else
            {
                sb.Append("    ARRAYSIZE(").Append(attach.Meshes.Name).AppendLine("),");
            }

            if (overriddenMaterialCount.HasValue)
            {
                sb.Append("    ").Append(overriddenMaterialCount.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)).AppendLine(",");
            }
            else if (attach.Materials == null)
            {
                sb.AppendLine("    0,");
            }
            else
            {
                sb.Append("    ARRAYSIZE(").Append(attach.Materials.Name).AppendLine("),");
            }

            return sb.Append("    ").Append(FormatVector3(attach.Center)).AppendLine(",")
                .Append("    ").Append(FormatSingle(attach.Radius)).AppendLine(",")
                .Append("    ").AppendLine(FormatInt32(attach.Null))
                .AppendLine("};").ToString();
        }

        /// <summary>
        /// Exports a collection of pointers.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the items in the collection. The type must implement the <see cref="INamed"/> interface.
        /// </typeparam>
        /// <param name="list">Collection of named items to export.</param>
        /// <param name="structName">Name of the C struct corresponding to the type of the items in the collection.</param>
        /// <param name="paramName">Name of the <paramref name="list"/> parameter in the caller method.</param>
        /// <returns>A string containing the C code representing the collection of pointers.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="list"/> is empty or a named item referenced by <paramref name="list"/> has a name that isn't a
        /// valid C identifier.
        /// </exception>
        private static string ExportPointerCollection<T>(NamedCollection<T> list, string structName, string paramName)
            where T : INamed
        {
            if (list == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (list.Count == 0)
            {
                throw new ArgumentException(Properties.Resources.EmptyCollection, paramName);
            }

            ValidateName(list.Name);

            StringBuilder sb = new StringBuilder();
            sb.Append("__declspec(dllexport) struct ").Append(structName).Append(" *").Append(list.Name).AppendLine("[] =");
            sb.AppendLine("{");
            for (int i = 0;;)
            {
                if (list[i] != null)
                {
                    ValidateName(list[i].Name);
                    sb.Append("    ");

                    // NamedCollection<>s are exported as arrays, so we don't need to use the & operator to get a pointer to
                    // them. The other structures are exported as standalone fields.
                    if (!(typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition().Equals(typeof(NamedCollection<>))))
                    {
                        sb.Append("&");
                    }

                    sb.Append(list[i].Name);
                }
                else
                {
                    sb.Append("    NULL");
                }

                if (++i >= list.Count)
                {
                    sb.AppendLine();
                    break;
                }

                sb.AppendLine(",");
            }

            return sb.AppendLine("};").ToString();
        }

        /// <summary>
        /// Formats the specified flags using named enumeration values.
        /// </summary>
        /// <param name="flags">Flags to format.</param>
        /// <returns>A string containing the flags represented as named enumeration values.</returns>
        private static string FormatAnimHead2Flags(ushort flags)
        {
            if (flags == 0)
            {
                return "AnimHead2Flags_None";
            }

            List<string> animHead2Flags = new List<string>();

            if ((flags & 0x1) != 0)
            {
                animHead2Flags.Add("AnimHead2Flags_HasPosition");
            }

            if ((flags & 0x2) != 0)
            {
                animHead2Flags.Add("AnimHead2Flags_HasRotation");
            }

            if ((flags & 0x4) != 0)
            {
                animHead2Flags.Add("AnimHead2Flags_HasScale");
            }

            if ((flags & 0x8) != 0)
            {
                animHead2Flags.Add("AnimHead2Flags_08");
            }

            if ((flags & 0x10) != 0)
            {
                animHead2Flags.Add("AnimHead2Flags_HasVertex");
            }

            if ((flags & 0x20) != 0)
            {
                animHead2Flags.Add("AnimHead2Flags_HasNormal");
            }

            if ((flags & 0xFFFFFFC0) != 0)
            {
                animHead2Flags.Add("0x" + (flags & 0xFFFFFFC0).ToString("X8", CultureInfo.InvariantCulture));
            }

            return string.Join(" | ", animHead2Flags.ToArray());
        }

        /// <summary>
        /// Formats the specified signed 16-bit integer as a C integer literal.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatInt16(short value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified signed 32-bit integer as a C integer literal.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatInt32(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified flags using named enumeration values.
        /// </summary>
        /// <param name="flags">Flags to format.</param>
        /// <returns>A string containing the flags represented as named enumeration values.</returns>
        private static string FormatObjectFlags(uint flags)
        {
            if (flags == 0)
            {
                return "ObjectFlags_None";
            }

            List<string> objectFlags = new List<string>();

            if ((flags & 0x1) != 0)
            {
                objectFlags.Add("ObjectFlags_NoTranslate");
            }

            if ((flags & 0x2) != 0)
            {
                objectFlags.Add("ObjectFlags_NoRotate");
            }

            if ((flags & 0x4) != 0)
            {
                objectFlags.Add("ObjectFlags_NoScale");
            }

            if ((flags & 0x8) != 0)
            {
                objectFlags.Add("ObjectFlags_NoDraw");
            }

            if ((flags & 0x10) != 0)
            {
                objectFlags.Add("ObjectFlags_NoChildren");
            }

            if ((flags & 0x20) != 0)
            {
                objectFlags.Add("ObjectFlags_UseZYXRotation");
            }

            if ((flags & 0x40) != 0)
            {
                objectFlags.Add("ObjectFlags_NoAnimate");
            }

            if ((flags & 0x80) != 0)
            {
                objectFlags.Add("ObjectFlags_80");
            }

            if ((flags & 0xFFFFFF00) != 0)
            {
                objectFlags.Add("0x" + (flags & 0xFFFFFF00).ToString("X8", CultureInfo.InvariantCulture));
            }

            return string.Join(" | ", objectFlags.ToArray());
        }

        /// <summary>
        /// Formats the specified <see cref="Rotation3"/> structure as a C structure initializer list.
        /// </summary>
        /// <param name="value"><see cref="Rotation3"/> structure to format.</param>
        /// <returns>
        /// A string containing the <see cref="Rotation3"/> structure formatted as a C structure initializer list.
        /// </returns>
        private static string FormatRotation3(Rotation3 value)
        {
            return "{ " + FormatInt32(value.X) + ", " + FormatInt32(value.Y) + ", " + FormatInt32(value.Z) + " }";
        }

        /// <summary>
        /// Formats the specified single-precision floating-point number as a C single-precision floating-point literal.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatSingle(float value)
        {
            string result = value.ToString("R", CultureInfo.InvariantCulture);

            // If the result doesn't contain the decimal separator, add it, unless there is an E
            // This is necessary, otherwise the f prefix will trigger a syntax error
            if (!result.Contains(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
                && !result.Contains("E"))
            {
                result += CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator
                    + CultureInfo.InvariantCulture.NumberFormat.NativeDigits[0];
            }

            return result + "f";
        }

        /// <summary>
        /// Formats the specified unsigned 16-bit integer as a C integer literal.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatUInt16(ushort value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified unsigned 16-bit integer as a C integer literal using hexadecimal syntax.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatUInt16Hex(ushort value)
        {
            return "0x" + value.ToString("X4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified unsigned 32-bit integer as a C integer literal using hexadecimal syntax.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatUInt32Hex(uint value)
        {
            return "0x" + value.ToString("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified unsigned 8-bit integer as a C integer literal using hexadecimal syntax.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>A string containing the value represented as a literal.</returns>
        private static string FormatUInt8Hex(byte value)
        {
            return "0x" + value.ToString("X2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified <see cref="Rotation3"/> structure as a C structure initializer list.
        /// </summary>
        /// <param name="value"><see cref="Rotation3"/> structure to format.</param>
        /// <returns>
        /// A string containing the <see cref="Rotation3"/> structure formatted as a C structure initializer list.
        /// </returns>
        private static string FormatVector3(Vector3 value)
        {
            return "{ " + FormatSingle(value.X) + ", " + FormatSingle(value.Y) + ", " + FormatSingle(value.Z) + " }";
        }

        /// <summary>
        /// Returns the name of the C struct corresponding to the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="typeOfAnimFrame">
        /// Instance of <see cref="Type"/> identifying the <see cref="AnimFrame_PosRot"/>, <see cref="AnimFrame_PosRotScale"/>
        /// or <see cref="AnimFrame_VertNrm"/> class.
        /// </param>
        /// <returns>A string containing the name of the C struct corresponding to the <see cref="Type"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="typeOfAnimFrame"/> does not identify the <see cref="AnimFrame_PosRot"/>,
        /// <see cref="AnimFrame_PosRotScale"/> or <see cref="AnimFrame_VertNrm"/> class.
        /// </exception>
        private static string GetCTypeFromAnimFrameType(Type typeOfAnimFrame)
        {
            if (typeOfAnimFrame == typeof(AnimFrame_PosRot))
            {
                return "AnimFrame_PosRot";
            }

            if (typeOfAnimFrame == typeof(AnimFrame_PosRotScale))
            {
                return "AnimFrame_PosRotScale";
            }

            if (typeOfAnimFrame == typeof(AnimFrame_VertNrm))
            {
                return "AnimFrame_VertNrm";
            }

            throw new ArgumentException(Properties.Resources.UnknownTypeOfAnimFrame, "typeOfAnimFrame");
        }

        /// <summary>
        /// Validates that the specified name is a valid C identifier.
        /// </summary>
        /// <param name="name">Name to validate.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is not a valid C identifier.</exception>
        private static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                goto invalidName;
            }

            char ch = name[0];
            if (!((ch >= 'A' && ch <= 'Z') || ch == '_' || (ch >= 'a' && ch <= 'z')))
            {
                goto invalidName;
            }

            for (int i = 1; i < name.Length; i++)
            {
                ch = name[i];
                if (!((ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z') || ch == '_' || (ch >= 'a' && ch <= 'z')))
                {
                    goto invalidName;
                }
            }

            return;

        invalidName:
            throw new ArgumentException("name", string.Format(CultureInfo.InvariantCulture, Properties.Resources.InvalidName, name));
        }

        /// <summary>
        /// Exports <see cref="AnimFrame"/> objects according to their concrete type.
        /// </summary>
        private class AnimFrameExporterVisitor : IAnimFrameVisitor
        {
            /// <summary>
            /// <see cref="StringBuilder"/> that contains the exported <see cref="AnimHead2"/>.
            /// </summary>
            private StringBuilder sb;

            /// <summary>
            /// <see cref="ExportTracker"/> to use to track exported <see cref="Vector3"/> collections.
            /// </summary>
            private ExportTracker exportTracker;

            /// <summary>
            /// <see cref="StringBuilder"/> that contains the C code for the dependencies.
            /// </summary>
            private StringBuilder previousDefinitions;

            /// <summary>
            /// Initializes a new instance of the <see cref="AnimFrameExporterVisitor"/> class.
            /// </summary>
            /// <param name="sb"><see cref="StringBuilder"/> that contains the exported <see cref="AnimHead2"/>.</param>
            /// <param name="exportTracker">
            /// <see cref="ExportTracker"/> to use to track exported <see cref="Vector3"/> collections.
            /// </param>
            /// <param name="previousDefinitions">
            /// <see cref="StringBuilder"/> that contains the C code for the dependencies. Specify a valid reference to export
            /// the dependencies of the <see cref="AnimFrame"/> items, or specify <c>null</c> to export only the
            /// <see cref="AnimFrame"/> itself.
            /// </param>
            public AnimFrameExporterVisitor(StringBuilder sb, ExportTracker exportTracker, StringBuilder previousDefinitions)
            {
                this.sb = sb;
                this.exportTracker = exportTracker;
                this.previousDefinitions = previousDefinitions;
            }

            /// <summary>
            /// Exports an <see cref="AnimFrame_PosRot"/> and its dependencies.
            /// </summary>
            /// <param name="afpr"><see cref="AnimFrame_PosRot"/> to export.</param>
            public void VisitAnimFrame_PosRot(AnimFrame_PosRot afpr)
            {
                string positionsCount, rotationsCount;

                this.sb.Append("    { ");
                if (afpr.Positions == null)
                {
                    this.sb.Append("NULL, ");
                    positionsCount = "0, ";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afpr.Positions));
                    }

                    this.sb.Append(afpr.Positions.Name).Append(", ");
                    positionsCount = "ARRAYSIZE(" + afpr.Positions.Name + "), ";
                }

                if (afpr.Rotations == null)
                {
                    this.sb.Append("NULL, ");
                    rotationsCount = "0";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afpr.Rotations));
                    }

                    this.sb.Append(afpr.Rotations.Name).Append(", ");
                    rotationsCount = "ARRAYSIZE(" + afpr.Rotations.Name + ")";
                }

                this.sb.Append(positionsCount).Append(rotationsCount).Append(" }");
            }

            /// <summary>
            /// Exports an <see cref="AnimFrame_PosRotScale"/> and its dependencies.
            /// </summary>
            /// <param name="afprs"><see cref="AnimFrame_PosRotScale"/> to export.</param>
            public void VisitAnimFrame_PosRotScale(AnimFrame_PosRotScale afprs)
            {
                string positionsCount, rotationsCount, scalesCount;

                this.sb.Append("    { ");
                if (afprs.Positions == null)
                {
                    this.sb.Append("NULL, ");
                    positionsCount = "0, ";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afprs.Positions));
                    }

                    this.sb.Append(afprs.Positions.Name).Append(", ");
                    positionsCount = "ARRAYSIZE(" + afprs.Positions.Name + "), ";
                }

                if (afprs.Rotations == null)
                {
                    this.sb.Append("NULL, ");
                    rotationsCount = "0, ";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afprs.Rotations));
                    }

                    this.sb.Append(afprs.Rotations.Name).Append(", ");
                    rotationsCount = "ARRAYSIZE(" + afprs.Rotations.Name + "), ";
                }

                if (afprs.Scales == null)
                {
                    this.sb.Append("NULL, ");
                    scalesCount = "0";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afprs.Scales));
                    }

                    this.sb.Append(afprs.Scales.Name).Append(", ");
                    scalesCount = "ARRAYSIZE(" + afprs.Scales.Name + ")";
                }

                this.sb.Append(positionsCount).Append(rotationsCount).Append(scalesCount).Append(" }");
            }

            /// <summary>
            /// Exports an <see cref="AnimFrame_VertNrm"/> and its dependencies.
            /// </summary>
            /// <param name="afvn"><see cref="AnimFrame_VertNrm"/> to export.</param>
            public void VisitAnimFrame_VertNrm(AnimFrame_VertNrm afvn)
            {
                string verticesCount, normalsCount;

                this.sb.Append("    { ");
                if (afvn.Vertices == null)
                {
                    this.sb.Append("NULL, ");
                    verticesCount = "0, ";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afvn.Vertices, this.exportTracker));
                    }

                    this.sb.Append(afvn.Vertices.Name).Append(", ");
                    verticesCount = "ARRAYSIZE(" + afvn.Vertices.Name + "), ";
                }

                if (afvn.Normals == null)
                {
                    this.sb.Append("NULL, ");
                    normalsCount = "0";
                }
                else
                {
                    if (this.previousDefinitions != null)
                    {
                        this.previousDefinitions.AppendLine(Export(afvn.Normals, this.exportTracker));
                    }

                    this.sb.Append(afvn.Normals.Name).Append(", ");
                    normalsCount = "ARRAYSIZE(" + afvn.Normals.Name + ")";
                }

                this.sb.Append(verticesCount).Append(normalsCount).Append(" }");
            }
        }

        /// <summary>
        /// Exports <see cref="Poly"/> objects according to their concrete type.
        /// </summary>
        private class PolyExporterVisitor : IPolyVisitor
        {
            /// <summary>
            /// <see cref="StringBuilder"/> that contains the exported <see cref="Poly"/> collection.
            /// </summary>
            private StringBuilder sb;

            /// <summary>
            /// Initializes a new instance of the <see cref="PolyExporterVisitor"/> class.
            /// </summary>
            /// <param name="sb"><see cref="StringBuilder"/> that contains the exported <see cref="Poly"/> collection.</param>
            public PolyExporterVisitor(StringBuilder sb)
            {
                this.sb = sb;
            }

            /// <summary>
            /// Exports a <see cref="Triangle"/>.
            /// </summary>
            /// <param name="triangle"><see cref="Triangle"/> to export.</param>
            public void VisitTriangle(Triangle triangle)
            {
                this.sb.Append("    ").Append(FormatUInt16(triangle.Vertex1))
                    .Append(", ").Append(FormatUInt16(triangle.Vertex2))
                    .Append(", ").Append(FormatUInt16(triangle.Vertex3));
            }

            /// <summary>
            /// Exports a <see cref="Quad"/>.
            /// </summary>
            /// <param name="quad"><see cref="Quad"/> to export.</param>
            public void VisitQuad(Quad quad)
            {
                this.sb.Append("    ").Append(FormatUInt16(quad.Vertex1))
                    .Append(", ").Append(FormatUInt16(quad.Vertex2))
                    .Append(", ").Append(FormatUInt16(quad.Vertex3))
                    .Append(", ").Append(FormatUInt16(quad.Vertex4));
            }

            /// <summary>
            /// Exports a <see cref="Strip"/>.
            /// </summary>
            /// <param name="strip"><see cref="Strip"/> to export.</param>
            /// <exception cref="ArgumentException">The strip contains more than 32767 vertices.</exception>
            public void VisitStrip(Strip strip)
            {
                if (strip.NumberOfVertices < 0 || strip.NumberOfVertices > short.MaxValue)
                {
                    throw new ArgumentException(Properties.Resources.VertexCountInStripTooHigh, "strip");
                }

                this.sb.Append("    ").Append(FormatUInt16((ushort)strip.NumberOfVertices));
                if (strip.IsReversed)
                {
                    this.sb.Append(" | 0x8000");
                }

                this.sb.AppendLine(",").Append("    ");
                for (int i = 0;;)
                {
                    this.sb.Append(FormatUInt16(strip.Vertices[i]));
                    if (++i >= strip.NumberOfVertices)
                    {
                        break;
                    }

                    this.sb.Append(", ");
                }
            }
        }
    }
}
