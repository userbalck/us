package Demo1;

import java.net.URL;
import java.util.concurrent.TimeUnit;

import org.openqa.selenium.By;
import org.openqa.selenium.remote.DesiredCapabilities;

import io.appium.java_client.android.AndroidDriver;

public class Test2 {

	public static void main(String[] args)  throws Exception{
		// TODO Auto-generated method stub
		 AndroidDriver driver;
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

}
