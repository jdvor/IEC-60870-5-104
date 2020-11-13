namespace Iec608705104
{
    public enum ErrorCode
    {
        None = 0,
        AcceptSocket,
        FillPipe,
        ReadPipe,
        InvalidApduLength,
        FailedToReadFrames,
        FailedToCreateFrameSpan,
        BufferDoesNotFit,
        InvalidStartByte,
        InvalidAsduType,
        NonIdentifiableFrame,
        NotSupportedFrameType,
        NotSupportedAsduType,
        UnknownControlFunc,
        InvalidCP56Time2a,
    }
}
