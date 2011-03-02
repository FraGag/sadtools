//------------------------------------------------------------------------------
// <copyright file="MATERIAL.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;
    using System.Security;

    // TODO: document field MATERIAL.Unknown08
    // TODO: document field MATERIAL.Unknown10
    // TODO: document the other flags in MATERIAL.Flags
    // TODO: document field MATERIAL.Unknown13

    /// <summary>
    /// Defines a material.
    /// </summary>
    public struct MATERIAL : IEquatable<MATERIAL>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MATERIAL" /> structure using the supplied values.
        /// </summary>
        /// <param name="diffuseColor">Diffuse color of the material.</param>
        /// <param name="specularColor">Specular color of the material.</param>
        /// <param name="unknown08">Unknown value at offset 0x08.</param>
        /// <param name="textureId">ID of the texture used by the material.</param>
        /// <param name="unknown10">Unknown value at offset 0x10.</param>
        /// <param name="flags">Flags concerning UV clamping.</param>
        /// <param name="unknown13">Unknown value at offset 0x13.</param>
        [CLSCompliant(false)]
        public MATERIAL(uint diffuseColor, uint specularColor, float unknown08, uint textureId, ushort unknown10, byte flags, byte unknown13)
            : this()
        {
            this.DiffuseColor = diffuseColor;
            this.SpecularColor = specularColor;
            this.Unknown08 = unknown08;
            this.TextureId = textureId;
            this.Unknown10 = unknown10;
            this.Flags = flags;
            this.Unknown13 = unknown13;
        }

        /// <summary>
        /// Gets or sets the diffuse color of the material.
        /// </summary>
        /// <value>The diffuse color of the material, in 0xAARRGGBB format.</value>
        [CLSCompliant(false)]
        public uint DiffuseColor { get; set; }

        /// <summary>
        /// Gets or sets the specular color of the material.
        /// </summary>
        /// <value>The specular color of the material, in 0xAARRGGBB format.</value>
        [CLSCompliant(false)]
        public uint SpecularColor { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        public float Unknown08 { get; set; }

        /// <summary>
        /// Gets or sets the ID of the texture used by the material.
        /// </summary>
        /// <value>The ID of the texture.</value>
        [CLSCompliant(false)]
        public uint TextureId { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        [CLSCompliant(false)]
        public ushort Unknown10 { get; set; }

        /// <summary>
        /// Gets or sets flags concerning UV clamping.
        /// </summary>
        /// <value>Flags concerning UV clamping.</value>
        /// <remarks>
        /// This field is a bitfield with the following flags:
        /// <list type="table">
        ///     <listheader>
        ///         <item>
        ///             <term>Flag value</term>
        ///             <description>Flag description</description>
        ///         </item>
        ///     </listheader>
        ///     <item>
        ///         <term>0x10</term>
        ///         <description>Enable alpha blending</description>
        ///     </item>
        ///     <item>
        ///         <term>0x40</term>
        ///         <description>Enable sphere mapping</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public byte Flags { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        public byte Unknown13 { get; set; }

        /// <summary>
        /// Indicates whether the two <see cref="MATERIAL"/> objects are equal.
        /// </summary>
        /// <param name="left">First <see cref="MATERIAL"/> object.</param>
        /// <param name="right">Second <see cref="MATERIAL"/> object.</param>
        /// <returns><c>true</c> if both <see cref="MATERIAL"/> objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MATERIAL left, MATERIAL right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the two <see cref="MATERIAL"/> objects are different.
        /// </summary>
        /// <param name="left">First <see cref="MATERIAL"/> object.</param>
        /// <param name="right">Second <see cref="MATERIAL"/> object.</param>
        /// <returns><c>true</c> if both <see cref="MATERIAL"/> objects are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MATERIAL left, MATERIAL right)
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
            MATERIAL? material = obj as MATERIAL?;
            if (material.HasValue)
            {
                return this.Equals(material.GetValueOrDefault());
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
        public bool Equals(MATERIAL other)
        {
            return this.DiffuseColor == other.DiffuseColor &&
                this.SpecularColor == other.SpecularColor &&
                this.Unknown08 == other.Unknown08 &&
                this.TextureId == other.TextureId &&
                this.Unknown10 == other.Unknown10 &&
                this.Flags == other.Flags &&
                this.Unknown13 == other.Unknown13;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SecuritySafeCritical]
        public override unsafe int GetHashCode()
        {
            fixed (MATERIAL* ptr = &this)
            {
                return FowlerNollVoHash.Fnv32((byte*)ptr, sizeof(MATERIAL));
            }
        }
    }
}
