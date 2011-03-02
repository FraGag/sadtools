//------------------------------------------------------------------------------
// <copyright file="ExportTracker.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.CExporter
{
    using System.Collections.Generic;
    using SonicRetro.SonicAdventure.Data;

    /// <summary>
    /// Maintains sets of items that have already been exported to avoid duplicate definitions.
    /// </summary>
    public sealed class ExportTracker
    {
        /// <summary>
        /// Set of already exported <see cref="OBJECT"/> instances.
        /// </summary>
        private HashSet<OBJECT> exportedObjects = new HashSet<OBJECT>();

        /// <summary>
        /// Set of already exported <see cref="ATTACH"/> instances.
        /// </summary>
        private HashSet<ATTACH> exportedModels = new HashSet<ATTACH>();

        /// <summary>
        /// Set of already exported collections of <see cref="MATERIAL"/>.
        /// </summary>
        private HashSet<NamedCollection<MATERIAL>> exportedMaterials = new HashSet<NamedCollection<MATERIAL>>();

        /// <summary>
        /// Set of already exported <see cref="AnimHead"/> instances.
        /// </summary>
        private HashSet<AnimHead> exportedActions = new HashSet<AnimHead>();

        /// <summary>
        /// Set of already exported <see cref="AnimHead2"/> instances.
        /// </summary>
        private HashSet<AnimHead2> exportedMotions = new HashSet<AnimHead2>();

        /// <summary>
        /// Set of already exported collections of <see cref="Vector3"/>.
        /// </summary>
        private HashSet<NamedCollection<Vector3>> exportedPoints = new HashSet<NamedCollection<Vector3>>();

        /// <summary>
        /// Adds the given OBJECT to the set of exported OBJECTs.
        /// </summary>
        /// <param name="obj">The OBJECT to add to the set.</param>
        /// <returns>
        /// <c>true</c> if the OBJECT has been added to the set, or <c>false</c> if the OBJECT was already part of the set.
        /// </returns>
        public bool Add(OBJECT obj)
        {
            return this.exportedObjects.Add(obj);
        }

        /// <summary>
        /// Adds the given ATTACH to the set of exported ATTACHes.
        /// </summary>
        /// <param name="model">The ATTACH to add to the set.</param>
        /// <returns>
        /// <c>true</c> if the ATTACH has been added to the set, or <c>false</c> if the ATTACH was already part of the set.
        /// </returns>
        public bool Add(ATTACH model)
        {
            return this.exportedModels.Add(model);
        }

        /// <summary>
        /// Adds the given MATERIAL collection to the set of exported MATERIAL collections.
        /// </summary>
        /// <param name="materials">The MATERIAL collection to add to the set.</param>
        /// <returns>
        /// <c>true</c> if the MATERIAL collection has been added to the set, or <c>false</c> if the MATERIAL collection was
        /// already part of the set.
        /// </returns>
        public bool Add(NamedCollection<MATERIAL> materials)
        {
            return this.exportedMaterials.Add(materials);
        }

        /// <summary>
        /// Adds the given AnimHead to the set of exported AnimHeads.
        /// </summary>
        /// <param name="action">The AnimHead to add to the set.</param>
        /// <returns>
        /// <c>true</c> if the AnimHead has been added to the set, or <c>false</c> if the AnimHead was already part of the set.
        /// </returns>
        public bool Add(AnimHead action)
        {
            return this.exportedActions.Add(action);
        }

        /// <summary>
        /// Adds the given AnimHead2 to the set of exported AnimHeads.
        /// </summary>
        /// <param name="motion">The AnimHead2 to add to the set.</param>
        /// <returns>
        /// <c>true</c> if the AnimHead2 has been added to the set, or <c>false</c> if the AnimHead2 was already part of the
        /// set.
        /// </returns>
        public bool Add(AnimHead2 motion)
        {
            return this.exportedMotions.Add(motion);
        }

        /// <summary>
        /// Adds the given Vector3 collection to the set of exported Vector3 collections.
        /// </summary>
        /// <param name="points">The Vector3 collection to add to the set.</param>
        /// <returns>
        /// <c>true</c> if the Vector3 collection has been added to the set, or <c>false</c> if the Vector3 collection was
        /// already part of the set.
        /// </returns>
        public bool Add(NamedCollection<Vector3> points)
        {
            return this.exportedPoints.Add(points);
        }
    }
}
