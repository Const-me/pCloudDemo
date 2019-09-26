using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PCloud
{
	static class Utils
	{
		public static void write( this Stream stm, byte[] buffer )
		{
			stm.Write( buffer, 0, buffer.Length );
		}

		public static void rewind( this Stream stm )
		{
			stm.Seek( 0, SeekOrigin.Begin );
		}

		public static V lookup<K, V>( this IReadOnlyDictionary<K, V> dict, K key )
		{
			V v;
			dict.TryGetValue( key, out v );
			return v;
		}

		public static async Task<byte[]> read( this Stream stm, int length )
		{
			byte[] buffer = new byte[ length ];
			int offset = 0;
			while( true )
			{
				int cb = await stm.ReadAsync( buffer, offset, length );
				if( 0 == cb )
					throw new EndOfStreamException();
				offset += cb;
				length -= cb;
				if( length <= 0 )
					return buffer;
			}
		}
	}
}