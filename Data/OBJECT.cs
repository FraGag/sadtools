//------------------------------------------------------------------------------
// <copyright file="OBJECT.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System;

    /// <summary>
    /// Represents an object in an object tree.
    /// </summary>
    public sealed class OBJECT : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBJECT" /> class.
        /// </summary>
        public OBJECT()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OBJECT" /> class using the supplied values.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="flags">Flags that apply to the object.</param>
        /// <param name="attach">Model associated with the object.</param>
        /// <param name="position">Relative position of the object.</param>
        /// <param name="rotation">Rotation of the object.</param>
        /// <param name="scale">Scale of the object.</param>
        /// <param name="child">Child object of the object.</param>
        /// <param name="sibling">Sibling object of the object.</param>
        [CLSCompliant(false)]
        public OBJECT(string name, uint flags, ATTACH attach, Vector3 position, Rotation3 rotation, Vector3 scale, OBJECT child, OBJECT sibling)
        {
            this.Name = name;
            this.Flags = flags;
            this.Attach = attach;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Child = child;
            this.Sibling = sibling;
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        /// <remarks>This property is metadata. It can be used to give a meaningful name to the object.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the flags that apply to the object.
        /// </summary>
        /// <value>The flags that apply to the object.</value>
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
        ///         <term>0x01</term>
        ///         <description>Don't apply position values. Seems to have no actual effect.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x02</term>
        ///         <description>Don't apply rotation values. Seems to have no actual effect.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x04</term>
        ///         <description>Don't apply scale values. Seems to have no actual effect.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x08</term>
        ///         <description>Don't render this model. Does not affect level geometry.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x10</term>
        ///         <description>This model has no children. Seems to have no actual effect.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x20</term>
        ///         <description>Rotation values are in ZYX order. Seems to have no actual effect.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x40</term>
        ///         <description>Don't include this model for animations. Does not affect level geometry.</description>
        ///     </item>
        ///     <item>
        ///         <term>0x80</term>
        ///         <description>Unknown.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [CLSCompliant(false)]
        public uint Flags { get; set; }

        /// <summary>
        /// Gets or sets the model associated with the object.
        /// </summary>
        /// <value>The model associated with the object.</value>
        public ATTACH Attach { get; set; }

        /// <summary>
        /// Gets or sets the relative position of the object.
        /// </summary>
        /// <value>The relative position of the object.</value>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the object.
        /// </summary>
        /// <value>The rotation of the object.</value>
        public Rotation3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the scale of the object.
        /// </summary>
        /// <value>The scale of the object.</value>
        /// <remarks>This value is ignored for level geometry.</remarks>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the child object of the object.
        /// </summary>
        /// <value>The child object of the object.</value>
        /// <remarks>The child object and all its descendants inherit this object's transforms.</remarks>
        public OBJECT Child { get; set; }

        /// <summary>
        /// Gets or sets the sibling object of the object.
        /// </summary>
        /// <value>The sibling object of the object.</value>
        /// <remarks>The sibling object does not inherit this object's transforms.</remarks>
        public OBJECT Sibling { get; set; }
    }
}
