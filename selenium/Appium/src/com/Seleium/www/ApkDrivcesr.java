package com.Seleium.www;
import java.io.File;
import org.openqa.selenium.remote.CapabilityType;
import org.openqa.selenium.remote.DesiredCapabilities;
public class ApkDrivcesr {
	DesiredCapabilities capabilities=new DesiredCapabilities();
	Config confing=new Config();
	//��װ��
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
				//����appium
	        capabilities.setCapability(CapabilityType.BROWSER_NAME, "");
	    	//capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
	        capabilities.setCapability("deviceName","Android Emulator");  //��׿��������
	        capabilities.setCapability("platformName", "Android"); //��׿�Զ�������IOS�Զ���
	        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
	        capabilities.setCapability("appPackage", Package);
	        Assist.ty(2);
	        capabilities.setCapability("appActivity", Activity); 
	        Assist.pn("�������...."+ap);
		}else if ("apk".equals(ap)) {
			System.out.println("��ȡ��ǰ·��apk");
	        File classpathRoot = new File(System.getProperty("user.dir")); 
	        System.out.println("SetupApi--------------1"+classpathRoot);
	        File appDir = new File(classpathRoot, "/apps");
	        File app = new File(appDir, apk);
	        System.out.println("app-"+app);
	       // capabilities.setCapability("noReset", true); 	//	�����Ѱ�װ��Ӧ�á�����װapp
	        capabilities.setCapability("deviceName","Android Emulator");  //
	        capabilities.setCapability("platformVersion", "4.4");    //���ð�׿ϵͳ�汾   
	        capabilities.setCapability("sessionOverride", true);//ÿ������ʱ����session������ڶ��κ����лᱨ�����½�session  
	        capabilities.setCapability("app", app.getAbsolutePath()); //��װapp
	        Assist.pn("�������...."+apk);
		} else {
			Assist.pn("app�������....app	||	apk:"+ap);
		}
		
	}
}
