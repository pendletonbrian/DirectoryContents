using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DirectoryContents.Classes
{
    public static class Enumerations
    {
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

        public enum PageControl
        {
            None = 0,
            Directory,
            Settings,
            FileChecksum
        }

        public static List<KeyValuePair<string, string>> GetEnumValueDescriptionPairs(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new KeyValuePair<string, string>(e.ToString(), e.GetDescription()))
                .ToList();
        }
    }
}