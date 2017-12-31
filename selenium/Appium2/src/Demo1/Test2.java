package Demo1;

import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.concurrent.TimeUnit;

import org.openqa.selenium.By;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.Test;

import io.appium.java_client.android.AndroidDriver;

public class Test2 {
	static AndroidDriver driver;
	public static void main(String[] args)  throws Exception{
		// TODO Auto-generated method stub
		 
		 DesiredCapabilities cap=new DesiredCapabilities();
		 cap.setCapability("automationName", "Appium");//appium���Զ���
		 //    cap.setCapability("app", "C:\\software\\jrtt.apk");//��װapk
		 //    cap.setCapability("browserName", "chrome");//����HTML5���Զ������򿪹ȸ������
		 cap.setCapability("deviceName", "S4");//�豸����
		 cap.setCapability("platformName", "Android"); //��׿�Զ�������IOS�Զ���
		 cap.setCapability("platformVersion", "4.4"); //��׿����ϵͳ�汾
		 cap.setCapability("udid", "127.0.0.1:62001"); //�豸��udid (adb devices �鿴����)
		 cap.setCapability("appPackage","com.android.calculator2");//����app�İ���
		 cap.setCapability("appActivity",".Calculator");//����app�����Activity����
		 cap.setCapability("unicodeKeyboard", "True"); //֧����������
		 cap.setCapability("resetKeyboard", "True"); //֧���������룬��������������
		 cap.setCapability("noSign", "True"); //������ǩ��apk
		 System.out.println("SetupApi--------------noSign");
		 cap.setCapability("newCommandTimeout", "30"); //û�������appium30���˳�
		 System.out.println("SetupApi--------------newCommandTimeout");
		 driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"),cap);//���������ô���appium����˲������ֻ�
		 System.out.println("SetupApi--------------driver");
		 driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);//��ʽ�ȴ�
		 System.out.println("SetupApi--------------manage");
		  //ͨ��resource name��λԪ��
		  driver.findElement(By.name("1")).click();
			 System.out.println("SetupApi--------------name1");
		 driver.findElement(By.name("+")).click();
		 driver.findElement(By.name("1")).click();
		  driver.findElement(By.name("=")).click();
	}
	@Test
	public void Demo1() throws MalformedURLException{
		
		System.out.println("Demo1");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
        System.out.println("app-"+app);
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
    	capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp

        
        //��ʼ�����Ӱ�׿ϵͳ��
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
  
        System.out.println("App is launched!"+driver);
	}

}
