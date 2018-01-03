package Demo1;

import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;

import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.CapabilityType;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.BeforeClass;
import org.testng.annotations.Test;

import com.Seleium.www.Assist;

import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;

public class Test3 {
	/**
	 * ����ϵͳӦ��,��������
	 */
	 private AppiumDriver driver; 
	   WebElement el;
	   @BeforeClass
		public void EeforClass() throws Exception{
			System.out.println("EeforClass");	     
	        //������ز���
	        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
	        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
	    	//capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�������½�session  
	        capabilities.setCapability("deviceName","Android Emulator");  //��׿��������
	        capabilities.setCapability("platformName", "Android"); //��׿�Զ�������IOS�Զ���
	        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
	        capabilities.setCapability("appPackage", "com.example.android.contactmanager");
	        Assist.ty(2);
	        capabilities.setCapability("appActivity", "ContactManager"); 
	        /**
	         * com.example.android.contactmanager  
	         * com.streamaxtech.mdvr.direct  ��ά��
	         *  com.streamaxtech.mdvr.direct.WifiListActivity
	         */
	        /**
	         * ContactManager
			 * MainActivity��WifiListActivity��MaintenanceActivity
			 * 
			 */
	        
	        
	      //��ʼ�����Ӱ�׿ϵͳ��
	        Assist.pn("��ʼ��...."+capabilities);
	        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
	        Assist.ty(10);
	        Assist.pn("��ʼ�����BeforeClass");
		}
	
	@Test
	public void Test1() throws Exception{
		Assist.pn("Test1�������");
		Assist.ty(10);
		
		
	}
	//@BeforeClass
		public void Eef() throws Exception{
			System.out.println("Demo1");
	        File classpathRoot = new File(System.getProperty("user.dir")); 
	        System.out.println("SetupApi--------------1"+classpathRoot);
	        File appDir = new File(classpathRoot, "/apps");
	        File app = new File(appDir, "ContactManager.apk");
	        System.out.println("app-"+app);
	        //������ز���
	        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
	    	capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�������½�session  
	        capabilities.setCapability("deviceName","Android Emulator");  //
	        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
	        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp
	      //��ʼ�����Ӱ�׿ϵͳ��
	        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
	        Assist.ty(10);
	        Assist.pn("��ʼ�����BeforeClass");
		}
}