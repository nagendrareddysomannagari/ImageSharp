// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.Primitives;

namespace SixLabors.ImageSharp.Processing.Processors.Normalization
{
    /// <summary>
    /// Defines a global histogram equalization applicable to an <see cref="Image"/>.
    /// </summary>
    public class GlobalHistogramEqualizationProcessor : HistogramEqualizationProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHistogramEqualizationProcessor"/> class.
        /// </summary>
        /// <param name="luminanceLevels">The number of luminance levels.</param>
        /// <param name="clipHistogram">A value indicating whether to clip the histogram bins at a specific value.</param>
        /// <param name="clipLimit">The histogram clip limit. Histogram bins which exceed this limit, will be capped at this value.</param>
        public GlobalHistogramEqualizationProcessor(int luminanceLevels, bool clipHistogram, int clipLimit)
            : base(luminanceLevels, clipHistogram, clipLimit)
        {
        }

        /// <inheritdoc />
        public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Image<TPixel> source, Rectangle sourceRectangle)
        {
            return new GlobalHistogramEqualizationProcessor<TPixel>(
                this.LuminanceLevels,
                this.ClipHistogram,
                this.ClipLimit,
                source,
                sourceRectangle);
        }
    }
}
