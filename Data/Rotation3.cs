//------------------------------------------------------------------------------
// <copyright file="Rotation3.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    /// <summary>
    /// Represents a rotation transformation in a 3-dimensional space.
    /// </summary>
    public struct Rotation3 : IEquatable<Rotation3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rotation3" /> structure using the supplied values.
        /// </summary>
        /// <param name="x">Rotation value on the X axis.</param>
        /// <param name="y">Rotation value on the Y axis.</param>
        /// <param name="z">Rotation value on the Z axis.</param>
        public Rotation3(int x, int y, int z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the rotation value on the X axis.
        /// </summary>
        /// <value>The rotation value on the X axis.</value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the rotation value on the Y axis.
        /// </summary>
        /// <value>The rotation value on the Y axis.</value>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the rotation value on the Z axis.
        /// </summary>
        /// <value>The rotation value on the Z axis.</value>
        public int Z { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="Rotation3"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="Rotation3"/> object.</param>
        /// <param name="right">Second <see cref="Rotation3"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Rotation3"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Rotation3 left, Rotation3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="Rotation3"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="Rotation3"/> object.</param>
        /// <param name="right">Second <see cref="Rotation3"/> object.</param>
        /// <returns><c>true</c> if both <see cref="Rotation3"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Rotation3 left, Rotation3 right)
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
            Rotation3? rotation3 = obj as Rotation3?;
            if (rotation3.HasValue)
            {
                return this.Equals(rotation3.GetValueOrDefault());
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
        public bool Equals(Rotation3 other)
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
            fixed (Rotation3* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(Rotation3));
            }
        }
    }
}
