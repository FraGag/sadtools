//------------------------------------------------------------------------------
// <copyright file="UV.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    /// <summary>
    /// Represents U and V coordinates.
    /// </summary>
    public struct UV : IEquatable<UV>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UV" /> structure using the supplied values.
        /// </summary>
        /// <param name="u">Value of the U coordinate.</param>
        /// <param name="v">Value of the V coordinate.</param>
        public UV(short u, short v)
            : this()
        {
            this.U = u;
            this.V = v;
        }

        /// <summary>
        /// Gets or sets the value of the U coordinate.
        /// </summary>
        /// <value>The value of the U coordinate.</value>
        public short U { get; set; }

        /// <summary>
        /// Gets or sets the value of the V coordinate.
        /// </summary>
        /// <value>The value of the V coordinate.</value>
        public short V { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="UV"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="UV"/> object.</param>
        /// <param name="right">Second <see cref="UV"/> object.</param>
        /// <returns><c>true</c> if both <see cref="UV"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(UV left, UV right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="UV"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="UV"/> object.</param>
        /// <param name="right">Second <see cref="UV"/> object.</param>
        /// <returns><c>true</c> if both <see cref="UV"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(UV left, UV right)
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
            UV? uv = obj as UV?;
            if (uv.HasValue)
            {
                return this.Equals(uv.GetValueOrDefault());
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
        public bool Equals(UV other)
        {
            return this.U == other.U &&
                this.V == other.V;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SecuritySafeCritical]
        public override unsafe int GetHashCode()
        {
            fixed (UV* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(UV));
            }
        }
    }
}
