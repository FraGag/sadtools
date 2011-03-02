//------------------------------------------------------------------------------
// <copyright file="ATTACH.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents a model.
    /// </summary>
    public sealed class ATTACH : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATTACH"/> class.
        /// </summary>
        public ATTACH()
        {
            this.Vertices = new NamedCollection<Vector3>();
            this.Normals = new NamedCollection<Vector3>();
            this.Meshes = new NamedCollection<MESH>();
            this.Materials = new NamedCollection<MATERIAL>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATTACH"/> class using the supplied values.
        /// </summary>
        /// <param name="name">Name of the model.</param>
        /// <param name="vertices">Vertices for the model.</param>
        /// <param name="normals">Normals for the model.</param>
        /// <param name="meshes">Meshes for the model.</param>
        /// <param name="materials">Materials for the model.</param>
        /// <param name="center">Center of the model.</param>
        /// <param name="radius">Radius of the model.</param>
        /// <param name="nullMember">Unused value. Set this value to 0.</param>
        public ATTACH(string name, NamedCollection<Vector3> vertices, NamedCollection<Vector3> normals, NamedCollection<MESH> meshes, NamedCollection<MATERIAL> materials, Vector3 center, float radius, int nullMember)
        {
            this.Name = name;
            this.Vertices = vertices;
            this.Normals = normals;
            this.Meshes = meshes;
            this.Materials = materials;
            this.Center = center;
            this.Radius = radius;
            this.Null = nullMember;
        }

        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        /// <value>The name of the model.</value>
        /// <remarks>This property is metadata. It can be used to give a meaningful name to the model.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets the collection of vertices for the model.
        /// </summary>
        /// <value>The collection of vertices for the model.</value>
        /// <remarks>
        /// The number of vertices should be equal to the number of normals, if normals are present.
        /// </remarks>
        public NamedCollection<Vector3> Vertices { get; private set; }

        /// <summary>
        /// Gets the collection of normals for the model.
        /// </summary>
        /// <value>The collection of normals for the model.</value>
        /// <remarks>
        /// The number of normals should be equal to the number of vertices, if normals are present.
        /// </remarks>
        public NamedCollection<Vector3> Normals { get; private set; }

        /// <summary>
        /// Gets a collection of the meshes for the model.
        /// </summary>
        /// <value>An instance of <see cref="NamedCollection{T}"/> containing the meshes for the model.</value>
        public NamedCollection<MESH> Meshes { get; private set; }

        /// <summary>
        /// Gets a collection of the materials for the model.
        /// </summary>
        /// <value>An instance of <see cref="NamedCollection{T}"/> containing the materials for the model.</value>
        public NamedCollection<MATERIAL> Materials { get; private set; }

        /// <summary>
        /// Gets or sets the center of the model.
        /// </summary>
        /// <value>The center of the model.</value>
        public Vector3 Center { get; set; }

        /// <summary>
        /// Gets or sets the radius of the model.
        /// </summary>
        /// <value>The radius of the model.</value>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets an unused value.
        /// </summary>
        /// <value>An unused value. Set this value to 0.</value>
        /// <remarks>This field is unused.</remarks>
        public int Null { get; set; }
    }
}
