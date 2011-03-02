//------------------------------------------------------------------------------
// <copyright file="AnimFrame_PosRot.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents an animation frame containing position and rotation data.
    /// </summary>
    public sealed class AnimFrame_PosRot : AnimFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame_PosRot"/> class.
        /// </summary>
        public AnimFrame_PosRot()
        {
            this.Positions = new NamedCollection<Vector3AnimData>();
            this.Rotations = new NamedCollection<Rotation3AnimData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame_PosRot"/> class using the supplied values.
        /// </summary>
        /// <param name="positions">Position data.</param>
        /// <param name="rotations">Rotation data.</param>
        public AnimFrame_PosRot(NamedCollection<Vector3AnimData> positions, NamedCollection<Rotation3AnimData> rotations)
        {
            this.Positions = positions;
            this.Rotations = rotations;
        }

        /// <summary>
        /// Gets a collection of position data.
        /// </summary>
        /// <value><see cref="NamedCollection{T}"/> containing the position data.</value>
        public NamedCollection<Vector3AnimData> Positions { get; private set; }

        /// <summary>
        /// Gets a collection of rotation data.
        /// </summary>
        /// <value><see cref="NamedCollection{T}"/> containing the rotation data.</value>
        public NamedCollection<Rotation3AnimData> Rotations { get; private set; }

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

            visitor.VisitAnimFrame_PosRot(this);
        }
    }
}
