using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.AttachmentService
{
    public class UploadFile
    {
    //    private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
    //    {
    //        var replyMessage = context.MakeMessage();
    //        Attachment returnCard;

    //        var message = await result as Activity;

    //        // Check to see if the user is sending the bot a file.
    //        if (message.Attachments != null && message.Attachments.Any())
    //        {
    //            var attachment = message.Attachments.First();

    //            if (attachment.ContentType == FileDownloadInfo.ContentType)
    //            {
    //                FileDownloadInfo downloadInfo = (attachment.Content as JObject).ToObject<FileDownloadInfo>();
    //                if (downloadInfo != null)
    //                {
    //                    returnCard = CreateFileInfoAttachment(downloadInfo, attachment.Name, attachment.ContentUrl);
    //                    replyMessage.Attachments.Add(returnCard);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            // Illustrates creating a file consent card.
    //            returnCard = CreateFileConsentAttachment();
    //            replyMessage.Attachments.Add(returnCard);
    //        }
    //        await context.PostAsync(replyMessage);
    //    }


    //    private static Attachment CreateFileInfoAttachment(FileDownloadInfo downloadInfo, string name, string contentUrl)
    //    {
    //        FileInfoCard card = new FileInfoCard()
    //        {
    //            FileType = downloadInfo.FileType,
    //            UniqueId = downloadInfo.UniqueId
    //        };

    //        Attachment att = card.ToAttachment();
    //        att.ContentUrl = contentUrl;
    //        att.Name = name;

    //        return att;
    //    }

    //    private static Attachment CreateFileConsentAttachment()
    //    {
    //        JObject acceptContext = new JObject();
    //        // Fill in any additional context to be sent back when the user accepts the file.

    //        JObject declineContext = new JObject();
    //        // Fill in any additional context to be sent back when the user declines the file.

    //        FileConsentCard card = new FileConsentCard()
    //        {
    //            AcceptContext = acceptContext,
    //            DeclineContext = declineContext,
    //            SizeInBytes = 102635,
    //            Description = "File description"
    //        };

    //        Attachment att = card.ToAttachment();
    //        att.Name = "Example file";

    //        return att;
    //    }
    }

  

    //internal interface IDialogContext
    //{
    //    object MakeMessage();
    //    Task PostAsync(object replyMessage);
    //}
}
