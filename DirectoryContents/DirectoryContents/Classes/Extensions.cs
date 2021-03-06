﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DirectoryContents.Classes.Checksums;

namespace DirectoryContents.Classes
{
    public static class Extensions
    {
        internal static bool Contains(this string sourceString, string term, StringComparison comparison)
        {
            return sourceString?.IndexOf(term, comparison) >= 0;
        }

        /// <summary>
        /// Gets a string representation of the given TimeSpan in a easily human
        /// readable format.
        /// </summary>
        /// <param name="timespan">
        /// </param>
        /// <returns>
        /// </returns>
        internal static string GetTimeFromTimeSpan(this TimeSpan timespan)
        {
            bool hasDays = false;
            bool hasHours = false;
            bool hasMinutes = false;

            StringBuilder s = new StringBuilder();

            if (0 < timespan.Days)
            {
                hasDays = true;

                if (1 == timespan.Days)
                {
                    s.Append(timespan.Days + " day");
                }
                else
                {
                    s.Append(timespan.Days + " days");
                }
            }

            if (0 < timespan.Hours)
            {
                hasHours = true;

                if (hasDays)
                {
                    s.Append(", ");
                }

                if (1 == timespan.Hours)
                {
                    s.Append(timespan.Hours + " hour");
                }
                else
                {
                    s.Append(timespan.Hours + " hours");
                }
            }

            if (0 < timespan.Minutes)
            {
                hasMinutes = true;

                if (hasDays ||
                    hasHours)
                {
                    s.Append(", ");
                }

                if (1 == timespan.Minutes)
                {
                    s.Append(timespan.Minutes + " minute");
                }
                else
                {
                    s.Append(timespan.Minutes + " minutes");
                }
            }

            if (0 < timespan.Seconds)
            {
                if (hasDays ||
                    hasHours ||
                    hasMinutes)
                {
                    s.Append(", ");
                }

                if (0 < timespan.Milliseconds)
                {
                    s.Append($"{timespan.Seconds}.{timespan.Milliseconds.ToString("###")} seconds");
                }
                else
                {
                    if (1 == timespan.Seconds)
                    {
                        s.Append(timespan.Seconds + " second");
                    }
                    else
                    {
                        s.Append(timespan.Seconds + " seconds");
                    }
                }
            }
            else if (0 < timespan.Milliseconds)
            {
                // No seconds.

                if (hasDays ||
                    hasHours ||
                    hasMinutes)
                {
                    s.Append(".");
                }
                else
                {
                    s.Append("0.");
                }

                s.Append($"{timespan.Milliseconds.ToString("###")} seconds");
            }

            return s.ToString();
        }

        public static string GetDescription(this Enum val)
        {
            var attribute = (DescriptionAttribute)val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault();

            return attribute == default(DescriptionAttribute) ? val.ToString() : attribute.Description;
        }

        public static IHashAlgorithim GetAlgorithimInterface(this Enumerations.ChecksumAlgorithim val)
        {
            switch (val)
            {
                case Enumerations.ChecksumAlgorithim.None:
                    return new Empty();

                case Enumerations.ChecksumAlgorithim.MD5:
                    return new MD5();

                case Enumerations.ChecksumAlgorithim.SHA1:
                    return new SHA1();

                case Enumerations.ChecksumAlgorithim.SHA256:
                    return new SHA256();

                case Enumerations.ChecksumAlgorithim.SHA384:
                    return new SHA384();

                case Enumerations.ChecksumAlgorithim.SHA512:
                    return new SHA512();

                default:
                    throw new Exception($"Unhandled {nameof(Enumerations.ChecksumAlgorithim)}: {val}");
            }
        }
    }
}
