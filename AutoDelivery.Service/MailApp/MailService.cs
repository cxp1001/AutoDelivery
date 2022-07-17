using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using AutoDelivery.Domain.User;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AutoDelivery.Service.MailApp
{
    public class MailService : IMailService
    {
        private readonly IRepository<MailContent> _mailRepo;
        private readonly IRepository<UserAccount> _userRepo;
        private readonly IRepository<Product> _productRepo;

        public MailService(IRepository<MailContent> mailRepo, IRepository<UserAccount> userRepo, IRepository<Product> productRepo)
        {
            this._mailRepo = mailRepo;
            this._userRepo = userRepo;
            this._productRepo = productRepo;
        }


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
        public bool SendActiveEmail(string mailTo, string mailTitle, string mailContent)
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

            MailMessage mailMessage = new MailMessage(from, to);//实例化邮件信息实体并设置发送方和接收方
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


        /// <summary>
        /// 为产品设置邮件内容
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public async Task<MailContent> ConfigMailContent(int userId, int productId, MailContent mailContent)
        {

            var targetProduct = await _productRepo.GetQueryable().Include(p => p.MailContent).FirstOrDefaultAsync(p => p.Id == productId);

            if (targetProduct == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 8,
                        ErrorMessage = "target product is not exist.",
                        Time = DateTimeOffset.Now
                    }));

            }

            // 判断用户拥有对当前产品进行更改的权限
            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 2,
                        ErrorMessage = "user is null",
                        Time = DateTimeOffset.Now
                    }
                    )

                );
            }
            if (!currentUser.Products.Select(p => p.Id).Contains(targetProduct.Id))
            {
                throw new Exception(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 5,
                        ErrorMessage = "The current user does not have permission to change the current product configuration",
                        Time = DateTimeOffset.Now
                    }));
            }


            targetProduct.MailContent = mailContent;

            await _productRepo.UpdateAsync(targetProduct);

            return mailContent;

        }



        //为订单中的每一个产品分别生成邮件

        public async Task<MailMessage> GenerateMail(int productId, List<Serial> serials)
        {
            var product = await _productRepo.GetQueryable().Include(p => p.MailContent).FirstAsync(p => p.Id == productId);
            // if (product.MailContent != null)
            {
                var mail = product.MailContent;
                var mailTitle = mail.MailTitle;
                var mailContent = mail.MainContent;
                StringBuilder sb = new(mailContent);

                var activeKey = (bool)product.HasActiveKey == true ? 1 : 0;
                var activeLink = (bool)product.HasActiveLink == true ? 2 : 0;
                var serialNum = (bool)product.HasSerialNum == true ? 4 : 0;
                var subActiveKey = (bool)product.HasSubActiveKey == true ? 8 : 0;

                // 判断当前产品存在的激活方式
                var res = activeKey | activeLink | serialNum | subActiveKey;

                switch (res)
                {
                    case 1:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey));
                        break;
                    case 2:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink));
                        break;
                    case 3:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink));
                        break;
                    case 4:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "シリアルキー" + s.SerialNumber));
                        break;
                    case 5:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "シリアルキー" + s.SerialNumber));
                        break;
                    case 6:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink).Append(Environment.NewLine + "シリアルキー" + s.SerialNumber));
                        break;
                    case 7:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink).Append(Environment.NewLine + "シリアルキー" + s.SerialNumber));
                        break;
                    case 8:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 9:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 10:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 11:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 12:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "シリアルキー" + s.SerialNumber).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 13:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "シリアルキー" + s.SerialNumber).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 14:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink).Append(Environment.NewLine + "シリアルキー" + s.SerialNumber).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    case 15:
                        serials.ForEach(s => sb.Append(Environment.NewLine + "アクティブキー：" + s.ActiveKey).Append(Environment.NewLine + "アクティブリンク：" + s.ActiveLink).Append(Environment.NewLine + "シリアルキー" + s.SerialNumber).Append(Environment.NewLine + "サブアクティブキー：" + s.SubActiveKey));
                        break;
                    default:
                        break;
                }

                MailMessage mailMessage = new();
                mailMessage.Body = sb.ToString();
                mailMessage.Subject = mailTitle;

                return mailMessage;

            }


        }


    }





}
