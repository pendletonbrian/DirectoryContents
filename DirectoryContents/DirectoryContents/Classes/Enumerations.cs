using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DirectoryContents.Classes
{
    public static class Enumerations
    {
        /// <summary>
        /// List of ways to move a page out of view, and a new one into view.
        /// </summary>
        public enum PageTransitionType
        {
            None = 0,

            Fade,

            /// <summary>
            /// Slide to the left.
            /// </summary>
            Slide,

            /// <summary>
            /// Slide to the left.
            /// </summary>
            SlideAndFade,
            Grow,
            GrowAndFade,
            Flip,
            FlipAndFade,
            Spin,
            SpinAndFade,

            /// <summary>
            /// Slide to the right.
            /// </summary>
            SlideBack,

            /// <summary>
            /// Slide to the right.
            /// </summary>
            SlideBackAndFade,

            Up,

            UpAndFade,

            Down,

            DownAndFade
        }

        /// <summary>
        /// List of the implemented checksum algorithims.
        /// </summary>
        public enum ChecksumAlgorithim
        {
            [Description("None")]
            None = 0,

            [Description("MD5")]
            MD5,

            [Description("SHA-1")]
            SHA1,

            [Description("SHA-256")]
            SHA256,

            [Description("SHA-384")]
            SHA384,

            [Description("SHA-512")]
            SHA512
        }

        /// <summary>
        /// List of the ways in which the directory can be exported.
        /// </summary>
        public enum ExportFileStructure
        {
            None = 0,

            /// <summary>
            /// Exports showing the file structure into a *.txt file.
            /// </summary>
            [Description("Folder structure text file")]
            TextFile,

            /// <summary>
            /// Exports a flat list into a *.txt file.
            /// </summary>
            [Description("Flat text file")]
            TextFlat
        }

        /// <summary>
        /// A comprehensive list of the pages that can be shown.
        /// </summary>
        public enum PageControl
        {
            /// <summary>
            /// User for initialization.
            /// </summary>
            None = 0,

            /// <summary>
            /// The directory tree view.
            /// </summary>
            Directory,

            /// <summary>
            /// Settings that can be saved.
            /// </summary>
            Settings,

            /// <summary>
            /// Generate the checksum for a single file.
            /// </summary>
            FileChecksum,

            /// <summary>
            /// Generate the checksum for an entire tree.  (Files only, obvs)
            /// </summary>
            TreeChecksum
        }

        /// <summary>
        /// Method to get a list of values and descriptions for generating a list of controls.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetEnumValueDescriptionPairs(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new KeyValuePair<string, string>(e.ToString(), e.GetDescription()))
                .ToList();
        }
    }
}