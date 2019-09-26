using System;
using System.IO;
using System.Threading.Tasks;

namespace PCloud
{
	public static class Api
	{
		/// <summary>Receive response from the server, and ensure its completed successfully. Throw an exception otherwise.</summary>
		static async Task receiveResponse( Stream connection )
		{
			await connection.FlushAsync();
			var response = await Response.receive( connection );
			response.throwIfFailed();
		}

		/// <summary>Upload a file. <b>Untested</b>, it needs API key of an app, I have not setup that on my account.</summary>
		public static async Task uploadFile( Stream connection, string fileName, Func<Stream> openStreamFunc, string auth, int folderid, bool nopartial = true )
		{
			if( string.IsNullOrWhiteSpace( fileName ) )
				throw new ArgumentException( "File name can't be empty", "fileName" );

			using( Stream payload = openStreamFunc() )
			{
				var req = new RequestBuilder( "uploadfile", payload.Length );
				req.add( "auth", auth );
				req.add( "folderid", folderid );
				req.add( "filename", fileName );
				req.add( "nopartial", nopartial );
				await req.close().CopyToAsync( connection );
				await payload.CopyToAsync( connection );
			}
			await receiveResponse( connection );
		}

		/// <summary>Anonymously upload file to an upload link.</summary>
		/// <remarks>The API documentation is silent about that, but "names" request parameter appears to be mandatory for this RPC.</remarks>
		public static async Task uploadToLink( Stream connection, string fileName, Func<Stream> openStreamFunc, string code, string from, bool nopartial = true )
		{
			if( string.IsNullOrWhiteSpace( fileName ) )
				throw new ArgumentException( "File name can't be empty", "fileName" );
			if( string.IsNullOrWhiteSpace( from ) )
				throw new ArgumentException( "Sender name can't be empty", "from" );

			using( Stream payload = openStreamFunc() )
			{
				var req = new RequestBuilder( "uploadtolink", payload.Length );
				req.add( "names", from );
				req.add( "filename", fileName );
				req.add( "code", code );
				req.add( "nopartial", nopartial );

				await req.close().CopyToAsync( connection );
				await payload.CopyToAsync( connection );
			}

			await receiveResponse( connection );
		}
	}
}