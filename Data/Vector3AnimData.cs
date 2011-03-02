//------------------------------------------------------------------------------
// <copyright file="Vector3AnimData.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    /// <summary>
    /// Represents a change on a Vector3 structure.
    /// </summary>
    public struct Vector3AnimData : IEquatable<Vector3AnimData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3AnimData" /> structure using the supplied values.
        /// </summary>
        /// <param name="frameNumber">Frame number in the animation.</param>
        /// <param name="vector">Vector that composes this animation frame.</param>
        public Vector3AnimData(int frameNumber, Vector3 vector)
            : this()
        {
            this.FrameNumber = frameNumber;
            this.Vector = vector;
        }

        /// <summary>
        /// Gets or sets the frame number in the animation.
        /// </summary>
        /// <value>The frame number in the animation.</value>
        public int FrameNumber { get; set; }

        /// <summary>
        /// Gets or sets the vector that composes this animation frame.
        /// </summary>
        /// <value><see cref="Vector3"/> for the animation frame.</value>
        public Vector3 Vector { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="Vector3AnimData"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="Vector3AnimData"/> object.</param>
        /// <param name="right">Second <see cref="Vector3AnimData"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Vector3AnimData"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Vector3AnimData left, Vector3AnimData right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="Vector3AnimData"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="Vector3AnimData"/> object.</param>
        /// <param name="right">Second <see cref="Vector3AnimData"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Vector3AnimData"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Vector3AnimData left, Vector3AnimData right)
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
            Vector3AnimData? vector3AnimData = obj as Vector3AnimData?;
            if (vector3AnimData.HasValue)
            {
                return this.Equals(vector3AnimData.GetValueOrDefault());
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
        public bool Equals(Vector3AnimData other)
        {
            return this.FrameNumber == other.FrameNumber &&
                this.Vector == other.Vector;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SecuritySafeCritical]
        public override unsafe int GetHashCode()
        {
            fixed (Vector3AnimData* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(Vector3AnimData));
            }
        }
    }
}
