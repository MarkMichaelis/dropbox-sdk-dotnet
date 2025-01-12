namespace SimpleTest
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using Dropbox.Api;
    using Dropbox.Api.Common;
    using Dropbox.Api.Files;
    using Dropbox.Api.Team;

    partial class Program
    {
        // This loopback host is for demo purpose. If this port is not
        // available on your machine you need to update this URL with an unused port.
        private const string LoopbackHost = "http://127.0.0.1:52475/";

        // URL to receive OAuth 2 redirect from Dropbox server.
        // You also need to register this redirect URL on https://www.dropbox.com/developers/apps.
        private readonly Uri RedirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private readonly Uri JSRedirectUri = new Uri(LoopbackHost + "token");


        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [STAThread]
        static async Task<int> Main()
        {
            return await Task.Run(new Program().Run);
        }

        private async Task<int> Run()
        {
            Console.WriteLine(nameof(SimpleTest));
            DropboxCertHelper.InitializeCertPinning();

            string accessToken = await GetOAuthTokens();
            if (string.IsNullOrEmpty(accessToken))
            {
                return 1;
            }

            // Specify socket level timeout which decides maximum waiting time when no bytes are
            // received by the socket.
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            using var httpClient = new HttpClient(httpClientHandler)
            {
                // Specify request level timeout which decides maximum time that can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var config = new DropboxClientConfig("SimpleTestApp")
                {
                    HttpClient = httpClient
                };

                var client = new DropboxClient(accessToken, config);
                await RunUserTests(client);

                // Tests below are for Dropbox Business endpoints. To run these tests, make sure the ApiKey is for
                // a Dropbox Business app and you have an admin account to log in.

                /*
                var client = new DropboxTeamClient(accessToken, userAgent: "SimpleTeamTestApp", httpClient: httpClient);
                await RunTeamTests(client);
                */
				
                Console.WriteLine("Exit with any key");
                Console.ReadKey();
            }
            catch (HttpException e)
            {
                Console.WriteLine("Exception reported from RPC layer");
                Console.WriteLine("    Status code: {0}", e.StatusCode);
                Console.WriteLine("    Message    : {0}", e.Message);
                if (e.RequestUri != null)
                {
                    Console.WriteLine("    Request uri: {0}", e.RequestUri);
                }
            }

            return 0;
        }


        /// <summary>
        /// Handles the redirect from Dropbox server. Because we are using token flow, the local
        /// http server cannot directly receive the URL fragment. We need to return a HTML page with
        /// inline JS which can send URL fragment to local server as URL parameter.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task HandleOAuth2Redirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to RedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            context.Response.ContentType = "text/html";

            // Respond with a page which runs JS and sends URL fragment as query string
            // to TokenRedirectUri.
            using (var file = File.OpenRead("index.html"))
            {
                file.CopyTo(context.Response.OutputStream);
            }

            context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Handle the redirect from JS and process raw redirect URI with fragment to
        /// complete the authorization flow.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="OAuth2Response"/></returns>
        private async Task<Uri> HandleJSRedirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to TokenRedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            return new Uri(context.Request.QueryString["url_with_fragment"]);
        }

        /// <summary>
        /// Acquires a dropbox OAuth tokens and saves them to the default settings for the app.
        /// <para>
        /// This fetches the OAuth tokens from the applications settings, if it is not found there
        /// (or if the user chooses to reset the settings) then the UI in <see cref="LoginForm"/> is
        /// displayed to authorize the user.
        /// </para>
        /// </summary>
        /// <returns>A valid access token if successful otherwise null.</returns>
        private async Task<string> GetOAuthTokens()
        {
            Settings.Default.Upgrade();
            Console.Write("Reset settings (Y/N) ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Settings.Default.Reset();
            }
            Console.WriteLine();

            if (string.IsNullOrEmpty(Settings.Default.AccessToken))
            {
                string apiKey = GetApiKey();
                using var http = new HttpListener();
                try
                {
                    string state = Guid.NewGuid().ToString("N");
                    var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(
                        OAuthResponseType.Token, apiKey, RedirectUri, state: state);

                    http.Prefixes.Add(LoopbackHost);

                    http.Start();

                    // Use StartInfo to ensure default browser launches.
                    ProcessStartInfo startInfo = new ProcessStartInfo(
                        authorizeUri.ToString()) { UseShellExecute = true };

                    try
                    {
                        // open browser for authentication
                        Console.WriteLine("Waiting for credentials and authorization.");
                        Process.Start(startInfo);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("An unexpected error occured while opening the browser.");
                    }

                    // Handle OAuth redirect and send URL fragment to local server using JS.
                    await HandleOAuth2Redirect(http);

                    // Handle redirect from JS and process OAuth response.
                    Uri redirectUri = await HandleJSRedirect(http);
                    http.Stop();

                    OAuth2Response result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);
                    if (result.State != state)
                    {
                        // The state in the response doesn't match the state in the request.
                        return null;
                    }
                    Console.WriteLine("OAuth token aquire complete");

                    // Bring console window to the front.
                    SetForegroundWindow(GetConsoleWindow());

                    DisplayOAuthResult(result);

                    UpdateSettings(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    return null;
                }
            }

            return Settings.Default.AccessToken;
        }

        private static void UpdateSettings(OAuth2Response result)
        {
            // Foreach Settting, save off the value retrieved from the result.
            foreach (System.Configuration.SettingsProperty item in Settings.Default.Properties)
            {
                if (typeof(OAuth2Response).GetProperty(item.Name) is PropertyInfo property)
                {
                    Settings.Default[item.Name] = property.GetValue(result);
                }
            }

            Settings.Default.Save();
            Settings.Default.Reload();
        }

        private static void DisplayOAuthResult(OAuth2Response result)
        {
            Console.WriteLine("OAuth Result:");
            Console.WriteLine("\tUid: {0}", result.Uid);
            Console.WriteLine("\tAccessToken: {0}", result.AccessToken);
            Console.WriteLine("\tRefreshToken: {0}", result.RefreshToken);
            Console.WriteLine("\tExpiresAt: {0}", result.ExpiresAt);
            Console.WriteLine("\tScopes: {0}", string.Join(" ", result.ScopeList?? new string[0]));
        }

        /// <summary>
        /// Retrieve the ApiKey from the user
        /// </summary>
        /// <returns>Return the ApiKey specified by the user</returns>
        private static string GetApiKey()
        {
            string apiKey = Settings.Default.ApiKey;
            
            while (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("Create a Dropbox App at https://www.dropbox.com/developers/apps.");
                Console.Write("Enter the API Key (or 'Quit' to exit): ");
                apiKey = Console.ReadLine();
                if (apiKey.ToLower() == "quit")
                {
                    Console.WriteLine("The API Key is required to connect to Dropbox.");
                    apiKey = null;
                    break;
                }
                else
                {
                    Settings.Default.ApiKey = apiKey;
                }
            }

            return string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;
        }

        /// <summary>
        /// Gets information about the currently authorized account.
        /// <para>
        /// This demonstrates calling a simple rpc style api from the Users namespace.
        /// </para>
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>An asynchronous task.</returns>
        static private async Task GetCurrentAccount(DropboxClient client)
        {
            Console.WriteLine("Current Account:");
            var full = await client.Users.GetCurrentAccountAsync();

            Console.WriteLine("Account id    : {0}", full.AccountId);
            Console.WriteLine("Country       : {0}", full.Country);
            Console.WriteLine("Email         : {0}", full.Email);
            Console.WriteLine("Is paired     : {0}", full.IsPaired ? "Yes" : "No");
            Console.WriteLine("Locale        : {0}", full.Locale);
            Console.WriteLine("Name");
            Console.WriteLine("  Display  : {0}", full.Name.DisplayName);
            Console.WriteLine("  Familiar : {0}", full.Name.FamiliarName);
            Console.WriteLine("  Given    : {0}", full.Name.GivenName);
            Console.WriteLine("  Surname  : {0}", full.Name.Surname);
            Console.WriteLine("Referral link : {0}", full.ReferralLink);

            if (full.Team != null)
            {
                Console.WriteLine("Team");
                Console.WriteLine("  Id   : {0}", full.Team.Id);
                Console.WriteLine("  Name : {0}", full.Team.Name);
            }
            else
            {
                Console.WriteLine("Team - None");
            }
        }

        /// <summary>
        /// Run tests for user-level operations.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task RunUserTests(DropboxClient client)
        {
            
            await GetCurrentAccount(client);

            var path = "/DotNetApi/Help";
            await DeleteFolder(client, path); // Removes items left older from the previous test run.

            var folder = await CreateFolder(client, path);

            var pathInTeamSpace = "/Test";
            await ListFolderInTeamSpace(client, pathInTeamSpace);

            await Upload(client, path, "Test.txt", "This is a text file");

            await ChunkUpload(client, path, "Binary");

            ListFolderResult list = await ListFolder(client, path);

            Metadata firstFile = list.Entries.FirstOrDefault(i => i.IsFile);
            if (firstFile != null)
            {
                await Download(client, path, firstFile.AsFile);
            }

            await DeleteFolder(client, path);
        }

        /// <summary>
        /// Run tests for team-level operations.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task RunTeamTests(DropboxTeamClient client)
        {
            var members = await client.Team.MembersListAsync();

            var member = members.Members.FirstOrDefault();

            if (member != null)
            {
                // A team client can perform action on a team member's behalf. To do this,
                // just pass in team member id in to AsMember function which returns a user client.
                // This client will operates on this team member's Dropbox.
                var userClient = client.AsMember(member.Profile.TeamMemberId);
                await RunUserTests(userClient);
            }
        }

        /// <summary>
        /// Delete the specified folder including any files within the folder
        /// </summary>
        /// <param name="client">The dropbox client object.</param>
        /// <param name="path">The path to the target folder to delete.</param>
        /// <returns></returns>
        static private async Task<bool> PathExists(DropboxClient client, string path)
        {
            try
            {
                await client.Files.GetMetadataAsync(path);
                return true;
            }
            catch (DropboxException exception) when (exception.Message.StartsWith("path/not_found/"))
            {
                return false;
            }
        }

        /// <summary>
        /// Delete the specified folder including any files within the folder
        /// </summary>
        /// <param name="client">The dropbox client object.</param>
        /// <param name="path">The path to the target folder to delete.</param>
        /// <returns></returns>
        static private async Task<Metadata> DeleteFolder(DropboxClient client, string path)
        {
            if(await PathExists(client, path))
            {
                Console.WriteLine("--- Deleting Folder ---");
                Metadata metadata =  await client.Files.DeleteAsync(path);
                Console.WriteLine($"Deleted {metadata.PathLower}");
                return metadata;
            }
            return null;
        }

        /// <summary>
        /// Creates the specified folder.
        /// </summary>
        /// <remarks>This demonstrates calling an rpc style api in the Files namespace.</remarks>
        /// <param name="path">The path of the folder to create.</param>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>The result from the ListFolderAsync call.</returns>
        private async Task<FolderMetadata> CreateFolder(DropboxClient client, string path)
        {
            Console.WriteLine("--- Creating Folder ---");
            var folderArg = new CreateFolderArg(path);
            try
            {
                var folder = await client.Files.CreateFolderV2Async(folderArg);

                Console.WriteLine("Folder: " + path + " created!");

                return folder.Metadata;
            }
            catch (ApiException<CreateFolderError> e)
            {
                if (e.Message.StartsWith("path/conflict/folder"))
                {
                    Console.WriteLine("Folder already exists... Skipping create");
                    return null;
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Lists the items within a folder inside team space. See
        /// https://www.dropbox.com/developers/reference/namespace-guide for details about
        /// user namespace vs team namespace.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="path">The path to list.</param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ListFolderInTeamSpace(DropboxClient client, string path)
        {
            // Fetch root namespace info from user's account info.
            var account = await client.Users.GetCurrentAccountAsync();

            if (!account.RootInfo.IsTeam)
            {
                Console.WriteLine("This user doesn't belong to a team with shared space.");
            }
            else
            {
                try
                {
                    // Point path root to namespace id of team space.
                    client = client.WithPathRoot(new PathRoot.Root(account.RootInfo.RootNamespaceId));
                    await ListFolder(client, path);
                }
                catch (PathRootException ex)
                {
                    Console.WriteLine(
                        "The user's root namespace ID has changed to {0}",
                        ex.ErrorResponse.AsInvalidRoot.Value);
                }
            }
        }

        /// <summary>
        /// Lists the items within a folder.
        /// </summary>
        /// <remarks>This demonstrates calling an rpc style api in the Files namespace.</remarks>
        /// <param name="path">The path to list.</param>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>The result from the ListFolderAsync call.</returns>
        private async Task<ListFolderResult> ListFolder(DropboxClient client, string path)
        {
            Console.WriteLine("--- Files ---");
            var list = await client.Files.ListFolderAsync(path);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                var file = item.AsFile;

                Console.WriteLine("F{0,8} {1}",
                    file.Size,
                    item.Name);
            }

            if (list.HasMore)
            {
                Console.WriteLine("   ...");
            }
            return list;
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <remarks>This demonstrates calling a download style api in the Files namespace.</remarks>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder path in which the file should be found.</param>
        /// <param name="file">The file to download within <paramref name="folder"/>.</param>
        /// <returns></returns>
        private async Task Download(DropboxClient client, string folder, FileMetadata file)
        {
            Console.WriteLine("Download file...");

            using (var response = await client.Files.DownloadAsync(folder + "/" + file.Name))
            {
                Console.WriteLine("Downloaded {0} Rev {1}", response.Response.Name, response.Response.Rev);
                Console.WriteLine("------------------------------");
                Console.WriteLine(await response.GetContentAsStringAsync());
                Console.WriteLine("------------------------------");
            }
        }

        /// <summary>
        /// Uploads given content to a file in Dropbox.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder to upload the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The file content.</param>
        /// <returns></returns>
        private async Task Upload(DropboxClient client, string folder, string fileName, string fileContent)
        {
            Console.WriteLine("Upload file...");

            using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(fileContent)))
            {
                var response = await client.Files.UploadAsync(folder + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);

                Console.WriteLine("Uploaded Id {0} Rev {1}", response.Id, response.Rev);
            }
        }

        /// <summary>
        /// Uploads a big file in chunk. The is very helpful for uploading large file in slow network condition
        /// and also enable capability to track upload progerss.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder to upload the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns></returns>
        private async Task ChunkUpload(DropboxClient client, string folder, string fileName)
        {
            Console.WriteLine("Chunk upload file...");
            // Chunk size is 128KB.
            const int chunkSize = 128 * 1024;

            // Create a random file of 1MB in size.
            var fileContent = new byte[1024 * 1024];
            new Random().NextBytes(fileContent);

            using (var stream = new MemoryStream(fileContent))
            {
                int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);

                byte[] buffer = new byte[chunkSize];
                string sessionId = null;

                for (var idx = 0; idx < numChunks; idx++)
                {
                    Console.WriteLine("Start uploading chunk {0}", idx);
                    var byteRead = stream.Read(buffer, 0, chunkSize);

                    using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                    {
                        if (idx == 0)
                        {
                            var result = await client.Files.UploadSessionStartAsync(body: memStream);
                            sessionId = result.SessionId;
                        }

                        else
                        {
                            UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                            if (idx == numChunks - 1)
                            {
                                await client.Files.UploadSessionFinishAsync(cursor, new CommitInfo(folder + "/" + fileName), memStream);
                            }

                            else
                            {
                                await client.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// List all members in the team.
        /// </summary>
        /// <param name="client">The Dropbox team client.</param>
        /// <returns>The result from the MembersListAsync call.</returns>
        private async Task<MembersListResult> ListTeamMembers(DropboxTeamClient client)
        {
            var members = await client.Team.MembersListAsync();

            foreach (var member in members.Members)
            {
                Console.WriteLine("Member id    : {0}", member.Profile.TeamMemberId);
                Console.WriteLine("Name         : {0}", member.Profile.Name);
                Console.WriteLine("Email        : {0}", member.Profile.Email);
            }

            return members;
        }
    }
}
