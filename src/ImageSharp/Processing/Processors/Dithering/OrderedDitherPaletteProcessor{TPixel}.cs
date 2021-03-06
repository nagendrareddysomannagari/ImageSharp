// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering
{
    /// <summary>
    /// An <see cref="IImageProcessor{TPixel}"/> that dithers an image using error diffusion.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    internal class OrderedDitherPaletteProcessor<TPixel> : PaletteDitherProcessor<TPixel>
        where TPixel : struct, IPixel<TPixel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDitherPaletteProcessor{TPixel}"/> class.
        /// </summary>
        /// <param name="definition">The <see cref="OrderedDitherPaletteProcessor"/> defining the processor parameters.</param>
        /// <param name="source">The source <see cref="Image{TPixel}"/> for the current processor instance.</param>
        /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
        public OrderedDitherPaletteProcessor(OrderedDitherPaletteProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
            : base(definition, source, sourceRectangle)
        {
        }

        private new OrderedDitherPaletteProcessor Definition => (OrderedDitherPaletteProcessor)base.Definition;

        /// <inheritdoc/>
        protected override void OnFrameApply(ImageFrame<TPixel> source)
        {
            var interest = Rectangle.Intersect(this.SourceRectangle, source.Bounds());
            int startY = interest.Y;
            int endY = interest.Bottom;
            int startX = interest.X;
            int endX = interest.Right;

            // Collect the values before looping so we can reduce our calculation count for identical sibling pixels
            TPixel sourcePixel = source[startX, startY];
            TPixel previousPixel = sourcePixel;
            PixelPair<TPixel> pair = this.GetClosestPixelPair(ref sourcePixel);
            Rgba32 rgba = default;
            sourcePixel.ToRgba32(ref rgba);

            // Convert to grayscale using ITU-R Recommendation BT.709 if required
            byte luminance = ImageMaths.Get8BitBT709Luminance(rgba.R, rgba.G, rgba.B);

            for (int y = startY; y < endY; y++)
            {
                Span<TPixel> row = source.GetPixelRowSpan(y);

                for (int x = startX; x < endX; x++)
                {
                    sourcePixel = row[x];

                    // Check if this is the same as the last pixel. If so use that value
                    // rather than calculating it again. This is an inexpensive optimization.
                    if (!previousPixel.Equals(sourcePixel))
                    {
                        pair = this.GetClosestPixelPair(ref sourcePixel);

                        // No error to spread, exact match.
                        if (sourcePixel.Equals(pair.First))
                        {
                            continue;
                        }

                        sourcePixel.ToRgba32(ref rgba);
                        luminance = ImageMaths.Get8BitBT709Luminance(rgba.R, rgba.G, rgba.B);

                        // Setup the previous pointer
                        previousPixel = sourcePixel;
                    }

                    this.Definition.Dither.Dither(source, sourcePixel, pair.Second, pair.First, luminance, x, y);
                }
            }
        }
    }
}
