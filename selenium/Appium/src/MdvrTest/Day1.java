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

import com.Seleium.www.LocatoSamaple;
import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;

public class Day1 {
	AppiumDriver drives;
	LocatoSamaple pl;
	@BeforeClass
	public void Bef() throws Exception{
		System.out.println("获取当前路径apk");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "MDVR.apk");
        System.out.println("app-"+app);
        //设置相关参数
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//启动appium
       // capabilities.setCapability("noReset", true); 	//	测试已安装的应用。不安装app
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //设置安卓系统版本   
        capabilities.setCapability("sessionOverride", true);//每次启动时覆盖session，否则第二次后运行会报错不能新建session  
        capabilities.setCapability("app", app.getAbsolutePath()); //安装app
        System.out.println("安装");
        //初始化连接安卓系统，
        drives = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        System.out.println("App is launched!"+drives);
		
	}
	@BeforeMethod
	public void Method(){	
		System.out.println("Method");
		pl=new LocatoSamaple(drives);
	}
	@Test
	public void Loginn() throws Exception{
		System.out.println("Loginn");
		String sip=pl.getElement("ip").getText();
		pl.getElement("ip").clear();
		pl.getElement("ip").sendKeys("192.168.20.57");
		pl.getElement("pwd").clear();
		String slogin=pl.getElement("login").getText();
		System.out.println("slogin:"+slogin);
		ty(5);
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
