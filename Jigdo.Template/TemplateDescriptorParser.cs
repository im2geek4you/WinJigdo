namespace Jigdo;

internal static class TemplateDescriptorParser
{
    public static IReadOnlyList<TemplateDescriptorEntry> Parse(ReadOnlySpan<byte> descTail)
    {
        if (descTail.Length < 10 + 6)
            throw new InvalidDataException("DESC section too small.");

        if (!descTail.StartsWith("DESC"u8))
            throw new InvalidDataException("Expected DESC at start of template tail.");

        ulong declared = EndianUtil.ReadLe48(descTail.Slice(4, 6));
        if ((ulong)descTail.Length != declared)
            throw new InvalidDataException($"DESC length mismatch: buffer has {descTail.Length}, header says {declared}.");

        var list = new List<TemplateDescriptorEntry>();
        int pos = 10;
        int end = descTail.Length - 6;

        while (pos < end)
        {
            byte t = descTail[pos];
            switch (t)
            {
                case JigdoBlockType.Data:
                    if (pos + JigdoSizes.DescEntryData > end)
                        throw new InvalidDataException("Truncated DATA descriptor.");
                    list.Add(new TemplateDataEntry(EndianUtil.ReadLe48(descTail.Slice(pos + 1, 6))));
                    pos += JigdoSizes.DescEntryData;
                    break;

                case JigdoBlockType.MatchSha256:
                    if (pos + JigdoSizes.DescEntryMatchSha256 > end)
                        throw new InvalidDataException("Truncated MATCH_SHA256 descriptor.");
                    {
                        ulong ext = EndianUtil.ReadLe48(descTail.Slice(pos + 1, 6));
                        ulong rsync = EndianUtil.ReadLe64(descTail.Slice(pos + 7, 8));
                        byte[] sha = descTail.Slice(pos + 15, JigdoSizes.Sha256Bytes).ToArray();
                        list.Add(new TemplateMatchSha256Entry(ext, rsync, sha));
                    }
                    pos += JigdoSizes.DescEntryMatchSha256;
                    break;

                case JigdoBlockType.ImageSha256:
                    if (pos + JigdoSizes.DescEntryImageSha256 > end)
                        throw new InvalidDataException("Truncated IMAGE_SHA256 descriptor.");
                    {
                        ulong imgLen = EndianUtil.ReadLe48(descTail.Slice(pos + 1, 6));
                        byte[] sha = descTail.Slice(pos + 7, JigdoSizes.Sha256Bytes).ToArray();
                        uint rsBlock = EndianUtil.ReadLe32(descTail.Slice(pos + 7 + JigdoSizes.Sha256Bytes, 4));
                        list.Add(new TemplateImageSha256Entry(imgLen, sha, rsBlock));
                    }
                    pos += JigdoSizes.DescEntryImageSha256;
                    break;

                case JigdoBlockType.MatchMd5:
                    if (pos + JigdoSizes.DescEntryMatchMd5 > end)
                        throw new InvalidDataException("Truncated MATCH_MD5 descriptor.");
                    {
                        ulong ext = EndianUtil.ReadLe48(descTail.Slice(pos + 1, 6));
                        ulong rsync = EndianUtil.ReadLe64(descTail.Slice(pos + 7, 8));
                        byte[] md5 = descTail.Slice(pos + 15, 16).ToArray();
                        list.Add(new TemplateMatchMd5Entry(ext, rsync, md5));
                    }
                    pos += JigdoSizes.DescEntryMatchMd5;
                    break;

                case JigdoBlockType.ImageMd5:
                    if (pos + JigdoSizes.DescEntryImageMd5 > end)
                        throw new InvalidDataException("Truncated IMAGE_MD5 descriptor.");
                    {
                        ulong imgLen = EndianUtil.ReadLe48(descTail.Slice(pos + 1, 6));
                        byte[] md5 = descTail.Slice(pos + 7, 16).ToArray();
                        uint rsBlock = EndianUtil.ReadLe32(descTail.Slice(pos + 23, 4));
                        list.Add(new TemplateImageMd5Entry(imgLen, md5, rsBlock));
                    }
                    pos += JigdoSizes.DescEntryImageMd5;
                    break;

                case JigdoBlockType.WrittenMd5:
                case JigdoBlockType.WrittenSha256:
                    throw new NotSupportedException($"Descriptor block type {t} is not implemented.");

                default:
                    throw new InvalidDataException($"Unknown descriptor block type 0x{t:X2} at offset {pos}.");
            }
        }

        ulong footer = EndianUtil.ReadLe48(descTail.Slice(descTail.Length - 6, 6));
        if (footer != declared)
            throw new InvalidDataException("DESC footer pointer does not match declared length.");

        return list;
    }
}
