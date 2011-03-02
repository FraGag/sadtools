//------------------------------------------------------------------------------
// <copyright file="AnimFrame_VertNrm.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    // TODO: determine whether it's really normals or if it's something else

    /// <summary>
    /// Represents an animation frame containing vertex and normal data.
    /// </summary>
    public sealed class AnimFrame_VertNrm : AnimFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame_VertNrm" /> class.
        /// </summary>
        public AnimFrame_VertNrm()
        {
            this.Vertices = new NamedCollection<Vector3ArrayAnimData>();
            this.Normals = new NamedCollection<Vector3ArrayAnimData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame_VertNrm" /> class using the supplied values.
        /// </summary>
        /// <param name="vertices">Vertex data.</param>
        /// <param name="normals">Normal data.</param>
        public AnimFrame_VertNrm(NamedCollection<Vector3ArrayAnimData> vertices, NamedCollection<Vector3ArrayAnimData> normals)
        {
            this.Vertices = vertices;
            this.Normals = normals;
        }

        /// <summary>
        /// Gets a collection of vertex data.
        /// </summary>
        /// <value><see cref="NamedCollection{T}"/> containing the vertex data.</value>
        public NamedCollection<Vector3ArrayAnimData> Vertices { get; private set; }

        /// <summary>
        /// Gets a collection of normal data.
        /// </summary>
        /// <value><see cref="NamedCollection{T}"/> containing the normal data.</value>
        public NamedCollection<Vector3ArrayAnimData> Normals { get; private set; }

        /// <summary>
        /// Visits the specified visitor.
        /// </summary>
        /// <param name="visitor">An animation frame visitor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visitor"/> is <c>null</c> (<c>Nothing</c> in Visual Basic).
        /// </exception>
        public override void Accept(IAnimFrameVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            visitor.VisitAnimFrame_VertNrm(this);
        }
    }
}
