//------------------------------------------------------------------------------
// <copyright file="FowlerNollVoHash.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.Data
{
    using System.Security;

    /// <summary>
    /// Contains implementations of the Fowler–Noll–Vo hash function.
    /// </summary>
    /// <remarks>
    /// The Fowler–Noll–Vo hash function is described at
    /// <a href="http://isthe.com/chongo/tech/comp/fnv/">http://isthe.com/chongo/tech/comp/fnv/</a>.
    /// </remarks>
    internal static class FowlerNollVoHash
    {
        /// <summary>
        /// Computes the hash code for an array of bytes using the FNV-1a hash function.
        /// </summary>
        /// <param name="ptr">Pointer to the first byte in the array.</param>
        /// <param name="size">Size of the array of bytes.</param>
        /// <returns>A 32-bit signed integer that is the hash code for the array of bytes.</returns>
        [SecurityCritical]
        public static unsafe int Fnv32(byte* ptr, int size)
        {
            const uint Prime = 0x1000193;
            uint hash = 0x811c9dc5;

            byte* endPtr = ptr + size;
            while (ptr < endPtr)
            {
                hash ^= *ptr;
                hash *= Prime;
                ++ptr;
            }

            return unchecked((int)hash);
        }
    }
}
