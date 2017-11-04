using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.GMail
{
    /// <summary>
    /// Encapsulates a recieved mail. This might have partial information or full information
    /// </summary>
    class RecievedMail
    {
        private bool _complete;
        /// <summary>Gets whether the object has all the mail message's values loaded</summary>
        public bool IsFullyLoaded { get { return _complete; } }

        private String _id;
        /// <summary>Gets the identifier that identifies this mail message</summary>
        public String Id { get { return _id; } }

        private String _snippet;
        public String Snippet { get { return _snippet; } }

        private String _text;
        /// <summary>Gets the mail message's body text</summary>
        public String Text { get { return _text; } }

        private String _from;
        /// <summary>Gets the address that sent this mail</summary>
        public String From { get { return _from; } }

        private String _subject;
        public String Subject { get { return _subject; } }

        public RecievedMail(String id)
        {
            _complete = false;
            this._id = id;
        }

        /// <summary>
        /// Attempts to fully load the mail. IsFullyLoaded will return true after this if it succeeded.
        /// </summary>
        /// <returns>Whether the operation succeeded in loading everything</returns>
        public bool LoadFull()
        {
            try
            {
                UsersResource.MessagesResource.GetRequest request = Account.service.Users.Messages.Get("me", _id);
                request.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                Message message = request.Execute();
                MessagePart part = message.Payload;
                _snippet = message.Snippet;
                try
                {
                    _text = Encoding.UTF8.GetString(Convert.FromBase64String(part.Parts[0].Body.Data));
                }
                catch
                {
                    
                }

                foreach (MessagePartHeader h in part.Headers)
                {
                    if (h.Name == "Return-Path")
                        _from = h.Value.Substring(1, h.Value.Length - 2);
                    else if (h.Name == "Subject")
                        _subject = h.Value;
                }

                _complete = true;
                return true;
            }
            catch //(Exception e)
            {

            }

            return false;
        }
    }
}
