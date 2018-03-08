using HelperUWP.Lib;
using HelperUWP.Pages;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.MailRef
{
    class MailUtil
    {
        public enum FolderType { Inbox,SentItem,JunkMail,Trash}
        private static IMailFolder[] folders = new IMailFolder[4];
        private static String[] strs = new String[] { "INBOX", "Sent Items", "Junk E-mail", "Trash" };
        public static ImapClient imapClient = new ImapClient();
        public static MailPage mailPage = null;
        public static async Task Login()
        {
            try
            {
                await imapClient.ConnectAsync("mail.pku.edu.cn", 993, true);
                //Connect("mail.pku.edu.cn", 993, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                imapClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await imapClient.AuthenticateAsync(Constants.username, Constants.password);
                //imapClient.Authenticate(Constants.username, Constants.password);
                var nss = imapClient.PersonalNamespaces;
                foreach (var x in nss)
                {
                    var boxes = imapClient.GetFolders(x);
                    foreach (var fold in boxes)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (fold.Name.Equals(strs[i]))
                                folders[i] = fold;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Constants.BoxPage.ShowMessage("      mail login failed!\n"+e.StackTrace+"\n      reason:\n"+e.Message);
            }
        }
        public static void Logout()
        {
            try
            {
                imapClient.Disconnect(true);
            }
            catch
            {

            }
        }
        public static IMailFolder GetMailFolder(FolderType type)
        {
            return folders[(int)type];
        }

        public static async Task<MimeMessage> GetMessageAsync(FolderType type, int index)
        {
            try
            {
                folders[(int)type].Open(FolderAccess.ReadOnly);
                return await folders[(int)type].GetMessageAsync(index);
            }
            catch
            {
                return null;
            }
        }
        public static void BackRequest()
        {
            if (mailPage == null) return;
            mailPage.InnerBackRequest();
        }
    }
}
