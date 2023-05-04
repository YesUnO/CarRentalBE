
using DataLayer.Entities.User;

namespace Core.ControllerModels.User
{
    public class VerifyDocumentRequestModel
    {
        public string CustomerMail { get; set; }
        public string DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public UserDocumentType UserDocumentType { get; set; }
    }
}
