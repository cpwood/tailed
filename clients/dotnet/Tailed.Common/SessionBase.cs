using QRCoder;
using System;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;

namespace Tailed.Common
{
    public abstract class SessionBase : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The API server hostname.
        /// </summary>
        public string Hostname { get; }

        /// <summary>
        /// The session identifier.
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// The tailed.live URL.
        /// </summary>
        public Uri Uri => new Uri($"https://{Hostname}/{SessionId}");

        public TailedClient Client { get; }

        protected SessionBase(string hostname)
        {
            Hostname = hostname;
            SessionId = ShortGuid.NewGuid().Value;
            Client = new TailedClient(Hostname, SessionId);
        }

        /// <summary>
        /// Renders the QR code for the tailed.live URL.
        /// </summary>
        protected void RenderSessionInformation()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Console.WriteLine($"   {Uri}");
            Console.WriteLine();

            var qrGenerator = new QRCodeGenerator();
            var moduleData = qrGenerator.CreateQrCode(Uri.ToString(), QRCodeGenerator.ECCLevel.Q).ModuleMatrix;

            var palette = new
            {
                WHITE_ALL = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes("\u2588"))),
                WHITE_BLACK = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes("\u2580"))),
                BLACK_WHITE = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes("\u2584"))),
                BLACK_ALL = "　",
            };

            var white = false;
            var black = true;

            var oddRow = moduleData.Count % 2 == 1;
            if (oddRow)
                moduleData.Add(new System.Collections.BitArray(moduleData[0].Count));

            Console.ForegroundColor = ConsoleColor.White;

            for (var row = 0; row < moduleData.Count; row += 2)
            {
                Console.Write("   ");

                for (var col = 0; col < moduleData[row].Count; col++)
                {
                    if (moduleData[row][col] == white && moduleData[row + 1][col] == white)
                        Console.Write(palette.WHITE_ALL);
                    else if (moduleData[row][col] == white && moduleData[row + 1][col] == black)
                        Console.Write(palette.WHITE_BLACK);
                    else if (moduleData[row][col] == black && moduleData[row + 1][col] == white)
                        Console.Write(palette.BLACK_WHITE);
                    else
                        Console.Write(palette.BLACK_ALL);
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.Write("   Press any key to continue..");

            Console.ReadKey();
            Console.ResetColor();
            Console.Clear();
        }

        public virtual void Dispose()
        {
            DisposeAsync().AsTask().WaitAndUnwrapException();
        }

        public virtual async ValueTask DisposeAsync()
        {
            await Client.DisposeAsync();
        }
    }
}
