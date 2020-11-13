using Iec608705104.Messages;

namespace Iec608705104
{
    public class EstablishedConnectionOptions
    {
        public int SocketReadMinBufferSize { get; set; } = 255;

        public int SocketCloseTimeoutSecs { get; set; } = 5;
    }
}
