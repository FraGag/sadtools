//------------------------------------------------------------------------------
// <copyright file="AnimFrame_PosRotScale.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents an animation frame containing position, rotation and scale data.
    /// </summary>
    public sealed class AnimFrame_PosRotScale : AnimFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame_PosRotScale"/> class.
        /// </summary>
        public AnimFrame_PosRotScale()
        {
            this.Positions = new NamedCollection<Vector3AnimData>();
            this.Rotations = new NamedCollection<Rotation3AnimData>();
            this.Scales = new NamedCollection<Vector3AnimData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimFrame_PosRotScale" /> class using the supplies values.
        /// </summary>
        /// <param name="positions">Position data.</param>
        /// <param name="rotations">Rotation data.</param>
        /// <param name="scales">Scale data.</param>
        public AnimFrame_PosRotScale(NamedCollection<Vector3AnimData> positions, NamedCollection<Rotation3AnimData> rotations, NamedCollection<Vector3AnimData> scales)
        {
            this.Positions = positions;
            this.Rotations = rotations;
            this.Scales = scales;
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
        /// Gets a collection of scale data.
        /// </summary>
        /// <value><see cref="NamedCollection{T}"/> containing the scale data.</value>
        public NamedCollection<Vector3AnimData> Scales { get; private set; }

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

            visitor.VisitAnimFrame_PosRotScale(this);
        }
    }
}
