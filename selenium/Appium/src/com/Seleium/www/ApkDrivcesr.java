package com.Seleium.www;
import java.io.File;
import org.openqa.selenium.remote.CapabilityType;
import org.openqa.selenium.remote.DesiredCapabilities;
public class ApkDrivcesr {
	DesiredCapabilities capabilities=new DesiredCapabilities();
	Config confing=new Config();
	//封装类
	public ApkDrivcesr() throws Exception {
		Assist.pn("ApkDrivcesr");
		this.AppDr();
		
	}
	public DesiredCapabilities GetCap() {
		Assist.pn("capabilities::"+capabilities);
		return capabilities;
		
	}
	public DesiredCapabilities GetCpab() {
		Assist.pn("GetCap  :"+capabilities);
		return capabilities;
		
	}
	
	private void AppDr() throws Exception {
		Assist.pn("AppDr");
		String ap,apk,Package,Activity;
		
		ap=confing.app;
		Assist.pn("AppDr  ap:"+ap);
		
		Package=confing.Package;
		Assist.pn("Package:"+Package);
		
		Activity=confing.Activity;
		Assist.pn("Activity:"+Activity);
		
		apk="MDVR.apk";
		Assist.pn("apk  if:"+ap);
		if ("app".equals(ap)) {
				//启动appium
	        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
	    	//capabilities.setCapability("sessionOverride", true);//每次启动时覆盖session，否则第二次后运行会报错不能新建session  
	        capabilities.setCapability("deviceName","Android Emulator");  //安卓设置名字
	        capabilities.setCapability("platformName", "Android"); //安卓自动化还是IOS自动化
	        capabilities.setCapability("platformVersion", "4.4");    //设置安卓系统版本   
	        capabilities.setCapability("appPackage", Package);
	        Assist.ty(2);
	        capabilities.setCapability("appActivity", Activity); 
	        Assist.pn("加载完成...."+ap);
		}else if ("apk".equals(ap)) {
			System.out.println("获取当前路径apk");
	        File classpathRoot = new File(System.getProperty("user.dir")); 
	        System.out.println("SetupApi--------------1"+classpathRoot);
	        File appDir = new File(classpathRoot, "/apps");
	        File app = new File(appDir, apk);
	        System.out.println("app-"+app);
	       // capabilities.setCapability("noReset", true); 	//	测试已安装的应用。不安装app
	        capabilities.setCapability("deviceName","Android Emulator");  //
	        capabilities.setCapability("platformVersion", "4.4");    //设置安卓系统版本   
	        capabilities.setCapability("sessionOverride", true);//每次启动时覆盖session，否则第二次后运行会报错不能新建session  
	        capabilities.setCapability("app", app.getAbsolutePath()); //安装app
	        Assist.pn("加载完成...."+apk);
		} else {
			Assist.pn("app输入错误....app	||	apk:"+ap);
		}
		
	}
}
