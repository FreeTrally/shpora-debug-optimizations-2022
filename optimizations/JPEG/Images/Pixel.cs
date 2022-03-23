﻿using System;
using System.Linq;

namespace JPEG.Images
{
    public readonly struct Pixel
    {
        private readonly PixelFormat format;

        public Pixel(double firstComponent, double secondComponent, double thirdComponent, PixelFormat pixelFormat)
        {
            if (!new[]{PixelFormat.RGB, PixelFormat.YCbCr}.Contains(pixelFormat))
                throw new FormatException("Unknown pixel format: " + pixelFormat);

            format = pixelFormat;
            first = firstComponent;
            second = secondComponent;
            third = thirdComponent;
        }

        private readonly double first;
        private readonly double second;
        private readonly double third;

        public double R => format == PixelFormat.RGB ? first : (298.082 * first + 408.583 * Cr) / 256.0 - 222.921;
        public double G => format == PixelFormat.RGB ? second : (298.082 * Y - 100.291 * Cb - 208.120 * Cr) / 256.0 + 135.576;
        public double B => format == PixelFormat.RGB ? third : (298.082 * Y + 516.412 * Cb) / 256.0 - 276.836;

        public double Y => format == PixelFormat.YCbCr ? first : 16.0 + (65.738 * R + 129.057 * G + 24.064 * B) / 256.0;
        public double Cb => format == PixelFormat.YCbCr ? second : 128.0 + (-37.945 * R - 74.494 * G + 112.439 * B) / 256.0;
        public double Cr => format == PixelFormat.YCbCr ? third : 128.0 + (112.439 * R - 94.154 * G - 18.285 * B) / 256.0;
    }
}