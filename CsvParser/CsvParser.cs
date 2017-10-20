using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvParser
{
	public class CsvParse
	{
		private Tuple<T, IEnumerable<T>> HeadAndTail<T>(IEnumerable<T> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			var en = source.GetEnumerator();
			en.MoveNext();
			return Tuple.Create(en.Current, EnumerateTail(en));
		}

		private IEnumerable<T> EnumerateTail<T>(IEnumerator<T> en)
		{
			while (en.MoveNext()) yield return en.Current;
		}

		public IEnumerable<IList<string>> Parse(string content, char delimiter, char qualifier)
		{
			var reader = new StringReader(content);
			return Parse(reader, delimiter, qualifier);

		}

		public Tuple<IList<string>, IEnumerable<IList<string>>> ParseHeadAndTail(TextReader reader, char delimiter, char qualifier)
		{
			return HeadAndTail(Parse(reader, delimiter, qualifier));
		}

		public IEnumerable<IList<string>> Parse(TextReader reader, char delimiter, char qualifier)
		{
			var inQuote = false;
			var record = new List<string>();
			var sb = new StringBuilder();

			//reader.
			while (reader.Peek() != -1)
			{
				var readChar = (char)reader.Read();

				if (readChar == '\n' || (readChar == '\r' && (char)reader.Peek() == '\n'))
				{
					// If it's a \r\n combo consume the \n part and throw it away.
					if (readChar == '\r')
						reader.Read();

					if (inQuote)
					{
						if (readChar == '\r')
							sb.Append('\r');
						sb.Append('\n');
					}
					else
					{
						if (record.Count > 0 || sb.Length > 0)
						{
							record.Add(sb.ToString());
							sb.Clear();
						}

						if (record.Count > 0)
							yield return record;

						record = new List<string>(record.Count);
					}
				}
				else if (sb.Length == 0 && !inQuote)
				{
					if (readChar == qualifier)
						inQuote = true;
					else if (readChar == delimiter)
					{
						record.Add(sb.ToString());
						sb.Clear();
					}
					else if (char.IsWhiteSpace(readChar))
					{
						// Ignore leading whitespace
					}
					else
						sb.Append(readChar);
				}
				else if (readChar == delimiter)
				{
					if (inQuote)
						sb.Append(delimiter);
					else
					{
						record.Add(sb.ToString());
						sb.Clear();
					}
				}
				else if (readChar == qualifier)
				{
					if (inQuote)
					{
						if ((char)reader.Peek() == qualifier)
						{
							reader.Read();
							sb.Append(qualifier);
						}
						else
							inQuote = false;
					}
					else
						sb.Append(readChar);
				}
				else
					sb.Append(readChar);
			}

			if (record.Count > 0 || sb.Length > 0)
				record.Add(sb.ToString());

			if (record.Count > 0)
				yield return record;
		}

		public Encoding GetEncoding(string filename)
		{
			// Read the BOM
			/*var bom = new byte[4];
			using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				file.Read(bom, 0, 4);
			}

			// Analyze the BOM
			if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
			if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
			if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
			if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
			if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;*/
			/*var file = new FileStream(filename,
				FileMode.Open, FileAccess.Read, FileShare.Read);

			if (file.CanSeek)
			{
				byte[] bom = new byte[4]; // Get the byte-order mark, if there is one 
				file.Read(bom, 0, 4);
				if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || // utf-8 
					(bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le 
					(bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2 
					(bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4 
				{
					return Encoding.Unicode;
				}
				else
				{
					return Encoding.ASCII;
				}
			}
			file.Close();
			return Encoding.ASCII;*/


			var textDetect = new TextEncodingDetect();
			var bytes = File.ReadAllBytes(filename);
			var encoding = textDetect.DetectEncoding(bytes, bytes.Length);

			Console.Write("Encoding: ");
			if (encoding == TextEncodingDetect.Encoding.None)
			{
				Console.WriteLine("Binary");
				return Encoding.Default;
			}
			if (encoding == TextEncodingDetect.Encoding.Ascii)
			{
				Console.WriteLine("ASCII (chars in the 0-127 range)");
				return Encoding.ASCII;
			}
			if (encoding == TextEncodingDetect.Encoding.Ansi)
			{
				Console.WriteLine("ANSI (chars in the range 0-255 range)");
			}
			else if (encoding == TextEncodingDetect.Encoding.Utf8Bom || encoding == TextEncodingDetect.Encoding.Utf8Nobom)
			{
				Console.WriteLine("UTF-8");
				return Encoding.UTF8;
			}
			else if (encoding == TextEncodingDetect.Encoding.Utf16LeBom || encoding == TextEncodingDetect.Encoding.Utf16LeNoBom)
			{
				Console.WriteLine("UTF-16 Little Endian");
				return Encoding.UTF8;
			}
			else if (encoding == TextEncodingDetect.Encoding.Utf16BeBom || encoding == TextEncodingDetect.Encoding.Utf16BeNoBom)
			{
				Console.WriteLine("UTF-16 Big Endian");
				return Encoding.UTF32;
			}

			return Encoding.ASCII;
		}
	}
}