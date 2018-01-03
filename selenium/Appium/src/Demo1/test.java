<<<<<<< HEAD
package Demo1;
import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.AfterClass;
import org.testng.annotations.BeforeClass;
import org.testng.annotations.Test;
import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;
import io.appium.java_client.android.AndroidElement;

public class test {
	   private AppiumDriver driver; 
	   WebElement el;
	   List<AndroidElement> textFieldsList;
	   
	
	public void DemoApp() throws MalformedURLException{
		
		System.out.println("获取当前路径apk");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
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
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
   
        System.out.println("App is launched!"+driver);

		 el = driver.findElement(By.id("Add Contact"));

	        el.click();

	        textFieldsList = driver.findElementsByClassName("android.widget.EditText");
	        textFieldsList.get(0).sendKeys("Some Name");
	        textFieldsList.get(2).sendKeys("Some@example.com");
	        //driver.swipe(100, 500, 100, 100, 2);
	        System.out.println("swipe");
	        //driver.findElementByName("Save").click();
	       
	}
	
	@Test
	public void Demo1() throws Exception{
		//测试安装程序
		System.out.println("Demo1");
 
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
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        System.out.println("安装wangc:"+driver);
        
        //XpathId(); //id定位
       // XpathName();  //name定位
        XpathClasNmae();
	}
	public void XpathId() throws Exception {
		//定位ID
		List<AndroidElement> textFieldsList = driver.findElementsByClassName("android.widget.EditText");
		String edget0=textFieldsList.get(0).getText();
		//com.streamaxtech.mdvr.direct:id/device_ip
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).clear();
		
		System.out.println("清楚内容");
		ty(5);
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).sendKeys("182");
		System.out.println("edget0"+edget0);
		ty(10);
		
		
	}
	public void XpathName() throws Exception {
		System.out.println("Name定位");
		String create=driver.findElement(By.name("运维平台登录")).getText();
		
		System.out.println("运维："+create);
		ty(5);

	}
	public void XpathClasNmae() throws Exception {
		//ClassName定位会有多个class
		System.out.println("XpathClasNmae定位");
		//多个editetext定位不建议使用
		//el=driver.findElement(By.className("android.widget.EditText"));
		//使用list列表定位
		textFieldsList=driver.findElementsByClassName("android.widget.EditText");
		String st=textFieldsList.get(0).getText();
		textFieldsList.get(1).clear();
		System.out.println("运维："+st);
		ty(5);

	}
    @AfterClass
    public void tearDown() throws Exception {  
    	  System.out.println("@AfterClass+quit");
        driver.quit();  
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
=======
package Demo1;
import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.AfterClass;
import org.testng.annotations.BeforeClass;
import org.testng.annotations.Test;
import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;
import io.appium.java_client.android.AndroidElement;

public class test {
	   private AppiumDriver driver; 
	   WebElement el;
	   List<AndroidElement> textFieldsList;
	   
	
	public void DemoApp() throws MalformedURLException{
		
		System.out.println("获取当前路径apk");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
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
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
   
        System.out.println("App is launched!"+driver);

		 el = driver.findElement(By.id("Add Contact"));

	        el.click();

	        textFieldsList = driver.findElementsByClassName("android.widget.EditText");
	        textFieldsList.get(0).sendKeys("Some Name");
	        textFieldsList.get(2).sendKeys("Some@example.com");
	        //driver.swipe(100, 500, 100, 100, 2);
	        System.out.println("swipe");
	        //driver.findElementByName("Save").click();
	       
	}
	
	@Test
	public void Demo1() throws Exception{
		//测试安装程序
		System.out.println("Demo1");
 
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
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        System.out.println("安装wangc:"+driver);
        
        //XpathId(); //id定位
       // XpathName();  //name定位
        XpathClasNmae();
	}
	public void XpathId() throws Exception {
		//定位ID
		List<AndroidElement> textFieldsList = driver.findElementsByClassName("android.widget.EditText");
		String edget0=textFieldsList.get(0).getText();
		//com.streamaxtech.mdvr.direct:id/device_ip
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).clear();
		
		System.out.println("清楚内容");
		ty(5);
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).sendKeys("182");
		System.out.println("edget0"+edget0);
		ty(10);
		
		
	}
	public void XpathName() throws Exception {
		System.out.println("Name定位");
		String create=driver.findElement(By.name("运维平台登录")).getText();
		
		System.out.println("运维："+create);
		ty(5);

	}
	public void XpathClasNmae() throws Exception {
		//ClassName定位会有多个class
		System.out.println("XpathClasNmae定位");
		//多个editetext定位不建议使用
		//el=driver.findElement(By.className("android.widget.EditText"));
		//使用list列表定位
		textFieldsList=driver.findElementsByClassName("android.widget.EditText");
		String st=textFieldsList.get(0).getText();
		textFieldsList.get(1).clear();
		System.out.println("运维："+st);
		ty(5);

	}
    @AfterClass
    public void tearDown() throws Exception {  
    	  System.out.println("@AfterClass+quit");
        driver.quit();  
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
>>>>>>> 1cce76996a288d93e2decf71290fc3556ef23947
