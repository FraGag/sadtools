//------------------------------------------------------------------------------
// <copyright file="NamedCollection.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a collection of objects that has a name.
    /// </summary>
    /// <typeparam name="T">The type of objects contained in the collection.</typeparam>
    public class NamedCollection<T> : Collection<T>, INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollection{T}"/> class.
        /// </summary>
        public NamedCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollection{T}"/> class as a wrapper for the specified list.
        /// </summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        public NamedCollection(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public string Name { get; set; }
    }
}
