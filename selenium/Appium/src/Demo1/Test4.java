package Demo1;

import java.net.URL;

import org.openqa.selenium.remote.DesiredCapabilities;

import com.Seleium.www.ApkDrivcesr;
import com.Seleium.www.Assist;
import com.Seleium.www.Config;
import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;

public class Test4 {
	private static AppiumDriver driver; 
	public static void main(String[] args) throws Exception  {
		Config confing=new Config();
		ApkDemo();
		int it=Config.waitTime;
		String  st=confing.Activity;
		Assist.pn("123:"+it);
		Assist.pn("123st:"+st);
	}
	public static void ApkDemo() throws Exception{
		
		Assist.pn("ApkDemo:");
		ApkDrivcesr ap=new ApkDrivcesr();
		Assist.pn("Test4");
		DesiredCapabilities capabilities = ap.GetCap();
		Assist.pn("capabilities:"+capabilities);
		driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(10);
        Assist.pn("初始化完成BeforeClass");
		
		
	}

}
