using System.Net;
using System.Net.Mail;
using System.Text;
using AutoDelivery.Domain.Mail;

namespace AutoDelivery.Service.MailHelper
{
    public static class MailService
    {

        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <param name="stmpServer">smtp服务器地址</param>
        /// <param name="mailAccount">发信邮箱账号</param>
        /// <param name="pwd">发信邮箱密码</param>
        /// <param name="mailTo">接收人邮件</param>
        /// <param name="mailTitle">发送邮件标题</param>
        /// <param name="mailContent">发送邮件内容</param>
        /// <returns></returns>
        public static bool SendActiveEmail(string stmpServer, string mailAccount, string pwd, string mailTo, string mailTitle, string mailContent)
        {
          
            //邮件服务设置
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = stmpServer;//指定发送方SMTP服务器
            smtpClient.EnableSsl = true;//使用安全加密连接
            smtpClient.UseDefaultCredentials = true;//不和请求一起发送
            smtpClient.Credentials = new NetworkCredential(mailAccount, pwd);//设置发送账号密码

            MailMessage mailMessage = new MailMessage(mailAccount, mailTo);//实例化邮件信息实体并设置发送方和接收方
            mailMessage.Subject = mailTitle;//设置发送邮件得标题
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