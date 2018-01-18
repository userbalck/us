
package MdvrTest;

import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;
import org.openqa.selenium.By;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.AfterClass;
import org.testng.annotations.BeforeClass;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import com.Seleium.www.ApkDrivcesr;
import com.Seleium.www.Assertion;
import com.Seleium.www.Assist;
import com.Seleium.www.Config;
import com.Seleium.www.LocatoSamaple;
import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;

public class Day1 {
	AppiumDriver drives;
	LocatoSamaple pl;
	
	@BeforeClass
	public void Befor() throws Exception{
		ApkDrivcesr apk=new ApkDrivcesr();
		DesiredCapabilities capabilities=apk.GetCap();
        //初始化连接安卓系统，
        drives = new AndroidDriver(new URL("http://192.168.30.80:4723/wd/hub"), capabilities);
        System.out.println("App is launched!"+drives);
		Assist.ty(5);
	}
	@BeforeMethod
	public void Method() throws Exception{	
		System.out.println("Method");
		Assist.ty(10);
		pl=new LocatoSamaple(drives);
	}
	@Test
	public void Loginn() throws Exception{
		//登陆用例
		String ip=Config.htp;
				System.out.println("Loginn");
				String sip=pl.getElement("ip").getText();
				pl.getElement("ip").clear();
				pl.getElement("ip").sendKeys(ip);
				pl.getElement("pwd").clear();
				String slogin=pl.getElement("login").getText();
				pl.getElement("login").click();
				ty(10);
				System.out.println("登陆成功");
				System.out.println("slogin:"+slogin);
				Assertion.verifyEquals(slogin, slogin);
				ty(5);
	}
	@Test
	public void Loginn2() throws Exception{
		//通用默写功能点击
		pl.getElement("info").click();
		ty(10);
		pl.getElement("module").click();
		ty(10);
		pl.getElement("storage").click();
		ty(10);
		pl.getElement("version").click();
		ty(10);
		pl.getElement("crocus").click();
		ty(10);
		pl.getElement("hwconfig").click();
		ty(10);
		pl.getElement("other").click();
		ty(20);
		
	}
	 @AfterClass
	    public void tearDown() throws Exception {  
	    	  System.out.println("quit");
	    	  drives.quit();  
	    }  
	 
	   public void ty(int tiem) throws Exception {
		  try {
			  System.out.println("等待秒："+tiem);
			  int t=tiem*1000;
			   Thread.sleep(t);
			   
		} catch (Exception e) {
			// TODO: handle exception
		}
		   
	   }
}
