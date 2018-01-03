package Demo1;

import java.net.URL;

import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.CapabilityType;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.AfterClass;

import com.Seleium.www.ApkDrivcesr;
import com.Seleium.www.Assist;

import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;

public class TestMdvr {
	private static AppiumDriver driver; 
	   WebElement el;
	public static void main(String[] args) throws Exception {
		// TODO Auto-generated method stub
		 /**
         * com.example.android.contactmanager  
         * com.streamaxtech.mdvr.direct  运维宝
         *  com.streamaxtech.mdvr.direct.WifiListActivity
         */
        /**
		 * MainActivity、ContactManager
		 * 
		 */
		//Demo1();
		//Demo2();
		ApkDrivcesr appDrivcesr=new ApkDrivcesr();
		DesiredCapabilities capabilities = appDrivcesr.GetCap();
		driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(10);
        Assist.pn("初始化完成BeforeClass");
        
		tearDown() ;
	}
	public static void Demo2() throws Exception{
		
		System.out.println("EeforClass");	     
        //设置相关参数
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//启动appium
        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
    	//capabilities.setCapability("sessionOverride", true);//每次启动时覆盖session，否则第二次后运行会报错不能新建session  
        capabilities.setCapability("deviceName","Android Emulator");  //安卓设置名字
        capabilities.setCapability("platformName", "Android"); //安卓自动化还是IOS自动化
        capabilities.setCapability("platformVersion", "4.4");    //设置安卓系统版本   
        capabilities.setCapability("appPackage", "com.streamaxtech.mdvr.direct");
        Assist.ty(2);
        capabilities.setCapability("appActivity", "MaintenanceActivity"); 
        Assist.pn("初始化...."+capabilities);
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(10);
        Assist.pn("初始化完成BeforeClass");
	}
	public static void Demo1() throws Exception{
		
		System.out.println("EeforClass");	     
        //设置相关参数
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//启动appium
        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
    	//capabilities.setCapability("sessionOverride", true);//每次启动时覆盖session，否则第二次后运行会报错不能新建session  
        capabilities.setCapability("deviceName","Android Emulator");  //安卓设置名字
        capabilities.setCapability("platformName", "Android"); //安卓自动化还是IOS自动化
        capabilities.setCapability("platformVersion", "4.4");    //设置安卓系统版本   
        capabilities.setCapability("appPackage", "com.example.android.contactmanager");
        Assist.ty(2);
        capabilities.setCapability("appActivity", "ContactManager"); 
        Assist.pn("初始化...."+capabilities);
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(3);
        Assist.pn("初始化完成BeforeClass");
	}

    public static void tearDown() throws Exception {  
    	  System.out.println("@AfterClass+quit");
        driver.quit();  
    }  
}
