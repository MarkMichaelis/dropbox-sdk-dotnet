// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.TeamLog
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>Resolved file comment.</para>
    /// </summary>
    public class FileResolveCommentDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<FileResolveCommentDetails> Encoder = new FileResolveCommentDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<FileResolveCommentDetails> Decoder = new FileResolveCommentDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileResolveCommentDetails" />
        /// class.</para>
        /// </summary>
        /// <param name="commentText">Comment text.</param>
        public FileResolveCommentDetails(string commentText = null)
        {
            this.CommentText = commentText;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileResolveCommentDetails" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public FileResolveCommentDetails()
        {
        }

        /// <summary>
        /// <para>Comment text.</para>
        /// </summary>
        public string CommentText { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="FileResolveCommentDetails" />.</para>
        /// </summary>
        private class FileResolveCommentDetailsEncoder : enc.StructEncoder<FileResolveCommentDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(FileResolveCommentDetails value, enc.IJsonWriter writer)
            {
                if (value.CommentText != null)
                {
                    WriteProperty("comment_text", value.CommentText, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="FileResolveCommentDetails" />.</para>
        /// </summary>
        private class FileResolveCommentDetailsDecoder : enc.StructDecoder<FileResolveCommentDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="FileResolveCommentDetails"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override FileResolveCommentDetails Create()
            {
                return new FileResolveCommentDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(FileResolveCommentDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "comment_text":
                        value.CommentText = enc.StringDecoder.Instance.Decode(reader);
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
