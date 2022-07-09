using System.Net;
using System.Net.Mail;
using System.Text;
using AutoDelivery.Domain.Mail;

namespace AutoDelivery.Service.MailApp
{
    public class MailService : IMailService
    {

        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <param name="smtpServer">smtp服务器地址</param>
        /// <param name="mailAccount">发信邮箱账号</param>
        /// <param name="pwd">发信邮箱密码</param>
        /// <param name="mailTo">接收人邮件</param>
        /// <param name="mailTitle">发送邮件标题</param>
        /// <param name="mailContent">发送邮件内容</param>
        /// <returns></returns>
        public  bool SendActiveEmail(string mailTo, string mailTitle, string mailContent)
        {

            string mailAccount = "udeiie@live.com";
            string pwd = "Sunrisep1001";


            //邮件服务设置
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.EnableSsl = true;//使用安全加密连接
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("udeiie@live.com", pwd);//设置发送账号密码
            MailAddress from = new MailAddress(mailAccount, "mailForm", Encoding.UTF8);
            MailAddress to = new MailAddress(mailTo, "mailTo", Encoding.UTF8);

            MailMessage mailMessage = new MailMessage(from,to);//实例化邮件信息实体并设置发送方和接收方
            // mailMessage.From = from;
            // mailMessage.To.Add(to);

            mailMessage.Subject = mailTitle;//设置发送邮件得标题
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Body = mailContent;//设置发送邮件内容
            mailMessage.BodyEncoding = Encoding.UTF8;//设置发送邮件的编码
            mailMessage.IsBodyHtml = false;//设置标题是否为HTML格式
            mailMessage.Priority = MailPriority.Normal;//设置邮件发送优先级

            try
            {
               smtpClient.Send(mailMessage);//发送邮件
                return true;
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }




    

    }
}