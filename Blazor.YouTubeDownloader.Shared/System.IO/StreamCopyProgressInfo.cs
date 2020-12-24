namespace System.IO
{
    public struct StreamCopyProgressInfo
    {
        public long BytesRead { get; set; }

        public long TotalBytesCopied { get; set; }

        public long SourceLength { get; set; }
    }
}