//------------------------------------------------------------------------------
// <copyright file="SADXPCModuleVisitors.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    public interface IVisitor<T>
    {
        void Visit(T target);
    }

    public interface ISADXPCModuleGroupExportChildVisitor :
        IVisitor<SADXPCModuleGroupExportObjects>,
        IVisitor<SADXPCModuleGroupExportModels>,
        IVisitor<SADXPCModuleGroupExportActions>,
        IVisitor<SADXPCModuleGroupExportMotions>,
        IVisitor<SADXPCModuleGroupExportMaterials>,
        IVisitor<SADXPCModuleGroupExportPoints>
    {
    }

    public abstract class SADXPCModuleGroupExportChild
    {
        public abstract void Accept(ISADXPCModuleGroupExportChildVisitor visitor);
    }

    public partial class SADXPCModuleGroupExportActions : SADXPCModuleGroupExportChild
    {
        public override void Accept(ISADXPCModuleGroupExportChildVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class SADXPCModuleGroupExportMaterials : SADXPCModuleGroupExportChild
    {
        public override void Accept(ISADXPCModuleGroupExportChildVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class SADXPCModuleGroupExportModels : SADXPCModuleGroupExportChild
    {
        public override void Accept(ISADXPCModuleGroupExportChildVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class SADXPCModuleGroupExportMotions : SADXPCModuleGroupExportChild
    {
        public override void Accept(ISADXPCModuleGroupExportChildVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class SADXPCModuleGroupExportObjects : SADXPCModuleGroupExportChild
    {
        public override void Accept(ISADXPCModuleGroupExportChildVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class SADXPCModuleGroupExportPoints : SADXPCModuleGroupExportChild
    {
        public override void Accept(ISADXPCModuleGroupExportChildVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
