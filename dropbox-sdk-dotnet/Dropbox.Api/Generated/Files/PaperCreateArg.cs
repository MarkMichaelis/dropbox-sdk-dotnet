// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Files
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The paper create arg object</para>
    /// </summary>
    public class PaperCreateArg
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<PaperCreateArg> Encoder = new PaperCreateArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<PaperCreateArg> Decoder = new PaperCreateArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PaperCreateArg" /> class.</para>
        /// </summary>
        /// <param name="path">The fully qualified path to the location in the user's Dropbox
        /// where the Paper Doc should be created. This should include the document's title and
        /// end with .paper.</param>
        /// <param name="importFormat">The format of the provided data.</param>
        public PaperCreateArg(string path,
                              ImportFormat importFormat)
        {
            if (path == null)
            {
                throw new sys.ArgumentNullException("path");
            }
            if (!re.Regex.IsMatch(path, @"\A(?:/(.|[\r\n])*)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("path", @"Value should match pattern '\A(?:/(.|[\r\n])*)\z'");
            }

            if (importFormat == null)
            {
                throw new sys.ArgumentNullException("importFormat");
            }

            this.Path = path;
            this.ImportFormat = importFormat;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PaperCreateArg" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public PaperCreateArg()
        {
        }

        /// <summary>
        /// <para>The fully qualified path to the location in the user's Dropbox where the
        /// Paper Doc should be created. This should include the document's title and end with
        /// .paper.</para>
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// <para>The format of the provided data.</para>
        /// </summary>
        public ImportFormat ImportFormat { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="PaperCreateArg" />.</para>
        /// </summary>
        private class PaperCreateArgEncoder : enc.StructEncoder<PaperCreateArg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(PaperCreateArg value, enc.IJsonWriter writer)
            {
                WriteProperty("path", value.Path, writer, enc.StringEncoder.Instance);
                WriteProperty("import_format", value.ImportFormat, writer, global::Dropbox.Api.Files.ImportFormat.Encoder);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="PaperCreateArg" />.</para>
        /// </summary>
        private class PaperCreateArgDecoder : enc.StructDecoder<PaperCreateArg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="PaperCreateArg" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override PaperCreateArg Create()
            {
                return new PaperCreateArg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(PaperCreateArg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "path":
                        value.Path = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "import_format":
                        value.ImportFormat = global::Dropbox.Api.Files.ImportFormat.Decoder.Decode(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        #endregion
    }
}
