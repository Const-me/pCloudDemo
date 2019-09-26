using PCloud;
using System;
using System.IO;
using System.Threading.Tasks;

namespace pCloudDemo
{
	static class Program
	{
		const bool ssl = true;
		// TODO: paste code parameter of your upload link
		const string code = "";
		const string localPath = @"C:\Temp\helloworld.txt";
		const string sender = @"NetCoreTest";

		static async Task mainImpl()
		{
			// Connect to the server
			using( var conn = await Connection.connect( ssl ) )
			{
				string name = Path.GetFileName( localPath );
				// Upload that file to the upload link
				await Api.uploadToLink( conn, name, () => File.OpenRead( localPath ), code, sender );
			}
			Console.WriteLine( "Uploaded OK" );
		}

		static async Task Main( string[] args )
		{
			try
			{
				await mainImpl();
			}
			catch( Exception ex )
			{
				Console.WriteLine( "Failed: {0}\n{1}", ex.Message, ex.ToString() );
			}
		}
	}
}