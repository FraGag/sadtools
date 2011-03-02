//------------------------------------------------------------------------------
// <copyright file="PolyNormal.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    // TODO: document structure PolyNormal
    // TODO: document field PolyNormal.Unknown00
    // TODO: document field PolyNormal.Unknown04

    /// <summary>
    /// The purpose of this structure is unknown.
    /// </summary>
    public struct PolyNormal : IEquatable<PolyNormal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyNormal"/> structure using the supplied values.
        /// </summary>
        /// <param name="unknown00">Unknown value at offset 0x00.</param>
        /// <param name="unknown04">Unknown value at offset 0x04.</param>
        public PolyNormal(float unknown00, float unknown04)
            : this()
        {
            this.Unknown00 = unknown00;
            this.Unknown04 = unknown04;
        }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        public float Unknown00 { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        public float Unknown04 { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="PolyNormal"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="PolyNormal"/> object.</param>
        /// <param name="right">Second <see cref="PolyNormal"/> object.</param>
        /// <returns><c>true</c> if both <see cref="PolyNormal"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(PolyNormal left, PolyNormal right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="PolyNormal"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="PolyNormal"/> object.</param>
        /// <param name="right">Second <see cref="PolyNormal"/> object.</param>
        /// <returns><c>true</c> if both <see cref="PolyNormal"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(PolyNormal left, PolyNormal right)
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
            PolyNormal? polyNormal = obj as PolyNormal?;
            if (polyNormal.HasValue)
            {
                return this.Equals(polyNormal.GetValueOrDefault());
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
        public bool Equals(PolyNormal other)
        {
            return this.Unknown00 == other.Unknown00 &&
                this.Unknown04 == other.Unknown04;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SecuritySafeCritical]
        public override unsafe int GetHashCode()
        {
            fixed (PolyNormal* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(PolyNormal));
            }
        }
    }
}
