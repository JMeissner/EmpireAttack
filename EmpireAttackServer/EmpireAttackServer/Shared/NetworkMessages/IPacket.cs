using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared
{
    public interface IPacket
    {
        #region Public Properties

        /// <summary>
        /// Gets MessageType.
        /// </summary>
        PacketTypes MessageType { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        void Decode(NetPacketReader im);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="om">
        /// The om.
        /// </param>
        void Encode(NetDataWriter om);

        #endregion Public Methods
    }
}