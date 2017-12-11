using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crocus.TestUI
{
    [TestFixture]
    public class TestUI
    {
        private RMXML xmlConfig;
        private string PlatformUrl = "";
        private OpenQA.Selenium.IWebDriver driver;
        private TimeSpan tsNav;

        [TestFixtureSetUp]
        public void init()
        {
			
			this.xmlConfig = Core.GetRMXML("config.xml", true);
            this.PlatformUrl = $"http://{this.xmlConfig.gNode("Platform/IP").InnerText}" +
                $":{this.xmlConfig.gNode("Platform/Port").InnerText}";
            this.tsNav = TimeSpan.FromMilliseconds(Convert.ToInt32(this.xmlConfig.gNode("Platform/TimeSpanNav").InnerText));
            driver = Core.GetDriver(Core.eDriver.Chrome);
            //driver.Url = "http://61.161.206.118:8000";
            driver.Url = this.PlatformUrl;
			Console.WriteLine("11111111");

		}

        [TestFixtureTearDown]
        public void exitTest()
        {
            //this.driver.Quit();
            this.driver.Close();
        }

        [Test]
        public void tc_UserLogin()
        {
			Console.WriteLine("111tfc");
			XWebElement eIptUser = new XWebElement(this.driver, "//*[@id='inp_username']");
            XWebElement eIptPwd = new XWebElement(this.driver, "//*[@id='inp_password']");
            XWebElement eBtnLogin = new XWebElement(this.driver, "//*[@id='bt_submit']");
            XWebElement eErrormsg = new XWebElement(this.driver, "//*[@id='p_errormsg']");
            XWebElement eImgAuth = new XWebElement(this.driver, "//*[@id='img_authcode']");
            XWebElement eHrefEditPwd = new XWebElement(this.driver, "//*[@id='bs-example-navbar-collapse-1']/ul[2]/li[3]/a");
            XWebElement eIptOldPwd = new XWebElement(this.driver, "//*[@id='inp_oldpassword']");
            XWebElement eIptNewPwd = new XWebElement(this.driver, "//*[@id=\"inp_password\"]");
            XWebElement eIptNewPwd2 = new XWebElement(this.driver, "//*[@id=\"inp_password2\"]");
            XWebElement eBtnEditPwd = new XWebElement(this.driver, "//*[@id=\"bt_submit\"]");
            XWebElement eTipOldPwd = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[1]/p");
            XWebElement eTipNewPwd = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[2]/p");
            XWebElement eTipNewPwd2 = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[3]/p");
            XWebElement eEditPwdDialog = new XWebElement(this.driver, "//*[@id=\"div_addeditdialog\"]");
            XWebElement eEditPwdOldPwdErrorDialog = new XWebElement(this.driver, "//*[@id=\"div_alertdialog\"]");
            XWebElement eTipEditPwdSame = new XWebElement(this.driver, "//*[@id=\"errorInfo\"]");

            string user = "admin"; string pwd = "a";
            Core.Print($"User={user}, Pwd={pwd}, 测试短密码, except error");
            eIptUser.e.Clear(); eIptUser.e.SendKeys(user);
            eIptPwd.e.Clear(); eIptPwd.e.SendKeys($"{pwd}\t");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            Assert.IsTrue(eErrormsg.wt("请输入长度为 6 至 18 之间的字符"), "请输入长度为 6 至 18 之间的字符");

            user = "admin"; pwd = "1234567891234567891";
            Core.Print($"User={user}, Pwd={pwd}, 测试长密码，except error");
            eIptUser.e.Clear(); eIptUser.e.SendKeys(user);
            eIptPwd.e.Clear(); eIptPwd.e.SendKeys($"{pwd}\t");
            Assert.IsTrue(eErrormsg.wt("请输入长度为 6 至 18 之间的字符"), "请输入长度为 6 至 18 之间的字符");
            
            this.Login("admin", "123456");

            Core.Print("测试修改密码");
            eHrefEditPwd.click();            

            Core.Print($"Pwd=123, 测试短密码!");
            eIptOldPwd.e.Clear(); eIptOldPwd.e.SendKeys("123\t");
            Assert.IsTrue(eTipOldPwd.wt("请输入长度为 6 至 18 之间的字符"), "旧密码长度提示: 请输入长度为 6 至 18 之间的字符");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("123\t");
            Assert.IsTrue(eTipNewPwd.wt("请输入长度为 6 至 18 之间的字符"), "新密码长度提示: 请输入长度为 6 至 18 之间的字符");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("123\t");
            Assert.IsTrue(eTipNewPwd2.wt("请输入长度为 6 至 18 之间的字符"), "新密码2长度提示: 请输入长度为 6 至 18 之间的字符");

            Core.Print($"Pwd='', 测试空密码!");
            eIptOldPwd.e.Clear(); eIptOldPwd.e.SendKeys("\t");
            Assert.IsTrue(eTipOldPwd.wt("此项为必填项"), "旧密码为空提示: 此项为必填项");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("\t");
            Assert.IsTrue(eTipNewPwd.wt("此项为必填项"), "新密码为空提示: 此项为必填项");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("\t");
            Assert.IsTrue(eTipNewPwd2.wt("此项为必填项"), "新密码2为空提示: 此项为必填项");

            Core.Print($"Pwd=123456/987654, 测试两次密码不一致!");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("123456");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("987654\t");
            Assert.IsTrue(eTipNewPwd2.wt("您2次的输入不相同"), "两次密码不一致提示: 您2次的输入不相同");

            Core.Print($"Pwd=234567/123456/123456, 测试原密码错误修改密码!");
            eIptOldPwd.e.Clear(); eIptOldPwd.e.SendKeys("234567");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("123456");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("123456");
            eBtnEditPwd.click();            
            Assert.IsTrue(eEditPwdOldPwdErrorDialog.wv(), "原错误密码修改！");

            Core.Print($"Pwd=123456/123456/123456, 测试正确密码，但密码全一致!");            
            eIptOldPwd.e.Clear(); eIptOldPwd.e.SendKeys("123456");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("123456");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("123456");
            eBtnEditPwd.click();
            Assert.IsTrue(eTipEditPwdSame.wt("新密码与旧密码一致"), "新密码与旧密码一致");

            Core.Print($"Pwd=123456/#￥%……&&/#￥%……&&, 密码修改!");
            eIptOldPwd.e.Clear(); eIptOldPwd.e.SendKeys("123456");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("#￥%……&&");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("#￥%……&&");
            eBtnEditPwd.click(); 
            Assert.IsTrue(eEditPwdDialog.wnv(), "密码修改完成！");

            this.ExitSystem();
            this.Login("admin", "#￥%……&&");

            Core.Print($"Pwd=#￥%……&&/123456/123456, 再次修改为初始密码!");
            eHrefEditPwd.click();
            eIptOldPwd.e.Clear(); eIptOldPwd.e.SendKeys("#￥%……&&");
            eIptNewPwd.e.Clear(); eIptNewPwd.e.SendKeys("123456");
            eIptNewPwd2.e.Clear(); eIptNewPwd2.e.SendKeys("123456");
            eBtnEditPwd.click(); 
            Assert.IsTrue(eEditPwdDialog.wnv(), "密码修改完成！");

            this.ExitSystem();

            Core.Print("测试第3次错误密码登录出现验证码");
            for (int i = 0; i < 3; i++)
            {
                user = "admin"; pwd = "987654";
                if (i < 2) Core.Print($"{i}, User={user}, Pwd={pwd}, 测试错误密码登录，except error");
                else Core.Print($"{i}, User={user}, Pwd={pwd}, 测试错误密码登录，except error and Show auth image");
                eIptUser.e.Clear(); eIptUser.e.SendKeys(user);
                eIptPwd.e.Clear(); eIptPwd.e.SendKeys($"{pwd}");
                eBtnLogin.click();
                eBtnLogin.e.SendKeys("\t");
                if (i < 2)
                {
                    Assert.IsTrue(eErrormsg.wt("用户名或是密码错误"), "用户名或是密码错误");
                }
                else
                {
                    //http://61.161.206.118:8000/Plugin/RegisterLogin/Images/loading.gif
                    //http://61.161.206.118:8000/RegisterLogin.do?Action=AuthCode&Guid=1500974035002
                    Assert.GreaterOrEqual(eImgAuth.e.GetAttribute("src").IndexOf("RegisterLogin"), 0
                        , "AuthImage url must include 'RegisterLogin'");
                }
            }
            
        }

        [Test]
        public void tc_TestRole()
        {
            
            XWebElement mBasicInfo = new XWebElement(this.driver, "//*[@id=\"basicInfo\"]/a");
            XWebElement mRole = new XWebElement(this.driver, "//*[@id=\"basicInfoMenu\"]/li[2]/a");
            //XWebElement tabRole = new XWebElement(this.driver, "//*[@id=\"div_content\"]/ul/li[last()]/a[contains(text(),\"角色用户\")]");
            XWebElement tabRole = new XWebElement(this.driver, "//*[@id=\"div_content\"]/ul/li[last()]/a[text()=\"角色用户\"]");
            XWebElement tabRoleClose = new XWebElement(this.driver, "//*[@id=\"div_content\"]/ul/li[last()]/span");
            
            XWebElement frmRoleList = new XWebElement(this.driver, "//iframe[contains(@src,'RoleUser')]");
            XWebElement itmAllRoles = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[1]", frmRoleList);
            XWebElement bAddRole = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[1]/span", frmRoleList);
            XWebElement dRoleDialog = new XWebElement(this.driver, "//*[@id=\"div_role\"]", frmRoleList);
            XWebElement dRoleDialogTitle = new XWebElement(this.driver, "//*[@id=\"div_role\"]/div/div/div[1]/h4", frmRoleList);
            XWebElement tRoleName = new XWebElement(this.driver, "//*[@id=\"inp_rolename\"]", frmRoleList);
            XWebElement tRoleNameNotNullTip = new XWebElement(this.driver, "//*[@id=\"form_role\"]/div[1]/p", frmRoleList);
            XWebElement chkAllRole = new XWebElement(this.driver, "//*[@id=\"inp_checkall\"]", frmRoleList);
            XWebElement chkRoleNotNullTip=new XWebElement(this.driver, "//*[@id=\"form_role\"]/div[2]/p", frmRoleList);
            XWebElement bSubmitRole = new XWebElement(this.driver, "//*[@id=\"bt_submit_role\"]", frmRoleList);
            //XWebElement itmRole = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[last() and contains(text(), \"123\")]", frmRoleList);
            //XWebElements chkRoles = new XWebElements(this.driver, "//*[@id='tb_power']/tbody//input[@type='checkbox']", frmRoleList);
            XWebElement itmRole = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[last()]", frmRoleList);
            XWebElement bEditRole = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[last()]/span[2]", frmRoleList);
            XWebElement itmRoleAdmin = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[2]", frmRoleList);
            XWebElement bDelRoleAdmin = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[2]/span[1]", frmRoleList);
            XWebElement dDelRoleAdminErrTip = new XWebElement(this.driver, "//*[@id=\"div_alertdialog\"]", frmRoleList);

            XWebElement bAddUser = new XWebElement(this.driver, "/html/body/div[1]/div[2]/div[2]/div[1]/div[2]/div/button[1]", frmRoleList);
            XWebElement dUserDialog = new XWebElement(this.driver, "//*[@id=\"div_addeditdialog\"]", frmRoleList);
            XWebElement dUserDialogTitle = new XWebElement(this.driver, "//*[@id=\"div_addeditdialog\"]/div/div/div[1]/h4", frmRoleList);
            XWebElement cboFolder = new XWebElement(this.driver, "//*[@id=\"inp_dirname\"]", frmRoleList);
            XWebElement chkFolder = new XWebElement(this.driver, "/html/body/div[11]/ul/li/div/span[3]", frmRoleList);
            XWebElement cboGroup = new XWebElement(this.driver, "//*[@id=\"inp_grouppower\"]", frmRoleList);
            XWebElement cboGroupErrTip = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[3]/p", frmRoleList);
            XWebElement chkGroup = new XWebElement(this.driver, "/html/body/div[10]/ul/li/div/span[3]", frmRoleList);
            XWebElement tUser = new XWebElement(this.driver, "//*[@id=\"inp_username\"]", frmRoleList);
            XWebElement tUserErrTip = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[4]/p", frmRoleList);
            XWebElement tPwd = new XWebElement(this.driver, "//*[@id=\"inp_password\"]", frmRoleList);
            XWebElement tPwdErrTip = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[5]/p", frmRoleList);
            XWebElement tPwd2 = new XWebElement(this.driver, "//*[@id=\"confirmpassword\"]", frmRoleList);
            XWebElement tPwd2ErrTip = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[6]/p", frmRoleList);
            XWebElement tPhone = new XWebElement(this.driver, "//*[@id=\"inp_telphone\"]", frmRoleList);
            XWebElement tPhoneErrTip = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[7]/p", frmRoleList);
            XWebElement tEmail = new XWebElement(this.driver, "//*[@id=\"inp_email\"]", frmRoleList);
            XWebElement tEmailErrTip = new XWebElement(this.driver, "//*[@id=\"form_user\"]/div[8]/p", frmRoleList);
            XWebElement bSubmitUser = new XWebElement(this.driver, "//*[@id=\"bt_submit\"]", frmRoleList);

            XWebElement bEditUser = new XWebElement(this.driver, "//*[@id=\"div_content\"]/div[1]/div[2]/div/table/tbody/tr[last()]/td[8]/div/a[1]", frmRoleList);
            XWebElement bDelUser = new XWebElement(this.driver, "//*[@id=\"div_content\"]/div[1]/div[2]/div/table/tbody/tr[last()]/td[8]/div/a[2]", frmRoleList);
            XWebElement bDelUserConfirm = new XWebElement(this.driver, "//*[@id=\"div_confirmdialog\"]/div/div/div[3]/button[2]", frmRoleList);
            XWebElement bDelRole = new XWebElement(this.driver, "//*[@id=\"div_rolelist\"]/a[last()]/span[1]", frmRoleList);
            XWebElement bDelRoleConfirm = new XWebElement(this.driver, "//*[@id=\"div_deleterole\"]/div/div/div[3]/button[2]", frmRoleList);
            
            this.Login("admin", "123456");

            mBasicInfo.click();
            mRole.click();
            Core.Print("添加角色");
            itmAllRoles.mmove();
            bAddRole.click();
            Assert.IsTrue(dRoleDialog.wv(),"显示添加角色对话框");
            Assert.AreEqual("添加", dRoleDialogTitle.e.Text, "添加权限对话框标题");
            Assert.IsTrue(dRoleDialogTitle.wt("添加"),"对话框标题为：添加");
            Core.Print("测试角色必填写项、角色名称长度>18、必需选择权限");
            bSubmitRole.click();
            Assert.IsTrue(tRoleNameNotNullTip.wt("此项为必填项"), "角色名空提示：此项为必填项");
            Assert.IsTrue(chkRoleNotNullTip.wt("请选择权限"), "角色权限选择提示：请选择权限");
            tRoleName.e.SendKeys("1234567899876543211\t");
            bSubmitRole.click();
            Assert.IsTrue(tRoleNameNotNullTip.wt("最多 18 个字"), "角色名长提示：最多 18 个字");            

            Core.Print("设置角色名为: ad12测试@，勾选所有权限");
            tRoleName.e.Clear(); tRoleName.e.SendKeys("ad12测试@");            
            chkAllRole.click();
            bSubmitRole.click();
            Assert.IsTrue(dRoleDialog.wnv(),"角色对话框关闭");

            Core.Print("修改角色名为：abc");
            itmRole.mmove(); bEditRole.click();
            Assert.IsTrue(dRoleDialog.wv(), "显示编辑角色对话框");
            Assert.AreEqual("编辑", dRoleDialogTitle.e.Text, "编辑权限对话框标题");
            tRoleName.e.Clear(); tRoleName.e.SendKeys("abc");
            bSubmitRole.click();
            Assert.IsTrue(dRoleDialog.wnv(), "角色对话框关闭");

            Core.Print("删除系统管理员角色");
            itmRoleAdmin.mmove();
            bDelRoleAdmin.click();
            Assert.IsTrue(dDelRoleAdminErrTip.wv(), "删除系统管理员角色错误提示");
            

            Core.Print("添加用户");
            itmRole.click();
            bAddUser.click();
            Assert.IsTrue(dUserDialog.wv(), "用户对话框显示");
            Assert.AreEqual("添加", dUserDialogTitle.e.Text, "添加用户对话框标题");

            Core.Print("项目必填项测试");
            bSubmitUser.click();            
            Assert.IsTrue(cboGroupErrTip.wt("此项为必填项"), "车组选择：此项为必填项");
            Assert.IsTrue(tUserErrTip.wt("此项为必填项"), "帐户名：此项为必填项");
            Assert.IsTrue(tPwdErrTip.wt("此项为必填项"), "密码：此项为必填项");
            Assert.IsTrue(tPwd2ErrTip.wt("此项为必填项"), "确认密码：此项为必填项");
            Assert.IsTrue(tEmailErrTip.wt("此项为必填项"), "邮件地址：此项为必填项");
            Core.Print("短密码和错误邮件地址测试、电话号码长度测试");
            tPwd.e.SendKeys("1"); tPwd2.e.SendKeys("1"); tEmail.e.SendKeys("1"); tPhone.e.SendKeys("1");
            bSubmitUser.click();
            Assert.IsTrue(tPwdErrTip.wt("请输入长度为 6 至 18 之间的字符"), "密码：请输入长度为 6 至 18 之间的字符");
            Assert.IsTrue(tPwd2ErrTip.wt("请输入长度为 6 至 18 之间的字符"), "确认密码：请输入长度为 6 至 18 之间的字符");
            Assert.IsTrue(tEmailErrTip.wt("请输入有效的电子邮件"), "邮件地址：请输入有效的电子邮件");
            Assert.IsTrue(tPhoneErrTip.wt("请输入长度为 11 至 11 之间的字符"), "邮件地址：请输入长度为 11 至 11 之间的字符");
            Core.Print("长密码测试");
            tPwd.e.Clear(); tPwd2.e.Clear();
            tPwd.e.SendKeys("1234567899876543211"); tPwd2.e.SendKeys("1234567899876543211");
            bSubmitUser.click();
            Assert.IsTrue(tPwdErrTip.wt("请输入长度为 6 至 18 之间的字符"), "密码：请输入长度为 6 至 18 之间的字符");
            Assert.IsTrue(tPwd2ErrTip.wt("请输入长度为 6 至 18 之间的字符"), "确认密码：请输入长度为 6 至 18 之间的字符");
            Core.Print("两次密码不同测试");
            tPwd.e.Clear(); tPwd2.e.Clear();
            tPwd.e.SendKeys("123456"); tPwd2.e.SendKeys("234567");
            bSubmitUser.click();
            Assert.IsTrue(tPwd2ErrTip.wt("您2次的输入不相同"), "确认密码：您2次的输入不相同");

            Core.Print("创建用户：选择根文件夹，选择根车组，用户名为：2a#测试，密码为123456，邮件地址为111@111.com");
            tPwd.e.Clear(); tPwd2.e.Clear(); tEmail.e.Clear(); tPhone.e.Clear();
            cboFolder.click(); chkFolder.click(); dUserDialogTitle.click();         
            cboGroup.click(); chkGroup.click(); dUserDialogTitle.click(); 
            tUser.e.SendKeys("2a#测试");
            tPwd.e.SendKeys("123456"); tPwd2.e.SendKeys("123456");
            tEmail.e.SendKeys("111@111.com");
            bSubmitUser.click();
            Assert.IsTrue(dUserDialog.wnv(), "用户对话框关闭");

            Core.Print("关闭角色用户Tab，重新打开");
            tabRoleClose.click(); mBasicInfo.click(); mRole.click();
            itmRole.click();

            Core.Print("修改用户：用户名为：123，密码234567，邮件地址为222@222.com");
            bEditUser.click();
            Assert.IsTrue(dUserDialog.wv(), "显示修改用户对话框");
            Assert.AreEqual("编辑", dUserDialogTitle.e.Text, "编辑用户对话框标题");
            Assert.AreEqual("2a#测试", tUser.e.GetAttribute("old"), "检查用户名是否为：2a#测试");
            tUser.e.Clear(); tUser.e.SendKeys("123");
            tPwd.e.Clear(); tPwd.e.SendKeys("234567");
            tPwd2.e.Clear(); tPwd2.e.SendKeys("234567");
            tEmail.e.Clear(); tEmail.e.SendKeys("222@222.com");
            bSubmitUser.click();

            this.ExitSystem();
            this.Login("123", "234567");

            this.ExitSystem();
            this.Login("admin", "123456");

            Core.Print("删除新建的用户");
            mBasicInfo.click();
            mRole.click();
            itmRole.click();
            bDelUser.click();
            bDelUserConfirm.click();

            Core.Print("删除新建的角色");
            itmRole.mmove();
            bDelRole.click();
            bDelRoleConfirm.click();
        }


        private void Login(string u, string p)
        {
            XWebElement iptUser = new XWebElement(this.driver, "//*[@id='inp_username']");
            XWebElement iptPwd = new XWebElement(this.driver, "//*[@id='inp_password']");
            XWebElement bLogin = new XWebElement(this.driver, "//*[@id='bt_submit']");
            Core.Print($"用户登录: {u}/{p}");
            iptUser.e.Clear(); iptPwd.e.Clear();
            iptUser.e.SendKeys(u); iptPwd.e.SendKeys(p);
            bLogin.click();
        }

        private void ExitSystem()
        {
            XWebElement mExitSystem = new XWebElement(this.driver, "//*[@id='bs-example-navbar-collapse-1']/ul[2]/li[4]/a");
            XWebElement bExit = new XWebElement(this.driver, "//*[@id='div_confirmdialog']/div/div/div[3]/button[2]");
            Core.Print("退出系统");
            mExitSystem.click(); bExit.click();
        }
    }

    
}
