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
         * com.streamaxtech.mdvr.direct  ��ά��
         *  com.streamaxtech.mdvr.direct.WifiListActivity
         */
        /**
		 * MainActivity��ContactManager
		 * 
		 */
		//Demo1();
		//Demo2();
		ApkDrivcesr appDrivcesr=new ApkDrivcesr();
		DesiredCapabilities capabilities = appDrivcesr.GetCap();
		driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(10);
        Assist.pn("��ʼ�����BeforeClass");
        
		tearDown() ;
	}
	public static void Demo2() throws Exception{
		
		System.out.println("EeforClass");	     
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
    	//capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("deviceName","Android Emulator");  //��׿��������
        capabilities.setCapability("platformName", "Android"); //��׿�Զ�������IOS�Զ���
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("appPackage", "com.streamaxtech.mdvr.direct");
        Assist.ty(2);
        capabilities.setCapability("appActivity", "MaintenanceActivity"); 
        Assist.pn("��ʼ��...."+capabilities);
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(10);
        Assist.pn("��ʼ�����BeforeClass");
	}
	public static void Demo1() throws Exception{
		
		System.out.println("EeforClass");	     
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
    	//capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("deviceName","Android Emulator");  //��׿��������
        capabilities.setCapability("platformName", "Android"); //��׿�Զ�������IOS�Զ���
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("appPackage", "com.example.android.contactmanager");
        Assist.ty(2);
        capabilities.setCapability("appActivity", "ContactManager"); 
        Assist.pn("��ʼ��...."+capabilities);
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(3);
        Assist.pn("��ʼ�����BeforeClass");
	}

    public static void tearDown() throws Exception {  
    	  System.out.println("@AfterClass+quit");
        driver.quit();  
    }  
}
