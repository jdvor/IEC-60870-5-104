namespace Iec608705104.Messages
{
    /// <summary>
    /// Application Service Data Unit (ASDU) type identification
    /// </summary>
    public enum AsduTypeId : byte
    {
        /// <summary>
        /// Not used
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// Single-point information
        /// </summary>
        M_SP_NA_1 = 0x01,

        /// <summary>
        /// Single-point information with time tag
        /// </summary>
        M_SP_TA_1 = 0x02,

        /// <summary>
        /// Double-point information
        /// </summary>
        M_DP_NA_1 = 0x03,

        /// <summary>
        /// Double-point information with time tag
        /// </summary>
        M_DP_TA_1 = 0x04,

        /// <summary>
        /// Step position information
        /// </summary>
        M_ST_NA_1 = 0x05,

        /// <summary>
        /// Step position information with time tag
        /// </summary>
        M_ST_TA_1 = 0x06,

        /// <summary>
        /// Bitstring of 32 bit
        /// </summary>
        M_BO_NA_1 = 0x07,

        /// <summary>
        /// Bitstring of 32 bit with time tag
        /// </summary>
        M_BO_TA_1 = 0x08,

        /// <summary>
        /// Measured value, normalised value
        /// </summary>
        M_ME_NA_1 = 0x09,

        /// <summary>
        /// Measured value, normalized value with time tag
        /// </summary>
        M_ME_TA_1 = 0x0A,

        /// <summary>
        /// Measured value, scaled value
        /// </summary>
        M_ME_NB_1 = 0x0B,

        /// <summary>
        /// Measured value, scaled value wit time tag
        /// </summary>
        M_ME_TB_1 = 0x0C,

        /// <summary>
        /// Measured value, short floating point number
        /// </summary>
        M_ME_NC_1 = 0x0D,

        /// <summary>
        /// Measured value, short floating point number with time tag
        /// </summary>
        M_ME_TC_1 = 0x0E,

        /// <summary>
        /// Integrated totals
        /// </summary>
        M_IT_NA_1 = 0x0F,

        /// <summary>
        /// Integrated totals with time tag
        /// </summary>
        M_IT_TA_1 = 0x10,

        /// <summary>
        /// Event of protection equipment with time tag
        /// </summary>
        M_EP_TA_1 = 0x11,

        /// <summary>
        /// Packed start events of protection equipment with time tag
        /// </summary>
        M_EP_TB_1 = 0x12,

        /// <summary>
        /// Packed output circuit information of protection equipment with time tag
        /// </summary>
        M_EP_TC_1 = 0x13,

        /// <summary>
        /// Packed single point information with status change detection
        /// </summary>
        M_PS_NA_1 = 0x14,

        /// <summary>
        /// Measured value, normalized value without quality descriptor
        /// </summary>
        M_ME_ND_1 = 0x15,

        /// <summary>
        /// Single-point information with time tag CP56Time2a
        /// </summary>
        M_SP_TB_1 = 0x1E,

        /// <summary>
        /// Double-point information with time tag CP56Time2a
        /// </summary>
        M_DP_TB_1 = 0x1F,

        /// <summary>
        /// Step position information with time tag CP56Time2a
        /// </summary>
        M_ST_TB_1 = 0x20,

        /// <summary>
        /// Bitstring of 32 bit with time tag CP56Time2a
        /// </summary>
        M_BO_TB_1 = 0x21,

        /// <summary>
        /// Measured value, normalised value with time tag CP56Time2a
        /// </summary>
        M_ME_TD_1 = 0x22,

        /// <summary>
        /// Measured value, scaled value with time tag CP56Time2a
        /// </summary>
        M_ME_TE_1 = 0x23,

        /// <summary>
        /// Measured value, short floating point number with time tag CP56Time2a
        /// </summary>
        M_ME_TF_1 = 0x24,

        /// <summary>
        /// Integrated totals with time tag CP56Time2a
        /// </summary>
        M_IT_TB_1 = 0x25,

        /// <summary>
        /// Event of protection equipment with time tag CP56Time2a
        /// </summary>
        M_EP_TD_1 = 0x26,

        /// <summary>
        /// Packed start events of protection equipment with time tag CP56Time2a
        /// </summary>
        M_EP_TE_1 = 0x27,

        /// <summary>
        /// Packed output circuit information of protection equipment with time tag CP56Time2a
        /// </summary>
        M_EP_TF_1 = 0x28,

        /// <summary>
        /// Single command
        /// </summary>
        C_SC_NA_1 = 0x2D,

        /// <summary>
        /// Double command
        /// </summary>
        C_DC_NA_1 = 0x2E,

        /// <summary>
        /// Regulating step command
        /// </summary>
        C_RC_NA_1 = 0x2F,

        /// <summary>
        /// Set-point Command, normalised value
        /// </summary>
        C_SE_NA_1 = 0x30,

        /// <summary>
        /// Set-point Command, scaled value
        /// </summary>
        C_SE_NB_1 = 0x31,

        /// <summary>
        /// Set-point Command, short floating point number
        /// </summary>
        C_SE_NC_1 = 0x32,

        /// <summary>
        /// Bitstring 32 bit command
        /// </summary>
        C_BO_NA_1 = 0x33,

        /// <summary>
        /// Single command with time tag CP56Time2a
        /// </summary>
        C_SC_TA_1 = 0x3A,

        /// <summary>
        /// Double command with time tag CP56Time2a
        /// </summary>
        C_DC_TA_1 = 0x3B,

        /// <summary>
        /// Regulating step command with time tag CP56Time2a
        /// </summary>
        C_RC_TA_1 = 0x3C,

        /// <summary>
        /// Measured value, normalised value command with time tag CP56Time2a
        /// </summary>
        C_SE_TA_1 = 0x3D,

        /// <summary>
        /// Measured value, scaled value command with time tag CP56Time2a
        /// </summary>
        C_SE_TB_1 = 0x3E,

        /// <summary>
        /// Measured value, short floating point number command with time tag CP56Time2a
        /// </summary>
        C_SE_TC_1 = 0x3F,

        /// <summary>
        /// Bitstring of 32 bit command with time tag CP56Time2a
        /// </summary>
        C_BO_TA_1 = 0x40,

        /// <summary>
        /// End of Initialisation
        /// </summary>
        M_EI_NA_1 = 0x46,

        /// <summary>
        /// Interrogation command
        /// </summary>
        C_IC_NA_1 = 0x64,

        /// <summary>
        /// Counter interrogation command
        /// </summary>
        C_CI_NA_1 = 0x65,

        /// <summary>
        /// Read command
        /// </summary>
        C_RD_NA_1 = 0x66,

        /// <summary>
        /// Clock synchronisation command
        /// </summary>
        C_CS_NA_1 = 0x67,

        /// <summary>
        /// Test command
        /// </summary>
        C_TS_NA_1 = 0x68,

        /// <summary>
        /// Reset process command
        /// </summary>
        C_RP_NA_1 = 0x69,

        /// <summary>
        /// Delay acquisition command
        /// </summary>
        C_CD_NA_1 = 0x6A,

        /// <summary>
        /// Test command with time tag CP56Time2a
        /// </summary>
        C_TS_TA_1 = 0x6B,

        /// <summary>
        /// Parameter of measured values, normalized value
        /// </summary>
        P_ME_NA_1 = 0x6E,

        /// <summary>
        /// Parameter of measured values, scaled value
        /// </summary>
        P_ME_NB_1 = 0x6F,

        /// <summary>
        /// Parameter of measured values, short floating point number
        /// </summary>
        P_ME_NC_1 = 0x70,

        /// <summary>
        /// Parameter activation
        /// </summary>
        P_AC_NA_1 = 0x71,

        /// <summary>
        /// File ready
        /// </summary>
        F_FR_NA_1 = 0x78,

        /// <summary>
        /// Section ready
        /// </summary>
        F_SR_NA_1 = 0x79,

        /// <summary>
        /// Call directory, select file, call file, call section
        /// </summary>
        F_SC_NA_1 = 0x7A,

        /// <summary>
        /// Last section, last segment
        /// </summary>
        F_LS_NA_1 = 0x7B,

        /// <summary>
        /// ACK file, ACK section
        /// </summary>
        F_FA_NA_1 = 0x7C,

        /// <summary>
        /// Segment
        /// </summary>
        F_SG_NA_1 = 0x7D,

        /// <summary>
        /// Directory
        /// </summary>
        F_DR_TA_1 = 0x7E,
    }
}
