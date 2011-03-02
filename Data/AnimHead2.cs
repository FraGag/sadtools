//------------------------------------------------------------------------------
// <copyright file="AnimHead2.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    // TODO: document the other flags in AnimHead2.Flags, if any
    // TODO: document field AnimHead2.Unknown0A

    /// <summary>
    /// Represents an animation.
    /// </summary>
    public sealed class AnimHead2 : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimHead2" /> class.
        /// </summary>
        public AnimHead2()
        {
            this.FrameData = new NamedCollection<AnimFrame>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimHead2" /> class.
        /// </summary>
        /// <param name="name">Name of the animation.</param>
        /// <param name="frameData">Animation frame data.</param>
        /// <param name="frameCount">Total number of frames in the animation.</param>
        /// <param name="flags">Flags indicating the type of data that the current animation contains.</param>
        /// <param name="unknown0A">Unknown value at offset 0x0A.</param>
        [CLSCompliant(false)]
        public AnimHead2(string name, NamedCollection<AnimFrame> frameData, int frameCount, ushort flags, ushort unknown0A)
        {
            this.Name = name;
            this.FrameData = frameData;
            this.FrameCount = frameCount;
            this.Flags = flags;
            this.Unknown0A = unknown0A;
        }

        /// <summary>
        /// Gets or sets the name of the animation.
        /// </summary>
        /// <value>The name of the animation.</value>
        /// <remarks>This property is metadata. It can be used to give a meaningful name to the object.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets a collection of the animation frame data.
        /// </summary>
        /// <value>An instance of <see cref="NamedCollection{T}"/> containing the data for each animation frame.</value>
        public NamedCollection<AnimFrame> FrameData { get; private set; }

        /// <summary>
        /// Gets or sets the total number of frames in the animation.
        /// </summary>
        /// <value>The total number of frames in the animation.</value>
        public int FrameCount { get; set; }

        /// <summary>
        /// Gets or sets flags indicating the type of data that the current animation contains.
        /// </summary>
        /// <value>A value indicating the type of data that the current animation contains.</value>
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
        ///         <term>0x1</term>
        ///         <description>Contains position data</description>
        ///     </item>
        ///     <item>
        ///         <term>0x2</term>
        ///         <description>Contains rotation data</description>
        ///     </item>
        ///     <item>
        ///         <term>0x4</term>
        ///         <description>Contains scale data</description>
        ///     </item>
        ///     <item>
        ///         <term>0x30</term>
        ///         <description>Contains vertex and normal data</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [CLSCompliant(false)]
        public ushort Flags { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>An unknown value.</value>
        /// <remarks>The purpose of this field is currently unknown.</remarks>
        [CLSCompliant(false)]
        public ushort Unknown0A { get; set; }
    }
}
