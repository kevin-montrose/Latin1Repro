using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Latin1Repro
{
    class Program
    {
        static void Main(string[] args)
        {
            // this is:
            //   utf-16
            //   utf-16BE
            //   utf-32
            //   utf-32BE
            //   us-ascii
            //   iso-8859-1   <- problem child is here
            //   utf-7
            //   utf-8 
            var availableEncodings = Encoding.GetEncodings();

            var naughtyStrings =
                 new[]
                {
                    @" ",
                    @"",
                    @"",
                    @"",
                    @"­؀؁؂؃؄؅؜۝܏᠎​‌‍‎‏‪‫‬‭‮⁠⁡⁢⁣⁤⁦⁧⁨⁩⁪⁫⁬⁭⁮⁯﻿￹￺￻𑂽𛲠𛲡𛲢𛲣𝅳𝅴𝅵𝅶𝅷𝅸𝅹𝅺󠀁󠀠󠀡󠀢󠀣󠀤󠀥󠀦󠀧󠀨󠀩󠀪󠀫󠀬󠀭󠀮󠀯󠀰󠀱󠀲󠀳󠀴󠀵󠀶󠀷󠀸󠀹󠀺󠀻󠀼󠀽󠀾󠀿󠁀󠁁󠁂󠁃󠁄󠁅󠁆󠁇󠁈󠁉󠁊󠁋󠁌󠁍󠁎󠁏󠁐󠁑󠁒󠁓󠁔󠁕󠁖󠁗󠁘󠁙󠁚󠁛󠁜󠁝󠁞󠁟󠁠󠁡󠁢󠁣󠁤󠁥󠁦󠁧󠁨󠁩󠁪󠁫󠁬󠁭󠁮󠁯󠁰󠁱󠁲󠁳󠁴󠁵󠁶󠁷󠁸󠁹󠁺󠁻󠁼󠁽󠁾󠁿",
                    @"ЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя",
                    @"ด้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็ ด้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็ ด้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็็้้้้้้้้็็็็็้้้้้็็็็",
                    @"田中さんにあげて下さい",
                    @"パーティーへ行かないか",
                    @"和製漢語",
                    @"사회과학원 어학연구소",
                    @"울란바토르",
                    @"𠜎𠜱𠝹𠱓𠱸𠲖𠳏",
                    @"表ポあA鷗ŒéＢ逍Üßªąñ丂㐀𠀀",
                    @"Ⱥ",
                    @"Ⱦ",
                    @"ヽ༼ຈل͜ຈ༽ﾉ ヽ༼ຈل͜ຈ༽ﾉ",
                    @"😍",
                    @"✋🏿 💪🏿 👐🏿 🙌🏿 👏🏿 🙏🏿",
                    @"🚾 🆒 🆓 🆕 🆖 🆗 🆙 🏧",
                    @"0️⃣ 1️⃣ 2️⃣ 3️⃣ 4️⃣ 5️⃣ 6️⃣ 7️⃣ 8️⃣ 9️⃣ 🔟",
                    @"🇺🇸🇷🇺🇸 🇦🇫🇦🇲🇸",
                    @"בְּרֵאשִׁית, בָּרָא אֱלֹהִים, אֵת הַשָּׁמַיִם, וְאֵת הָאָרֶץ",
                    @"הָיְתָהtestالصفحات التّحول",
                    @"﷽",
                    @"ﷺ",
                    @"مُنَاقَشَةُ سُبُلِ اِسْتِخْدَامِ اللُّغَةِ فِي النُّظُمِ الْقَائِمَةِ وَفِيم يَخُصَّ التَّطْبِيقَاتُ الْحاسُوبِيَّةُ، ",
                    @"˙ɐnbᴉlɐ ɐuƃɐɯ ǝɹolop ʇǝ ǝɹoqɐl ʇn ʇunpᴉpᴉɔuᴉ ɹodɯǝʇ poɯsnᴉǝ op pǝs 'ʇᴉlǝ ƃuᴉɔsᴉdᴉpɐ ɹnʇǝʇɔǝsuoɔ 'ʇǝɯɐ ʇᴉs ɹolop ɯnsdᴉ ɯǝɹo˥",
                    @"00˙Ɩ$-",
                    @"𝚃𝚑𝚎 𝚚𝚞𝚒𝚌𝚔 𝚋𝚛𝚘𝚠𝚗 𝚏𝚘𝚡 𝚓𝚞𝚖𝚙𝚜 𝚘𝚟𝚎𝚛 𝚝𝚑𝚎 𝚕𝚊𝚣𝚢 𝚍𝚘𝚐",
                    @"⒯⒣⒠ ⒬⒰⒤⒞⒦ ⒝⒭⒪⒲⒩ ⒡⒪⒳ ⒥⒰⒨⒫⒮ ⒪⒱⒠⒭ ⒯⒣⒠ ⒧⒜⒵⒴ ⒟⒪⒢"
                };

            foreach (var encInfo in availableEncodings)
            {
                var enc = encInfo.GetEncoding();
                var first = true;

                foreach (var str in naughtyStrings)
                {
                    var encoderRes = CompareEncodingEncoder(enc, str);
                    var decoderRes = CompareEncodingDecoder(enc, str);

                    if (encoderRes.Success && decoderRes.Success) continue;

                    if (first)
                    {
                        Console.WriteLine($"{encInfo.DisplayName} ({encInfo.Name})");
                        Console.WriteLine("===");
                        first = false;
                    }

                    if (!encoderRes.Success)
                    {
                        Console.WriteLine($"\t{encoderRes.Message}");
                    }

                    if (!decoderRes.Success)
                    {
                        Console.WriteLine($"\t{decoderRes.Message}");
                    }
                }
            }
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
