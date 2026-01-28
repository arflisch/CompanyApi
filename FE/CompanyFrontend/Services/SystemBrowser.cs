using Duende.IdentityModel.OidcClient.Browser;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityModel.OidcClient.Browser;

namespace CompanyFrontend.Services
{
    public class SystemBrowser : IBrowser
    {
        private readonly int _port;

        public SystemBrowser(int port = 1033)
        {
            _port = port;
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            using (var listener = new HttpListener())
            {
                // 1. Listen on both localhost and 127.0.0.1 to handle different OS resolutions
                listener.Prefixes.Add($"http://localhost:{_port}/");
                listener.Prefixes.Add($"http://127.0.0.1:{_port}/");

                try
                {
                    listener.Start();
                }
                catch (HttpListenerException ex)
                {
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.UnknownError,
                        Error = $"Could not start listener on port {_port}. Port might be in use. Error: {ex.Message}"
                    };
                }

                // 2. Open the browser
                try
                {
                    OpenBrowser(options.StartUrl);
                }
                catch (Exception ex)
                {
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.UnknownError,
                        Error = $"Could not open browser: {ex.Message}"
                    };
                }

                // 3. Wait for the redirect
                try
                {
                    // Use a task that completes when the cancellation token is cancelled
                    var getContextTask = listener.GetContextAsync();
                    
                    // Basic cancellation support
                    if(cancellationToken != default)
                    {
                        using (cancellationToken.Register(() => listener.Stop()))
                        {
                             try 
                             {
                                await getContextTask;
                             }
                             catch (ObjectDisposedException)
                             {
                                 // Listener stopped
                                 return new BrowserResult { ResultType = BrowserResultType.UserCancel };
                             }
                             catch (HttpListenerException)
                             {
                                 // Listener stopped
                                 return new BrowserResult { ResultType = BrowserResultType.UserCancel };
                             }
                        }
                    }

                    var context = await getContextTask;

                    // 4. Send a nice HTML response
                    var response = context.Response;
                    string responseString = "<html><head><title>Login Successful</title></head><body><h1 style='color:green; font-family:sans-serif;'>Login Successful</h1><p style='font-family:sans-serif;'>You can close this tab and return to the application.</p><script>window.setTimeout(function(){window.close();}, 2000);</script></body></html>";
                    var buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    
                    // Use try-finally to ensure stream is closed even if write fails
                    try 
                    {
                        using var output = response.OutputStream;
                        await output.WriteAsync(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        // Ignore write errors (browser might have closed connection already)
                    }

                    // 5. Return the result
                    var responseUrl = context.Request.Url?.ToString();
                    if (!string.IsNullOrEmpty(responseUrl) && responseUrl.StartsWith("http://127.0.0.1", StringComparison.OrdinalIgnoreCase))
                    {
                         responseUrl = responseUrl.Replace("http://127.0.0.1", "http://localhost", StringComparison.OrdinalIgnoreCase);
                    }

                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.Success,
                        Response = responseUrl
                    };
                }
                catch (Exception ex)
                {
                     return new BrowserResult
                    {
                        ResultType = BrowserResultType.UnknownError,
                        Error = $"Error handling request: {ex.Message}"
                    };
                }
            }
        }

        private void OpenBrowser(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // LE FIX POUR MAC QUE NOUS AVONS VU ENSEMBLE
                    Process.Start("open", url);
                }
            }
            catch
            {
                // Fallback (optionnel)
                Console.WriteLine($"Impossible d'ouvrir le navigateur. Veuillez ouvrir : {url}");
            }
        }
    }
}