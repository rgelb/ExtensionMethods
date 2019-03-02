using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace Widgets.Images {

    /// <summary>
    /// Extension methods to help with image manipulation
    /// </summary>
    public static class ImageExtensions {

        public static void DrawImage(this Image thisImage, Image imageToDraw, PointF location) {
            using (var g = Graphics.FromImage(thisImage)) {
                g.DrawImage(imageToDraw, location);
            }
        }

        public static void DrawRectangles(this Image thisImage, List<RectangleF> rectangles, Color color) {
            using (var g = Graphics.FromImage(thisImage)) {
                var brush = new SolidBrush(color);
                g.FillRectangles(brush, rectangles.ToArray());

            }
        }

        public static void DrawRectangles(this Image thisImage, List<Rectangle> rectangles, Color color) {
            using (var g = Graphics.FromImage(thisImage)) {
                var brush = new SolidBrush(color);
                g.FillRectangles(brush, rectangles.ToArray());
            }
        }

        public static void DrawText(this Image thisImage, Color color, RectangleF rect, Font font, string text) {
            using (var g = Graphics.FromImage(thisImage)) {
                var brush = new SolidBrush(color);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                var sf = new StringFormat {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(text, font, brush, rect, sf);
            }
        }

        public static Image Resize(this Image thisImage, int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(thisImage.HorizontalResolution, thisImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(thisImage, destRect, 0, 0, thisImage.Width, thisImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image Crop(this Image thisImage, int width, int height) {
            Rectangle cropArea = new Rectangle(0, 0, width, height);
            Bitmap bitmap = new Bitmap(thisImage);
            return bitmap.Clone(cropArea, bitmap.PixelFormat);
        }

        public static Image TrimOnBottom(this Image thisImage, Color? backgroundColor = null, int margin = 30) {
            var bitmap = (Bitmap) thisImage;
            int foundContentOnRow = -1;

            // handle empty optional parameter
            var backColor = backgroundColor ?? Color.White;

            // scan the image from the bottom up, left to right
            for (int y = bitmap.Height - 1; y >= 0; y--) {
                for (int x = 0; x < bitmap.Width; x++) {
                    Color color = bitmap.GetPixel(x, y);
                    if (color.R != backColor.R || color.G != backColor.G || color.B != backColor.B) {
                        foundContentOnRow = y;
                        break;
                    }
                }

                // exit loop if content found
                if (foundContentOnRow > -1) {
                    break;
                }
            }

            if (foundContentOnRow > -1) {
                int proposedHeight = foundContentOnRow + margin;

                // only trim if proposed height smaller than existing image
                if (proposedHeight < bitmap.Height) {
                    return thisImage.Crop(bitmap.Width, proposedHeight);
                }
            }

            // if trim isn't possible, return the original image
            return thisImage;
        }

    }

}
