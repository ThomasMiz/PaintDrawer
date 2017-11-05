using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using PaintDrawer.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace PaintDrawer.GMail
{
    /// <summary>
    /// Encapsulates static methods to connect to GMail with the GMail API and read and answer mails
    /// </summary>
    static class Account
    {
        // Last mail read, Last mail successful read, last mail recieved. Pretty self explanatory.
        // All time variables are based of Program.Time (in seconds)
        public static double LastRead, LastSuccessfullRead, LastMailRecieved;
        private static bool hasAddedDisconnectMessage = false;

        /// <summary>The file containing the client_secret downloaded from Google</summary>
        public const String ClientSecretFile = "client_secret.json";
        
        /// <summary>The path to store the credentials at</summary>
        public const String CredentialsPath = ".credentials/cred";

        /// <summary>The email address the program is using. I read mine from a file at Init()</summary>
        public static String MyMailAddress = "insertyour@gmailaddress.com";

        // GMail credentials should be stored in client_secret.json. My file isn't tracked by GitHub for obvious reasons.
        public static UserCredential credential;
        public static GmailService service;

        /// <summary>
        /// Initializes the class, connecting to GMail with the loaded credentials and stuff.
        /// </summary>
        public static void Init()
        {
            Console.ForegroundColor = Colors.Message;
            Console.WriteLine("[Gmail] Connecting to GMail API, loading credentials...");

            // I store my mail address on a file not tracked by GitHub
            MyMailAddress = File.ReadAllText("mail.txt");

            // Loads the mail credentials. The first time, a web browser tab will open prompting for GMail API permissions and stuff.
            using (FileStream stream = new FileStream(ClientSecretFile, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // Which permissions do I need? Fuck it, i'll take all of them.
                    new String[] { GmailService.Scope.GmailLabels, GmailService.Scope.GmailSettingsBasic, GmailService.Scope.GmailSettingsSharing, GmailService.Scope.GmailCompose, GmailService.Scope.GmailInsert, GmailService.Scope.GmailModify, GmailService.Scope.GmailReadonly, GmailService.Scope.GmailSend, GmailService.Scope.MailGoogleCom },
                    "me",
                    CancellationToken.None,
                    new FileDataStore(CredentialsPath, true)
                ).Result;

                Console.ForegroundColor = Colors.Success;
                Console.WriteLine("[GMail] Success getting credentials!");
                Console.WriteLine("[GMail] Credential file saved to: " + CredentialsPath);
            }

            // Create Gmail API service.
            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "PaintDrawer",
            });

            Console.ForegroundColor = Colors.Success;
            Console.WriteLine("[GMail] GMail API successfully initiated & connected to Google and stuff");
        }

        /// <summary>
        /// Tries once to get all unread mail messages.
        /// <para>This method does not print information to the console.</para>
        /// <para>This method will throw exceptions if the list request fails.</para>
        /// </summary>
        /// <returns>The list with the unread mail messages</returns>
        public static List<RecievedMail> GetAllUnreadMails()
        {
            LastRead = Program.Time;
            List<RecievedMail> mails = new List<RecievedMail>(32);

            UsersResource.MessagesResource.ListRequest request = Account.service.Users.Messages.List("me");
            request.Q = "is:unread in:inbox";
            ListMessagesResponse response = request.Execute();

            if (response != null && response.Messages != null)
                for (int i = 0; i < response.Messages.Count; i++)
                    mails.Add(new RecievedMail(response.Messages[i].Id));

            LastRead = Program.Time;
            LastSuccessfullRead = LastRead;
            return mails;
        }

        /// <summary>
        /// Tries to get all the unread mail messages the specified amount of times. Returns whether successful
        /// <para>This method will print information to the console (only in cases of failures)</para>
        /// </summary>
        /// <param name="tries">How many times to try to get unread messages</param>
        /// <param name="list">The list with the recieved mails</param>
        /// <returns>Whether the mail request was successful</returns>
        public static bool ForceGetAllUnreadMails(int tries, out List<RecievedMail> list)
        {
            while(tries > 0)
            {
                try
                {
                    list = GetAllUnreadMails();
                    return true;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = Colors.Error;
                    Console.WriteLine("[GMailAccount] ERROR: Coudln't read mails: (quote)\n" + e.Message + "(end quote).\nThere are " + tries + " tries left.");
                }
                tries--;
            }

            list = null;
            return false;
        }

        /// <summary>
        /// Tries once to trash a mail message.
        /// <para>This method does NOT print information to the console.</para>
        /// <para>This method will throw exceptions if the trash request fails.</para>
        /// </summary>
        /// <param name="id">The id of the mail message to trash.</param>
        public static void TrashMail(String id)
        {
            UsersResource.MessagesResource.TrashRequest request = service.Users.Messages.Trash("me", id);
            Message message = request.Execute();
        }

        /// <summary>
        /// Tries to trash a specified mail the specified amount of times. Returns whether it succeeded.
        /// <para>This method prints information to the console.</para>
        /// </summary>
        /// <param name="id">The ID of the mail to trash</param>
        /// <param name="tries">The amount of times to try to trash it</param>
        /// <returns>Whether the mail was trashed. </returns>
        public static bool ForceTrashMail(String id, int tries)
        {
            while (tries > 0)
            {
                try
                {
                    TrashMail(id);
                    Console.ForegroundColor = Colors.Message;
                    Console.WriteLine("[GMail] Mail id=" + id + " trashed.");
                    return true;
                }
                catch
                {

                }
                tries--;
            }

            Console.ForegroundColor = Colors.Error;
            Console.WriteLine("[GMail] Couldn't trash mail id=" + id);
            return false;
        }

        /// <summary>
        /// Tries 5 times to sends a mail indicating the status of the program to the specified address. Returns whether it was successful
        /// <para>This method does NOT print information to the console.</para>
        /// </summary>
        /// <param name="address">The mail address to send status data to</param>
        /// <returns>Whether the mail was sent successfully</returns>
        public static bool SendStatusTo(String address)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    StringBuilder builder = new StringBuilder(512);
                    IAction[] actions = Program.queue.ToArray();
                    builder.Append("A PaintDrawer Status Request was recieved from this address.\r\nQueue<IAction>().ToArray() returned: {");
                    for (int a = 0; a < actions.Length; a++)
                    {
                        builder.Append("    ");
                        builder.Append(actions[a].ToString());
                        builder.Append("\r\n");
                    }
                    builder.Append("}\r\nThat is a total of ");
                    builder.Append(actions.Length);
                    builder.Append(" IAction-s.\r\n\r\n");

                    builder.Append("Program.Time = ");
                    builder.Append(Program.Time);

                    builder.Append("\r\nProgram.LastDraw = ");
                    builder.Append(Program.LastDraw);

                    builder.Append("\r\nAccount.LastMailRecieved = ");
                    builder.Append(Account.LastMailRecieved);

                    builder.Append("\r\nAccount.LastRead = ");
                    builder.Append(Account.LastRead);

                    builder.Append("\r\nAccount.LastSuccessfullRead = ");
                    builder.Append(Account.LastSuccessfullRead);
                    
                    var msg = new AE.Net.Mail.MailMessage
                    {
                        Subject = "PaintDrawer Status Request",
                        Body = builder.ToString(),
                        From = new MailAddress(MyMailAddress)
                    };
                    msg.To.Add(new MailAddress(address));
                    msg.ReplyTo.Add(msg.From); // Bounces without this!!
                    var msgStr = new StringWriter();
                    msg.Save(msgStr);

                    GmailService gmail = Account.service;
                    Message result = gmail.Users.Messages.Send(new Message { Raw = _base64UrlEncode(msgStr.ToString()) }, "me").Execute();

                    return true;
                }
                catch //(Exception e)
                {

                }
            }

            return false;
        }

        /// <summary>
        /// Tries 5 times to send a mail indicating the valid characters. Returns whether it was successful
        /// <para>This method does NOT print information to the console.</para>
        /// </summary>
        /// <param name="address">The mail address to send valid char data to</param>
        /// <returns>Whether the mail was sent successfully</returns>
        public static bool SendValidCharsTo(String address)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    StringBuilder builder = new StringBuilder(512);
                    builder.Append("Valid Characters: \r\n");

                    for (int ci = 0; ci < PaintDrawer.Letters.CharFont.MaxCharNumericValue; ci++)
                    {
                        char c = (char)ci;
                        if (Program.font.DoesCharExist(c))
                        {
                            builder.Append("\r\nChar ");
                            builder.Append(ci);
                            builder.Append("; '");
                            builder.Append(c);
                            builder.Append("'.");
                        }
                    }

                    builder.Append("Keep in mind these are encoded in UTF-16, which is the format chosen by the .NET Framework.");

                    var msg = new AE.Net.Mail.MailMessage
                    {
                        Subject = "PaintDrawer Valid Characters Request",
                        Body = builder.ToString(),
                        From = new MailAddress(MyMailAddress)
                    };
                    msg.To.Add(new MailAddress(address));
                    msg.ReplyTo.Add(msg.From); // Bounces without this!!
                    var msgStr = new StringWriter();
                    msg.Save(msgStr);

                    GmailService gmail = Account.service;
                    Message result = gmail.Users.Messages.Send(new Message { Raw = _base64UrlEncode(msgStr.ToString()) }, "me").Execute();

                    return true;
                }
                catch
                {

                }
            }

            return false;
        }

        /// <summary>
        /// Gets the new mails and adds new entries to the Queue. Also handles status requests, draw requests, etc.
        /// <para>Maybe I should have named this "ProcessMails" or something like that...</para>
        /// <para>This method prints information to the console.</para>
        /// </summary>
        /// <param name="queue">... the fucking queue</param>
        public static void AddNewToQueue(Queue<IAction> queue)
        {
            List<String> statusToList = new List<string>(3);
            List<RecievedMail> mails;
            if (ForceGetAllUnreadMails(5, out mails))
            {
                if (mails.Count == 0)
                {

                }
                else
                {
                    hasAddedDisconnectMessage = false;
                    LastMailRecieved = Program.Time;

                    #region ProcessMails
                    for (int i=0; i<mails.Count; i++)
                    {
                        RecievedMail m = mails[i];

                        #region ProcessMail
                        m.LoadFull();
                        Console.ForegroundColor = Colors.Message;
                        Console.WriteLine("\n[GMail] Got mail from " + (m.From ?? "unknown") + "; subject: " + (m.Subject ?? "unknown"));

                        if (String.IsNullOrEmpty(m.Text))
                        {
                            Console.ForegroundColor = Colors.Error;
                            Console.WriteLine("[GMail] Couldn't interpret mail's body. Discarding :(");
                        }
                        else
                        {
                            if (m.From.Contains("tic") && m.From.Contains("ort.edu"))
                            {
                                #region DaroMail

                                Console.ForegroundColor = Colors.Message;
                                Console.WriteLine("[GMail] Mail from tic@ort detected, extracting content from Fw.");

                                int until = m.Text.Length-1; // Ignores characters until it sees no more spaces or new lines.
                                while ((m.Text[until] == ' ' || m.Text[until] == '\n' || m.Text[until] == '\r') && until > 0) until--;

                                // Searches for the end of the "Daro Forward" marked by an "Asunto: <subject>\r\n"
                                // and ignores characters until it finds something useful. (no spaces, \n or \r)
                                int start = m.Text.Length > 200 ? (m.Text.IndexOf('\n', m.Text.IndexOf("Asunto: ", 200))) : 0;
                                if (start != -1)
                                    while ((m.Text[start] == ' ' || m.Text[start] == '\n' || m.Text[start] == '\r') && start < m.Text.Length) start++;

                                String extract;
                                if (until > start && until < m.Text.Length && start >= 0)
                                {
                                    extract = m.Text.Substring(start, until - start + 1);
                                }
                                else
                                {
                                    until = m.Text.Length - 1;
                                    while ((m.Text[until] == ' ' || m.Text[until] == '\n' || m.Text[until] == '\r') && until > 0) until--;
                                    start = 0;
                                    while ((m.Text[start] == ' ' || m.Text[start] == '\n' || m.Text[start] == '\r') && start < m.Text.Length) start++;

                                    if (until > start && until > 0 && until < m.Text.Length && start > 0 && start < m.Text.Length)
                                    {
                                        Console.ForegroundColor = Colors.Message;
                                        Console.WriteLine("[GMail] The Message could not be extracted from the Fw. Utilizing it 'as is'.");
                                        extract = m.Text.Substring(start, until - start + 1);
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = Colors.Error;
                                        Console.WriteLine("[GMail] Could not parse text from message. Discarding :(");
                                        extract = "";
                                    }
                                }

                                if (extract.Length != 0)
                                {
                                    Console.ForegroundColor = Colors.Success;
                                    Console.WriteLine("[GMail] Extracted: " + extract);
                                    _addWrite(queue, ref extract);
                                }
                                else
                                {
                                    Console.ForegroundColor = Colors.Message;
                                    Console.WriteLine("[GMail] Message discarded.");
                                }

                                #endregion
                            }
                            else
                            {
                                // Normal mail.
                                _processNormalMail(statusToList, queue, m);
                            }
                        }
                        #endregion

                        ForceTrashMail(m.Id, 5);
                    }
                    #endregion

                    #region RespondStatusRequests
                    for (int i = 0; i < statusToList.Count; i++)
                    {
                        String f = statusToList[i];
                        Console.ForegroundColor = Colors.Message;
                        Console.WriteLine("[GMail] Got Status Request from " + f);
                        if (SendStatusTo(f))
                        {
                            Console.ForegroundColor = Colors.Success;
                            Console.WriteLine("[GMail] Success sending status info to " + f);
                        }
                        else
                        {
                            Console.ForegroundColor = Colors.Error;
                            Console.WriteLine("[GMail] ERROR: Couldn't send status info to " + f + ".");
                        }
                    }
                    #endregion
                }
            }
            else
            {
                // if more than 3 minutes passed since it was able to read mails, we might have gotten disconnected...
                if (!hasAddedDisconnectMessage && LastRead - LastSuccessfullRead > 3*60 && queue.Count == 0 && Program.Time - Program.LastDraw > 5*60)
                {
                    Console.ForegroundColor = Colors.Error;
                    DateTime now = DateTime.Now;
                    Console.WriteLine("[GMail] Possibly lost connection. (" + now.Hour + ":" + now.Minute + ")");
                    queue.Enqueue(new SimpleWrite(Program.font, "Creo q me quede sin internet\n(" + now.Hour + ":" + now.Minute + ") (llamen a miz)"));
                    hasAddedDisconnectMessage = true;
                }
            }
        }

        /// <summary>
        /// Processes mails from non-tic@ort. This function is used in AddNewToQueue
        /// <para>This method prints information to the console.</para>
        /// </summary>
        /// <param name="statusToList">The list that holds the status requests to later be answered</param>
        /// <param name="queue">The main IAction queue</param>
        /// <param name="mail">The Mail to process</param>
        private static void _processNormalMail(List<String> statusToList, Queue<IAction> queue, RecievedMail mail)
        {
            if (mail.Subject.Contains("TEXT"))
            {
                #region WriteText
                int until = mail.Text.Length - 1;
                while ((mail.Text[until] == ' ' || mail.Text[until] == '\n' || mail.Text[until] == '\r') && until > 0) until--;
                int start = 0;
                while ((mail.Text[start] == ' ' || mail.Text[start] == '\n' || mail.Text[start] == '\r') && start < mail.Text.Length) start++;

                if (until > start && start >= 0 && until < mail.Text.Length)
                {
                    String extract = mail.Text.Substring(start, until - start + 1);
                    Console.ForegroundColor = Colors.Success;
                    Console.WriteLine("[GMail] Extracted: " + extract);
                    _addWrite(queue, ref extract);
                }
                else
                {
                    Console.ForegroundColor = Colors.Error;
                    Console.WriteLine("[GMail] Error parsing message.");
                }
                #endregion
            }
            else if (mail.Subject.Contains("STATUS"))
            {
                #region SendStatus
                statusToList.Add(mail.From);
                #endregion
            }
            else if (mail.Subject.Contains("DRAW"))
            {
                #region Draw
                if (String.IsNullOrEmpty(mail.Text))
                {
                    Console.ForegroundColor = Colors.Error;
                    Console.WriteLine("[GMail] Error parsing message.");
                }
                else
                {
                    if (mail.Text.Contains("ANGRY_CROMULON"))
                    {
                        queue.Enqueue(new DrawUndistortedChar(Program.font, (char)4));
                        Console.ForegroundColor = Colors.Success;
                        Console.WriteLine("[GMail] DRAW Command: ANGRY_CROMULON Enqueued.");
                    }
                    else if (mail.Text.Contains("LOLO"))
                    {
                        queue.Enqueue(new DrawUndistortedChar(Program.font, (char)3));
                        Console.ForegroundColor = Colors.Success;
                        Console.WriteLine("[GMail] DRAW Command: LOLO Enqueued.");
                    }
                    else if (mail.Text.Contains("CWMAP"))
                    {
                        queue.Enqueue(new DrawUndistortedChar(Program.font, (char)0));
                        Console.ForegroundColor = Colors.Success;
                        Console.WriteLine("[GMail] DRAW Command: CWMAP Enqueued.");
                    }
                    else if (mail.Text.Contains("REALMEN"))
                    {
                        queue.Enqueue(new DrawUndistortedChar(Program.font, (char)2));
                        Console.ForegroundColor = Colors.Success;
                        Console.WriteLine("[GMail] DRAW Command: REALMEN Enqueued.");
                    }
                    else if (mail.Text.Contains("CUAC"))
                    {
                        queue.Enqueue(new DrawUndistortedChar(Program.font, (char)1));
                        Console.ForegroundColor = Colors.Success;
                        Console.WriteLine("[GMail] DRAW Command: CUAC Enqueued.");
                    }
                    else
                    {
                        Console.ForegroundColor = Colors.Error;
                        Console.WriteLine("[GMail] DRAW Command Error: No such drawable found.");
                    }
                }
                #endregion
            }
            else if (mail.Subject.Contains("VALIDCHARS"))
            {
                #region SendValidChars
                SendValidCharsTo(mail.From);
                #endregion
            }
            else
            {
                Console.ForegroundColor = Colors.Message;
                Console.WriteLine("[GMail] Message from=" + mail.From + "; Subject=" + mail.Subject + " has no meaning! Discarding.");
            }
        }

        /// <summary>
        /// Checks a text and decides whether to add it or not to the queue.
        /// <para>This method prints information to the console.</para>
        /// </summary>
        /// <param name="queue">The main IAction queue</param>
        /// <param name="text">The text to add for printing</param>
        private static void _addWrite(Queue<IAction> queue, ref String text)
        {
            if (Program.font.IsStringOk(ref text))
            {
                /*if (SimpleWrite.IsSizeOk(Program.font, text, SimpleWrite.DefaultAt, SimpleWrite.DefaultSize))
                    queue.Enqueue(new SimpleWrite(Program.font, text));
                else
                {
                    Console.ForegroundColor = Colors.Error;
                    Console.WriteLine("[GMail] Text not added. Reason: the text is too big!");
                }*/
                queue.Enqueue(Actions.Actions.CreateSimpleWrite(Program.font, text));
            }
            else
            {
                Console.ForegroundColor = Colors.Error;
                Console.WriteLine("[GMail] Text not added. Reason: not all characters were valid.");
            }
        }

        /// <summary>
        /// converts from fucking stupid base64 string encoding that I dont understand...
        /// </summary>
        /// <param name="input">The string in base64 format</param>
        /// <returns>A string in non-base64 format</returns>
        private static string _base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
    }
}
