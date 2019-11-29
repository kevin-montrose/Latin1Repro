using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Latin1Repro
{
    class Program
    {
        static void Main()
        {
            var utf7InfiteLoopStr = @"00˙Ɩ$-";
            var latin1VariableLengthStr = @"0️⃣ 1️⃣ 2️⃣ 3️⃣ 4️⃣ 5️⃣ 6️⃣ 7️⃣ 8️⃣ 9️⃣ 🔟";

            var utf8 = Encoding.GetEncoding("utf-8");
            var utf7 = Encoding.GetEncoding("utf-7");
            var latin1 = Encoding.GetEncoding("iso-8859-1");

            CompareEncodingDecodingMethods(utf7, utf7InfiteLoopStr);
            CompareEncodingDecodingMethods(latin1, latin1VariableLengthStr);

            // use utf8 just to show correctness, no output expected
            CompareEncodingDecodingMethods(utf8, utf7InfiteLoopStr);
            CompareEncodingDecodingMethods(utf8, latin1VariableLengthStr);

            static void CompareEncodingDecodingMethods(Encoding enc, string str)
            {
                var first = true;

                var encoderRes = CompareEncodingEncoder(enc, str);
                PrintOnFailure(enc, encoderRes, ref first);

                var decoderRes = CompareEncodingDecoder(enc, str);
                PrintOnFailure(enc, decoderRes, ref first);

                for (var destBuffSize = enc.GetMaxByteCount(1); destBuffSize <= enc.GetByteCount(str); destBuffSize++)
                {
                    var encoderConvertRes = CompareEncodingEncoderConvert(enc, str, destBuffSize);
                    PrintOnFailure(enc, encoderConvertRes, ref first);
                }

                for (var destBuffSize = enc.GetMaxCharCount(1); destBuffSize <= str.Length; destBuffSize++)
                {
                    var decoderConvertRes = CompareEncodingDecoderConvert(enc, str, destBuffSize);
                    PrintOnFailure(enc, decoderConvertRes, ref first);
                }
            }

            static void PrintOnFailure(Encoding enc, (bool Success, string Message) result, ref bool first)
            {
                if (result.Success) return;

                if (first)
                {
                    Console.WriteLine($"{enc.EncodingName} ({enc.WebName})");
                    Console.WriteLine("===");
                    first = false;
                }

                Console.WriteLine($"\t{result.Message}");
            }
        }

        static (bool Success, string Message) CompareEncodingEncoderConvert(Encoding encoding, string text, int destBufferSize)
        {
            var encodingBytes = encoding.GetBytes(text);
            var encoder = encoding.GetEncoder();

            var chars = text.ToCharArray();

            var sourceSpan = chars.AsSpan();
            var destSpan = new byte[destBufferSize].AsSpan();
            var encoderBytes = new List<byte>();

            var completed = false;

            // write everything in sourceSpan
            while (!completed)
            {
                var flush = sourceSpan.Length == 0;
                encoder.Convert(sourceSpan, destSpan, flush, out var charsConsumed, out var bytesProduced, out completed);
                encoderBytes.AddRange(destSpan.Slice(0, bytesProduced).ToArray());

                sourceSpan = sourceSpan.Slice(charsConsumed);

                if (charsConsumed == 0 && bytesProduced == 0 && flush)
                {
                    return (false, $@"Encoding Convert failure for destBufferSize={destBufferSize}, stopped making progress");
                }
            }

            var eq = encodingBytes.SequenceEqual(encoderBytes);

            if (eq)
            {
                return (true, null);
            }

            var encodingAsStr = encoding.GetString(encodingBytes);
            var encoderAsStr = encoding.GetString(encoderBytes.ToArray());

            return (false, $@"Encoding Convert failure for destBufferSize={destBufferSize} - {encodingBytes.Length}:""{encodingAsStr}"" vs {encoderBytes.Count}:""{encoderAsStr}""");


        }

        static (bool Success, string Message) CompareEncodingEncoder(Encoding encoding, string text)
        {
            var encodingBytes = encoding.GetBytes(text);
            var encoder = encoding.GetEncoder();

            var chars = text.ToCharArray();

            for (var stride = 1; stride <= chars.Length; stride++)
            {
                // go through `stride` chars at a time with an Encoder
                var byteBuffer = new byte[encoding.GetMaxByteCount(stride)];
                var encoderBytes = new List<byte>();
                var ix = 0;
                while (ix + stride <= chars.Length)
                {
                    var byteCount = encoder.GetBytes(chars, ix, stride, byteBuffer, 0, false);
                    var written = byteBuffer.Take(byteCount);
                    encoderBytes.AddRange(written);

                    ix += stride;
                }

                // handle anything that falls after the last `stride` sized chunk
                var remainder = chars.Length % stride;
                var remainderByteCount = encoder.GetBytes(chars, chars.Length - remainder, remainder, byteBuffer, 0, false);
                var remainderWritten = byteBuffer.Take(remainderByteCount);
                encoderBytes.AddRange(remainderWritten);

                // flush any remaining bytes out of Encoder
                var flushByteCount = encoder.GetBytes(Array.Empty<char>(), 0, 0, byteBuffer, 0, true);
                var flushWritten = byteBuffer.Take(flushByteCount);
                encoderBytes.AddRange(flushWritten);

                var eq = encodingBytes.SequenceEqual(encoderBytes);

                if (eq)
                {
                    continue;
                }

                var encodingAsStr = encoding.GetString(encodingBytes);
                var encoderAsStr = encoding.GetString(encoderBytes.ToArray());

                return (false, $@"Encoding failure for stride = {stride} - {encodingBytes.Length}:""{encodingAsStr}"" vs {encoderBytes.Count}:""{encoderAsStr}""");
            }

            return (true, null);
        }

        static (bool Success, string Message) CompareEncodingDecoderConvert(Encoding encoding, string text, int destBufferSize)
        {
            var encodingBytes = encoding.GetBytes(text);
            var encodingStr = encoding.GetString(encodingBytes);

            var decoder = encoding.GetDecoder();

            for (var buff = encoding.GetMaxCharCount(1); buff <= encodingBytes.Length; buff++)
            {
                var sourceSpan = encodingBytes.AsSpan();

                var destSpan = new char[buff].AsSpan();

                var decodedChars = new List<char>();

                // write everything in sourceSpan
                while (true)
                {
                    decoder.Convert(sourceSpan, destSpan, false, out var consumedBytes, out var producedChars, out var complete);
                    decodedChars.AddRange(destSpan.Slice(0, producedChars).ToArray());

                    sourceSpan = sourceSpan.Slice(consumedBytes);

                    if (complete) break;
                }

                // now flush
                while (true)
                {
                    decoder.Convert(ReadOnlySpan<byte>.Empty, destSpan, true, out _, out var producedChars, out var complete);
                    decodedChars.AddRange(destSpan.Slice(0, producedChars).ToArray());

                    if (complete) break;
                }

                var decoderStr = new string(decodedChars.ToArray());

                if(decoderStr != encodingStr)
                {
                    return (false, $@"Decoding Convert failure - {encodingStr.Length}:""{encodingStr}"" vs {decoderStr.Length}:""{decoderStr}""");
                }
            }

            return (true, null);
        }

        static (bool Success, string Message) CompareEncodingDecoder(Encoding encoding, string text)
        {
            var encodingBytes = encoding.GetBytes(text);
            var encodingStr = encoding.GetString(encodingBytes);

            var decoder = encoding.GetDecoder();

            for (var stride = 1; stride < encodingBytes.Length; stride++)
            {
                // go through `stride` bytes at a time with a Decoder
                var charBuffer = new char[encoding.GetMaxCharCount(stride)];
                var decoderChars = new List<char>();
                var ix = 0;
                while(ix + stride <= encodingBytes.Length)
                {
                    var charCount = decoder.GetChars(encodingBytes, ix, stride, charBuffer, 0, false);
                    var written = charBuffer.Take(charCount);
                    decoderChars.AddRange(written);

                    ix += stride;
                }

                var remainder = encodingBytes.Length % stride;
                var remainderCharCount = decoder.GetChars(encodingBytes, encodingBytes.Length - remainder, remainder, charBuffer, 0);
                var remainderWritten = charBuffer.Take(remainderCharCount);
                decoderChars.AddRange(remainderWritten);

                // flush any remaining chars out of the decode
                var flushCharCount = decoder.GetChars(encodingBytes, 0, 0, charBuffer, 0, true);
                var flushWritten = charBuffer.Take(flushCharCount);
                decoderChars.AddRange(flushWritten);

                var decoderStr = new string(decoderChars.ToArray());

                var eq = encodingStr == decoderStr;
                if (eq)
                {
                    continue;
                }

                return (false, $@"Decoding failure - {encodingStr.Length}:""{encodingStr}"" vs {decoderStr.Length}:""{decoderStr}""");
            }

            return (true, null);
        }
    }
}
