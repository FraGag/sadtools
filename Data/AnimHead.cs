//------------------------------------------------------------------------------
// <copyright file="AnimHead.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    /// <summary>
    /// Represents an animation on an object.
    /// </summary>
    public sealed class AnimHead : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimHead" /> class.
        /// </summary>
        public AnimHead()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimHead" /> class using the supplied values.
        /// </summary>
        /// <param name="name">Name of the AnimHead.</param>
        /// <param name="model"><see cref="OBJECT"/> associated with the animation.</param>
        /// <param name="motion"><see cref="AnimHead2"/> containing the animation data.</param>
        public AnimHead(string name, OBJECT model, AnimHead2 motion)
        {
            this.Name = name;
            this.Model = model;
            this.Motion = motion;
        }

        /// <summary>
        /// Gets or sets the name of the AnimHead.
        /// </summary>
        /// <value>The name of the AnimHead.</value>
        /// <remarks>This property is metadata. It can be used to give a meaningful name to the object.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the object associated with the animation.
        /// </summary>
        /// <value>An <see cref="OBJECT"/> associated with the animation.</value>
        public OBJECT Model { get; set; }

        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        /// <value>An <see cref="AnimHead2"/>.</value>
        public AnimHead2 Motion { get; set; }
    }
}
