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
		
		System.out.println("��ȡ��ǰ·��apk");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
        System.out.println("app-"+app);
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
       // capabilities.setCapability("noReset", true); 	//	�����Ѱ�װ��Ӧ�á�����װapp
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp
        System.out.println("��װ");
        //��ʼ�����Ӱ�׿ϵͳ��
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
		//���԰�װ����
		System.out.println("Demo1");
 
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "MDVR.apk");
        System.out.println("app-"+app);
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
       // capabilities.setCapability("noReset", true); 	//	�����Ѱ�װ��Ӧ�á�����װapp
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp
        System.out.println("��װ");
        //��ʼ�����Ӱ�׿ϵͳ��
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        System.out.println("��װwangc:"+driver);
        
        //XpathId(); //id��λ
       // XpathName();  //name��λ
        XpathClasNmae();
	}
	public void XpathId() throws Exception {
		//��λID
		List<AndroidElement> textFieldsList = driver.findElementsByClassName("android.widget.EditText");
		String edget0=textFieldsList.get(0).getText();
		//com.streamaxtech.mdvr.direct:id/device_ip
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).clear();
		
		System.out.println("�������");
		ty(5);
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).sendKeys("182");
		System.out.println("edget0"+edget0);
		ty(10);
		
		
	}
	public void XpathName() throws Exception {
		System.out.println("Name��λ");
		String create=driver.findElement(By.name("��άƽ̨��¼")).getText();
		
		System.out.println("��ά��"+create);
		ty(5);

	}
	public void XpathClasNmae() throws Exception {
		//ClassName��λ���ж��class
		System.out.println("XpathClasNmae��λ");
		//���editetext��λ������ʹ��
		//el=driver.findElement(By.className("android.widget.EditText"));
		//ʹ��list�б�λ
		textFieldsList=driver.findElementsByClassName("android.widget.EditText");
		String st=textFieldsList.get(0).getText();
		textFieldsList.get(1).clear();
		System.out.println("��ά��"+st);
		ty(5);

	}
    @AfterClass
    public void tearDown() throws Exception {  
    	  System.out.println("@AfterClass+quit");
        driver.quit();  
    }  
   public void ty(int tiem) throws Exception {
	  try {
		  System.out.println("�ȴ��룺"+tiem);
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
		
		System.out.println("��ȡ��ǰ·��apk");
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "ContactManager.apk");
        System.out.println("app-"+app);
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
       // capabilities.setCapability("noReset", true); 	//	�����Ѱ�װ��Ӧ�á�����װapp
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp
        System.out.println("��װ");
        //��ʼ�����Ӱ�׿ϵͳ��
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
		//���԰�װ����
		System.out.println("Demo1");
 
        File classpathRoot = new File(System.getProperty("user.dir")); 
        System.out.println("SetupApi--------------1"+classpathRoot);
        File appDir = new File(classpathRoot, "/apps");
        File app = new File(appDir, "MDVR.apk");
        System.out.println("app-"+app);
        //������ز���
        DesiredCapabilities capabilities = new DesiredCapabilities(); 	//����appium
       // capabilities.setCapability("noReset", true); 	//	�����Ѱ�װ��Ӧ�á�����װapp
        capabilities.setCapability("deviceName","Android Emulator");  //
        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
        capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp
        System.out.println("��װ");
        //��ʼ�����Ӱ�׿ϵͳ��
        driver = new AndroidDriver(new URL("http://127.0.0.1:4723/wd/hub"), capabilities);
        System.out.println("��װwangc:"+driver);
        
        //XpathId(); //id��λ
       // XpathName();  //name��λ
        XpathClasNmae();
	}
	public void XpathId() throws Exception {
		//��λID
		List<AndroidElement> textFieldsList = driver.findElementsByClassName("android.widget.EditText");
		String edget0=textFieldsList.get(0).getText();
		//com.streamaxtech.mdvr.direct:id/device_ip
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).clear();
		
		System.out.println("�������");
		ty(5);
		driver.findElement(By.id("com.streamaxtech.mdvr.direct:id/device_ip")).sendKeys("182");
		System.out.println("edget0"+edget0);
		ty(10);
		
		
	}
	public void XpathName() throws Exception {
		System.out.println("Name��λ");
		String create=driver.findElement(By.name("��άƽ̨��¼")).getText();
		
		System.out.println("��ά��"+create);
		ty(5);

	}
	public void XpathClasNmae() throws Exception {
		//ClassName��λ���ж��class
		System.out.println("XpathClasNmae��λ");
		//���editetext��λ������ʹ��
		//el=driver.findElement(By.className("android.widget.EditText"));
		//ʹ��list�б�λ
		textFieldsList=driver.findElementsByClassName("android.widget.EditText");
		String st=textFieldsList.get(0).getText();
		textFieldsList.get(1).clear();
		System.out.println("��ά��"+st);
		ty(5);

	}
    @AfterClass
    public void tearDown() throws Exception {  
    	  System.out.println("@AfterClass+quit");
        driver.quit();  
    }  
   public void ty(int tiem) throws Exception {
	  try {
		  System.out.println("�ȴ��룺"+tiem);
		  int t=tiem*1000;
		   Thread.sleep(t);
		   
	} catch (Exception e) {
		// TODO: handle exception
	}
	   
   }

	
}
>>>>>>> 1cce76996a288d93e2decf71290fc3556ef23947
