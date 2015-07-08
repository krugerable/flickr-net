﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace FlickrNet
{
    /// <summary>
    /// The information about the number of photos a user has.
    /// </summary>
    public sealed class PhotoCountCollection : Collection<PhotoCount>, IFlickrParsable
    {
        #region IFlickrParsable Members

        void IFlickrParsable.Load(XmlReader reader)
        {
            if (reader.LocalName != "photocounts")
                UtilityMethods.CheckParsingException(reader);

            reader.Read();

            while (reader.LocalName == "photocount")
            {
                var c = new PhotoCount();
                ((IFlickrParsable) c).Load(reader);
                Add(c);
            }

            // Skip to next element (if any)
            reader.Skip();
        }

        #endregion
    }

    /// <summary>
    /// The specifics of a particular count.
    /// </summary>
    public sealed class PhotoCount : IFlickrParsable
    {
        /// <summary>Total number of photos between the FromDate and the ToDate.</summary>
        /// <remarks/>
        public int Count { get; set; }

        /// <summary>The From date as a <see cref="DateTime"/> object.</summary>
        public DateTime FromDate { get; set; }

        /// <summary>The To date as a <see cref="DateTime"/> object.</summary>
        public DateTime ToDate { get; set; }

        #region IFlickrParsable Members

        void IFlickrParsable.Load(XmlReader reader)
        {
            if (reader.LocalName != "photocount")
                UtilityMethods.CheckParsingException(reader);

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "count":
                        Count = int.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
                        break;
                    case "fromdate":
                        if (Regex.IsMatch(reader.Value, "^\\d+$"))
                            FromDate = UtilityMethods.UnixTimestampToDate(reader.Value);
                        else
                            FromDate = UtilityMethods.MySqlToDate(reader.Value);
                        break;
                    case "todate":
                        if (Regex.IsMatch(reader.Value, "^\\d+$"))
                            ToDate = UtilityMethods.UnixTimestampToDate(reader.Value);
                        else
                            ToDate = UtilityMethods.MySqlToDate(reader.Value);
                        break;
                    default:
                        UtilityMethods.CheckParsingException(reader);
                        break;
                }
            }

            reader.Read();
        }

        #endregion
    }
}