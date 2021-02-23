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
    /// <para>Invited team members to shared folder.</para>
    /// </summary>
    public class SfTeamInviteDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SfTeamInviteDetails> Encoder = new SfTeamInviteDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SfTeamInviteDetails> Decoder = new SfTeamInviteDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SfTeamInviteDetails" />
        /// class.</para>
        /// </summary>
        /// <param name="targetAssetIndex">Target asset position in the Assets list.</param>
        /// <param name="originalFolderName">Original shared folder name.</param>
        /// <param name="sharingPermission">Sharing permission.</param>
        public SfTeamInviteDetails(ulong targetAssetIndex,
                                   string originalFolderName,
                                   string sharingPermission = null)
        {
            if (originalFolderName == null)
            {
                throw new sys.ArgumentNullException("originalFolderName");
            }

            this.TargetAssetIndex = targetAssetIndex;
            this.OriginalFolderName = originalFolderName;
            this.SharingPermission = sharingPermission;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SfTeamInviteDetails" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public SfTeamInviteDetails()
        {
        }

        /// <summary>
        /// <para>Target asset position in the Assets list.</para>
        /// </summary>
        public ulong TargetAssetIndex { get; protected set; }

        /// <summary>
        /// <para>Original shared folder name.</para>
        /// </summary>
        public string OriginalFolderName { get; protected set; }

        /// <summary>
        /// <para>Sharing permission.</para>
        /// </summary>
        public string SharingPermission { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="SfTeamInviteDetails" />.</para>
        /// </summary>
        private class SfTeamInviteDetailsEncoder : enc.StructEncoder<SfTeamInviteDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SfTeamInviteDetails value, enc.IJsonWriter writer)
            {
                WriteProperty("target_asset_index", value.TargetAssetIndex, writer, enc.UInt64Encoder.Instance);
                WriteProperty("original_folder_name", value.OriginalFolderName, writer, enc.StringEncoder.Instance);
                if (value.SharingPermission != null)
                {
                    WriteProperty("sharing_permission", value.SharingPermission, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="SfTeamInviteDetails" />.</para>
        /// </summary>
        private class SfTeamInviteDetailsDecoder : enc.StructDecoder<SfTeamInviteDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="SfTeamInviteDetails" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SfTeamInviteDetails Create()
            {
                return new SfTeamInviteDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(SfTeamInviteDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "target_asset_index":
                        value.TargetAssetIndex = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    case "original_folder_name":
                        value.OriginalFolderName = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "sharing_permission":
                        value.SharingPermission = enc.StringDecoder.Instance.Decode(reader);
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
