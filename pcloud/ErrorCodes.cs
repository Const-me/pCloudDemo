using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace PCloud
{
	/// <summary>Utility class to resolve error codes into messages.</summary>
	static class ErrorCodes
	{
		const string manifestResourceName = "pCloudDemo.pcloud.errorCodes.gz";

		static readonly Dictionary<int, string> dict = new Dictionary<int, string>();

		static ErrorCodes()
		{
			Assembly ass = Assembly.GetExecutingAssembly();
			using( var stm = ass.GetManifestResourceStream( manifestResourceName ) )
			using( var unzip = new GZipStream( stm, CompressionMode.Decompress ) )
			using( var reader = new StreamReader( unzip, Encoding.UTF8 ) )
			{
				var space = " ".ToCharArray();
				while( true )
				{
					string line = reader.ReadLine();
					if( null == line )
						return;
					if( string.IsNullOrWhiteSpace( line ) )
						continue;
					string[] fields = line.Split( space, 2 );
					dict.Add( int.Parse( fields[ 0 ] ), fields[ 1 ] );
				}
			}
		}

		/// <summary>Try to resolve error code into message, returns null if the code wasn't found in the local dictionary.</summary>
		public static string lookup( int code )
		{
			return dict.lookup( code );
		}
	}
}