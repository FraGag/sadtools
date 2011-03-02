//------------------------------------------------------------------------------
// <copyright file="VirtualMemoryStream.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.PCTools
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Emulates a 32-bit virtual memory address space as a stream.
    /// </summary>
    internal sealed class VirtualMemoryStream : Stream
    {
        /// <summary>
        /// <see cref="Stream"/> providing the contents of an executable image.
        /// </summary>
        private Stream underlyingStream;

        /// <summary>
        /// Base address of the image.
        /// </summary>
        private long imageBase;

        /// <summary>
        /// Set of the sections in the executable.
        /// </summary>
        private SortedSet<ImageSection> sections;

        /// <summary>
        /// Current position in the virtual memory address space.
        /// </summary>
        private long position;

        /// <summary>
        /// Flag indicating whether the object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMemoryStream"/> class.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> providing the contents of an executable image.</param>
        /// <param name="imageBase">Base address of the image.</param>
        /// <param name="sections">Set of the sections in the executable.</param>
        public VirtualMemoryStream(Stream stream, long imageBase, SortedSet<ImageSection> sections)
        {
            this.underlyingStream = stream;
            this.imageBase = imageBase;
            this.sections = sections;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value>Always <c>true</c>, indicating that the stream supports reading.</value>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value>Always <c>true</c>, indicating that the stream supports seeking.</value>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value>Always <c>false</c>, indicating that the stream does not support writing.</value>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>A long integer equal to 2147483648 (0x80000000).</value>
        /// <exception cref="ObjectDisposedException">
        /// Operations on a disposed <see cref="VirtualMemoryStream"/> instance are not allowed.
        /// </exception>
        public override long Length
        {
            get
            {
                this.ThrowIfDisposed();
                return 0x80000000;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>The current position within the stream.</value>
        /// <exception cref="ObjectDisposedException">
        /// Operations on a disposed <see cref="VirtualMemoryStream"/> instance are not allowed.
        /// </exception>
        public override long Position
        {
            get
            {
                this.ThrowIfDisposed();
                return this.position;
            }

            set
            {
                this.ThrowIfDisposed();
                this.position = value;
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <remarks>For <see cref="VirtualMemoryStream"/> objects, this method does nothing.</remarks>
        public override void Flush()
        {
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes
        /// read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with the values between
        /// <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read
        /// from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many
        /// bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Operations on a disposed <see cref="VirtualMemoryStream"/> instance are not allowed.
        /// </exception>
        /// <exception cref="IOException">The data being read is located outside of a section in the image.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException(Properties.Resources.OffsetPlusCountGreaterThanBufferLength);
            }

            this.ThrowIfDisposed();

            // Early return (avoids a potential exception when calling GetCurrentSection)
            if (count == 0)
            {
                return 0;
            }

            // Loop until all bytes have been read (we need to loop when we have to read across sections)
            for (;;)
            {
                // Find the section we are currently reading from
                ImageSection section = this.GetCurrentSection();

                // When reading past the end of the raw data but before the end of the section, read zeroes
                long startOfSection = this.imageBase + section.VirtualAddress;
                long endOfRawData = startOfSection + section.SizeOfRawData;
                long endOfSection = startOfSection + section.VirtualSize;

                if (this.position + count < endOfRawData)
                {
                    // All the data we have to read is in the section's raw data
                    this.underlyingStream.Seek(this.position - startOfSection + section.PointerToRawData, SeekOrigin.Begin);
                    this.underlyingStream.Read(buffer, offset, count);
                    this.position += count;
                    break;
                }

                int countInRawData = 0;
                if (this.position < endOfRawData)
                {
                    // A part of the data is in the section's raw data
                    this.underlyingStream.Seek(this.position - startOfSection + section.PointerToRawData, SeekOrigin.Begin);
                    countInRawData = checked((int)(endOfRawData - this.position));
                    this.underlyingStream.Read(buffer, offset, countInRawData);

                    if (this.position + count < endOfSection)
                    {
                        // All the rest of the data is in the section, but beyond the end of the raw data, so read zeroes
                        for (int i = countInRawData; i < count; i++)
                        {
                            buffer[offset + i] = 0;
                        }

                        this.position += count;
                        break;
                    }
                }

                if (this.position + count < endOfSection)
                {
                    // Read zeroes until the end of the requested range
                    for (int i = countInRawData; i < count; i++)
                    {
                        buffer[offset + i] = 0;
                    }

                    this.position += count;
                    break;
                }

                // Read zeroes until the end of the section
                int countInSection = checked((int)(endOfSection - this.position));
                for (int i = countInRawData; i < countInSection; i++)
                {
                    buffer[offset + i] = 0;
                }

                // We have not read everything yet
                this.position += countInSection;
                count -= countInSection;
            }

            return count;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin" /> indicating the reference point used to obtain the new position.
        /// </param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="ObjectDisposedException">
        /// Operations on a disposed <see cref="VirtualMemoryStream"/> instance are not allowed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.ThrowIfDisposed();

            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.position = offset;
                    break;
                case SeekOrigin.Current:
                    this.position += offset;
                    break;
                case SeekOrigin.End:
                    this.position = 0x80000000 + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }

            return this.position;
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="NotSupportedException">
        /// Setting the length of a <see cref="VirtualMemoryStream"/> is not supported.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(Properties.Resources.VirtualMemoryStreamSetLengthNotSupported);
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
        /// position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current
        /// stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="NotSupportedException">Writing to a <see cref="VirtualMemoryStream"/> is not supported.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(Properties.Resources.VirtualMemoryStreamWriteNotSupported);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="VirtualMemoryStream" /> and optionally releases the managed
        /// resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // Release unmanaged resources
                this.underlyingStream.Dispose();

                if (disposing)
                {
                    // Release managed resources
                    this.underlyingStream = null;
                    this.sections = null;
                }

                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the <see cref="ImageSection"/> in which the stream is currently positioned.
        /// </summary>
        /// <returns>
        /// An <see cref="ImageSection"/> corresponding to the section in which the stream is current positioned.
        /// </returns>
        /// <exception cref="IOException">The current address is located outside of a section in the image.</exception>
        private ImageSection GetCurrentSection()
        {
            foreach (ImageSection section in this.sections)
            {
                long virtualAddress = this.imageBase + section.VirtualAddress;
                if (this.position >= virtualAddress && this.position < virtualAddress + section.VirtualSize)
                {
                    return section;
                }
            }

            throw new IOException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.VirtualMemoryStreamOutOfASection, this.position.ToString("X8", CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the <see cref="VirtualMemoryStream"/> instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Operations on a disposed <see cref="VirtualMemoryStream"/> instance are not allowed.
        /// </exception>
        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(Properties.Resources.PEReaderDisposed);
            }
        }
    }
}
