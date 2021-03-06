﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization
{
    /// <summary>
    /// Allows the quantization of images pixels using Octrees.
    /// <see href="http://msdn.microsoft.com/en-us/library/aa479306.aspx"/>
    /// <para>
    /// By default the quantizer uses <see cref="KnownDiffusers.FloydSteinberg"/> dithering and a color palette of a maximum length of <value>255</value>
    /// </para>
    /// </summary>
    public class OctreeQuantizer : IQuantizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeQuantizer"/> class.
        /// </summary>
        public OctreeQuantizer()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeQuantizer"/> class.
        /// </summary>
        /// <param name="maxColors">The maximum number of colors to hold in the color palette.</param>
        public OctreeQuantizer(int maxColors)
            : this(GetDiffuser(true), maxColors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeQuantizer"/> class.
        /// </summary>
        /// <param name="dither">Whether to apply dithering to the output image.</param>
        public OctreeQuantizer(bool dither)
            : this(GetDiffuser(dither), QuantizerConstants.MaxColors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeQuantizer"/> class.
        /// </summary>
        /// <param name="maxColors">The maximum number of colors to hold in the color palette.</param>
        /// <param name="dither">Whether to apply dithering to the output image.</param>
        public OctreeQuantizer(bool dither, int maxColors)
            : this(GetDiffuser(dither), maxColors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeQuantizer"/> class.
        /// </summary>
        /// <param name="diffuser">The error diffusion algorithm, if any, to apply to the output image.</param>
        public OctreeQuantizer(IErrorDiffuser diffuser)
            : this(diffuser, QuantizerConstants.MaxColors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeQuantizer"/> class.
        /// </summary>
        /// <param name="diffuser">The error diffusion algorithm, if any, to apply to the output image.</param>
        /// <param name="maxColors">The maximum number of colors to hold in the color palette.</param>
        public OctreeQuantizer(IErrorDiffuser diffuser, int maxColors)
        {
            this.Diffuser = diffuser;
            this.MaxColors = maxColors.Clamp(QuantizerConstants.MinColors, QuantizerConstants.MaxColors);
        }

        /// <inheritdoc />
        public IErrorDiffuser Diffuser { get; }

        /// <summary>
        /// Gets the maximum number of colors to hold in the color palette.
        /// </summary>
        public int MaxColors { get; }

        /// <param name="configuration"></param>
        /// <inheritdoc />
        public IFrameQuantizer<TPixel> CreateFrameQuantizer<TPixel>(Configuration configuration)
            where TPixel : struct, IPixel<TPixel>
            => new OctreeFrameQuantizer<TPixel>(this);

        /// <inheritdoc/>
        public IFrameQuantizer<TPixel> CreateFrameQuantizer<TPixel>(Configuration configuration, int maxColors)
            where TPixel : struct, IPixel<TPixel>
        {
            maxColors = maxColors.Clamp(QuantizerConstants.MinColors, QuantizerConstants.MaxColors);
            return new OctreeFrameQuantizer<TPixel>(this, maxColors);
        }

        private static IErrorDiffuser GetDiffuser(bool dither) => dither ? KnownDiffusers.FloydSteinberg : null;
    }
}