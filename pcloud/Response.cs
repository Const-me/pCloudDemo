using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hash = System.Collections.Generic.Dictionary<string, object>;

namespace PCloud
{
	/// <summary>Parsed response from pCloud's server.</summary>
	class Response
	{
		/// <summary>Payload object of the response.</summary>
		public struct Data
		{
			public readonly long payloadLength;
			public Data( long len )
			{
				payloadLength = len;
			}
		}

		/// <summary>First JSON dictionary in the response.</summary>
		public readonly IReadOnlyDictionary<string, object> firstHash;
		/// <summary>Payload portions of the response, if any. Otherwise null.</summary>
		public readonly Data[] payload = null;
		/// <summary>Other values in the response, if any. Otherwise null.</summary>
		public readonly object[] otherValues = null;

		/// <summary>Read response from the network stream.</summary>
		/// <remarks>If the response contains payload portion, i.e. files, they are <b>not</b> read from the stream.
		/// It's caller's responsibility to iterate through <see cref="payload" /> values and read all the data before reusing the connection for other requests.</remarks>
		public static async Task<Response> receive( Stream connection )
		{
			// Receive into array of bytes
			int length = BitConverter.ToInt32( await connection.read( 4 ), 0 );
			byte[] buffer = await connection.read( length );

			// Parse
			Hash firstHash = null;
			List<object> otherValues = null;
			List<Data> payloads = null;
			var parser = new ResponseParser( buffer );

			foreach( object obj in parser.parse() )
			{
				switch( obj )
				{
					case Hash h:
						if( null == firstHash )
						{
							firstHash = h;
							break;
						}
						goto default;
					case Data data:
						if( null == payloads )
							payloads = new List<Data>();
						payloads.Add( data );
						break;
					default:
						if( null == otherValues )
							otherValues = new List<object>();
						otherValues.Add( obj );
						break;
				}
			}

			return new Response( firstHash, otherValues, payloads );
		}

		Response( Hash firstHash, List<object> otherValues, List<Data> payload )
		{
			this.firstHash = firstHash;
			if( null != otherValues )
				this.otherValues = otherValues.ToArray();
			if( null != payload )
				this.payload = payload.ToArray();
		}

		/// <summary>if this response is empty, or contains failed status code, throw an exception telling that. Do nothing if it says it was completed successfully.</summary>
		public void throwIfFailed()
		{
			if( null == firstHash )
				throw new ApplicationException( "The response has no dictionary" );

			object objResult;
			if( !firstHash.TryGetValue( "result", out objResult ) )
				throw new ApplicationException( "The response has no \"result\" property" );

			int result = Convert.ToInt32( objResult );
			if( 0 == result )
				return; // Normally result: 0 means success while a non-zero result means an error

			string localCode = ErrorCodes.lookup( result );
			string remoteCode = firstHash.lookup( "error" ) as string;
			if( null == localCode )
			{
				if( null == remoteCode )
					throw new ApplicationException( $"Operation failed, code { result }" );
				throw new ApplicationException( $"Operation failed, code { result }: { remoteCode }" );
			}
			if( localCode == remoteCode )
				throw new ApplicationException( $"Operation failed, code { result }: { localCode }" );
			throw new ApplicationException( $"Operation failed, code { result }: { localCode } / { remoteCode }" );
		}
	}
}