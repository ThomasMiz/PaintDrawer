using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.GMail
{
    class RecievedMail
    {
        private bool _complete;
        public bool IsFullyLoaded { get { return _complete; } }

        private String _id;
        public String Id { get { return _id; } }

        private String _snippet;
        public String Snippet { get { return _snippet; } }

        private String _text;
        public String Text { get { return _text; } }

        private String _from;
        public String From { get { return _from; } }

        private String _subject;
        public String Subject { get { return _subject; } }

        public RecievedMail(String id)
        {
            _complete = false;
            this._id = id;
        }

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
            catch (Exception e)
            {

            }

            return false;
        }
    }
}
