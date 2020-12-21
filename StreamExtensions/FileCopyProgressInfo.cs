// ReSharper disable once CheckNamespace
namespace System.IO
{
    public struct FileCopyProgressInfo
    {
        public long BytesRead { get; set; }

        public long TotalBytesCopied { get; set; }

        public long SourceLength { get; set; }
    }
}