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
		 cap.setCapability("automationName", "Appium");//appium做自动化
		 //    cap.setCapability("app", "C:\\software\\jrtt.apk");//安装apk
		 //    cap.setCapability("browserName", "chrome");//设置HTML5的自动化，打开谷歌浏览器
		 cap.setCapability("deviceName", "S4");//设备名称
		 cap.setCapability("platformName", "Android"); //安卓自动化还是IOS自动化
		 cap.setCapability("platformVersion", "4.4"); //安卓操作系统版本
		 cap.setCapability("udid", "127.0.0.1:62001"); //设备的udid (adb devices 查看到的)
		 cap.setCapability("appPackage","com.android.calculator2");//被测app的包名
		 cap.setCapability("appActivity",".Calculator");//被测app的入口Activity名称
		 cap.setCapability("unicodeKeyboard", "True"); //支持中文输入
		 cap.setCapability("resetKeyboard", "True"); //支持中文输入，必须两条都配置
		 cap.setCapability("noSign", "True"); //不重新签名apk
		 System.out.println("SetupApi--------------noSign");
		 cap.setCapability("newCommandTimeout", "30"); //没有新命令，appium30秒退出
		 System.out.println("SetupApi--------------newCommandTimeout");
		 driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"),cap);//把以上配置传到appium服务端并连接手机
		 System.out.println("SetupApi--------------driver");
		 driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);//隐式等待
		 System.out.println("SetupApi--------------manage");
		  //通过resource name定位元素
		  driver.findElement(By.name("1")).click();
			 System.out.println("SetupApi--------------name1");
		 driver.findElement(By.name("+")).click();
		 driver.findElement(By.name("1")).click();
		  driver.findElement(By.name("=")).click();
	}

}
