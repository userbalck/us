package Demo1;

import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;

import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.BeforeClass;
import org.testng.annotations.Test;

import com.Seleium.www.Assist;

import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;

public class Test3 {
	 private AppiumDriver driver; 
	   WebElement el;
	@BeforeClass
	public void Eef() throws Exception{
		System.out.println("Demo1");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
        System.out.println("app-"+app);
        //设置相关参数
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//启动appium
    	capabilities.setCapability("sessionOverride", true);//每次启动时覆盖session，否则第二次后运行会报错不能新建session  
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //设置安卓系统版本   
        capabilities.setCapability("app", app.getAbsolutePath()); //安装app
      //初始化连接安卓系统，
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        Assist.ty(10);
        Assist.pn("初始化完成BeforeClass");
	}
	@Test
	public void Test1(){
		Assist.pn("123");
		
		
	}
}
