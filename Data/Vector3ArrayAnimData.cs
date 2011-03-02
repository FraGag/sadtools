//------------------------------------------------------------------------------
// <copyright file="Vector3ArrayAnimData.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents a change on an array of Vector3 structures.
    /// </summary>
    public struct Vector3ArrayAnimData : IEquatable<Vector3ArrayAnimData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3ArrayAnimData" /> structure using the supplied values.
        /// </summary>
        /// <param name="frameNumber">Frame number in the animation.</param>
        /// <param name="vectors">Vector that composes this animation frame.</param>
        public Vector3ArrayAnimData(int frameNumber, NamedCollection<Vector3> vectors)
            : this()
        {
            this.FrameNumber = frameNumber;
            this.Vectors = vectors;
        }

        /// <summary>
        /// Gets or sets the frame number in the animation.
        /// </summary>
        /// <value>The frame number in the animation.</value>
        public int FrameNumber { get; set; }

        /// <summary>
        /// Gets a collection of the vectors that compose this animation frame.
        /// </summary>
        /// <value><see cref="NamedCollection{T}"/> for the animation frame.</value>
        public NamedCollection<Vector3> Vectors { get; private set; }

        /// <summary>
        /// Indicates whether the two <see cref="Vector3ArrayAnimData"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="Vector3ArrayAnimData"/> object.</param>
        /// <param name="right">Second <see cref="Vector3ArrayAnimData"/> object.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Vector3ArrayAnimData"/> objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Vector3ArrayAnimData left, Vector3ArrayAnimData right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="Vector3ArrayAnimData"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="Vector3ArrayAnimData"/> object.</param>
        /// <param name="right">Second <see cref="Vector3ArrayAnimData"/> object.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Vector3ArrayAnimData"/> objects are different; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Vector3ArrayAnimData left, Vector3ArrayAnimData right)
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
            Vector3ArrayAnimData? vector3ArrayAnimData = obj as Vector3ArrayAnimData?;
            if (vector3ArrayAnimData.HasValue)
            {
                return this.Equals(vector3ArrayAnimData.GetValueOrDefault());
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
        public bool Equals(Vector3ArrayAnimData other)
        {
            return this.FrameNumber == other.FrameNumber &&
                this.Vectors == other.Vectors;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
