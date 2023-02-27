using System;
using Xunit;

namespace Tests {
    public class Utils {
        #region CompareWithRef
        public static void CompareWithRef(PSILib.MyImage img, string reference) {
            img.Save(GetPath($"out/{reference}.bmp"));
            if (!System.IO.File.Exists(GetPath($"ref/{reference}.bmp"))) {
                img.Save(GetPath($"ref/{reference}.bmp"));
                return;
            }
            var imgRef = new PSILib.MyImage(GetPath($"ref/{reference}.bmp"));

            if (img.Width != imgRef.Width || img.Height != imgRef.Height) {
                throw new Exception($"Check failed for {reference} : Images are not the same size : {img.Width}x{img.Height} vs {imgRef.Width}x{imgRef.Height}.");
            }

            var img2 = img.Clone();
            if (!img2.Diff(imgRef)) {
                var errorFile = $"err/{reference}.bmp";
                img2.Save(GetPath(errorFile));
                throw new Exception($"Check failed for {reference} : Images are not the same, see {errorFile} for the diff.");
            }
            
        }
        #endregion

        #region Path
        public static string GetPath(string path) {
            return System.IO.Path.Combine(System.Environment.CurrentDirectory, "../../../..", path);
        }
        public static void InitOutputFolder() {
            // create out & err folders if they don't exist
            string outPath = Utils.GetPath("out");
            if (!System.IO.Directory.Exists(outPath))
                System.IO.Directory.CreateDirectory(outPath);

            string errPath = Utils.GetPath("err");
            if (!System.IO.Directory.Exists(errPath))
                System.IO.Directory.CreateDirectory(errPath);
            
            // delete all files in the error folder
            foreach (var file in System.IO.Directory.GetFiles(errPath)) {
                System.IO.File.Delete(file);
            }
        }
        #endregion

        #region misc - unused
        /// <summary>
        /// Compare two images.
        /// </summary>
        /// <param name="name">The name of the images to compare.</param>
        public void CompareImages(string name) {
            var img1 = new PSILib.MyImage(System.IO.Path.Combine("out", name));
            var img2 = new PSILib.MyImage(System.IO.Path.Combine("ref", name));

            if (img1.Width != img2.Width || img1.Height != img2.Height)
            {
                throw new Exception($"Check failed for {name} : Images are not the same size.");
            }

            bool isSame = img1.Diff(img2);

            if (!isSame)
            {
                var errorFile = "DiffFailed.bmp";
                img1.Save(System.IO.Path.Combine(errorFile));
                throw new Exception($"Check failed for {name} : Images are not the same, see {errorFile} for the diff.");
            }
        }

        /// <summary>
        /// Compare all images in a folder with a reference image.
        /// </summary>
        /// <param name="output">The folder containing the images to compare.</param>
        /// <param name="reference">The folder containing the reference images.</param>
        public void CompareBulkImages() {
            var files = System.IO.Directory.GetFiles("ref");
            foreach (var file in files)
            {
                CompareImages(System.IO.Path.GetFileName(file));
            }
        }
        #endregion
    }
}
