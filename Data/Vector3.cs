//------------------------------------------------------------------------------
// <copyright file="Vector3.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    /// <summary>
    /// Represents a floating-point vector in a 3-dimensional space.
    /// </summary>
    public struct Vector3 : IEquatable<Vector3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3" /> structure using the supplied values.
        /// </summary>
        /// <param name="x">Vector value on the X axis.</param>
        /// <param name="y">Vector value on the Y axis.</param>
        /// <param name="z">Vector value on the Z axis.</param>
        public Vector3(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the vector value on the X axis.
        /// </summary>
        /// <value>The vector value on the X axis.</value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the vector value on the Y axis.
        /// </summary>
        /// <value>The vector value on the Y axis.</value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the vector value on the Z axis.
        /// </summary>
        /// <value>The vector value on the Z axis.</value>
        public float Z { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="Vector3"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="Vector3"/> object.</param>
        /// <param name="right">Second <see cref="Vector3"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Vector3"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="Vector3"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="Vector3"/> object.</param>
        /// <param name="right">Second <see cref="Vector3"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Vector3"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Vector3 left, Vector3 right)
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
            Vector3? vector3 = obj as Vector3?;
            if (vector3.HasValue)
            {
                return this.Equals(vector3.GetValueOrDefault());
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
        public bool Equals(Vector3 other)
        {
            return this.X == other.X &&
                this.Y == other.Y &&
                this.Z == other.Z;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SecuritySafeCritical]
        public override unsafe int GetHashCode()
        {
            fixed (Vector3* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(Vector3));
            }
        }
    }
}
