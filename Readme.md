This code uploads files to public links of [pcloud.com](https://www.pcloud.com/).To test, create upload link in pcloud’s web interface, copy-paste the “code” parameter from the link to `code` constant in Program.cs. Also create a file ` C:\Temp\helloworld.txt`.I don’t work for pcloud, that’s why I stopped once I have the functionality I need. The code can be trivially extended to support the rest of the RPCs. I’ve already completed the hard parts i.e. networking and their custom binary protocol. To support more RPC methods, add methods to `Api` static class.The code requires modern .NET. Tested with .NET Core 2.2, C# 7.1.