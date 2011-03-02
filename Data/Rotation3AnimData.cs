//------------------------------------------------------------------------------
// <copyright file="Rotation3AnimData.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    /// <summary>
    /// Represents a change on a Rotation3 structure.
    /// </summary>
    public struct Rotation3AnimData : IEquatable<Rotation3AnimData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rotation3AnimData" /> structure using the supplied values.
        /// </summary>
        /// <param name="frameNumber">Frame number in the animation.</param>
        /// <param name="rotation">Rotation that composes this animation frame.</param>
        public Rotation3AnimData(int frameNumber, Rotation3 rotation)
            : this()
        {
            this.FrameNumber = frameNumber;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Gets or sets the frame number in the animation.
        /// </summary>
        /// <value>The frame number in the animation.</value>
        public int FrameNumber { get; set; }

        /// <summary>
        /// Gets or sets the rotation that composes this animation frame.
        /// </summary>
        /// <value><see cref="Rotation3"/> for the animation frame.</value>
        public Rotation3 Rotation { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="Rotation3AnimData"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="Rotation3AnimData"/> object.</param>
        /// <param name="right">Second <see cref="Rotation3AnimData"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Rotation3AnimData"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Rotation3AnimData left, Rotation3AnimData right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="Rotation3AnimData"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="Rotation3AnimData"/> object.</param>
        /// <param name="right">Second <see cref="Rotation3AnimData"/> object.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Rotation3AnimData"/> objects are different; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Rotation3AnimData left, Rotation3AnimData right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise,
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Rotation3AnimData? rotation3AnimData = obj as Rotation3AnimData?;
            if (rotation3AnimData.HasValue)
            {
                return this.Equals(rotation3AnimData.GetValueOrDefault());
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Rotation3AnimData other)
        {
            return this.FrameNumber == other.FrameNumber &&
                this.Rotation == other.Rotation;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SecuritySafeCritical]
        public override unsafe int GetHashCode()
        {
            fixed (Rotation3AnimData* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(Rotation3AnimData));
            }
        }
    }
}
