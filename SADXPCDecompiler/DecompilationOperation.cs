//------------------------------------------------------------------------------
// <copyright file="DecompilationOperation.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;
    using SonicRetro.SonicAdventure.CExporter;
    using SonicRetro.SonicAdventure.Data;
    using SonicRetro.SonicAdventure.PCTools;

    internal sealed class DecompilationOperation
    {
        private const string IncludeLine = @"#include ""../../Structs.h""";
        private string sourceModuleFilePath;
        private string moduleDescriptionFilePath;
        private string outputDirectoryPath;
        private DecompilationStatus decompilationStatus;
        private CancellationToken cancellationToken;

        public DecompilationOperation(string sourceModuleFilePath, string moduleDescriptionFilePath, string outputDirectoryPath, DecompilationStatus decompilationStatus, CancellationToken cancellationToken)
        {
            this.sourceModuleFilePath = sourceModuleFilePath;
            this.moduleDescriptionFilePath = moduleDescriptionFilePath;
            this.outputDirectoryPath = outputDirectoryPath;
            this.decompilationStatus = decompilationStatus;
            this.cancellationToken = cancellationToken;
        }

        public void Decompile()
        {
            this.decompilationStatus.StatusText = "Opening source module file...";
            FileStream sourceModuleFileStream = this.TryOpenSourceModuleFileForRead();
            try
            {
                this.cancellationToken.ThrowIfCancellationRequested();
            }
            catch
            {
                sourceModuleFileStream.Dispose();
                throw;
            }

            this.decompilationStatus.StatusText = "Initializing PEReader...";
            using (PEReader peReader = new PEReader(sourceModuleFileStream))
            {
                this.decompilationStatus.StatusText = "Opening module description file...";
                FileStream moduleDescriptionFileStream = this.TryOpenModuleDescriptionFileForRead();
                try
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                }
                catch
                {
                    moduleDescriptionFileStream.Dispose();
                    throw;
                }

                // Keep references to the objects being decompiled
                List<Tuple<string, DataTracker>> dataTrackers = new List<Tuple<string, DataTracker>>();

                // Keep track of the objects that we have already exported
                ExportTracker exportTracker = new ExportTracker();

                // Remember the ATTACHes on which the material count is to be faked
                Dictionary<ATTACH, int> attachOverriddenMaterialCounts = new Dictionary<ATTACH, int>();

                using (moduleDescriptionFileStream)
                {
                    this.decompilationStatus.StatusText = "Reading module description file...";
                    XmlSerializer serializer = new XmlSerializer(typeof(SADXPCModule));
                    SADXPCModule module;
                    module = (SADXPCModule)serializer.Deserialize(moduleDescriptionFileStream);

                    foreach (SADXPCModuleGroup group in module.Group)
                    {
                        this.cancellationToken.ThrowIfCancellationRequested();
                        this.decompilationStatus.StatusText = "Reading data for Group '" + group.Name + "'...";
                        DataTracker dataTracker = new DataTracker();
                        dataTrackers.Add(new Tuple<string, DataTracker>(group.Name, dataTracker));

                        if (group.StandAloneMaterial != null && group.StandAloneMaterial.Length > 0)
                        {
                            this.decompilationStatus.StatusText = "Reading data for StandAloneMaterial elements in Group '" + group.Name + "'...";
                            foreach (SADXPCModuleGroupStandAloneMaterial material in group.StandAloneMaterial)
                            {
                                uint address;
                                if (!uint.TryParse(material.Address, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address))
                                {
                                    throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.InvalidAddress, material.Address));
                                }

                                dataTracker.StandAloneMaterials.Add(peReader.ReadMaterialArray(address, material.Count));
                            }
                        }

                        foreach (SADXPCModuleGroupExport export in group.Export)
                        {
                            this.cancellationToken.ThrowIfCancellationRequested();
                            this.decompilationStatus.StatusText = "Reading data for Export '" + export.Name + "' in Group '" + group.Name + "'...";
                            uint addressOfExport = peReader.GetExport(export.Name);
                            if (addressOfExport == 0)
                            {
                                throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.MissingExport, export.Name));
                            }

                            SADXPCModuleGroupExportChild child = export.Item as SADXPCModuleGroupExportChild;
                            if (child == null)
                            {
                                throw new PrettyMessageException(Properties.Resources.UnknownExportChild);
                            }

                            child.Accept(new SADXPCModuleGroupExportChildVisitor(dataTracker, peReader, addressOfExport, export.Name));
                        }
                    }

                    this.cancellationToken.ThrowIfCancellationRequested();
                    if (module.Attach != null && module.Attach.Length > 0)
                    {
                        this.decompilationStatus.StatusText = "Reading data for Attach elements...";
                        foreach (SADXPCModuleAttach attach in module.Attach)
                        {
                            uint address;
                            if (!uint.TryParse(attach.Address, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address))
                            {
                                throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.InvalidAddress, attach.Address));
                            }

                            INamed named = peReader.GetKnownObject(address);
                            if (named == null)
                            {
                                throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.UnknownObject, address));
                            }

                            ATTACH att = named as ATTACH;
                            if (att == null)
                            {
                                throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.AddressDoesNotReferToAttach, address));
                            }

                            if (attach.OverrideMaterialCountSpecified)
                            {
                                attachOverriddenMaterialCounts[att] = attach.OverrideMaterialCount;
                            }
                        }
                    }

                    if (module.Rename != null && module.Rename.Length > 0)
                    {
                        this.decompilationStatus.StatusText = "Reading data for Rename elements...";
                        foreach (SADXPCModuleRename rename in module.Rename)
                        {
                            uint address;
                            if (!uint.TryParse(rename.Address, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address))
                            {
                                throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.InvalidAddress, rename.Address));
                            }

                            INamed named = peReader.GetKnownObject(address);
                            if (named == null)
                            {
                                throw new PrettyMessageException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.UnknownObject, address));
                            }

                            named.Name = rename.NewName;
                        }
                    }
                }

                foreach (Tuple<string, DataTracker> trackGroup in dataTrackers)
                {
                    this.decompilationStatus.StatusText = "Decompiling Group '" + trackGroup.Item1 + "'...";
                    string groupDirectory = Path.Combine(this.outputDirectoryPath, trackGroup.Item1);
                    if (!Directory.Exists(groupDirectory))
                    {
                        CreateDirectory(groupDirectory);
                    }

                    this.cancellationToken.ThrowIfCancellationRequested();
                    DataTracker dataTracker = trackGroup.Item2;
                    foreach (NamedCollection<MATERIAL> materials in dataTracker.StandAloneMaterials)
                    {
                        this.decompilationStatus.StatusText = "Decompiling StandAloneMaterials '" + materials.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        if (exportTracker.Add(materials))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(IncludeLine).AppendLine();
                            sb.Append(CExporter.Export(materials, Linkage.ExternLinkage));
                            this.WriteOutputFile(Path.Combine(groupDirectory, materials.Name + ".c"), sb.ToString());
                        }
                    }

                    foreach (NamedCollection<NamedCollection<MATERIAL>> materialList in dataTracker.Materials)
                    {
                        this.decompilationStatus.StatusText = "Decompiling Materials '" + materialList.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(IncludeLine);
                        foreach (NamedCollection<MATERIAL> materials in materialList)
                        {
                            if (materials != null)
                            {
                                if (exportTracker.Add(materials))
                                {
                                    sb.AppendLine().Append(CExporter.Export(materials, Linkage.ExternLinkage));
                                }
                                else
                                {
                                    sb.AppendLine().Append("extern struct MATERIAL ").Append(materials.Name).AppendLine("[];");
                                }
                            }
                        }

                        sb.AppendLine().Append(CExporter.Export(materialList));
                        this.WriteOutputFile(Path.Combine(groupDirectory, materialList.Name + ".c"), sb.ToString());
                    }

                    foreach (NamedCollection<NamedCollection<Vector3>> pointList in dataTracker.Points)
                    {
                        this.decompilationStatus.StatusText = "Decompiling Points '" + pointList.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(IncludeLine);
                        foreach (NamedCollection<Vector3> points in pointList)
                        {
                            if (points != null)
                            {
                                if (exportTracker.Add(points))
                                {
                                    sb.AppendLine().Append(CExporter.Export(points, Linkage.ExternLinkage));
                                }
                                else
                                {
                                    sb.AppendLine().Append("extern struct Vector3 ").Append(points.Name).AppendLine("[];");
                                }
                            }
                        }

                        sb.AppendLine().Append(CExporter.Export(pointList));
                        this.WriteOutputFile(Path.Combine(groupDirectory, pointList.Name + ".c"), sb.ToString());
                    }

                    foreach (NamedCollection<OBJECT> objectList in dataTracker.Objects)
                    {
                        this.decompilationStatus.StatusText = "Decompiling Objects '" + objectList.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(IncludeLine).AppendLine();
                        foreach (OBJECT obj in objectList)
                        {
                            if (obj != null)
                            {
                                sb.AppendLine("extern struct OBJECT ").Append(obj.Name).AppendLine(";");
                            }
                        }

                        sb.AppendLine().Append(CExporter.Export(objectList));
                        this.WriteOutputFile(Path.Combine(groupDirectory, objectList.Name + ".c"), sb.ToString());

                        foreach (OBJECT obj in objectList)
                        {
                            this.ExportObject(exportTracker, groupDirectory, obj, attachOverriddenMaterialCounts);
                        }
                    }

                    foreach (NamedCollection<ATTACH> modelList in dataTracker.Models)
                    {
                        this.decompilationStatus.StatusText = "Decompiling Models '" + modelList.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(IncludeLine).AppendLine();
                        foreach (ATTACH model in modelList)
                        {
                            if (model != null)
                            {
                                sb.Append("extern struct ATTACH ").Append(model.Name).AppendLine(";");
                            }
                        }

                        sb.AppendLine().Append(CExporter.Export(modelList));
                        this.WriteOutputFile(Path.Combine(groupDirectory, modelList.Name + ".c"), sb.ToString());

                        foreach (ATTACH model in modelList)
                        {
                            this.ExportModel(exportTracker, groupDirectory, model, attachOverriddenMaterialCounts);
                        }
                    }

                    foreach (NamedCollection<AnimHead> actionList in dataTracker.Actions)
                    {
                        this.decompilationStatus.StatusText = "Decompiling Actions '" + actionList.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(IncludeLine);
                        foreach (AnimHead action in actionList)
                        {
                            if (action != null)
                            {
                                this.ExportAction(exportTracker, groupDirectory, action, sb, attachOverriddenMaterialCounts);
                            }
                        }

                        sb.AppendLine().Append(CExporter.Export(actionList));
                        this.WriteOutputFile(Path.Combine(groupDirectory, actionList.Name + ".c"), sb.ToString());
                    }

                    foreach (NamedCollection<AnimHead2> motionList in dataTracker.Motions)
                    {
                        this.decompilationStatus.StatusText = "Decompiling Motions '" + motionList.Name + "' in Group '" + trackGroup.Item1 + "'...";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(IncludeLine);
                        foreach (AnimHead2 motion in motionList)
                        {
                            if (motion != null)
                            {
                                this.ExportMotion(exportTracker, groupDirectory, motion);
                                sb.Append("extern struct AnimHead2 ").Append(motion.Name).AppendLine(";");
                            }
                        }

                        sb.AppendLine().Append(CExporter.Export(motionList));
                        this.WriteOutputFile(Path.Combine(groupDirectory, motionList.Name + ".c"), sb.ToString());
                    }
                }
            }
        }

        private static void CreateDirectory(string groupDirectory)
        {
            try
            {
                Directory.CreateDirectory(groupDirectory);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PrettyMessageException(Properties.Resources.UnauthorizedAccessToOutputDirectory, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new PrettyMessageException(Properties.Resources.PathTooLongForOutputDirectory, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new PrettyMessageException(Properties.Resources.DirectoryNotFoundForOutputDirectory, ex);
            }
            catch (IOException ex)
            {
                throw new PrettyMessageException(ex.Message, ex);
            }
            catch (ArgumentException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForOutputDirectory, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForOutputDirectory, ex);
            }
        }

        private FileStream TryOpenSourceModuleFileForRead()
        {
            try
            {
                return File.OpenRead(this.sourceModuleFilePath);
            }
            catch (ArgumentException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForSourceModule, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForSourceModule, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PrettyMessageException(Properties.Resources.UnauthorizedAccessToSourceModule, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new PrettyMessageException(Properties.Resources.PathTooLongForSourceModule, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new PrettyMessageException(Properties.Resources.SourceModuleFileNotFound, ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new PrettyMessageException(Properties.Resources.SourceModuleFileNotFound, ex);
            }
        }

        private FileStream TryOpenModuleDescriptionFileForRead()
        {
            try
            {
                return File.OpenRead(this.moduleDescriptionFilePath);
            }
            catch (ArgumentException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForModuleDescription, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForModuleDescription, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PrettyMessageException(Properties.Resources.UnauthorizedAccessToModuleDescription, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new PrettyMessageException(Properties.Resources.PathTooLongForModuleDescription, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new PrettyMessageException(Properties.Resources.ModuleDescriptionFileNotFound, ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new PrettyMessageException(Properties.Resources.ModuleDescriptionFileNotFound, ex);
            }
        }

        private static StreamWriter TryCreateOutputFile(string path)
        {
            try
            {
                return File.CreateText(path);
            }
            catch (ArgumentException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForOutputFile, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new PrettyMessageException(Properties.Resources.InvalidPathForOutputFile, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PrettyMessageException(Properties.Resources.UnauthorizedAccessToOutputFile, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new PrettyMessageException(Properties.Resources.PathTooLongForOutputFile, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new PrettyMessageException(Properties.Resources.DirectoryOfOutputFileNotFound, ex);
            }
            catch (IOException ex)
            {
                throw new PrettyMessageException(Properties.Resources.IOErrorForOutputFile, ex);
            }
        }

        private void WriteOutputFile(string path, string contents)
        {
            // We cannot use the 'using' statement here, because we need to alter the variables, so we have to call Dispose
            // directly.
            StreamWriter outputFileStreamWriter = TryCreateOutputFile(path);
            try
            {
                Action<string> writeAction = outputFileStreamWriter.Write;
                IAsyncResult asyncWriteResult = writeAction.BeginInvoke(contents, null, null);
                try
                {
                    WaitHandle asyncWriteWaitHandle = asyncWriteResult.AsyncWaitHandle;
                    try
                    {
                        // Wait until either the data has been written to the file, or the user cancelled the operation,
                        // whichever comes first. If the user cancelled before the data was written to the file, finish the
                        // writing on a background thread and return immediately.
                        switch (WaitHandle.WaitAny(new[] { this.cancellationToken.WaitHandle, asyncWriteWaitHandle }))
                        {
                            case 0: // cancellation token
                                // Copy the references and make them accessible to the thread through a closure. We are
                                // going to clear the original references in a moment (if the thread wants to start).
                                StreamWriter outputFileStreamWriterForThread = outputFileStreamWriter;
                                IAsyncResult asyncWriteResultForThread = asyncWriteResult;
                                WaitHandle asyncWriteWaitHandleForThread = asyncWriteWaitHandle;

                                // This thread waits for the writing operation to finish by calling EndInvoke on the
                                // delegate. This is done because we "must" call EndInvoke, according to MSDN. However, it's
                                // done on a background thread so that we don't block and return immediately.
                                ThreadPool.QueueUserWorkItem(
                                    state =>
                                    {
                                        try
                                        {
                                            try
                                            {
                                                // Each of these operations is done in its own finally block so that each of
                                                // them gets a chance of being executed, even if a previous one throws an
                                                // exception. Also, by being in a finally block, they will not be
                                                // interrupted by Thread.Abort() (see below).
                                                try
                                                {
                                                }
                                                finally
                                                {
                                                    asyncWriteWaitHandleForThread.Dispose();
                                                }
                                            }
                                            finally
                                            {
                                                try
                                                {
                                                    writeAction.EndInvoke(asyncWriteResultForThread);
                                                }
                                                catch (IOException ex)
                                                {
                                                    throw new PrettyMessageException(Properties.Resources.IOErrorWhileWritingInOutputFile + " " + ex.Message, ex);
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            outputFileStreamWriterForThread.Dispose();
                                        }
                                    });

                                // These variables must be set to null to avoid disposing the objects twice. They are set to
                                // null in a finally block to make sure it is done even if the thread is being aborted by
                                // Thread.Abort().
                                // http://blog.somecreativity.com/2008/04/10/the-empty-try-block-mystery/
                                RuntimeHelpers.PrepareConstrainedRegions();
                                try
                                {
                                }
                                finally
                                {
                                    asyncWriteWaitHandle = null;
                                    asyncWriteResult = null;
                                    outputFileStreamWriter = null;
                                }

                                // Since the cancellation token caused WaitAny to return, this will always throw.
                                this.cancellationToken.ThrowIfCancellationRequested();
                                break;
                        }
                    }
                    finally
                    {
                        if (asyncWriteWaitHandle != null)
                        {
                            asyncWriteWaitHandle.Dispose();
                        }
                    }
                }
                finally
                {
                    if (asyncWriteResult != null)
                    {
                        try
                        {
                            writeAction.EndInvoke(asyncWriteResult);
                        }
                        catch (IOException ex)
                        {
                            throw new PrettyMessageException(Properties.Resources.IOErrorWhileWritingInOutputFile + " " + ex.Message, ex);
                        }
                    }
                }
            }
            finally
            {
                if (outputFileStreamWriter != null)
                {
                    outputFileStreamWriter.Dispose();
                }
            }
        }

        private void ExportObject(ExportTracker exportTracker, string groupDirectory, OBJECT obj, Dictionary<ATTACH, int> attachOverriddenMaterialCounts)
        {
            if (obj != null && exportTracker.Add(obj))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(IncludeLine).AppendLine();
                if (obj.Attach != null)
                {
                    this.ExportModel(exportTracker, groupDirectory, obj.Attach, attachOverriddenMaterialCounts);
                    sb.AppendLine("extern struct ATTACH " + obj.Attach.Name + ";");
                }

                if (obj.Child != null)
                {
                    this.ExportObject(exportTracker, groupDirectory, obj.Child, attachOverriddenMaterialCounts);
                    sb.AppendLine("extern struct OBJECT " + obj.Child.Name + ";");
                }

                if (obj.Sibling != null)
                {
                    this.ExportObject(exportTracker, groupDirectory, obj.Sibling, attachOverriddenMaterialCounts);
                    sb.AppendLine("extern struct OBJECT " + obj.Sibling.Name + ";");
                }

                if (obj.Attach != null || obj.Child != null || obj.Sibling != null)
                {
                    sb.AppendLine();
                }

                sb.Append(CExporter.Export(obj));
                this.WriteOutputFile(Path.Combine(groupDirectory, obj.Name + ".c"), sb.ToString());
            }
        }

        private void ExportModel(ExportTracker exportTracker, string groupDirectory, ATTACH model, Dictionary<ATTACH, int> attachOverriddenMaterialCounts)
        {
            if (model != null && exportTracker.Add(model))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(IncludeLine).AppendLine();
                if (model.Vertices != null)
                {
                    if (exportTracker.Add(model.Vertices))
                    {
                        sb.AppendLine(CExporter.Export(model.Vertices, Linkage.StaticLinkage));
                    }
                    else
                    {
                        sb.Append("extern struct Vector3 ").Append(model.Vertices.Name).Append("[").Append(model.Vertices.Count.ToString(CultureInfo.InvariantCulture)).AppendLine("];").AppendLine();
                    }
                }

                if (model.Normals != null && exportTracker.Add(model.Normals))
                {
                    sb.AppendLine(CExporter.Export(model.Normals, Linkage.StaticLinkage));
                }

                if (model.Meshes != null)
                {
                    foreach (MESH mesh in model.Meshes)
                    {
                        if (mesh.Polys != null)
                        {
                            sb.AppendLine(CExporter.Export(mesh.Polys));
                        }

                        if (mesh.PolyNormals != null)
                        {
                            sb.AppendLine(CExporter.Export(mesh.PolyNormals));
                        }

                        if (mesh.VertexColors != null)
                        {
                            sb.AppendLine(CExporter.Export(mesh.VertexColors));
                        }

                        if (mesh.UV != null)
                        {
                            sb.AppendLine(CExporter.Export(mesh.UV));
                        }
                    }

                    sb.AppendLine(CExporter.Export(model.Meshes));
                }

                if (model.Materials != null)
                {
                    if (exportTracker.Add(model.Materials))
                    {
                        sb.AppendLine(CExporter.Export(model.Materials, Linkage.StaticLinkage));
                    }
                    else
                    {
                        sb.Append("extern struct MATERIAL ").Append(model.Materials.Name).Append("[").Append(model.Materials.Count.ToString(CultureInfo.InvariantCulture)).AppendLine("];").AppendLine();
                    }
                }

                int overriddenMaterialCount;
                if (attachOverriddenMaterialCounts.TryGetValue(model, out overriddenMaterialCount))
                {
                    sb.Append(CExporter.Export(model, overriddenMaterialCount));
                }
                else
                {
                    sb.Append(CExporter.Export(model));
                }

                this.WriteOutputFile(Path.Combine(groupDirectory, model.Name + ".c"), sb.ToString());
            }
        }

        private void ExportAction(ExportTracker exportTracker, string groupDirectory, AnimHead action, StringBuilder sb, Dictionary<ATTACH, int> attachOverriddenMaterialCounts)
        {
            if (action != null && exportTracker.Add(action))
            {
                sb.AppendLine();
                if (action.Model != null)
                {
                    this.ExportObject(exportTracker, groupDirectory, action.Model, attachOverriddenMaterialCounts);
                    sb.Append("extern struct OBJECT ").Append(action.Model.Name).AppendLine(";");
                }

                if (action.Motion != null)
                {
                    this.ExportMotion(exportTracker, groupDirectory, action.Motion);
                    sb.Append("extern struct AnimHead2 ").Append(action.Motion.Name).AppendLine(";");
                }

                sb.Append(CExporter.Export(action));
            }
        }

        private void ExportMotion(ExportTracker exportTracker, string groupDirectory, AnimHead2 motion)
        {
            if (motion != null && exportTracker.Add(motion))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(IncludeLine).AppendLine();
                if (motion.FrameData != null)
                {
                    sb.AppendLine(CExporter.Export(motion.FrameData, exportTracker));
                }

                sb.Append(CExporter.Export(motion));
                this.WriteOutputFile(Path.Combine(groupDirectory, motion.Name + ".c"), sb.ToString());
            }
        }

        private sealed class DataTracker
        {
            public DataTracker()
            {
                this.Objects = new List<NamedCollection<OBJECT>>();
                this.Models = new List<NamedCollection<ATTACH>>();
                this.Actions = new List<NamedCollection<AnimHead>>();
                this.Motions = new List<NamedCollection<AnimHead2>>();
                this.Materials = new List<NamedCollection<NamedCollection<MATERIAL>>>();
                this.Points = new List<NamedCollection<NamedCollection<Vector3>>>();
                this.StandAloneMaterials = new List<NamedCollection<MATERIAL>>();
            }

            public List<NamedCollection<OBJECT>> Objects { get; private set; }

            public List<NamedCollection<ATTACH>> Models { get; private set; }

            public List<NamedCollection<AnimHead>> Actions { get; private set; }

            public List<NamedCollection<AnimHead2>> Motions { get; private set; }

            public List<NamedCollection<NamedCollection<MATERIAL>>> Materials { get; private set; }

            public List<NamedCollection<NamedCollection<Vector3>>> Points { get; private set; }

            public List<NamedCollection<MATERIAL>> StandAloneMaterials { get; private set; }
        }

        private sealed class SADXPCModuleGroupExportChildVisitor : ISADXPCModuleGroupExportChildVisitor
        {
            private DataTracker dataTracker;
            private PEReader peReader;
            private long address;
            private string exportName;

            public SADXPCModuleGroupExportChildVisitor(DataTracker dataTracker, PEReader peReader, long address, string exportName)
            {
                this.dataTracker = dataTracker;
                this.peReader = peReader;
                this.address = address;
                this.exportName = exportName;
            }

            public void Visit(SADXPCModuleGroupExportObjects target)
            {
                NamedCollection<OBJECT> objects = peReader.ReadObjectPointerArray(this.address, target.Count);
                objects.Name = this.exportName;
                this.dataTracker.Objects.Add(objects);
            }

            public void Visit(SADXPCModuleGroupExportModels target)
            {
                NamedCollection<ATTACH> models = peReader.ReadAttachPointerArray(this.address, target.Count);
                models.Name = this.exportName;
                this.dataTracker.Models.Add(models);
            }

            public void Visit(SADXPCModuleGroupExportActions target)
            {
                NamedCollection<AnimHead> actions = peReader.ReadAnimHeadPointerArray(this.address, target.Count);
                actions.Name = this.exportName;
                this.dataTracker.Actions.Add(actions);
            }

            public void Visit(SADXPCModuleGroupExportMotions target)
            {
                int[][] vector3CountsByModel = new int[target.Motion.Length][];
                for (int i = 0; i < target.Motion.Length; i++)
                {
                    object[] motionItems = target.Motion[i].Items;
                    if (motionItems.Length == 1 && motionItems[0].GetType() == typeof(object))
                    {
                        vector3CountsByModel[i] = null;
                    }
                    else
                    {
                        vector3CountsByModel[i] = motionItems.Select(item => (int)item).ToArray();
                    }
                }

                NamedCollection<AnimHead2> motions = peReader.ReadAnimHead2PointerArray(this.address, vector3CountsByModel);
                motions.Name = this.exportName;
                this.dataTracker.Motions.Add(motions);
            }

            public void Visit(SADXPCModuleGroupExportMaterials target)
            {
                NamedCollection<NamedCollection<MATERIAL>> materials =
                    peReader.ReadMaterialArrayPointerArray(this.address, target.Material.Select(material => material.Count).ToArray());
                materials.Name = this.exportName;
                this.dataTracker.Materials.Add(materials);
            }

            public void Visit(SADXPCModuleGroupExportPoints target)
            {
                NamedCollection<NamedCollection<Vector3>> points =
                    peReader.ReadVector3ArrayPointerArray(this.address, target.Vector3Count);
                points.Name = this.exportName;
                this.dataTracker.Points.Add(points);
            }
        }
    }
}
