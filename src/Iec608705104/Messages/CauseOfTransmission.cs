namespace Iec608705104.Messages
{
    /// <summary>
    /// The cause of transmission (COT)
    /// </summary>
    public enum CauseOfTransmission : byte
    {
        UNKNOWN = 0,

        /// <summary>
        /// Cyclic data
        /// </summary>
        CYCLIC = 1,

        /// <summary>
        /// Background request
        /// </summary>
        BACKGROUND_SCAN = 2,

        /// <summary>
        /// Spontaneous data
        /// </summary>
        SPONTANEOUS = 3,

        /// <summary>
        /// End of initialisation
        /// </summary>
        INITIALIZED = 4,

        /// <summary>
        /// Read-Request
        /// </summary>
        REQUEST = 5,

        /// <summary>
        /// Command activation
        /// </summary>
        ACTIVATION = 6,

        /// <summary>
        /// Acknowledgement of command activation
        /// </summary>
        ACTIVATION_ACK = 7,

        /// <summary>
        /// Command abort
        /// </summary>
        DEACTIVATION = 8,

        /// <summary>
        /// Acknowledgement of command abort
        /// </summary>
        DEACTIVATION_ACK = 9,

        /// <summary>
        /// Termination of command activation
        /// </summary>
        ACTIVATION_TERMINATION = 10,

        /// <summary>
        /// Return because of remote command
        /// </summary>
        RETURN_INFO_REMOTE = 11,

        /// <summary>
        /// Return because local command
        /// </summary>
        RETURN_INFO_LOCAL = 12,

        /// <summary>
        /// File access
        /// </summary>
        FILE_TRANSFER = 13,

        AUTHENTICATION = 14,
        MAINTENANCE_OF_AUTH_SESSION_KEY = 15,
        MAINTENANCE_OF_USER_ROLE_AND_UPDATE_KEY = 16,
        INTERROGATED_BY_STATION = 20,
        INTERROGATED_BY_GROUP_1 = 21,
        INTERROGATED_BY_GROUP_2 = 22,
        INTERROGATED_BY_GROUP_3 = 23,
        INTERROGATED_BY_GROUP_4 = 24,
        INTERROGATED_BY_GROUP_5 = 25,
        INTERROGATED_BY_GROUP_6 = 26,
        INTERROGATED_BY_GROUP_7 = 27,
        INTERROGATED_BY_GROUP_8 = 28,
        INTERROGATED_BY_GROUP_9 = 29,
        INTERROGATED_BY_GROUP_10 = 30,
        INTERROGATED_BY_GROUP_11 = 31,
        INTERROGATED_BY_GROUP_12 = 32,
        INTERROGATED_BY_GROUP_13 = 33,
        INTERROGATED_BY_GROUP_14 = 34,
        INTERROGATED_BY_GROUP_15 = 35,
        INTERROGATED_BY_GROUP_16 = 36,
        REQUESTED_BY_GENERAL_COUNTER = 37,
        REQUESTED_BY_GROUP_1_COUNTER = 38,
        REQUESTED_BY_GROUP_2_COUNTER = 39,
        REQUESTED_BY_GROUP_3_COUNTER = 40,
        REQUESTED_BY_GROUP_4_COUNTER = 41,
        UNKNOWN_TYPE_ID = 44,
        UNKNOWN_CAUSE_OF_TRANSMISSION = 45,
        UNKNOWN_COMMON_ADDRESS_OF_ASDU = 46,
        UNKNOWN_INFORMATION_OBJECT_ADDRESS = 47,
    }
}
