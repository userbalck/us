package Demo1;
import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;

import org.junit.After;
import org.junit.Before;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.testng.annotations.Test;
import io.appium.java_client.AppiumDriver;
import io.appium.java_client.android.AndroidDriver;
import io.appium.java_client.android.AndroidElement;

public class test {
	   private AppiumDriver driver; 
	@Test
	public void DemoApp() throws MalformedURLException{
		System.out.println("SetupApi--------------");
        File classpathRoot = new File(System.getProperty("user.dir"));
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
        System.out.println("SetupApi---app-----------1"+app);
        DesiredCapabilities capabilities = new DesiredCapabilities();
        System.out.println("SetupApi--------------2"+capabilities);
        capabilities.setCapability("deviceName","Android Emulator");
        capabilities.setCapability("platformVersion", "4.4");
        capabilities.setCapability("app", app.getAbsolutePath());
        System.out.println("SetupApi--------------51");
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
  
        System.out.println("App is launched!"+driver);

		 WebElement el = driver.findElement(By.id("Add Contact"));

	        el.click();

	        List<AndroidElement> textFieldsList = driver.findElementsByClassName("android.widget.EditText");

	        textFieldsList.get(0).sendKeys("Some Name");

	        textFieldsList.get(2).sendKeys("Some@example.com");

	        //driver.swipe(100, 500, 100, 100, 2);
	        System.out.println("swipe");
	        //driver.findElementByName("Save").click();
	       
	       
		
	}


	
}
