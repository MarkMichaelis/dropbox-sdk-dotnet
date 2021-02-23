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
    /// <para>The export metadata object</para>
    /// </summary>
    /// <seealso cref="ExportResult" />
    public class ExportMetadata
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<ExportMetadata> Encoder = new ExportMetadataEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<ExportMetadata> Decoder = new ExportMetadataDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ExportMetadata" /> class.</para>
        /// </summary>
        /// <param name="name">The last component of the path (including extension). This never
        /// contains a slash.</param>
        /// <param name="size">The file size in bytes.</param>
        /// <param name="exportHash">A hash based on the exported file content. This field can
        /// be used to verify data integrity. Similar to content hash. For more information see
        /// our <a href="https://www.dropbox.com/developers/reference/content-hash">Content
        /// hash</a> page.</param>
        /// <param name="paperRevision">If the file is a Paper doc, this gives the latest doc
        /// revision which can be used in <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.PaperUpdateAsync" />.</param>
        public ExportMetadata(string name,
                              ulong size,
                              string exportHash = null,
                              long? paperRevision = null)
        {
            if (name == null)
            {
                throw new sys.ArgumentNullException("name");
            }

            if (exportHash != null)
            {
                if (exportHash.Length < 64)
                {
                    throw new sys.ArgumentOutOfRangeException("exportHash", "Length should be at least 64");
                }
                if (exportHash.Length > 64)
                {
                    throw new sys.ArgumentOutOfRangeException("exportHash", "Length should be at most 64");
                }
            }

            this.Name = name;
            this.Size = size;
            this.ExportHash = exportHash;
            this.PaperRevision = paperRevision;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ExportMetadata" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public ExportMetadata()
        {
        }

        /// <summary>
        /// <para>The last component of the path (including extension). This never contains a
        /// slash.</para>
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// <para>The file size in bytes.</para>
        /// </summary>
        public ulong Size { get; protected set; }

        /// <summary>
        /// <para>A hash based on the exported file content. This field can be used to verify
        /// data integrity. Similar to content hash. For more information see our <a
        /// href="https://www.dropbox.com/developers/reference/content-hash">Content hash</a>
        /// page.</para>
        /// </summary>
        public string ExportHash { get; protected set; }

        /// <summary>
        /// <para>If the file is a Paper doc, this gives the latest doc revision which can be
        /// used in <see cref="Dropbox.Api.Files.Routes.FilesUserRoutes.PaperUpdateAsync"
        /// />.</para>
        /// </summary>
        public long? PaperRevision { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="ExportMetadata" />.</para>
        /// </summary>
        private class ExportMetadataEncoder : enc.StructEncoder<ExportMetadata>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(ExportMetadata value, enc.IJsonWriter writer)
            {
                WriteProperty("name", value.Name, writer, enc.StringEncoder.Instance);
                WriteProperty("size", value.Size, writer, enc.UInt64Encoder.Instance);
                if (value.ExportHash != null)
                {
                    WriteProperty("export_hash", value.ExportHash, writer, enc.StringEncoder.Instance);
                }
                if (value.PaperRevision != null)
                {
                    WriteProperty("paper_revision", value.PaperRevision.Value, writer, enc.Int64Encoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="ExportMetadata" />.</para>
        /// </summary>
        private class ExportMetadataDecoder : enc.StructDecoder<ExportMetadata>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="ExportMetadata" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ExportMetadata Create()
            {
                return new ExportMetadata();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(ExportMetadata value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "name":
                        value.Name = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "size":
                        value.Size = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    case "export_hash":
                        value.ExportHash = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "paper_revision":
                        value.PaperRevision = enc.Int64Decoder.Instance.Decode(reader);
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
