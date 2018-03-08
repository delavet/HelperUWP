using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HelperUWP.MailRef.MailUtil;

namespace HelperUWP.MailRef
{
    /// <summary>
    /// 该类用于被传递给MailDetailPage作为定位信息来获取制定邮件
    /// 包括两个信息：邮件文件夹和邮件在文件夹中的编号
    /// </summary>
    class MailInfo
    {
        public FolderType MailFolderType;
        public int Index;
        public MailInfo(FolderType type,int idx)
        {
            MailFolderType = type;
            Index = idx;
        }
    }
}
